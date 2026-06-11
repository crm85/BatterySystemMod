using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Search;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo;

internal class SearchAction : CombatAction, ISAINAction
{
	private bool _subscribed;

	private float _nextCheckWeaponTime;

	private float _nextUpdateSearchTime;

	private bool _haveTalked = false;

	private bool _sprintEnabled = false;

	private float _sprintTimer = 0f;

	private Enemy _searchTarget => Search?.SearchTarget;

	private SAINSearchClass Search => base.Bot.Search;

	public override void Start()
	{
		subscribeToBotEvents();
		setSearchTarget(base.Bot.Enemy);
		Toggle(value: true);
	}

	public override void Stop()
	{
		clearSearchTarget();
		Toggle(value: false);
		BotMover mover = ((CustomLogic)this).BotOwner.Mover;
		if (mover != null)
		{
			mover.MovementResume();
		}
		_haveTalked = false;
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		setTargetEnemy();
		updateSearch();
		EndProfilingSample();
	}

	private void updateSearch()
	{
		Enemy searchTarget = _searchTarget;
		if (searchTarget == null)
		{
			return;
		}
		if (base.Shoot.ShootAnyVisibleEnemies(searchTarget))
		{
			base.Bot.Steering.SteerByPriority(searchTarget);
			return;
		}
		bool enemyHeardFromPeace = searchTarget.Hearing.EnemyHeardFromPeace;
		if (enemyHeardFromPeace)
		{
			_sprintEnabled = false;
		}
		else
		{
			checkShouldSprint();
			talk();
		}
		steer();
		if (_nextUpdateSearchTime < Time.time)
		{
			_nextUpdateSearchTime = Time.time + 0.1f;
			Search.Search(_sprintEnabled, searchTarget);
		}
		if (!_sprintEnabled && !enemyHeardFromPeace)
		{
			if (base.Bot.Decision.SelfActionDecisions.AmmoRatio > 0.5f)
			{
				base.Bot.Suppression.TrySuppressEnemy(searchTarget);
			}
			else
			{
				base.Bot.Suppression.ResetSuppressing();
			}
			checkWeapon();
		}
	}

	private void checkClearEnemy(string profileId, Enemy enemy)
	{
		if (_searchTarget != null && _searchTarget.EnemyProfileId == profileId)
		{
			clearSearchTarget();
		}
	}

	private void enemyChanged(Enemy enemy, Enemy lastEnemy)
	{
		if (_searchTarget != null)
		{
			clearSearchTarget();
			if (enemy != null)
			{
				setSearchTarget(enemy);
			}
		}
	}

	private void clearSearchTarget()
	{
		Search.ToggleSearch(value: false, _searchTarget);
	}

	private void setTargetEnemy()
	{
		Enemy searchTarget = _searchTarget;
		if (searchTarget != null && (!searchTarget.EnemyKnown || !searchTarget.Person.Active || !searchTarget.CheckValid()))
		{
			clearSearchTarget();
		}
		if (_searchTarget == null)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null)
			{
				setSearchTarget(enemy);
			}
		}
	}

	private void setSearchTarget(Enemy enemy)
	{
		Search.ToggleSearch(value: true, enemy);
		_nextUpdateSearchTime = 0f;
	}

	private void talk()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!Search.FinalDestination.HasValue || _haveTalked || !base.Bot.Info.Profile.IsScav)
		{
			return;
		}
		Vector3 val = ((CustomLogic)this).BotOwner.Position - Search.FinalDestination.Value;
		if (((Vector3)(ref val)).sqrMagnitude < 2500f)
		{
			_haveTalked = true;
			if (EFTMath.RandomBool(40f))
			{
				base.Bot.Talk.Say((EPhraseTrigger)28, (ETagStatus)2, withGroupDelay: true);
			}
		}
	}

	private void checkWeapon()
	{
		if (!(_nextCheckWeaponTime < Time.time))
		{
			return;
		}
		_nextCheckWeaponTime = Time.time + 180f * Random.Range(0.5f, 1.5f);
		if (_searchTarget.TimeSinceLastKnownUpdated > 30f)
		{
			if (EFTMath.RandomBool())
			{
				base.Bot.Player.HandsController.FirearmsAnimator.CheckAmmo();
			}
			else
			{
				base.Bot.Player.HandsController.FirearmsAnimator.CheckChamber();
			}
		}
	}

	private void checkShouldSprint()
	{
		if (Search.CurrentState == ESearchMove.MoveToEndPeek || Search.CurrentState == ESearchMove.Wait)
		{
			_sprintEnabled = false;
			return;
		}
		Enemy searchTarget = _searchTarget;
		if (searchTarget != null && searchTarget.IsVisible)
		{
			_sprintEnabled = false;
			return;
		}
		if (base.Bot.Decision.CurrentSquadDecision == ESquadDecision.Help)
		{
			_sprintEnabled = true;
			return;
		}
		if (_searchTarget.IsSniper && GlobalSettingsClass.Instance.Mind.ENEMYSNIPER_ALWAYS_SPRINT_SEARCH)
		{
			_sprintEnabled = true;
			return;
		}
		PersonalityBehaviorSettings personalitySettings = base.Bot.Info.PersonalitySettings;
		float num = personalitySettings.Search.SprintWhileSearchChance;
		if (_sprintTimer < Time.time && num > 0f)
		{
			float powerLevel = base.Bot.Info.Profile.PowerLevel;
			if ((Object)(object)_searchTarget?.EnemyPlayer != (Object)null && _searchTarget.EnemyPlayer.AIData.PowerOfEquipment < powerLevel * 0.5f)
			{
				num = 100f;
			}
			_sprintEnabled = EFTMath.RandomBool(num);
			float num2 = ((!_sprintEnabled) ? (4f * Random.Range(0.5f, 1.5f)) : (4f * Random.Range(0.5f, 2f)));
			_sprintTimer = Time.time + num2;
		}
	}

	private void steer()
	{
		if (!base.Bot.Steering.SteerByPriority(_searchTarget, lookRandom: false))
		{
			base.Bot.Steering.LookToLastKnownEnemyPosition(_searchTarget);
		}
	}

	private void subscribeToBotEvents()
	{
		if (!_subscribed)
		{
			base.Bot.EnemyController.Events.OnEnemyRemoved += checkClearEnemy;
			base.Bot.EnemyController.Events.OnEnemyChanged += enemyChanged;
			_subscribed = true;
		}
	}

	public SearchAction(BotOwner bot)
		: base(bot, "Search")
	{
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		stringBuilder.AppendLine("Search Target " + _searchTarget?.EnemyName);
		base.BuildDebugText(stringBuilder);
	}
}
