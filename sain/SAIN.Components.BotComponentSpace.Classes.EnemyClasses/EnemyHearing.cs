using System;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.BotComponentSpace.Classes.EnemyClasses;

public class EnemyHearing : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private const float REPORT_HEARD_FREQUENCY = 1f;

	private float _nextReportHeardTime;

	private float _timeLastHeard;

	public bool Heard { get; private set; }

	public bool EnemyHeardFromPeace { get; set; }

	public float TimeSinceHeard => Heard ? (Time.time - _timeLastHeard) : float.MaxValue;

	public BotSound LastSoundHeard { get; set; }

	public Vector3? LastHeardPosition { get; private set; }

	public float DispersionModifier => 1f;

	public EnemyHearing(Enemy enemy)
		: base(enemy)
	{
	}

	public override void Init()
	{
		base.Enemy.Events.OnFirstSeen += resetHeardFromPeace;
		base.Init();
	}

	public override void ManualUpdate()
	{
		if (base.Enemy.Seen && EnemyHeardFromPeace)
		{
			EnemyHeardFromPeace = false;
		}
		base.ManualUpdate();
	}

	private void resetHeardFromPeace(Enemy enemy)
	{
		EnemyHeardFromPeace = false;
	}

	public override void Dispose()
	{
		base.Enemy.Events.OnFirstSeen -= resetHeardFromPeace;
		base.Dispose();
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known)
		{
			Heard = false;
			LastSoundHeard = null;
			LastHeardPosition = null;
			_timeLastHeard = 0f;
			EnemyHeardFromPeace = false;
		}
	}

	public EnemyPlace SetHeard(SAINHearingReport report)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (base.Enemy.IsVisible)
		{
			report.position = base.Enemy.EnemyPosition;
		}
		Heard = true;
		bool flag = report.soundType.IsGunShot();
		base.Enemy.Status.HeardRecently = true;
		_timeLastHeard = Time.time;
		EnemyPlace enemyPlace = UpdateHeardPosition(report);
		if (!base.Bot.HasEnemy)
		{
			EnemyHeardFromPeace = true;
		}
		if (enemyPlace != null)
		{
			base.Enemy.Events.EnemyHeard(report.soundType, report.isDanger, enemyPlace);
		}
		if (flag || !report.shallReportToSquad)
		{
			return enemyPlace;
		}
		updateEnemyAction(report.soundType, report.position);
		return enemyPlace;
	}

	private void updateEnemyAction(SAINSoundType soundType, Vector3 soundPosition)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		EEnemyAction eEnemyAction;
		switch (soundType)
		{
		case SAINSoundType.GrenadePin:
		case SAINSoundType.GrenadeDraw:
			eEnemyAction = EEnemyAction.HasGrenade;
			break;
		case SAINSoundType.Reload:
		case SAINSoundType.DryFire:
			eEnemyAction = EEnemyAction.Reloading;
			break;
		case SAINSoundType.Looting:
			eEnemyAction = EEnemyAction.Looting;
			break;
		case SAINSoundType.Heal:
			eEnemyAction = EEnemyAction.Healing;
			break;
		case SAINSoundType.Surgery:
			eEnemyAction = EEnemyAction.UsingSurgery;
			break;
		default:
			eEnemyAction = EEnemyAction.None;
			break;
		}
		if (eEnemyAction != EEnemyAction.None)
		{
			base.Enemy.Status.SetVulnerableAction(eEnemyAction);
			base.Bot.Squad.SquadInfo.UpdateSharedEnemyStatus(base.EnemyIPlayer, eEnemyAction, base.Bot, soundType, soundPosition);
		}
	}

	public EnemyPlace UpdateHeardPosition(SAINHearingReport report)
	{
		EnemyPlace enemyPlace = base.Enemy.KnownPlaces.UpdatePersonalHeardPosition(report);
		if (report.shallReportToSquad && enemyPlace != null && _nextReportHeardTime < Time.time)
		{
			_nextReportHeardTime = Time.time + 1f;
			base.Bot.Squad?.SquadInfo?.ReportEnemyPosition(base.Enemy, enemyPlace, seen: false);
		}
		return enemyPlace;
	}

	public void OnEnemyKnownChanged(Enemy enemy, bool known)
	{
	}

	private float angleModifier()
	{
		return 1f;
	}

	private float distanceModifier()
	{
		return 1f;
	}

	private float soundTypeModifier(SAINSoundType soundType)
	{
		return 1f;
	}

	private float weaponTypeModifier()
	{
		return 1f;
	}
}
