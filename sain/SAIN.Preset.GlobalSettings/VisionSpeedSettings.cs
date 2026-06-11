using System.Collections.Generic;

namespace SAIN.Preset.GlobalSettings;

public class VisionSpeedSettings : SAINSettingsBase<VisionSpeedSettings>, ISAINSettings
{
	public ElevationVisionSettings Elevation = new ElevationVisionSettings();

	public MovementVisibilitySettings Movement = new MovementVisibilitySettings();

	public PartsVisibilitySettings PartsVisibility = new PartsVisibilitySettings();

	public PeripheralVisionSettings Peripheral = new PeripheralVisionSettings();

	public PoseVisibilitySettings Pose = new PoseVisibilitySettings();

	public ThirdPartySettings ThirdParty = new ThirdPartySettings();

	public override void Init(List<ISAINSettings> list)
	{
		list.Add(this);
		list.Add(Elevation);
		list.Add(Movement);
		list.Add(PartsVisibility);
		list.Add(Peripheral);
		list.Add(Pose);
		list.Add(ThirdParty);
	}
}
