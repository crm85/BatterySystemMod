using System.Collections.Generic;
using EFT;
using SAIN.Components.BotController;
using SAIN.Helpers;

namespace SAIN.Preset;

public class BotTypeDefinitions
{
	public static Dictionary<WildSpawnType, BotType> BotTypes;

	public static List<BotType> BotTypesList;

	public static readonly List<string> BotTypesNames;

	private static readonly string FileName;

	private static readonly List<BotType> _typesToRemove;

	static BotTypeDefinitions()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		BotTypes = new Dictionary<WildSpawnType, BotType>();
		BotTypesNames = new List<string>();
		FileName = "BotTypes";
		_typesToRemove = new List<BotType>();
		BotTypesList = ImportBotTypes();
		for (int i = 0; i < BotTypesList.Count; i++)
		{
			BotType botType = BotTypesList[i];
			WildSpawnType wildSpawnType = botType.WildSpawnType;
			BotTypesNames.Add(botType.Name);
			BotTypes.Add(wildSpawnType, botType);
		}
	}

	public static List<BotType> ImportBotTypes()
	{
		List<BotType> list = CreateBotTypes();
		removeExcluded(list, out var _);
		if (JsonUtility.Load.LoadObject<List<BotType>>(out var obj, FileName))
		{
			CheckImportedList(obj, list);
			return obj;
		}
		JsonUtility.SaveObjectToJson(list, FileName);
		return list;
	}

	private static void CheckImportedList(List<BotType> importedList, List<BotType> tempList)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < tempList.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < importedList.Count; j++)
			{
				if (tempList[i].WildSpawnType == importedList[j].WildSpawnType)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				importedList.Add(tempList[i]);
			}
		}
		removeExcluded(importedList, out var removed);
		if (removed)
		{
			JsonUtility.SaveObjectToJson(importedList, FileName);
		}
	}

	private static void removeExcluded(List<BotType> list, out bool removed)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		removed = false;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (BotSpawnController.StrictExclusionList.Contains(list[num].WildSpawnType))
			{
				list.RemoveAt(num);
				removed = true;
			}
		}
	}

	public static void ExportBotTypes()
	{
		JsonUtility.SaveObjectToJson(BotTypesList, FileName);
	}

	public static BotType GetBotType(WildSpawnType wildSpawnType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (BotTypes.ContainsKey(wildSpawnType))
		{
			return BotTypes[wildSpawnType];
		}
		Logger.LogError($"WildSpawnType {wildSpawnType} does not exist in BotType Dictionary");
		return BotTypes[(WildSpawnType)1];
	}

	private static List<BotType> CreateBotTypes()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		return new List<BotType>
		{
			new BotType
			{
				WildSpawnType = (WildSpawnType)1,
				Name = "Scav",
				Section = "Scavs",
				Description = "Scavs!"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)19,
				Name = "Scav Group",
				Section = "Scavs",
				Description = "Scavs in a Group!"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)37,
				Name = "Crazy Scav Event",
				Section = "Scavs",
				Description = "Scavs!"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)52,
				Name = "Usec",
				Section = "PMCs",
				Description = "A PMC of the Usec Faction"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)51,
				Name = "Bear",
				Section = "PMCs",
				Description = "A PMC of the Bear Faction"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)0,
				Name = "Scav Sniper",
				Section = "Scavs",
				Description = "The Scav Snipers that spawn on rooftops on certain maps"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)10,
				Name = "Tagged and Cursed Scav",
				Section = "Scavs",
				Description = "The type a scav is assigned when the player is marked as Tagged and Cursed"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)26,
				Name = "Knight",
				Section = "Goons",
				Description = "Goons leader. Close proximity to the goons has been noted to cause smashed keyboards"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)27,
				Name = "BigPipe",
				Section = "Goons",
				Description = "Goons follower. Close proximity to the goons has been noted to cause smashed keyboards\""
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)28,
				Name = "BirdEye",
				Section = "Goons",
				Description = "Goons follower. Close proximity to the goons has been noted to cause smashed keyboards\""
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)24,
				Name = "Rogue",
				Section = "Other",
				Description = "Ex Usec Personel on Lighthouse usually found around the water treatment plant"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)9,
				Name = "Raider",
				Section = "Other",
				Description = "Heavily armed scavs typically found on reserve and Labs by default"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)35,
				Name = "Bloodhound",
				Section = "Other",
				Description = "From the Live Event, nearly identical to raiders except with different voicelines and better gear. Found in"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)21,
				Name = "Cultist Priest",
				Section = "Other",
				Description = "Found on Customs, Woods, Factory, Shoreline at night"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)20,
				Name = "Cultist",
				Section = "Other",
				Description = "Found on Customs, Woods, Factory, Shoreline at night"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)6,
				Name = "Killa",
				Section = "Bosses",
				Description = "He shoot. Found on Interchange and Streets"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)47,
				Name = "Partisan",
				Section = "Bosses",
				Description = "Crazy mall santa"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)3,
				Name = "Rashala",
				Section = "Bosses",
				Description = "Customs Boss"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)5,
				Name = "Rashala Guard",
				Section = "Followers",
				Description = "Customs Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)7,
				Name = "Shturman",
				Section = "Bosses",
				Description = "Woods Boss"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)8,
				Name = "Shturman Guard",
				Section = "Followers",
				Description = "Woods Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)22,
				Name = "Tagilla",
				Section = "Bosses",
				Description = "He Smash"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)23,
				Name = "Tagilla Guard",
				Section = "Followers",
				Description = "They Smash Too?"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)17,
				Name = "Sanitar",
				Section = "Bosses",
				Description = "Shoreline Boss"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)16,
				Name = "Sanitar Guard",
				Section = "Followers",
				Description = "Shoreline Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)11,
				Name = "Gluhar",
				Section = "Bosses",
				Description = "Reserve Boss. Also can be found on Streets."
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)15,
				Name = "Gluhar Guard Snipe",
				Section = "Followers",
				Description = "Reserve Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)14,
				Name = "Gluhar Guard Scout",
				Section = "Followers",
				Description = "Reserve Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)13,
				Name = "Gluhar Guard Security",
				Section = "Followers",
				Description = "Reserve Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)12,
				Name = "Gluhar Guard Assault",
				Section = "Followers",
				Description = "Reserve Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)29,
				Name = "Zryachiy",
				Section = "Bosses",
				Description = "Lighthouse Island Sniper Boss"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)30,
				Name = "Zryachiy Guard",
				Section = "Followers",
				Description = "Lighthouse Island Sniper Boss Follower"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)32,
				Name = "Kaban",
				Section = "Bosses",
				Description = "Streets Gangster"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)33,
				Name = "Kaban Guard",
				Section = "Followers",
				Description = "Gangster Cannon Fodder"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)41,
				Name = "Basmach",
				Section = "Followers",
				Description = "Gangster 1"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)42,
				Name = "Gus",
				Section = "Followers",
				Description = "Gangster 2"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)36,
				Name = "Kaban Sniper",
				Section = "Followers",
				Description = "Gangster Sniper"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)43,
				Name = "Kollontay",
				Section = "Bosses",
				Description = "Crooked Cop"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)44,
				Name = "Kollantay Assault",
				Section = "Followers",
				Description = "Aggressive Guard"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)45,
				Name = "Kollantay Security",
				Section = "Followers",
				Description = "Defensive Guard"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)47,
				Name = "Partisan",
				Section = "Bosses",
				Description = "A scav legend.. who scavs hate"
			},
			new BotType
			{
				WildSpawnType = (WildSpawnType)46,
				Name = "BTR",
				Section = "Other",
				Description = "Zoom. Zoom. Bang. Bang."
			}
		};
	}
}
