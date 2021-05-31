/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

public class Micom : Device
{
	public struct WheelInfo
	{
		public float wheelRadius;
		public float wheelTread;
		public float divideWheelRadius; // for computational performance

		public WheelInfo(in float radius = 0.1f, in float tread = 0)
		{
			this.wheelRadius = radius;
			this.wheelTread = tread;
			this.divideWheelRadius = 1.0f / wheelRadius;
		}
	}

	private MicomSensor micomSensor = null;
	private MicomInput micomInput = null;

	public bool debugging = false;

	protected override void OnAwake()
	{
		Mode = ModeType.NONE;
		DeviceName = "Micom";
	}

	protected override void OnStart()
	{
	}

	protected override void OnReset()
	{
		micomSensor.Reset();
		micomInput.Reset();
	}

	public MicomInput GetInput()
	{
		if (micomInput == null)
		{
			micomInput = gameObject.AddComponent<MicomInput>();
			micomInput.SetMotorControl(GetSensor().MotorControl);
			micomInput.EnableDebugging = EnableDebugging;
		}

		return micomInput;
	}

	public MicomSensor GetSensor()
	{
		if (micomSensor == null)
		{
			micomSensor = gameObject.AddComponent<MicomSensor>();
			micomSensor.SetPluginParameters(GetPluginParameters());
			micomSensor.EnableDebugging = EnableDebugging;
		}

		return micomSensor;
	}
}