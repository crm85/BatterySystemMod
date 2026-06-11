using System;
using SAIN.Classes.Coverfinder;
using SAIN.Components;
using SAIN.Helpers.Events;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class BotDecisionManager : BotSubClass<SAINDecisionClass>, IBotClass, IDisposable
{
	private const float DECISION_FREQUENCY = 1f / 30f;

	private const float DECISION_FREQUENCY_PEACE = 0.1f;

	private float _nextGetDecisionTime;

	public ToggleEvent HasDecisionToggle { get; } = new ToggleEvent();

	public ECombatDecision CurrentCombatDecision { get; private set; }

	public ECombatDecision PreviousCombatDecision { get; private set; }

	public ESquadDecision CurrentSquadDecision { get; private set; }

	public ESquadDecision PreviousSquadDecision { get; private set; }

	public ESelfDecision CurrentSelfDecision { get; private set; }

	public ESelfDecision PreviousSelfDecision { get; private set; }

	public bool HasDecision => HasDecisionToggle.Value;

	public float ChangeDecisionTime { get; private set; }

	public float TimeSinceChangeDecision => Time.time - ChangeDecisionTime;

	public event Action<ECombatDecision, ESquadDecision, ESelfDecision, BotComponent> OnDecisionMade;

	public BotDecisionManager(SAINDecisionClass decisionClass)
		: base(decisionClass)
	{
	}

	public override void Init()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Combine(botActiveToggle.OnToggle, new Action<bool>(resetDecisions));
		base.Init();
	}

	public override void ManualUpdate()
	{
		updateDecision();
	}

	public override void Dispose()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Remove(botActiveToggle.OnToggle, new Action<bool>(resetDecisions));
		base.Dispose();
	}

	private void updateDecision()
	{
		if (_nextGetDecisionTime < Time.time)
		{
			getDecision();
			float num = (HasDecision ? (1f / 30f) : 0.1f);
			_nextGetDecisionTime = Time.time + num;
		}
	}

	private bool shallTagillaHammerAttack(Enemy enemy)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		if (enemy == null)
		{
			return false;
		}
		bool flag = CurrentCombatDecision == ECombatDecision.MeleeAttack;
		ETagStatus healthStatus = base.Bot.Memory.Health.HealthStatus;
		if (!flag)
		{
			if (CurrentSelfDecision != ESelfDecision.None)
			{
				return false;
			}
			if ((int)healthStatus != 1024 && (int)healthStatus != 2048)
			{
				return false;
			}
			if ((int)enemy.Path.PathToEnemyStatus > 0)
			{
				return false;
			}
			if (enemy.RealDistance < 30f && enemy.Path.PathLength < 20f && enemy.Status.VulnerableAction != EEnemyAction.None)
			{
				enemy.BotOwner.WeaponManager.Melee.ShallEndRun = false;
				return true;
			}
			return false;
		}
		if (enemy.BotOwner.WeaponManager.Melee.ShallEndRun)
		{
			return false;
		}
		if ((int)healthStatus != 8192 && enemy.RealDistance < 40f && enemy.Path.PathLength < 35f)
		{
			return true;
		}
		return false;
	}

	private void getDecision()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		Enemy enemy = base.Bot.Enemy;
		EnemyList knownEnemies = base.Bot.EnemyController.EnemyLists.KnownEnemies;
		base.BaseClass.EnemyDecisions.DebugShallSearch = null;
		if ((int)base.Bot.Info.Profile.WildSpawnType == 22)
		{
			if (shallTagillaHammerAttack(enemy))
			{
				SetDecisions(ECombatDecision.MeleeAttack, ESquadDecision.None, ESelfDecision.None);
				return;
			}
			if (base.BotOwner.WeaponManager.IsMelee)
			{
				base.BotOwner.WeaponManager.Selector.ChangeToMain();
			}
		}
		if (enemy != null && enemy.IsZombie)
		{
			bool flag = false;
			foreach (Enemy item in knownEnemies)
			{
				if (item == null || !item.IsZombie)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.BaseClass.SelfActionDecisions.GetDecision(out var Decision, enemy);
				base.BaseClass.SquadDecisions.GetDecision(out var Decision2, enemy);
				SetDecisions(ECombatDecision.FightZombies, Decision2, Decision);
				return;
			}
		}
		ESelfDecision Decision3;
		ESquadDecision Decision4;
		ECombatDecision result;
		if (base.BaseClass.DogFightDecision.ShallDogFight(knownEnemies))
		{
			SetDecisions(ECombatDecision.DogFight, ESquadDecision.None, ESelfDecision.None);
		}
		else if (base.BotOwner.WeaponManager.IsMelee)
		{
			SetDecisions(ECombatDecision.MeleeAttack, ESquadDecision.None, ESelfDecision.None);
		}
		else if (base.BaseClass.SelfActionDecisions.GetDecision(out Decision3, enemy))
		{
			ECombatDecision solo = ((!base.Bot.Cover.InCover) ? ECombatDecision.Retreat : ECombatDecision.HoldInCover);
			SetDecisions(solo, ESquadDecision.None, Decision3);
		}
		else if (CheckContinueRetreat())
		{
			SetDecisions(ECombatDecision.Retreat, ESquadDecision.None, ESelfDecision.None);
		}
		else if (base.BaseClass.SquadDecisions.GetDecision(out Decision4, enemy))
		{
			SetDecisions(ECombatDecision.None, Decision4, ESelfDecision.None);
		}
		else if (base.BaseClass.EnemyDecisions.GetDecision(out result, enemy, knownEnemies))
		{
			SetDecisions(result, ESquadDecision.None, ESelfDecision.None);
		}
		else
		{
			SetDecisions(ECombatDecision.None, ESquadDecision.None, ESelfDecision.None);
		}
	}

	private void SetDecisions(ECombatDecision solo, ESquadDecision squad, ESelfDecision self)
	{
		if (SAINPlugin.DebugMode)
		{
			if (SAINPlugin.ForceSoloDecision != ECombatDecision.None)
			{
				solo = SAINPlugin.ForceSoloDecision;
			}
			if (SAINPlugin.ForceSquadDecision != ESquadDecision.None)
			{
				squad = SAINPlugin.ForceSquadDecision;
			}
			if (SAINPlugin.ForceSelfDecision != ESelfDecision.None)
			{
				self = SAINPlugin.ForceSelfDecision;
			}
		}
		if (checkForNewDecision(solo, squad, self))
		{
			bool value = solo != ECombatDecision.None || self != ESelfDecision.None || squad != ESquadDecision.None;
			HasDecisionToggle.CheckToggle(value);
			base.Bot.ManualShoot.Reset();
			base.Bot.Suppression.ResetSuppressing();
			ChangeDecisionTime = Time.time;
			this.OnDecisionMade?.Invoke(solo, squad, self, base.Bot);
		}
	}

	private bool checkForNewDecision(ECombatDecision newSoloDecision, ESquadDecision newSquadDecision, ESelfDecision newSelfDecision)
	{
		bool result = false;
		if (newSoloDecision != CurrentCombatDecision)
		{
			PreviousCombatDecision = CurrentCombatDecision;
			CurrentCombatDecision = newSoloDecision;
			result = true;
		}
		if (newSquadDecision != CurrentSquadDecision)
		{
			PreviousSquadDecision = CurrentSquadDecision;
			CurrentSquadDecision = newSquadDecision;
			result = true;
		}
		if (newSelfDecision != CurrentSelfDecision)
		{
			PreviousSelfDecision = CurrentSelfDecision;
			CurrentSelfDecision = newSelfDecision;
			result = true;
		}
		return result;
	}

	public void ResetDecisions(bool active)
	{
		bool hasDecision = HasDecision;
		resetDecisions(value: false);
		if (active && hasDecision)
		{
			base.BotOwner.CalcGoal();
		}
	}

	private void resetDecisions(bool value)
	{
		if (!value)
		{
			SetDecisions(ECombatDecision.None, ESquadDecision.None, ESelfDecision.None);
		}
	}

	private bool CheckContinueRetreat()
	{
		if (CurrentCombatDecision != ECombatDecision.Retreat && CurrentCombatDecision != ECombatDecision.RunToCover)
		{
			return false;
		}
		if (!base.Bot.Mover.PathFollower.Running)
		{
			return false;
		}
		if (base.Bot.Cover.InCover)
		{
			return false;
		}
		float timeSinceChangeDecision = base.Bot.Decision.TimeSinceChangeDecision;
		if (timeSinceChangeDecision < 0.5f)
		{
			return true;
		}
		if (timeSinceChangeDecision > 3f && !base.Bot.BotStuck.BotHasChangedPosition)
		{
			return false;
		}
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse == null)
		{
			return false;
		}
		return coverInUse.PathDistanceStatus switch
		{
			CoverStatus.InCover => false, 
			CoverStatus.CloseToCover => true, 
			_ => !coverInUse.CoverData.IsBad, 
		};
	}
}
