using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class PoseVisibilitySettings : SAINSettingsBase<PoseVisibilitySettings>, ISAINSettings
{
	public string Description = "Scales vision speed based on the pose of their enemy. Only applies to Non-AI targets.";

	public bool Enabled = true;

	[MinMax(1f, 3f, 100f)]
	[Advanced]
	public float PRONE_VISION_SPEED_COEF = 1.75f;

	[MinMax(1f, 3f, 100f)]
	[Advanced]
	public float DUCK_VISION_SPEED_COEF = 1.25f;
}
