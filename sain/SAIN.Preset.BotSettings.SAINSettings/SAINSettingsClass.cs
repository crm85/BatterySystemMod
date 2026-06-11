using SAIN.Preset.BotSettings.SAINSettings.Categories;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.Personalities;

namespace SAIN.Preset.BotSettings.SAINSettings;

public class SAINSettingsClass : SettingsGroupBase<SAINSettingsClass>
{
	public DifficultySettings Difficulty = new DifficultySettings();

	public SAINCoreSettings Core = new SAINCoreSettings();

	public SAINAimingSettings Aiming = new SAINAimingSettings();

	public SAINBossSettings Boss = new SAINBossSettings();

	public SAINChangeSettings Change = new SAINChangeSettings();

	public SAINGrenadeSettings Grenade = new SAINGrenadeSettings();

	public SAINHearingSettings Hearing = new SAINHearingSettings();

	public SAINLaySettings Lay = new SAINLaySettings();

	public SAINLookSettings Look = new SAINLookSettings();

	public SAINMindSettings Mind = new SAINMindSettings();

	public SAINMoveSettings Move = new SAINMoveSettings();

	public SAINPatrolSettings Patrol = new SAINPatrolSettings();

	public SAINScatterSettings Scattering = new SAINScatterSettings();

	public SAINShootSettings Shoot = new SAINShootSettings();

	public override void InitList()
	{
		base.SettingsList.Clear();
		base.SettingsList.Add(Difficulty);
		base.SettingsList.Add(Core);
		base.SettingsList.Add(Aiming);
		base.SettingsList.Add(Boss);
		base.SettingsList.Add(Change);
		base.SettingsList.Add(Grenade);
		base.SettingsList.Add(Hearing);
		base.SettingsList.Add(Lay);
		base.SettingsList.Add(Look);
		base.SettingsList.Add(Mind);
		base.SettingsList.Add(Patrol);
		base.SettingsList.Add(Scattering);
		base.SettingsList.Add(Shoot);
	}
}
