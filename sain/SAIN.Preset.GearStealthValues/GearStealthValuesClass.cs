using System;
using System.Collections.Generic;
using SAIN.Helpers;

namespace SAIN.Preset.GearStealthValues;

public class GearStealthValuesClass
{
	public Dictionary<EEquipmentType, List<ItemStealthValue>> ItemStealthValues = new Dictionary<EEquipmentType, List<ItemStealthValue>>();

	public readonly List<ItemStealthValue> Defaults = new List<ItemStealthValue>();

	private const string backpack_pilgrim = "59e763f286f7742ee57895da";

	private const string backpack_raid = "5df8a4d786f77412672a1e3b";

	private const string boonie_MILTEC = "5b4327aa5acfc400175496e0";

	private const string boonie_CHIMERA = "60b52e5bc7d8103275739d67";

	private const string boonie_DOORKICKER = "5d96141523f0ea1b7f2aacab";

	private const string boonie_JACK_PYKE = "618aef6d0a5a59657e5f55ee";

	private const string helmet_TAN_ULACH = "5b40e2bc5acfc40016388216";

	private const string helmet_UNTAR_BLUE = "5aa7d03ae5b5b00016327db5";

	public GearStealthValuesClass(SAINPresetDefinition preset)
	{
		try
		{
			import(preset);
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
		initDefaults();
		Export(this, preset);
	}

	private void import(SAINPresetDefinition preset)
	{
		if (!preset.IsCustom || !JsonUtility.DoesFolderExist("Presets", preset.Name, "ItemStealthValues"))
		{
			return;
		}
		List<ItemStealthValue> list = new List<ItemStealthValue>();
		JsonUtility.Load.LoadStealthValues(list, "Presets", preset.Name, "ItemStealthValues");
		EEquipmentType[] array = EnumValues.GetEnum<EEquipmentType>();
		foreach (EEquipmentType eEquipmentType in array)
		{
			List<ItemStealthValue> list2 = getList(eEquipmentType);
			foreach (ItemStealthValue item in list)
			{
				if (item.EquipmentType == eEquipmentType)
				{
					Logger.LogDebug("Adding " + item.Name);
					addItem(item.Name, item.EquipmentType, item.ItemID, item.StealthValue, list2);
				}
			}
		}
	}

	public static void Export(GearStealthValuesClass stealthValues, SAINPresetDefinition preset)
	{
		if (!preset.IsCustom)
		{
			return;
		}
		JsonUtility.CreateFolder("Presets", preset.Name, "ItemStealthValues");
		JsonUtility.SaveObjectToJson(EnumValues.GetEnum<EEquipmentType>(), "Possible Item Types For Stealth Modifiers", "Presets", preset.Name);
		foreach (List<ItemStealthValue> value in stealthValues.ItemStealthValues.Values)
		{
			foreach (ItemStealthValue item in value)
			{
				JsonUtility.SaveObjectToJson(item, item.Name, "Presets", preset.Name, "ItemStealthValues");
			}
		}
	}

	private void initDefaults()
	{
		List<ItemStealthValue> list = getList(EEquipmentType.Headwear);
		addItem("MILTEC", EEquipmentType.Headwear, "5b4327aa5acfc400175496e0", 1.2f, list, addAsDefault: true);
		addItem("CHIMERA", EEquipmentType.Headwear, "60b52e5bc7d8103275739d67", 1.2f, list, addAsDefault: true);
		addItem("DOORKICKER", EEquipmentType.Headwear, "5d96141523f0ea1b7f2aacab", 1.2f, list, addAsDefault: true);
		addItem("JACK_PYKE", EEquipmentType.Headwear, "618aef6d0a5a59657e5f55ee", 1.2f, list, addAsDefault: true);
		addItem("TAN_ULACH", EEquipmentType.Headwear, "5b40e2bc5acfc40016388216", 0.9f, list, addAsDefault: true);
		addItem("UNTAR_BLUE", EEquipmentType.Headwear, "5aa7d03ae5b5b00016327db5", 0.85f, list, addAsDefault: true);
		List<ItemStealthValue> list2 = getList(EEquipmentType.BackPack);
		addItem("Pilgrim", EEquipmentType.BackPack, "59e763f286f7742ee57895da", 0.85f, list2, addAsDefault: true);
		addItem("Raid", EEquipmentType.BackPack, "5df8a4d786f77412672a1e3b", 0.875f, list2, addAsDefault: true);
	}

	private List<ItemStealthValue> getList(EEquipmentType type)
	{
		if (!ItemStealthValues.TryGetValue(type, out var value))
		{
			value = new List<ItemStealthValue>();
			ItemStealthValues.Add(type, value);
		}
		return value;
	}

	private void addItem(string name, EEquipmentType type, string id, float stealthValue, List<ItemStealthValue> list, bool addAsDefault = false)
	{
		if (!doesItemExist(name, list))
		{
			list.Add(new ItemStealthValue
			{
				Name = name,
				EquipmentType = type,
				ItemID = id,
				StealthValue = stealthValue
			});
		}
		if (addAsDefault)
		{
			addItem(name, type, id, stealthValue, Defaults);
		}
	}

	private bool doesItemExist(string name, List<ItemStealthValue> list)
	{
		foreach (ItemStealthValue item in list)
		{
			if (item.Name == name)
			{
				return true;
			}
		}
		return false;
	}
}
