using System;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using SAIN.Preset.GearStealthValues;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.Classes.Equipment;

public class AIGearModifierClass : AIDataBase
{
	private static float _nextLogTime;

	private float _gearStealthModifier = 1f;

	private float _calcGearTime;

	private const string backpack_pilgrim = "59e763f286f7742ee57895da";

	private const string backpack_raid = "5df8a4d786f77412672a1e3b";

	private const string boonie_MILTEC = "5b4327aa5acfc400175496e0";

	private const string boonie_CHIMERA = "60b52e5bc7d8103275739d67";

	private const string boonie_DOORKICKER = "5d96141523f0ea1b7f2aacab";

	private const string boonie_JACK_PYKE = "618aef6d0a5a59657e5f55ee";

	private const string helmet_TAN_ULACH = "5b40e2bc5acfc40016388216";

	private const string helmet_UNTAR_BLUE = "5aa7d03ae5b5b00016327db5";

	private float gearStealthModifier
	{
		get
		{
			if (_calcGearTime < Time.time)
			{
				_calcGearTime = Time.time + 1f;
				float num = 1f;
				bool flag = false;
				try
				{
					num = calcGearEffects();
					flag = true;
				}
				catch (Exception data)
				{
					if (SAINPlugin.DebugMode)
					{
						Logger.LogError(data);
					}
				}
				if (flag)
				{
					if (_nextLogTime < Time.time && num != 1f)
					{
						_nextLogTime = Time.time + 60f;
					}
					_gearStealthModifier = num;
				}
				else
				{
					float backpackMod = getBackpackMod();
					float headWearMod = getHeadWearMod();
					float faceCoverMod = getFaceCoverMod();
					_gearStealthModifier = backpackMod * headWearMod * faceCoverMod;
				}
			}
			return _gearStealthModifier;
		}
	}

	private Item _backpack => base.GearInfo.GetItem((EquipmentSlot)4);

	private Item _headwear => base.GearInfo.GetItem((EquipmentSlot)11);

	private Item _facecover => base.GearInfo.GetItem((EquipmentSlot)10);

	public AIGearModifierClass(SAINAIData sAINAIData)
		: base(sAINAIData)
	{
	}

	public float StealthModifier(float distance)
	{
		return getSightMod(distance);
	}

	private float calcGearEffects()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<EEquipmentType, List<ItemStealthValue>> itemStealthValues = SAINPlugin.LoadedPreset.GearStealthValuesClass.ItemStealthValues;
		float num = 1f;
		foreach (KeyValuePair<EEquipmentType, List<ItemStealthValue>> item2 in itemStealthValues)
		{
			if (item2.Value.Count == 0)
			{
				continue;
			}
			switch (item2.Key)
			{
			case EEquipmentType.Headwear:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)11);
				if (item != null)
				{
					num *= calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value);
				}
				break;
			}
			case EEquipmentType.BackPack:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)4);
				num = ((item == null) ? (num * 1.1f) : (num * calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value)));
				break;
			}
			case EEquipmentType.FaceCover:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)10);
				if (item != null)
				{
					float num2 = calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value);
					if (num2 == 1f)
					{
						num *= 1.05f;
					}
				}
				break;
			}
			case EEquipmentType.Rig:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)6);
				if (item != null)
				{
					num *= calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value);
				}
				break;
			}
			case EEquipmentType.ArmorVest:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)7);
				if (item != null)
				{
					num *= calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value);
				}
				break;
			}
			case EEquipmentType.EyeWear:
			{
				Item item = base.GearInfo.GetItem((EquipmentSlot)9);
				if (item != null)
				{
					num *= calcEffect(MongoID.op_Implicit(item.TemplateId), item2.Value);
				}
				break;
			}
			}
		}
		return num;
	}

	private float calcEffect(string id, List<ItemStealthValue> values)
	{
		foreach (ItemStealthValue value in values)
		{
			if (value.ItemID == id)
			{
				return value.StealthValue;
			}
		}
		return 1f;
	}

	private float getSightMod(float distance)
	{
		float num = 30f;
		float num2 = 60f;
		if (distance <= num)
		{
			return 1f;
		}
		float num3 = gearStealthModifier;
		if (distance >= num2)
		{
			return num3;
		}
		float num4 = num2 - num;
		float num5 = distance - num;
		float num6 = num5 / num4;
		return Mathf.Lerp(1f, num3, num6);
	}

	private float getBackpackMod()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Item item = base.GearInfo.GetItem((EquipmentSlot)4);
		if (item == null)
		{
			return 1.15f;
		}
		string text = MongoID.op_Implicit(item.TemplateId);
		string text2 = text;
		if (!(text2 == "59e763f286f7742ee57895da"))
		{
			if (text2 == "5df8a4d786f77412672a1e3b")
			{
				return 0.925f;
			}
			return 1f;
		}
		return 0.875f;
	}

	private float getHeadWearMod()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Item item = base.GearInfo.GetItem((EquipmentSlot)11);
		if (item == null)
		{
			return 1f;
		}
		return MongoID.op_Implicit(item.TemplateId) switch
		{
			"5b4327aa5acfc400175496e0" => 1.2f, 
			"60b52e5bc7d8103275739d67" => 1.2f, 
			"5d96141523f0ea1b7f2aacab" => 1.2f, 
			"618aef6d0a5a59657e5f55ee" => 1.2f, 
			"5b40e2bc5acfc40016388216" => 0.925f, 
			"5aa7d03ae5b5b00016327db5" => 0.9f, 
			_ => 1f, 
		};
	}

	private float getFaceCoverMod()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Item item = base.GearInfo.GetItem((EquipmentSlot)10);
		if (item == null)
		{
			return 1f;
		}
		string text = MongoID.op_Implicit(item.TemplateId);
		string text2 = text;
		return 1.05f;
	}
}
