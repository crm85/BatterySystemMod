using System.Collections.Generic;
using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class GlobalDifficultySettings : SAINSettingsBase<GlobalDifficultySettings>, ISAINSettings
{
	[Name("Vision Distance Multiplier")]
	[Description("Higher is more difficult.")]
	[DifficultyMod]
	public float VisibleDistCoef = 1f;

	[Name("Vision Speed Multiplier")]
	[Description("Lower is more difficult.")]
	[DifficultyMod]
	public float GainSightCoef = 1f;

	[Name("Scatter Multiplier")]
	[Description("Lower is more difficult.")]
	[DifficultyMod]
	public float ScatteringCoef = 1f;

	[Name("Hearing Distance Multiplier")]
	[Description("Higher is more difficult.")]
	[DifficultyMod]
	public float HearingDistanceCoef = 1f;

	[Name("Aggression Multiplier")]
	[Description("Higher is more difficult.")]
	[DifficultyMod]
	public float AggressionCoef = 1f;

	[Name("Precision Speed Multiplier")]
	[Description("Lower is more difficult.")]
	[DifficultyMod]
	public float PrecisionSpeedCoef = 1f;

	[Name("Accuracy Speed Multiplier")]
	[Description("Lower is more difficult.")]
	[DifficultyMod]
	public float AccuracySpeedCoef = 1f;

	public override void Init(List<ISAINSettings> list)
	{
		list.Add(this);
	}
}
