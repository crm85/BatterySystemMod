using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Layers.Combat.Solo.Cover;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo;

internal class CombatSoloLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("Combat Layer");

	private bool _doSurgeryAction;

	private ECombatDecision _lastDecision = ECombatDecision.None;

	private ESelfDecision _lastSelfDecision = ESelfDecision.None;

	public ECombatDecision _currentDecision => base.Bot.Decision.CurrentCombatDecision;

	public ESelfDecision _currentSelfDecision => base.Bot.Decision.CurrentSelfDecision;

	public CombatSoloLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.Combat)
	{
	}

	public override Action GetNextAction()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected O, but got Unknown
		_lastSelfDecision = _currentSelfDecision;
		_lastDecision = _currentDecision;
		if (_doSurgeryAction)
		{
			_doSurgeryAction = false;
			return new Action(typeof(DoSurgeryAction), "Surgery", (ActionData)null);
		}
		switch (_lastDecision)
		{
		case ECombatDecision.MoveToEngage:
			return new Action(typeof(MoveToEngageAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.MeleeAttack:
			return new Action(typeof(MeleeAttackAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.FightZombies:
			return new Action(typeof(FightZombiesAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.RushEnemy:
			return new Action(typeof(RushEnemyAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.ThrowGrenade:
			return new Action(typeof(ThrowGrenadeAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.ShiftCover:
			return new Action(typeof(ShiftCoverAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.RunToCover:
			return new Action(typeof(RunToCoverAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.Retreat:
			return new Action(typeof(RunToCoverAction), $"{_lastDecision} + {_lastSelfDecision}", (ActionData)null);
		case ECombatDecision.MoveToCover:
			return new Action(typeof(WalkToCoverAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.DogFight:
			return new Action(typeof(DogFightAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.StandAndShoot:
		case ECombatDecision.ShootDistantEnemy:
			return new Action(typeof(StandAndShootAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.HoldInCover:
		{
			string text = ((_lastSelfDecision == ESelfDecision.None) ? $"{_lastDecision}" : $"{_lastDecision} + {_lastSelfDecision}");
			return new Action(typeof(HoldinCoverAction), text, (ActionData)null);
		}
		case ECombatDecision.Search:
			return new Action(typeof(SearchAction), $"{_lastDecision}", (ActionData)null);
		case ECombatDecision.Freeze:
			return new Action(typeof(FreezeAction), $"{_lastDecision}", (ActionData)null);
		default:
			return new Action(typeof(StandAndShootAction), $"DEFAULT! {_lastDecision}", (ActionData)null);
		}
	}

	public override bool IsActive()
	{
		base.IsActive();
		if ((Object)(object)base.Bot == (Object)null)
		{
			return false;
		}
		bool flag = _currentDecision != ECombatDecision.None;
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
		if (!_doSurgeryAction && _currentSelfDecision == ESelfDecision.Surgery && base.Bot.Cover.BotIsAtCoverInUse())
		{
			_doSurgeryAction = true;
			return true;
		}
		if (_lastSelfDecision == ESelfDecision.Surgery && _currentSelfDecision != ESelfDecision.Surgery)
		{
			return true;
		}
		return _currentDecision != _lastDecision;
	}
}
