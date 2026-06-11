using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class MovementVisibilitySettings : SAINSettingsBase<MovementVisibilitySettings>, ISAINSettings
{
	public string Description = "Scales vision speed based on the movement speed of their enemy. Faster movement = faster vision speed.";

	public bool Enabled = true;

	[Name("Movement Vision Modifier")]
	[Description("Bots will see moving players this much faster, at any range.Higher is slower speed, so 0.66 would result in bots spotting an enemy who is moving 0.66x faster. So if they usually would take 10 seconds to spot someone, it would instead take around 6.6 seconds.")]
	[MinMax(0.01f, 1f, 100f)]
	[Advanced]
	public float MOVEMENT_VISION_MULTIPLIER = 0.5f;
}
