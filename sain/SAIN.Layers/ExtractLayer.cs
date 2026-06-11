using DrakiaXYZ.BigBrain.Brains;
using EFT;
using LootingBots;
using SAIN.Components.BotController;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Memory;
using UnityEngine;

namespace SAIN.Layers;

internal class ExtractLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("Extract");

	private float _nextSayImLeavingTime;

	private float _nextSayNeedMedsTime;

	private bool _loggedExtractLoot;

	private SAINLootingBotsIntegration SAINLootingBotsIntegration;

	private bool _loggedExtractExternal;

	private bool Logged = false;

	private bool FullOnLoot => SAINLootingBotsIntegration?.FullOnLoot ?? false;

	public ExtractLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.Extract)
	{
	}

	public override Action GetNextAction()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		return new Action(typeof(ExtractAction), $"Extract : {base.Bot.Memory.Extract.ExtractReason}", (ActionData)null);
	}

	public override bool IsActive()
	{
		base.IsActive();
		bool flag = (Object)(object)base.Bot != (Object)null && allowedToExtract() && hasExtractReason() && hasExtractLocation();
		setLayer(flag);
		return flag;
	}

	public override bool IsCurrentActionEnding()
	{
		return false;
	}

	private bool allowedToExtract()
	{
		return base.Bot.Info.FileSettings.Mind.EnableExtracts && GlobalSettingsClass.Instance.General.Extract.SAIN_EXTRACT_TOGGLE && BotExtractManager.IsBotAllowedToExfil(base.Bot);
	}

	private bool hasExtractReason()
	{
		return ExtractFromTime() || ExtractFromInjury() || ExtractFromLoot() || ExtractFromExternal();
	}

	private bool hasExtractLocation()
	{
		if (!base.Bot.Memory.Extract.ExfilPosition.HasValue)
		{
			SAINLayer.BotController.BotExtractManager.TryFindExfilForBot(base.Bot);
			return false;
		}
		if (!SAINLayer.BotController.BotExtractManager.CanBotsUseExtract(base.Bot.Memory.Extract.ExfilPoint) && !IsInExtractArea())
		{
			base.Bot.Memory.Extract.ExfilPoint = null;
			base.Bot.Memory.Extract.ExfilPosition = null;
			return false;
		}
		return true;
	}

	private bool IsInExtractArea()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((CustomLayer)this).BotOwner.Position - base.Bot.Memory.Extract.ExfilPosition.Value;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		return sqrMagnitude < ExtractAction.MinDistanceToStartExtract;
	}

	private bool ExtractFromTime()
	{
		if (ModDetection.QuestingBotsLoaded)
		{
			return false;
		}
		float percentageRemaining = SAINLayer.BotController.BotExtractManager.PercentageRemaining;
		if (percentageRemaining <= base.Bot.Info.PercentageBeforeExtract)
		{
			if (!Logged)
			{
				Logged = true;
				Logger.LogInfo($"[{((Object)((CustomLayer)this).BotOwner).name}] Is Moving to Extract with [{percentageRemaining}] of the raid remaining.");
			}
			if (base.Bot.Enemy == null || SAINLayer.BotController.BotExtractManager.TimeRemaining < 120f)
			{
				base.Bot.Memory.Extract.ExtractReason = EExtractReason.Time;
				return true;
			}
		}
		return false;
	}

	private bool ExtractFromInjury()
	{
		if (base.Bot.Memory.Health.Dying && !((GClass469)((CustomLayer)this).BotOwner.Medecine.FirstAid).HaveSmth2Use)
		{
			if (_nextSayNeedMedsTime < Time.time)
			{
				_nextSayNeedMedsTime = Time.time + 10f;
				base.Bot.Talk.GroupSay((EPhraseTrigger)90, null, withGroupDelay: true, 20f);
			}
			if (!Logged)
			{
				Logged = true;
				Logger.LogInfo("[" + ((Object)((CustomLayer)this).BotOwner).name + "] Is Moving to Extract because of heavy injury and lack of healing items.");
			}
			if (base.Bot.Enemy == null || base.Bot.Enemy.TimeSinceSeen > 30f)
			{
				if (_nextSayImLeavingTime < Time.time)
				{
					_nextSayImLeavingTime = Time.time + 10f;
					base.Bot.Talk.GroupSay((EPhraseTrigger)40, null, withGroupDelay: true, 20f);
				}
				base.Bot.Memory.Extract.ExtractReason = EExtractReason.Injured;
				return true;
			}
		}
		return false;
	}

	private bool ExtractFromLoot()
	{
		if (!SAINPlugin.LoadedPreset.GlobalSettings.General.LootingBots.ExtractFromLoot || !LootingBotsInterop.Init())
		{
			return false;
		}
		if (SAINLootingBotsIntegration == null)
		{
			SAINLootingBotsIntegration = new SAINLootingBotsIntegration(((CustomLayer)this).BotOwner, base.Bot);
		}
		SAINLootingBotsIntegration?.Update();
		if (FullOnLoot && !HasActiveThreat())
		{
			if (!_loggedExtractLoot)
			{
				_loggedExtractLoot = true;
				Logger.LogInfo($"[{((Object)((CustomLayer)this).BotOwner).name}] Is Moving to Extract because of Loot found in raid. Net Loot Value: [{SAINLootingBotsIntegration?.NetLootValue}]");
			}
			base.Bot.Memory.Extract.ExtractReason = EExtractReason.Loot;
			return true;
		}
		return false;
	}

	private bool HasActiveThreat()
	{
		if (base.Bot.Enemy == null || base.Bot.Enemy.TimeSinceSeen > 30f)
		{
			return false;
		}
		return true;
	}

	private bool ExtractFromExternal()
	{
		if (base.Bot.Info.ForceExtract)
		{
			if (!_loggedExtractExternal)
			{
				_loggedExtractExternal = true;
				Logger.LogInfo("[" + ((Object)((CustomLayer)this).BotOwner).name + "] Is Moving to Extract because of external call.");
			}
			base.Bot.Memory.Extract.ExtractReason = EExtractReason.External;
		}
		return base.Bot.Info.ForceExtract;
	}
}
