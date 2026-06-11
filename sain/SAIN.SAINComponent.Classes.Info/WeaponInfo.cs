using System;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using SAIN.Helpers;
using SAIN.Plugin;
using SAIN.Preset;

namespace SAIN.SAINComponent.Classes.Info;

public class WeaponInfo
{
	private const float SuperSonicSpeed = 343.2f;

	private static readonly string SuppressorTypeId = "550aa4cd4bdc2dd8348b456c";

	private static readonly string CollimatorTypeId = "55818ad54bdc2ddc698b4569";

	private static readonly string CompactCollimatorTypeId = "55818acf4bdc2dde698b456b";

	private static readonly string AssaultScopeTypeId = "55818add4bdc2d5b648b456f";

	private static readonly string OpticScopeTypeId = "55818ae44bdc2dde698b456c";

	private static readonly string SpecialScopeTypeId = "55818aeb4bdc2ddc698b456a";

	private static readonly string[] OpticTypes = new string[3] { AssaultScopeTypeId, OpticScopeTypeId, SpecialScopeTypeId };

	private static readonly string[] RedDotTypes = new string[2] { CollimatorTypeId, CompactCollimatorTypeId };

	public EWeaponClass WeaponClass { get; private set; }

	public ECaliber AmmoCaliber { get; private set; }

	public float CalculatedAudibleRange { get; private set; }

	public AISoundType AISoundType => (AISoundType)(HasSuppressor ? 1 : 2);

	public SAINSoundType SoundType => HasSuppressor ? SAINSoundType.SuppressedShot : SAINSoundType.Shot;

	public bool HasRedDot => RedDot != null;

	public bool HasOptic => Optic != null;

	public bool HasSuppressor { get; private set; }

	public float BaseAudibleRange { get; private set; } = 150f;

	public float MuzzleLoudness { get; private set; }

	public bool Subsonic => Weapon != null && BulletSpeed < 343.2f;

	public Mod Suppressor { get; private set; }

	public Mod RedDot { get; private set; }

	public Mod Optic { get; private set; }

	public float BulletSpeed { get; private set; } = 600f;

	public float EngagementDistance { get; private set; } = 150f;

	public Weapon Weapon { get; private set; }

	public float Durability => Weapon.Repairable.Durability / (float)Weapon.Repairable.TemplateDurability;

	public WeaponInfo(Weapon weapon)
	{
		Weapon = weapon;
		WeaponClass = TryGetWeaponClass(weapon);
		AmmoCaliber = TryGetAmmoCaliber(weapon);
		updateSettings(SAINPresetClass.Instance);
		PresetHandler.OnPresetUpdated += updateSettings;
	}

	private void updateSettings(SAINPresetClass preset)
	{
		if (preset.GlobalSettings.Shoot.EngagementDistance.TryGetValue(WeaponClass, out var value))
		{
			EngagementDistance = value;
		}
		if (preset.GlobalSettings.Hearing.HearingDistances.TryGetValue(AmmoCaliber, out var value2))
		{
			BaseAudibleRange = value2;
		}
	}

	public void Update(Player Player)
	{
		if (Weapon != null)
		{
			UpdateWeaponData(Player, Weapon.Mods);
		}
	}

	private void UpdateWeaponData(Player Player, IEnumerable<Mod> mods)
	{
		Suppressor = FindModType(mods, EModType.Suppressor);
		RedDot = FindModType(mods, EModType.RedDot);
		Optic = FindModType(mods, EModType.Optic);
		MuzzleLoudness = ((Suppressor != null) ? ((float)Suppressor.Template.Loudness) : 0f);
		BulletSpeed = Weapon.CurrentAmmoTemplate.InitialSpeed * Weapon.SpeedFactor;
		CalculatedAudibleRange = BaseAudibleRange + MuzzleLoudness;
		if (Suppressor == null)
		{
			AbstractHandsController handsController = Player.HandsController;
			FirearmController val = (FirearmController)(object)((handsController is FirearmController) ? handsController : null);
			if (val == null || !val.IsSilenced)
			{
				HasSuppressor = false;
				return;
			}
		}
		CalculatedAudibleRange *= SuppressorModifier(BulletSpeed);
		HasSuppressor = true;
	}

