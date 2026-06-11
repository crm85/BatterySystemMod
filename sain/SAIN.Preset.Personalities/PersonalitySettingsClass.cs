using Newtonsoft.Json;
using SAIN.Preset.GlobalSettings;

namespace SAIN.Preset.Personalities;

public class PersonalitySettingsClass : SettingsGroupBase<PersonalitySettingsClass>
{
	public string Name;

	public string Description;

	public PersonalityAssignmentSettings Assignment = new PersonalityAssignmentSettings();

	public PersonalityBehaviorSettings Behavior = new PersonalityBehaviorSettings();

	public DifficultySettings Difficulty = new DifficultySettings();

	[JsonConstructor]
	public PersonalitySettingsClass()
	{
	}

	public PersonalitySettingsClass(EPersonality personality)
	{
		Name = personality.ToString();
		Description = PersonalityDescriptionsClass.PersonalityDescriptions[personality];
	}

	public override void Init()
	{
		InitList();
		CreateDefaults();
		Behavior.Init();
		Update();
	}

	public override void InitList()
	{
		base.SettingsList.Clear();
		base.SettingsList.Add(Assignment);
		base.SettingsList.Add(Difficulty);
	}
}
