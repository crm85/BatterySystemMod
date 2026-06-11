using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class ThirdPartySettings : SAINSettingsBase<ThirdPartySettings>, ISAINSettings
{
	public string Description = "When an enemy is a certain angle away from their active enemies last known position, this will reduce their vision speed of that target up to the maximum set amount.";

	public bool Enabled = true;

	[MinMax(5f, 60f, 1f)]
	[Advanced]
	public float THIRDPARTY_VISION_START_ANGLE = 30f;

	[MinMax(1f, 3f, 100f)]
	[Advanced]
	public float THIRDPARTY_VISION_MAX_COEF = 1.5f;
}
