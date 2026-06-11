using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;

namespace SAIN.SAINComponent.Classes;

public class BotDifficultyClass : BotBase
{
	public TemporaryStatModifiers GlobalDifficultyModifiers { get; }

	public TemporaryStatModifiers BotDifficultyModifiers { get; }

	public TemporaryStatModifiers PersonalityDifficultyModifiers { get; }

	public TemporaryStatModifiers LocationDifficultyModifiers { get; }

	public float AggressionModifier { get; private set; } = 1f;

	public float HearingDistanceModifier { get; private set; } = 1f;

	public BotDifficultyClass(BotComponent sain)
		: base(sain)
	{
		GlobalDifficultyModifiers = new TemporaryStatModifiers();
		BotDifficultyModifiers = new TemporaryStatModifiers();
		PersonalityDifficultyModifiers = new TemporaryStatModifiers();
		LocationDifficultyModifiers = new TemporaryStatModifiers();
	}

	public override void Dispose()
	{
		dismiss();
		base.Dispose();
	}

	public void UpdateSettings(SAINPresetClass preset)
	{
		dismiss();
		applyGlobal(preset);
		applyBot(preset);
		applyLocation(preset);
		applyPersonality(preset);
		apply();
		createCustomMods(preset);
	}

	private void createCustomMods(SAINPresetClass preset)
	{
		DifficultySettings difficulty = preset.GlobalSettings.Difficulty;
		DifficultySettings difficulty2 = base.Bot.Info.FileSettings.Difficulty;
		DifficultySettings difficulty3 = base.Bot.Info.PersonalitySettingsClass.Difficulty;
		HearingDistanceModifier = 1f * difficulty.HearingDistanceCoef * difficulty2.HearingDistanceCoef * difficulty3.HearingDistanceCoef;
		AggressionModifier = 1f * difficulty.AggressionCoef * difficulty2.AggressionCoef * difficulty3.AggressionCoef;
		DifficultySettings difficultySettings = preset.GlobalSettings.Location.Current();
		if (difficultySettings != null)
		{
			HearingDistanceModifier *= difficultySettings.HearingDistanceCoef;
			AggressionModifier *= difficultySettings.AggressionCoef;
		}
	}

	private void applyGlobal(SAINPresetClass preset)
	{
		DifficultySettings difficulty = preset.GlobalSettings.Difficulty;
		GClass595 modifiers = GlobalDifficultyModifiers.Modifiers;
		modifiers.AccuratySpeedCoef = difficulty.AccuracySpeedCoef;
		modifiers.PrecicingSpeedCoef = difficulty.PrecisionSpeedCoef;
		modifiers.VisibleDistCoef = difficulty.VisibleDistCoef;
		modifiers.ScatteringCoef = difficulty.ScatteringCoef;
		modifiers.GainSightCoef = difficulty.GainSightCoef;
		modifiers.HearingDistCoef = difficulty.HearingDistanceCoef;
	}

	private void apply(DifficultySettings settings, TemporaryStatModifiers mods)
	{
		mods.Modifiers.AccuratySpeedCoef = settings.AccuracySpeedCoef;
		mods.Modifiers.PrecicingSpeedCoef = settings.PrecisionSpeedCoef;
		mods.Modifiers.VisibleDistCoef = settings.VisibleDistCoef;
		mods.Modifiers.ScatteringCoef = settings.ScatteringCoef;
		mods.Modifiers.GainSightCoef = settings.GainSightCoef;
		mods.Modifiers.HearingDistCoef = settings.HearingDistanceCoef;
	}

	private void applyBot(SAINPresetClass preset)
	{
		DifficultySettings difficulty = base.Bot.Info.FileSettings.Difficulty;
		GClass595 modifiers = BotDifficultyModifiers.Modifiers;
		modifiers.AccuratySpeedCoef = difficulty.AccuracySpeedCoef;
		modifiers.PrecicingSpeedCoef = difficulty.PrecisionSpeedCoef;
		modifiers.VisibleDistCoef = difficulty.VisibleDistCoef;
		modifiers.ScatteringCoef = difficulty.ScatteringCoef;
		modifiers.GainSightCoef = difficulty.GainSightCoef;
		modifiers.HearingDistCoef = difficulty.HearingDistanceCoef;
	}

	private void applyLocation(SAINPresetClass preset)
	{
		DifficultySettings difficultySettings = preset.GlobalSettings.Location.Current();
		if (difficultySettings != null)
		{
			GClass595 modifiers = LocationDifficultyModifiers.Modifiers;
			modifiers.AccuratySpeedCoef = difficultySettings.AccuracySpeedCoef;
			modifiers.PrecicingSpeedCoef = difficultySettings.PrecisionSpeedCoef;
			modifiers.VisibleDistCoef = difficultySettings.VisibleDistCoef;
			modifiers.ScatteringCoef = difficultySettings.ScatteringCoef;
			modifiers.GainSightCoef = difficultySettings.GainSightCoef;
			modifiers.HearingDistCoef = difficultySettings.HearingDistanceCoef;
		}
	}

	private void applyPersonality(SAINPresetClass preset)
	{
		DifficultySettings difficulty = base.Bot.Info.PersonalitySettingsClass.Difficulty;
		GClass595 modifiers = PersonalityDifficultyModifiers.Modifiers;
		modifiers.AccuratySpeedCoef = difficulty.AccuracySpeedCoef;
		modifiers.PrecicingSpeedCoef = difficulty.PrecisionSpeedCoef;
		modifiers.VisibleDistCoef = difficulty.VisibleDistCoef;
		modifiers.ScatteringCoef = difficulty.ScatteringCoef;
		modifiers.GainSightCoef = difficulty.GainSightCoef;
		modifiers.HearingDistCoef = difficulty.HearingDistanceCoef;
	}

	private void apply()
	{
		GClass596 current = base.BotOwner.Settings.Current;
		current.Apply(GlobalDifficultyModifiers.Modifiers, -1f);
		current.Apply(BotDifficultyModifiers.Modifiers, -1f);
		current.Apply(PersonalityDifficultyModifiers.Modifiers, -1f);
		current.Apply(LocationDifficultyModifiers.Modifiers, -1f);
	}

	private void dismiss()
	{
		GClass596 current = base.BotOwner.Settings.Current;
		current.Dismiss(GlobalDifficultyModifiers.Modifiers);
		current.Dismiss(BotDifficultyModifiers.Modifiers);
		current.Dismiss(PersonalityDifficultyModifiers.Modifiers);
		current.Dismiss(LocationDifficultyModifiers.Modifiers);
	}

	private void applyMods(TemporaryStatModifiers mods)
	{
		base.BotOwner.Settings.Current.Apply(mods.Modifiers, -1f);
	}
}
