using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Components;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Layers;

public abstract class SAINLayer : CustomLayer
{
	private bool foundBot = false;

	protected bool ResetAction = false;

	private readonly string LayerName;

	private readonly ESAINLayer ELayer;

	private BotComponent _bot;

	public static BotManagerComponent BotController => BotManagerComponent.Instance;

	public BotComponent Bot => _bot;

	public static string BuildLayerName(string name)
	{
		return "SAIN : " + name;
	}

	public SAINLayer(BotOwner botOwner, int priority, string layerName, ESAINLayer eSAINLayer)
		: base(botOwner, priority)
	{
		LayerName = layerName;
		ELayer = eSAINLayer;
		tryGetBot(botOwner);
	}

	public override bool IsActive()
	{
		if (!foundBot)
		{
			tryGetBot(((CustomLayer)this).BotOwner);
		}
		return false;
	}

	private void tryGetBot(BotOwner botOwner)
	{
		if (foundBot)
		{
			return;
		}
		if ((Object)(object)_bot == (Object)null)
		{
			_bot = ((Component)botOwner).GetComponent<BotComponent>();
		}
		if ((Object)(object)_bot != (Object)null)
		{
			foundBot = true;
			if (_bot.Decision == null || _bot.Decision.DecisionManager == null)
			{
				_bot.OnBotActivated += OnBotActivated;
			}
			else
			{
				OnBotActivated(_bot);
			}
		}
	}

	protected virtual void OnBotActivated(BotComponent bot)
	{
		bot.Decision.DecisionManager.OnDecisionMade += BotDecisionMade;
	}

	protected void BotDecisionMade(ECombatDecision combatDecision, ESquadDecision squadDecision, ESelfDecision selfDecision, BotComponent bot)
	{
		if (Bot.ActiveLayer == ELayer)
		{
			ResetAction = true;
		}
	}

	protected void setLayer(bool active)
	{
		if (!((Object)(object)Bot == (Object)null))
		{
			if (active)
			{
				Bot.ActiveLayer = ELayer;
			}
			else if ((Object)(object)Bot != (Object)null && Bot.ActiveLayer == ELayer)
			{
				Bot.ActiveLayer = ESAINLayer.None;
			}
		}
	}

	public override string GetName()
	{
		return LayerName;
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		if ((Object)(object)Bot != (Object)null)
		{
			DebugOverlay.AddBaseInfo(Bot, ((CustomLayer)this).BotOwner, stringBuilder);
		}
	}
}
