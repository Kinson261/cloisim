/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using System;
using NetMQ;
using NetMQ.Sockets;

public partial class DeviceTransporter
{
	private PublisherSocket publisherSocket = null;

	private byte[] hashValueForPublish = null;
	private byte[] dataToPublish = null;

	private void DestroyPublisherSocket()
	{
		if (publisherSocket != null)
		{
			publisherSocket.Close();
			publisherSocket = null;
		}
	}

	public void SetHashForPublish(in ulong hash)
	{
		hashValueForPublish = BitConverter.GetBytes(hash);
	}

	protected bool InitializePublisher(in ushort targetPort)
	{
		var initialized = false;
		publisherSocket = new PublisherSocket();

		if (publisherSocket != null)
		{
			publisherSocket.Options.Linger = TimeSpan.FromTicks(0);
			publisherSocket.Options.IPv4Only = true;
			publisherSocket.Options.TcpKeepalive = true;
			publisherSocket.Options.DisableTimeWait = true;
			publisherSocket.Options.SendHighWatermark = highwatermark;

			publisherSocket.Bind(GetAddress(targetPort));
			// publisherSocket.BindRandomPort(GetAddress());
			// Console.WriteLine("Publisher socket binding for - " + targetPort);
			initialized = StoreTag(ref dataToPublish, hashValueForPublish);
		}

		return initialized;
	}

	protected bool Publish(in DeviceMessage messageToSend)
	{
		if (messageToSend.IsValid())
		{
			var buffer = messageToSend.GetBuffer();
			var bufferLength = (int)messageToSend.Length;
			return Publish(buffer, bufferLength);
		}
		return false;
	}

	protected bool Publish(in string stringToSend)
	{
		var buffer = System.Text.Encoding.UTF8.GetBytes(stringToSend);
		return Publish(buffer, stringToSend.Length);
	}

	protected bool Publish(in byte[] buffer, in int bufferLength)
	{
		var wasSucessful = false;

		if (StoreData(ref dataToPublish, buffer, bufferLength))
		{
			if (publisherSocket != null && !publisherSocket.IsDisposed)
			{
				var dataLength = tagSize + bufferLength;
				wasSucessful = publisherSocket.TrySendFrame(dataToPublish, dataLength);
				// Debug.LogFormat("Publish data({0}) length({1})", buffer, bufferLength);
			}
			else
			{
				(Console.Out as DebugLogWriter).SetWarningOnce();
			Console.WriteLine("Socket for publisher is not ready yet.");
			}
		}

		return wasSucessful;
	}
}