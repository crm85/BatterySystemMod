namespace SAIN.Preset.Personalities;

public class PersonalityBehaviorSettings : SettingsGroupBase<PersonalityBehaviorSettings>, ISettingsGroup
{
	public PersonalityGeneralSettings General = new PersonalityGeneralSettings();

	public PersonalitySearchSettings Search = new PersonalitySearchSettings();

	public PersonalityRushSettings Rush = new PersonalityRushSettings();

	public PersonalityCoverSettings Cover = new PersonalityCoverSettings();

	public PersonalityTalkSettings Talk = new PersonalityTalkSettings();

	public override void InitList()
	{
		base.SettingsList.Clear();
		base.SettingsList.Add(Cover);
		base.SettingsList.Add(General);
		base.SettingsList.Add(Rush);
		base.SettingsList.Add(Search);
		base.SettingsList.Add(Talk);
	}
}
