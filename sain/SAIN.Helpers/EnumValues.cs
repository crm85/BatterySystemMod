using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EFT;

namespace SAIN.Helpers;

internal class EnumValues
{
	internal class WildSpawn
	{
		public static WildSpawnType[] Scavs;

		public static WildSpawnType[] Goons;

		public static WildSpawnType[] Cultists;

		public static List<WildSpawnType> Bosses;

		public static List<WildSpawnType> Followers;

		static WildSpawn()
		{
			WildSpawnType[] array = new WildSpawnType[5];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			Scavs = (WildSpawnType[])(object)array;
			WildSpawnType[] array2 = new WildSpawnType[3];
			RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			Goons = (WildSpawnType[])(object)array2;
			WildSpawnType[] array3 = new WildSpawnType[3];
			RuntimeHelpers.InitializeArray(array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			Cultists = (WildSpawnType[])(object)array3;
			Bosses = CheckAdd("boss");
			Followers = CheckAdd("follower");
		}

		private unsafe static List<WildSpawnType> CheckAdd(string search)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			List<WildSpawnType> list = new List<WildSpawnType>();
			WildSpawnType[] array = GetEnum<WildSpawnType>();
			for (int i = 0; i < array.Length; i++)
			{
				WildSpawnType item = array[i];
				if (((object)(*(WildSpawnType*)(&item))/*cast due to .constrained prefix*/).ToString().ToLower().StartsWith(search))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static bool IsFollower(WildSpawnType type)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Followers.Contains(type);
		}

		public static bool IsBoss(WildSpawnType type)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Bosses.Contains(type);
		}

		public static bool IsScav(WildSpawnType type)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Scavs.Contains(type);
		}

		public static bool IsPMC(WildSpawnType type)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			return (int)type == 52 || (int)type == 51;
		}

		public static bool IsGoons(WildSpawnType type)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Goons.Contains(type);
		}

		public static bool IsCultist(WildSpawnType type)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Cultists.Contains(type);
		}
	}

	public static readonly BotDifficulty[] Difficulties;

	public static readonly WildSpawnType[] WildSpawnTypes;

	public static readonly ECaliber[] AmmoCalibers;

	public static readonly EWeaponClass[] WeaponClasses;

	public static readonly EPersonality[] Personalities;

	public static readonly ECombatDecision[] SoloDecisions;

	public static readonly ESquadDecision[] SquadDecisions;

	public static readonly ESelfDecision[] SelfDecisions;

	public static T Parse<T>(string value)
	{
		return (T)Enum.Parse(typeof(T), value);
	}

	public static ECaliber ParseCaliber(string caliber)
	{
		if (Enum.TryParse<ECaliber>(caliber, out var result))
		{
			return result;
		}
		Logger.LogError("Caliber [" + caliber + "] does not exist in Caliber Enum!");
		return ECaliber.Default;
	}

	public static EWeaponClass ParseWeaponClass(string weaponClass)
	{
		if (Enum.TryParse<EWeaponClass>(weaponClass, out var result))
		{
			return result;
		}
		Logger.LogError("Weapon Class [" + weaponClass + "] does not exist in IWeaponClass Enum!");
		return EWeaponClass.Default;
	}

	public static T TryParse<T>(string _string) where T : struct, Enum
	{
		if (Enum.TryParse<T>(_string, out var result))
		{
			return result;
		}
		Logger.LogError($"[{_string}] does not exist in [{typeof(T)}] Enum!");
		return default(T);
	}

	public static T[] GetEnum<T>()
	{
		return (T[])Enum.GetValues(typeof(T));
	}

	static EnumValues()
	{
		BotDifficulty[] array = new BotDifficulty[4];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Difficulties = (BotDifficulty[])(object)array;
		WildSpawnTypes = GetEnum<WildSpawnType>();
		AmmoCalibers = GetEnum<ECaliber>();
		WeaponClasses = GetEnum<EWeaponClass>();
		Personalities = GetEnum<EPersonality>();
		SoloDecisions = GetEnum<ECombatDecision>();
		SquadDecisions = GetEnum<ESquadDecision>();
		SelfDecisions = GetEnum<ESelfDecision>();
	}
}
