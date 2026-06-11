using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.Components;
using SAIN.Layers.Combat.Solo;
using SAIN.Layers.Combat.Solo.Cover;
using SAIN.Models.Enums;

namespace SAIN.Layers;

internal class SAINAvoidThreatLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("Avoid Threat");

	private ECombatDecision _lastActionDecision;

	public ECombatDecision CurrentDecision => base.Bot.Decision.CurrentCombatDecision;

	public SAINAvoidThreatLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.AvoidThreat)
	{
	}

	public override Action GetNextAction()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		_lastActionDecision = CurrentDecision;
		switch (_lastActionDecision)
		{
		case ECombatDecision.DogFight:
		{
			if (base.Bot.Decision.DogFightDecision.DogFightTarget != null)
			{
				return new Action(typeof(DogFightAction), "Dog Fight - Enemy Close!", (ActionData)null);
			}
			CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
			if (coverInUse != null && coverInUse.Spotted)
			{
				return new Action(typeof(DogFightAction), "Dog Fight - My Cover is Spotted!", (ActionData)null);
			}
			if (base.Bot.Cover.SpottedInCover)
			{
				return new Action(typeof(DogFightAction), "Dog Fight - Shot while in cover!", (ActionData)null);
			}
			return new Action(typeof(DogFightAction), "Dog Fight - No Reason", (ActionData)null);
		}
		case ECombatDecision.AvoidGrenade:
			return new Action(typeof(RunToCoverAction), "Avoid Grenade", (ActionData)null);
		default:
			return new Action(typeof(DogFightAction), "NO DECISION - ERROR IN LOGIC", (ActionData)null);
		}
	}

	public override bool IsActive()
	{
		base.IsActive();
		BotComponent bot = base.Bot;
		bool flag = bot != null && bot.BotActive && (CurrentDecision == ECombatDecision.DogFight || CurrentDecision == ECombatDecision.AvoidGrenade);
		setLayer(flag);
		return flag;
	}

	public override bool IsCurrentActionEnding()
	{
		if (ResetAction)
		{
			ResetAction = false;
			return true;
		}
		BotComponent bot = base.Bot;
		return bot != null && bot.BotActive && _lastActionDecision != CurrentDecision;
	}
}
