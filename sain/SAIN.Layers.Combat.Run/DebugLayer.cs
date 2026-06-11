using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Layers.Combat.Run;

internal class DebugLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("SAIN Debug");

	public ECombatDecision CurrentDecision => base.Bot.Decision.CurrentCombatDecision;

	public DebugLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.Run)
	{
	}

	public override Action GetNextAction()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		if (SAINPlugin.DebugSettings.Logs.ForceBotsToRunAround)
		{
			return new Action(typeof(RunningAction), "RUNNING", (ActionData)null);
		}
		if (SAINPlugin.DebugSettings.Logs.ForceBotsToTryCrawl)
		{
			return new Action(typeof(CrawlAction), "CRAWL", (ActionData)null);
		}
		if (!SAINPlugin.DebugSettings.Logs.TestGrenadeThrow || ((CustomLayer)this).BotOwner.WeaponManager.Grenades.HaveGrenade)
		{
		}
		return new Action(typeof(RunningAction), "RUNNING", (ActionData)null);
	}

	public override bool IsActive()
	{
		base.IsActive();
		bool flag = SAINPlugin.DebugSettings.Logs.ForceBotsToRunAround || SAINPlugin.DebugSettings.Logs.ForceBotsToTryCrawl;
		setLayer(flag);
		return flag;
	}

	public override bool IsCurrentActionEnding()
	{
		if ((Object)(object)base.Bot == (Object)null)
		{
			return true;
		}
		return false;
	}
}