	public void WeaponEquiped(Player player)
	{
		Update(player);
	}

	public void WeaponModified(Player player)
	{
		Update(player);
	}

	public void Dispose()
	{
		PresetHandler.OnPresetUpdated -= updateSettings;
	}

	private static Mod FindModType(IEnumerable<Mod> mods, EModType ModType)
	{
		if (mods != null)
		{
			foreach (Mod mod in mods)
			{
				if (CheckItemType(((object)mod).GetType()) == ModType)
				{
					return mod;
				}
				Slot[] slots = ((CompoundItem)mod).Slots;
				Slot[] array = slots;
				foreach (Slot val in array)
				{
					Item containedItem = val.ContainedItem;
					Mod val2 = (Mod)(object)((containedItem is Mod) ? containedItem : null);
					if (val2 != null && CheckItemType(((object)val2).GetType()) == ModType)
					{
						return val2;
					}
				}
			}
		}
		return null;
	}

	private static float SuppressorModifier(float bulletspeed)
	{
		if (bulletspeed < 343.2f)
		{
			return SAINPlugin.LoadedPreset.GlobalSettings.Hearing.SubsonicModifier;
		}
		return SAINPlugin.LoadedPreset.GlobalSettings.Hearing.SuppressorModifier;
	}

	private static EModType CheckItemType(Type type)
	{
		if (CheckTemplateType(type, SuppressorTypeId))
		{
			return EModType.Suppressor;
		}
		for (int i = 0; i < RedDotTypes.Length; i++)
		{
			if (CheckTemplateType(type, RedDotTypes[i]))
			{
				return EModType.RedDot;
			}
		}
		for (int j = 0; j < OpticTypes.Length; j++)
		{
			if (CheckTemplateType(type, OpticTypes[j]))
			{
				return EModType.Optic;
			}
		}
		return EModType.None;
	}

	private static bool CheckTemplateType(Type modType, string id)
	{
		if (TemplateIdToObjectMappingsClass.TypeTable.TryGetValue(id, out var value) && value == modType)
		{
			return true;
		}
		if (TemplateIdToObjectMappingsClass.TemplateTypeTable.TryGetValue(id, out value) && value == modType)
		{
			return true;
		}
		return false;
	}

	private static EWeaponClass TryGetWeaponClass(Weapon weapon)
	{
		EWeaponClass eWeaponClass = EnumValues.TryParse<EWeaponClass>(weapon.Template.weapClass);
		if (eWeaponClass == EWeaponClass.Default)
		{
			eWeaponClass = EnumValues.TryParse<EWeaponClass>(weapon.WeapClass);
		}
		return eWeaponClass;
	}

	private static ECaliber TryGetAmmoCaliber(Weapon weapon)
	{
		ECaliber eCaliber = EnumValues.TryParse<ECaliber>(weapon.Template.ammoCaliber);
		if (eCaliber == ECaliber.Default)
		{
			eCaliber = EnumValues.TryParse<ECaliber>(weapon.AmmoCaliber);
		}
		return eCaliber;
	}

	private void Log()
	{
		Logger.LogDebug("Found Weapon Info: Weapon: [" + ((Item)Weapon).ShortName + "] " + $"Weapon Class: [{WeaponClass}] " + $"Ammo Caliber: [{AmmoCaliber}] " + $"Calculated Audible Range: [{CalculatedAudibleRange}] " + $"Base Audible Range: [{BaseAudibleRange}] " + $"Muzzle Loudness: [{MuzzleLoudness}] " + $"Speed Factor: [{Weapon.SpeedFactor}] " + $"Subsonic: [{Subsonic}] " + $"Has Red Dot? [{HasRedDot}] " + $"Has Optic? [{HasOptic}] " + $"Has Suppressor? [{HasSuppressor}]");
	}
}
