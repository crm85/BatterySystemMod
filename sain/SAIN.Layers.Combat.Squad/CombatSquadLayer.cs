using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Components;
using SAIN.Layers.Combat.Solo;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.Decision;
using UnityEngine;

namespace SAIN.Layers.Combat.Squad;

internal class CombatSquadLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("Squad Layer");

	private ESquadDecision LastActionDecision = ESquadDecision.None;

	public CombatSquadLayer(BotOwner botOwner, int priority)
		: base(botOwner, priority, Name, ESAINLayer.Squad)
	{
	}

	public override Action GetNextAction()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		LastActionDecision = base.Bot.Decision.CurrentSquadDecision;
		switch (LastActionDecision)
		{
		case ESquadDecision.Regroup:
			return new Action(typeof(RegroupAction), $"{LastActionDecision}", (ActionData)null);
		case ESquadDecision.Suppress:
			return new Action(typeof(SuppressAction), $"{LastActionDecision}", (ActionData)null);
		case ESquadDecision.Search:
			return new Action(typeof(SearchAction), $"{LastActionDecision}", (ActionData)null);
		case ESquadDecision.GroupSearch:
			if (base.Bot.Squad.IAmLeader)
			{
				return new Action(typeof(SearchAction), $"{LastActionDecision} : Lead Search Party", (ActionData)null);
			}
			return new Action(typeof(FollowSearchParty), $"{LastActionDecision} : Follow Squad Leader", (ActionData)null);
		case ESquadDecision.Help:
			return new Action(typeof(SearchAction), $"{LastActionDecision}", (ActionData)null);
		case ESquadDecision.PushSuppressedEnemy:
			return new Action(typeof(RushEnemyAction), $"{LastActionDecision}", (ActionData)null);
		default:
			return new Action(typeof(RegroupAction), "DEFAULT!", (ActionData)null);
		}
	}

	public override bool IsActive()
	{
		base.IsActive();
		BotComponent bot = base.Bot;
		if ((Object)(object)bot != (Object)null && bot.BotActive)
		{
			SAINDecisionClass decision = bot.Decision;
			if (decision.CurrentSelfDecision == ESelfDecision.None && decision.CurrentCombatDecision != ECombatDecision.DogFight && decision.CurrentSquadDecision != ESquadDecision.None)
			{
				if (bot.Cover.CoverInUse != null)
				{
					bot.Cover.CoverInUse = null;
				}
				setLayer(active: true);
				return true;
			}
		}
		setLayer(active: false);
		return false;
	}

	public override bool IsCurrentActionEnding()
	{
		if (ResetAction)
		{
			ResetAction = false;
			return true;
		}
		BotComponent bot = base.Bot;
		if ((Object)(object)bot != (Object)null && bot.BotActive && bot.Decision.CurrentSquadDecision != LastActionDecision)
		{
			return true;
		}
		return false;
	}
}
