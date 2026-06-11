using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Attributes;
using SAIN.Components.BotController;
using SAIN.Helpers;
using SAIN.Preset.BotSettings.SAINSettings;

namespace SAIN.Preset.BotSettings;

public class SAINBotSettingsClass : BasePreset
{
	private static List<WildSpawnType> _enemyTypeList;

	public Dictionary<WildSpawnType, SAINSettingsGroupClass> SAINSettings = new Dictionary<WildSpawnType, SAINSettingsGroupClass>();

	public Dictionary<WildSpawnType, EFTBotSettings> EFTSettings = new Dictionary<WildSpawnType, EFTBotSettings>();

	public static readonly Dictionary<WildSpawnType, float> DefaultDifficultyModifier;

	public SAINBotSettingsClass(SAINPresetClass preset)
		: base(preset)
	{
		LoadEFTSettings();
		LoadSAINSettings();
	}

	public void Update()
	{
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in SAINSettings)
		{
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Update();
			}
		}
	}

	public void Init()
	{
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in SAINSettings)
		{
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				setting.Value.Init();
			}
		}
	}

	public void UpdateDefaults(SAINBotSettingsClass replacement)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<WildSpawnType, SAINSettingsGroupClass> sAINSetting in SAINSettings)
		{
			SAINSettingsGroupClass sAINSettingsGroupClass = replacement?.SAINSettings[sAINSetting.Key];
			foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sAINSetting.Value.Settings)
			{
				SAINSettingsClass replacementGroup = sAINSettingsGroupClass?.Settings[setting.Key];
				setting.Value.UpdateDefaults(replacementGroup);
			}
		}
	}

	private void LoadSAINSettings()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		BotDifficulty[] difficulties = EnumValues.Difficulties;
		foreach (BotType botTypes in BotTypeDefinitions.BotTypesList)
		{
			string name = botTypes.Name;
			WildSpawnType wildSpawnType = botTypes.WildSpawnType;
			if (BotSpawnController.StrictExclusionList.Contains(wildSpawnType))
			{
			}
			if (!Preset.Info.IsCustom || !SAINPresetClass.Import<SAINSettingsGroupClass>(out var result, Preset.Info.Name, name, "BotSettings"))
			{
				result = new SAINSettingsGroupClass(difficulties)
				{
					Name = name,
					WildSpawnType = wildSpawnType,
					DifficultyModifier = DefaultDifficultyModifier[wildSpawnType]
				};
				UpdateSAINSettingsToEFTDefault(wildSpawnType, result);
				if (Preset.Info.IsCustom)
				{
					SAINPresetClass.Export(result, Preset.Info.Name, name, "BotSettings");
				}
			}
			SAINSettings.Add(wildSpawnType, result);
		}
	}

	private void UpdateSAINSettingsToEFTDefault(WildSpawnType wildSpawnType, SAINSettingsGroupClass sainSettingsGroup)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<BotDifficulty, SAINSettingsClass> setting in sainSettingsGroup.Settings)
		{
			SAINSettingsClass value = setting.Value;
			BotDifficulty key = setting.Key;
			object eFTSettings = GetEFTSettings(wildSpawnType, key);
			if (eFTSettings != null)
			{
				CopyValuesAtoB(eFTSettings, value, (FieldInfo field) => ShallUseEFTBotDefault(field));
			}
		}
	}

	private void CopyValuesAtoB(object A, object B, Func<FieldInfo, bool> shouldCopyFieldFunc = null)
	{
		List<string> fieldNames = AccessTools.GetFieldNames(A);
		FieldInfo[] fieldsInType = Reflection.GetFieldsInType(B.GetType());
		foreach (FieldInfo fieldInfo in fieldsInType)
		{
			if (!fieldNames.Contains(fieldInfo.Name))
			{
				continue;
			}
			object value = fieldInfo.GetValue(B);
			FieldInfo[] fieldsInType2 = Reflection.GetFieldsInType(fieldInfo.FieldType);
			FieldInfo fieldInfo2 = AccessTools.Field(A.GetType(), fieldInfo.Name);
			if (!(fieldInfo2 != null))
			{
				continue;
			}
			object value2 = fieldInfo2.GetValue(A);
			List<string> fieldNames2 = AccessTools.GetFieldNames(value2);
			FieldInfo[] array = fieldsInType2;
			foreach (FieldInfo fieldInfo3 in array)
			{
				if (fieldNames2.Contains(fieldInfo3.Name) && (shouldCopyFieldFunc == null || shouldCopyFieldFunc(fieldInfo3)))
				{
					FieldInfo fieldInfo4 = AccessTools.Field(value2.GetType(), fieldInfo3.Name);
					if (fieldInfo4 != null)
					{
						object value3 = fieldInfo4.GetValue(value2);
						fieldInfo3.SetValue(value, value3);
					}
				}
			}
		}
	}

	private bool ShallUseEFTBotDefault(FieldInfo field)
	{
		return AttributesGUI.GetAttributeInfo(field)?.CopyValue ?? false;
	}

	public void LoadEFTSettings()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between Unknown and I4
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		BotDifficulty[] difficulties = EnumValues.Difficulties;
		foreach (BotType botTypes in BotTypeDefinitions.BotTypesList)
		{
			string name = botTypes.Name;
			WildSpawnType wildSpawnType = botTypes.WildSpawnType;
			if (EFTSettings.ContainsKey(wildSpawnType))
			{
				continue;
			}
			if (!JsonUtility.Load.LoadObject<EFTBotSettings>(out var obj, name, "Default Bot Config Values"))
			{
				Logger.LogError("Failed to Import EFT Bot Settings for " + name);
				obj = new EFTBotSettings(name, wildSpawnType, difficulties);
				JsonUtility.SaveObjectToJson(obj, name, "Default Bot Config Values");
			}
			if ((int)wildSpawnType != 46 && (int)wildSpawnType != 29 && (int)wildSpawnType != 30)
			{
				foreach (KeyValuePair<BotDifficulty, BotSettingsComponents> setting in obj.Settings)
				{
				}
			}
			EFTSettings.Add(wildSpawnType, obj);
		}
	}

	public SAINSettingsClass GetSAINSettings(WildSpawnType type, BotDifficulty difficulty)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		LoadEFTSettings();
		if (SAINSettings.TryGetValue(type, out var value))
		{
			if (value.Settings.TryGetValue(difficulty, out var value2))
			{
				return value2;
			}
			Logger.LogError($"[{difficulty}] does not exist in [{type}] SAIN Settings!");
		}
		else
		{
			Logger.LogError($"[{type}] does not exist in SAINSettings Dictionary!");
		}
		return SAINSettings[(WildSpawnType)52].Settings[(BotDifficulty)1];
	}

	public object GetEFTSettings(WildSpawnType type, BotDifficulty difficulty)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		LoadEFTSettings();
		if (EFTSettings.TryGetValue(type, out var value))
		{
			if (value.Settings.TryGetValue(difficulty, out var value2))
			{
				return value2;
			}
			Logger.LogError($"[{difficulty}] does not exist in [{type}] Settings Group!");
		}
		else
		{
			Logger.LogError($"[{type}] does not exist in EFTSettings Dictionary!");
		}
		return EFTSettings[(WildSpawnType)52].Settings[(BotDifficulty)1];
	}

	static SAINBotSettingsClass()
	{
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		_enemyTypeList = new List<WildSpawnType>();
		DefaultDifficultyModifier = new Dictionary<WildSpawnType, float>
		{
			{
				(WildSpawnType)1,
				0.3f
			},
			{
				(WildSpawnType)0,
				0.3f
			},
			{
				(WildSpawnType)37,
				0.35f
			},
			{
				(WildSpawnType)10,
				0.35f
			},
			{
				(WildSpawnType)19,
				0.35f
			},
			{
				(WildSpawnType)3,
				0.75f
			},
			{
				(WildSpawnType)32,
				0.75f
			},
			{
				(WildSpawnType)11,
				0.75f
			},
			{
				(WildSpawnType)6,
				0.75f
			},
			{
				(WildSpawnType)17,
				0.75f
			},
			{
				(WildSpawnType)7,
				0.75f
			},
			{
				(WildSpawnType)29,
				0.75f
			},
			{
				(WildSpawnType)21,
				0.75f
			},
			{
				(WildSpawnType)47,
				0.75f
			},
			{
				(WildSpawnType)26,
				1f
			},
			{
				(WildSpawnType)20,
				0.7f
			},
			{
				(WildSpawnType)5,
				0.55f
			},
			{
				(WildSpawnType)12,
				0.55f
			},
			{
				(WildSpawnType)14,
				0.55f
			},
			{
				(WildSpawnType)13,
				0.55f
			},
			{
				(WildSpawnType)15,
				0.55f
			},
			{
				(WildSpawnType)8,
				0.55f
			},
			{
				(WildSpawnType)16,
				0.55f
			},
			{
				(WildSpawnType)23,
				0.55f
			},
			{
				(WildSpawnType)30,
				0.55f
			},
			{
				(WildSpawnType)33,
				0.55f
			},
			{
				(WildSpawnType)41,
				0.55f
			},
			{
				(WildSpawnType)42,
				0.55f
			},
			{
				(WildSpawnType)36,
				0.55f
			},
			{
				(WildSpawnType)44,
				0.55f
			},
			{
				(WildSpawnType)45,
				0.55f
			},
			{
				(WildSpawnType)27,
				1f
			},
			{
				(WildSpawnType)28,
				1f
			},
			{
				(WildSpawnType)9,
				0.66f
			},
			{
				(WildSpawnType)24,
				0.66f
			},
			{
				(WildSpawnType)34,
				0.66f
			},
			{
				(WildSpawnType)35,
				0.66f
			},
			{
				(WildSpawnType)52,
				1f
			},
			{
				(WildSpawnType)51,
				1f
			}
		};
		foreach (WildSpawnType key in BotTypeDefinitions.BotTypes.Keys)
		{
			if (!DefaultDifficultyModifier.ContainsKey(key))
			{
				DefaultDifficultyModifier.Add(key, 0.5f);
			}
		}
	}
}
