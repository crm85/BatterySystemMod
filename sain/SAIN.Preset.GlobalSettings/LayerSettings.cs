using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class LayerSettings : SAINSettingsBase<LayerSettings>, ISAINSettings
{
	[Description("Requires Restart. Dont touch unless you know what this is")]
	[DeveloperOption]
	[MinMax(0f, 100f, 100f)]
	public int SAINCombatSquadLayerPriority = 22;

	[Description("Requires Restart. Dont touch unless you know what this is")]
	[DeveloperOption]
	[MinMax(0f, 100f, 100f)]
	public int SAINExtractLayerPriority = 24;

	[Description("Requires Restart. Dont touch unless you know what this is")]
	[DeveloperOption]
	[MinMax(0f, 100f, 100f)]
	public int SAINCombatSoloLayerPriority = 20;
}
