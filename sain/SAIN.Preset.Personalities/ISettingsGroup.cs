using System.Collections.Generic;
using SAIN.Preset.GlobalSettings;

namespace SAIN.Preset.Personalities;

public interface ISettingsGroup
{
	List<ISAINSettings> SettingsList { get; }

	void Init();

	void Update();

	void InitList();

	void CreateDefaults();

	void UpdateDefaults(ISettingsGroup replacementValues = null);
}
