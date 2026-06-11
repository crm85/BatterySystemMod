using System.Collections.Generic;
using System.Text;
using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.BotController.Classes;

public class BotSquads : BotManagerBase
{
	private readonly HashSet<Squad> _squadsToRemove = new HashSet<Squad>();

	private float DebugTimer = 0f;

	public Dictionary<string, Squad> Squads { get; } = new Dictionary<string, Squad>();

	public HashSet<Squad> SquadArray { get; } = new HashSet<Squad>();

	public BotSquads(BotManagerComponent botController)
		: base(botController)
	{
	}

	public void Update(float currentTime, float deltaTime)
	{
		foreach (Squad item in SquadArray)
		{
			item?.Update(currentTime, deltaTime);
		}
		ClearEmptySquads();
		if (SAINPlugin.DebugMode && DebugTimer < Time.time)
		{
			LogDebug();
		}
	}

	private void ClearEmptySquads()
	{
		foreach (Squad item in _squadsToRemove)
		{
			if (item != null)
			{
				Squads.Remove(item.GUID);
				SquadArray.Remove(item);
			}
		}
		_squadsToRemove.Clear();
	}

	private void LogDebug()
	{
		int num = 0;
		foreach (Squad item in SquadArray)
		{
			if (item != null)
			{
				LogDebug(num, item);
			}
			num++;
		}
	}

	private void LogDebug(int count, Squad squad)
	{
		DebugTimer = Time.time + 60f;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Squad [{count}]: " + "ID: [" + squad.GetId() + "] " + $"Count: [{squad.Members.Count}] " + $"Power: [{squad.SquadPowerLevel}] " + "Members:");
		foreach (MemberInfo value in squad.MemberInfos.Values)
		{
			stringBuilder.AppendLine($" [{value.Nickname}, {value.PowerLevel}]");
		}
		Logger.LogDebug(stringBuilder);
	}

	public Squad GetSquad(BotOwner botOwner)
	{
		Squad squad = null;
		BotsGroup botsGroup = botOwner.BotsGroup;
		if (botsGroup != null)
		{
			int membersCount = botsGroup.MembersCount;
			if (SAINPlugin.DebugMode)
			{
				Logger.LogDebug($"Member Count: {membersCount} Checking for existing squad object");
			}
			for (int i = 0; i < membersCount; i++)
			{
				BotOwner val = botsGroup.Member(i);
				if (!((Object)(object)val != (Object)null) || !(val.ProfileId != botOwner.ProfileId) || !base.BotController.GetSAIN(val, out var bot))
				{
					continue;
				}
				if (SAINPlugin.DebugMode)
				{
					Logger.LogInfo("Found SAIN Bot for squad");
				}
				squad = bot.Squad.SquadInfo;
				if (squad != null)
				{
					if (SAINPlugin.DebugMode)
					{
						Logger.LogInfo("Adding bot to squad [" + squad.GUID + "]");
					}
					break;
				}
			}
		}
		if (squad == null)
		{
			squad = new Squad();
			if (SAINPlugin.DebugMode)
			{
				Logger.LogWarning("Created New Squad [" + squad.GUID + "]");
			}
			if (!Squads.ContainsKey(squad.GUID))
			{
				squad.OnSquadEmpty += RemoveSquad;
				Squads.Add(squad.GUID, squad);
				SquadArray.Add(squad);
			}
		}
		return squad;
	}

	private void RemoveSquad(Squad squad)
	{
		if (squad != null)
		{
			squad.OnSquadEmpty -= RemoveSquad;
			squad.Dispose();
			_squadsToRemove.Add(squad);
		}
	}
}
