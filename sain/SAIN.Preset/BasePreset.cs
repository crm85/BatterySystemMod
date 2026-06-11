namespace SAIN.Preset;

public abstract class BasePreset
{
	public readonly SAINPresetClass Preset;

	public readonly SAINPresetDefinition Info;

	public BasePreset(SAINPresetClass presetClass)
	{
		Preset = presetClass;
		Info = presetClass.Info;
	}
}
