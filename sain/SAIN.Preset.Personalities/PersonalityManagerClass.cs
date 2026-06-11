using System.Collections.Generic;
using SAIN.Helpers;

namespace SAIN.Preset.Personalities;

public class PersonalityManagerClass : BasePreset
{
	public PersonalityDictionary PersonalityDictionary = new PersonalityDictionary();

	public PersonalityManagerClass(SAINPresetClass preset)
		: base(preset)
	{
		import();
		PersonalityDefaultsClass.InitDefaults(PersonalityDictionary, Preset);
	}

	public void Init()
	{
		foreach (PersonalitySettingsClass value in PersonalityDictionary.Values)
		{
			value.Init();
		}
	}

	public void UpdateDefaults(PersonalityManagerClass replacementClass = null)
	{
		foreach (KeyValuePair<EPersonality, PersonalitySettingsClass> item in PersonalityDictionary)
		{
			PersonalitySettingsClass replacementGroup = replacementClass?.PersonalityDictionary[item.Key];
			item.Value.UpdateDefaults(replacementGroup);
		}
	}

	public void Update()
	{
		foreach (PersonalitySettingsClass value in PersonalityDictionary.Values)
		{
			value.Update();
		}
	}

	private void import()
	{
		if (!Preset.Info.IsCustom)
		{
			return;
		}
		EPersonality[] personalities = EnumValues.Personalities;
		for (int i = 0; i < personalities.Length; i++)
		{
			EPersonality key = personalities[i];
			if (SAINPresetClass.Import<PersonalitySettingsClass>(out var result, Preset.Info.Name, key.ToString(), "Personalities"))
			{
				PersonalityDictionary.Add(key, result);
			}
		}
	}

	public void ResetAllToDefaults()
	{
		PersonalityDictionary.Remove(EPersonality.Wreckless);
		PersonalityDictionary.Remove(EPersonality.SnappingTurtle);
		PersonalityDictionary.Remove(EPersonality.GigaChad);
		PersonalityDictionary.Remove(EPersonality.Chad);
		PersonalityDictionary.Remove(EPersonality.Rat);
		PersonalityDictionary.Remove(EPersonality.Coward);
		PersonalityDictionary.Remove(EPersonality.Timmy);
		PersonalityDictionary.Remove(EPersonality.Normal);
		PersonalityDefaultsClass.InitDefaults(PersonalityDictionary, Preset);
	}

	public void ResetToDefault(EPersonality personality)
	{
		PersonalityDictionary.Remove(personality);
		PersonalityDefaultsClass.InitDefaults(PersonalityDictionary, Preset);
	}
}
