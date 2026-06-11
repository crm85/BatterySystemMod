using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class PeripheralVisionSettings : SAINSettingsBase<PeripheralVisionSettings>, ISAINSettings
{
	public string Description = "Adds additional vision speed reduction to targets in a bot's peripheral vision.Scales with the angle from their look direction.";

	public bool Enabled = true;

	[MinMax(5f, 60f, 1f)]
	[Advanced]
	public float PERIPHERAL_VISION_START_ANGLE = 30f;

	[MinMax(1f, 3f, 100f)]
	[Advanced]
	public float PERIPHERAL_VISION_MAX_REDUCTION_COEF = 2f;
}
