/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using UnityEngine;
using messages = cloisim.msgs;
using Any = cloisim.msgs.Any;

public class JointControlPlugin : CLOiSimPlugin
{
	protected override void OnAwake()
	{
		type = ICLOiSimPlugin.Type.JOINTCONTROL;
		targetDevice = gameObject.AddComponent<Joints>();
	}

	protected override void OnStart()
	{
		RegisterRxDevice("Rx");
		RegisterTxDevice("Tx");

		AddThread(SenderThread, targetDevice);
		AddThread(ReceiverThread, targetDevice);

		LoadJoints();
	}

	protected override void OnReset()
	{
	}

	private void LoadJoints()
	{
		if (GetPluginParameters().GetValues<string>("joints/link", out var links))
		{
			var joints = targetDevice as Joints;
			foreach (var link in links)
			{
				if (joints != null)
				{
					joints.AddTarget(link);
					Debug.Log(link);
				}
			}
		}
	}


	protected override void HandleCustomRequestMessage(in string requestType, in Any requestValue, ref DeviceMessage response)
	{
		switch (requestType)
		{
			case "request_transform":
				var micomSensor = (targetDevice as Micom).GetSensor();
				var transformPartsName = requestValue.StringValue;
				var devicePose = micomSensor.GetPartsPose(transformPartsName);
				SetTransformInfoResponse(ref response, devicePose);
				break;

			default:
				break;
		}
	}
}