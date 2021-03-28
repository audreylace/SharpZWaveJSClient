namespace AudreysCloud.Community.SharpZWaveJSClient
{


	// MIT License

	// Copyright (c) 2018-2020 AlCalzone

	// Permission is hereby granted, free of charge, to any person obtaining a copy
	// of this software and associated documentation files (the "Software"), to deal
	// in the Software without restriction, including without limitation the rights
	// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	// copies of the Software, and to permit persons to whom the Software is
	// furnished to do so, subject to the following conditions:

	// The above copyright notice and this permission notice shall be included in all
	// copies or substantial portions of the Software.

	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	// SOFTWARE.

	public enum CommandClasses
	{
		AlarmSensor = 156,
		AlarmSilence = 157,
		AllSwitch = 39,
		AntiTheft = 93,
		AntiTheftUnlock = 126,
		ApplicationCapability = 87,
		ApplicationStatus = 34,
		Association = 133,
		AssociationCommandConfiguration = 155,
		AssociationGroupInformation = 89,
		Authentication = 161,
		AuthenticationMediaWrite = 162,
		BarrierOperator = 102,
		Basic = 32,
		BasicTariffInformation = 54,
		BasicWindowCovering = 80,
		Battery = 128,
		BinarySensor = 48,
		BinarySwitch = 37,
		BinaryToggleSwitch = 40,
		ClimateControlSchedule = 70,
		CentralScene = 91,
		Clock = 129,
		ColorSwitch = 51,
		Configuration = 112,
		ControllerReplication = 33,
		CRC16Encapsulation = 86,
		DemandControlPlanConfiguration = 58,
		DemandControlPlanMonitor = 59,
		DeviceResetLocally = 90,
		DoorLock = 98,
		DoorLockLogging = 76,
		EnergyProduction = 144,
		EntryControl = 111,
		FirmwareUpdateMetaData = 122,
		GenericSchedule = 163,
		GeographicLocation = 140,
		GroupingName = 123,
		Hail = 130,
		HRVStatus = 55,
		HRVControl = 57,
		HumidityControlMode = 109,
		HumidityControlOperatingState = 110,
		HumidityControlSetpoint = 100,
		InclusionController = 116,
		Indicator = 135,
		IPAssociation = 92,
		IPConfiguration = 154,
		IRRepeater = 160,
		Irrigation = 107,
		Language = 137,
		Lock = 118,
		Mailbox = 105,
		ManufacturerProprietary = 145,
		ManufacturerSpecific = 114,
		SupportControlMark = 239,
		Meter = 50,
		MeterTableConfiguration = 60,
		MeterTableMonitor = 61,
		MeterTablePushConfiguration = 62,
		MoveToPositionWindowCovering = 81,
		MultiChannel = 96,
		MultiChannelAssociation = 142,
		MultiCommand = 143,
		MultilevelSensor = 49,
		MultilevelSwitch = 38,
		MultilevelToggleSwitch = 41,
		NetworkManagementBasicNode = 77,
		NetworkManagementInclusion = 52,
		NetworkManagementInstallationAndMaintenance = 103,
		NetworkManagementPrimary = 84,
		NetworkManagementProxy = 82,
		NoOperation = 0,
		NodeNamingAndLocation = 119,
		NodeProvisioning = 120,
		Notification = 113,
		Powerlevel = 115,
		Prepayment = 63,
		PrepaymentEncapsulation = 65,
		Proprietary = 136,
		Protection = 117,
		PulseMeter = 53,
		RateTableConfiguration = 72,
		RateTableMonitor = 73,
		RemoteAssociationActivation = 124,
		RemoteAssociationConfiguration = 125,
		SceneActivation = 43,
		SceneActuatorConfiguration = 44,
		SceneControllerConfiguration = 45,
		Schedule = 83,
		ScheduleEntryLock = 78,
		ScreenAttributes = 147,
		ScreenMetaData = 146,
		Security = 152,
		Security2 = 159,
		SecurityMark = 61696,
		SensorConfiguration = 158,
		SimpleAVControl = 148,
		SoundSwitch = 121,
		Supervision = 108,
		TariffTableConfiguration = 74,
		TariffTableMonitor = 75,
		ThermostatFanMode = 68,
		ThermostatFanState = 69,
		ThermostatMode = 64,
		ThermostatOperatingState = 66,
		ThermostatSetback = 71,
		ThermostatSetpoint = 67,
		Time = 138,
		TimeParameters = 139,
		TransportService = 85,
		UserCode = 99,
		Version = 134,
		WakeUp = 132,
		WindowCovering = 106,
		ZIP = 35,
		ZIP6LoWPAN = 79,
		ZIPGateway = 95,
		ZIPNamingAndLocation = 104,
		ZIPND = 88,
		ZIPPortal = 97,
		ZWavePlusInfo = 94
	}
}