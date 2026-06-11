using Newtonsoft.Json;
using SAIN.Attributes;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings.Categories;
using SAIN.Preset.Personalities;

namespace SAIN.Preset.GlobalSettings;

public class GlobalSettingsClass : SettingsGroupBase<GlobalSettingsClass>
{
	[Hidden]
	[JsonIgnore]
	public static GlobalSettingsClass Instance;

	public DifficultySettings Difficulty = new DifficultySettings();

	public GeneralSettings General = new GeneralSettings();

	public AimSettings Aiming = new AimSettings();

	public HearingSettings Hearing = new HearingSettings();

	public LocationSettingsClass Location = new LocationSettingsClass();

	public LookSettings Look = new LookSettings();

	public MindSettings Mind = new MindSettings();

	public MoveSettings Move = new MoveSettings();

	public SteeringSettings Steering = new SteeringSettings();

	public ShootSettings Shoot = new ShootSettings();

	public TalkSettings Talk = new TalkSettings();

	[Name("Squad Talk")]
	public SquadTalkSettings SquadTalk = new SquadTalkSettings();

	[Name("Power Level Calculation")]
	public PowerCalcSettings PowerCalc = new PowerCalcSettings();

	public GlobalSettingsClass()
	{
		Instance = this;
	}

	public static GlobalSettingsClass ImportGlobalSettings(SAINPresetDefinition Preset)
	{
		string fileName = JsonUtility.FileAndFolderNames[JsonEnum.GlobalSettings];
		string text = JsonUtility.FileAndFolderNames[JsonEnum.Presets];
		if (!JsonUtility.Load.LoadObject<GlobalSettingsClass>(out var obj, fileName, text, Preset.Name))
		{
			obj = new GlobalSettingsClass();
			JsonUtility.SaveObjectToJson(obj, fileName, text, Preset.Name);
		}
		return obj;
	}

	public override void Init()
	{
		InitList();
		CreateDefaults();
		Update();
	}

	public override void InitList()
	{
		base.SettingsList.Clear();
		Difficulty.Init(base.SettingsList);
		General.Init(base.SettingsList);
		Aiming.Init(base.SettingsList);
		Hearing.Init(base.SettingsList);
		Location.Init(base.SettingsList);
		Look.Init(base.SettingsList);
		Mind.Init(base.SettingsList);
		Move.Init(base.SettingsList);
		Shoot.Init(base.SettingsList);
		Talk.Init(base.SettingsList);
		SquadTalk.Init(base.SettingsList);
		PowerCalc.Init(base.SettingsList);
		Steering.Init(base.SettingsList);
	}
}
