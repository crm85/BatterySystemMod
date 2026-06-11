using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class PartsVisibilitySettings : SAINSettingsBase<PartsVisibilitySettings>, ISAINSettings
{
	public string Description = "Scales vision speed based on the number of body parts that are within line of sight to their enemy. Only applies to Non-AI targets.";

	public bool Enabled = true;

	[MinMax(1f, 3f, 100f)]
	[Advanced]
	public float PARTS_VISIBLE_MAX_COEF = 2f;

	[MinMax(0.25f, 1f, 100f)]
	[Advanced]
	public float PARTS_VISIBLE_MIN_COEF = 0.9f;
}
