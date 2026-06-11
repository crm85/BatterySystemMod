using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using Newtonsoft.Json;
using SAIN.Attributes;
using SAIN.Components.PlayerComponentSpace;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.Preset.GlobalSettings;

public class PowerCalcSettings : SAINSettingsBase<PowerCalcSettings>, ISAINSettings
{
	[Name("PMC Power")]
	[Description("Add X points to a bot's power level if they are a PMC")]
	[Category("Bot Type Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float PMC_POWER = 20f;

	[Name("Scav Power")]
	[Description("Add X points to a bot's power level if they are a Scav")]
	[Category("Bot Type Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float SCAV_POWER = -20f;

	[Name("Shotgun Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float SHOTGUN_POWER = 40f;

	[Name("Smg Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float SMG_POWER = 75f;

	[Name("Assault Carbine Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float ASSAULT_CARBINE_POWER = 60f;

	[Name("Assault Rifle Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float ASSAULT_RIFLE_POWER = 45f;

	[Name("Machinegun Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float MG_POWER = 55f;

	[Name("Sniper Rifle Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float SNIPE_POWER = -30f;

	[Name("Marksman Rifle Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float MARKSMAN_RIFLE_POWER = 10f;

	[Name("Pistol Power")]
	[Description("Add X points to a bot's power level if they are using this type of weapon as their primary.")]
	[Category("Weapon Class Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float PISTOL_POWER = -10f;

	[Name("Red Dot / 1x Holo Sight Power")]
	[Description("Add X points to a bot's power level if they are using this type of attachment on their primary.")]
	[Category("Attachment Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float RED_DOT_POWER = 30f;

	[Name("Magnified Optic Power")]
	[Description("Add X points to a bot's power level if they are using this type of attachment on their primary.")]
	[Category("Attachment Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float OPTIC_POWER = -20f;

	[Name("Suppressor Power")]
	[Description("Add X points to a bot's power level if they are using this type of attachment on their primary.")]
	[Category("Attachment Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float SUPPRESSOR_POWER = 20f;

	[Name("Body Armor Class Power")]
	[Description("For each AC level, add X to a bot's power level. So if they have level 4 armor, add this value 4 times.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float ARMOR_CLASS_COEF = 30f;

	[Name("Body Armor Class Power - Realism Mod")]
	[Description("If Realism Mod is loaded, use this AC Power value. For each AC level, add X to a bot's power level. So if they have level 4 armor, add this value 4 times.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float ARMOR_CLASS_COEF_REALISM = 20f;

	[Name("Helmet Class Power")]
	[Description("If a bot has an armored helmet above class 1, but lower than 5, add X to thier power level.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float HELMET_POWER = 30f;

	[Name("Heavy Helmet Class Power")]
	[Description("If a bot has an armored helmet above class 4, add X to thier power level.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float HELMET_HEAVY_POWER = 60f;

	[Name("Faceshield Power")]
	[Description("If a bot has an armored face shield, add X to thier Power Level.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float FACESHIELD_POWER = 20f;

	[Name("Headphones Power")]
	[Description("If a bot has headphones, add X to thier Power Level.")]
	[Category("Armor Power Value")]
	[MinMax(-100f, 100f, 10f)]
	public float EARPRO_POWER = 20f;

	[JsonIgnore]
	private static readonly List<ArmorComponent> armorComponents = new List<ArmorComponent>();

	[JsonIgnore]
	private static readonly List<WildSpawnType> _PMCS = new List<WildSpawnType>
	{
		(WildSpawnType)52,
		(WildSpawnType)51
	};

	[JsonIgnore]
	private static readonly List<WildSpawnType> _SCAVS = new List<WildSpawnType>
	{
		(WildSpawnType)1,
		(WildSpawnType)10,
		(WildSpawnType)19,
		(WildSpawnType)37,
		(WildSpawnType)0
	};

	[JsonIgnore]
	private float ArmorClassCoef
	{
		get
		{
			if (ModDetection.RealismLoaded)
			{
				return ARMOR_CLASS_COEF_REALISM;
			}
			return ARMOR_CLASS_COEF;
		}
	}

	public override void Init(List<ISAINSettings> list)
	{
		list.Add(this);
	}

	public bool CalcPower(PlayerComponent playerComponent, out float power)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		power = 0f;
		if ((Object)(object)playerComponent == (Object)null)
		{
			return false;
		}
		power += WeaponPower(playerComponent);
		if (power == 0f)
		{
			return false;
		}
		power += RolePower(playerComponent.Player.Profile.Info.Settings.Role);
		power += ArmorPower(playerComponent.Player);
		IAIData aIData = playerComponent.Player.AIData;
		GClass567 val = (GClass567)(object)((aIData is GClass567) ? aIData : null);
		if (val != null)
		{
			val.PowerOfEquipment = power;
		}
		return true;
	}

	private float RolePower(WildSpawnType type)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_PMCS.Contains(type))
		{
			return PMC_POWER;
		}
		if (_SCAVS.Contains(type))
		{
			return SCAV_POWER;
		}
		return 0f;
	}

	private float WeaponPower(PlayerComponent player)
	{
		float num = 0f;
		WeaponInfo weaponInfo = player.Equipment.CurrentWeaponInfo ?? player.Equipment.WeaponInInventory;
		if (weaponInfo == null)
		{
			return 1f;
		}
		if (weaponInfo.HasSuppressor)
		{
			num += SUPPRESSOR_POWER;
		}
		if (weaponInfo.HasRedDot)
		{
			num += RED_DOT_POWER;
		}
		if (weaponInfo.HasOptic)
		{
			num += OPTIC_POWER;
		}
		switch (weaponInfo.WeaponClass)
		{
		case EWeaponClass.pistol:
			num += PISTOL_POWER;
			break;
		case EWeaponClass.smg:
			num += SMG_POWER;
			break;
		case EWeaponClass.assaultCarbine:
			num += ASSAULT_CARBINE_POWER;
			break;
		case EWeaponClass.assaultRifle:
			num += ASSAULT_RIFLE_POWER;
			break;
		case EWeaponClass.machinegun:
			num += MG_POWER;
			break;
		case EWeaponClass.marksmanRifle:
			num += MARKSMAN_RIFLE_POWER;
			break;
		case EWeaponClass.sniperRifle:
			num += SNIPE_POWER;
			break;
		case EWeaponClass.shotgun:
			num += SHOTGUN_POWER;
			break;
		}
		return num;
	}

	private float ArmorPower(Player player)
	{
		armorComponents.Clear();
		float num = 0f;
		InventoryEquipment val = player.Inventory?.Equipment;
		if (val != null)
		{
			Slot slot = val.GetSlot((EquipmentSlot)7);
			Item val2 = ((slot != null) ? slot.ContainedItem : null);
			if (val2 != null)
			{
				GClass3176.GetItemComponentsInChildrenNonAlloc<ArmorComponent>(val2, armorComponents, true);
				float num2 = FindHighestArmorClass(armorComponents);
				num += num2 * ArmorClassCoef;
				armorComponents.Clear();
			}
			else
			{
				Slot slot2 = val.GetSlot((EquipmentSlot)6);
				Item val3 = ((slot2 != null) ? slot2.ContainedItem : null);
				if (val3 != null)
				{
					GClass3176.GetItemComponentsInChildrenNonAlloc<ArmorComponent>(val3, armorComponents, true);
					if (armorComponents.Count > 0)
					{
						float num3 = FindHighestArmorClass(armorComponents);
						num += num3 * ArmorClassCoef;
						armorComponents.Clear();
					}
				}
			}
			Slot slot3 = val.GetSlot((EquipmentSlot)11);
			Item val4 = ((slot3 != null) ? slot3.ContainedItem : null);
			if (val4 != null)
			{
				GClass3176.GetItemComponentsInChildrenNonAlloc<ArmorComponent>(val4, armorComponents, true);
				if (armorComponents.Count > 0)
				{
					float num4 = FindHighestArmorClass(armorComponents);
					if (num4 > 4f)
					{
						num += HELMET_HEAVY_POWER;
					}
					else if (num4 > 1f)
					{
						num += HELMET_POWER;
					}
					armorComponents.Clear();
				}
			}
			Slot slot4 = val.GetSlot((EquipmentSlot)10);
			Item val5 = ((slot4 != null) ? slot4.ContainedItem : null);
			if (val5 != null)
			{
				GClass3176.GetItemComponentsInChildrenNonAlloc<ArmorComponent>(val5, armorComponents, true);
				if (armorComponents.Count > 0)
				{
					num += FACESHIELD_POWER;
				}
			}
			Slot slot5 = val.GetSlot((EquipmentSlot)12);
			Item val6 = ((slot5 != null) ? slot5.ContainedItem : null);
			if (val6 != null)
			{
				num += EARPRO_POWER;
			}
		}
		armorComponents.Clear();
		return num;
	}

	private float FindHighestArmorClass(List<ArmorComponent> armorComponents)
	{
		float num = 0f;
		foreach (ArmorComponent armorComponent in armorComponents)
		{
			float num2 = armorComponent.ArmorClass;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}
}
