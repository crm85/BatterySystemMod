using System;
using System.Collections.Generic;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyVisionClass : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private const float _repeatContactMinSeenTime = 12f;

	private const float _lostContactMinSeenTime = 12f;

	public KeyValuePair<EnemyPart, EnemyPartData> _bodyPart;

	public KeyValuePair<EnemyPart, EnemyPartData> _headPart;

	private readonly EnemyGainSightClass _gainSight;

	private readonly EnemyVisionDistanceClass _visionDistance;

	private float _nextReportLostVisualTime;

	public float EnemyVelocity => base.EnemyTransform.VelocityMagnitudeNormal;

	public bool FirstContactOccured { get; private set; }

	public bool ShallReportRepeatContact { get; set; }

	public bool ShallReportLostVisual { get; set; }

	public bool InLineOfSight => VisionChecker.LineOfSight;

	public bool IsVisible { get; private set; }

	public bool CanShoot { get; private set; }

	public float VisibleStartTime { get; private set; }

	public float TimeSinceSeen => Seen ? (Time.time - TimeLastSeen) : (-1f);

	public bool Seen { get; private set; }

	public float TimeFirstSeen { get; private set; }

	public float TimeLastSeen { get; private set; }

	public float LastChangeVisionTime { get; private set; }

	public float LastGainSightResult { get; set; }

	public float GainSightCoef => _gainSight.GainSightModifier;

	public float VisionDistance => _visionDistance.Value;

	public EnemyAnglesClass Angles { get; }

	public EnemyVisionChecker VisionChecker { get; }

	public EnemyVisionClass(Enemy enemy)
		: base(enemy)
	{
		Angles = new EnemyAnglesClass(enemy);
		_gainSight = new EnemyGainSightClass(enemy);
		_visionDistance = new EnemyVisionDistanceClass(enemy);
		VisionChecker = new EnemyVisionChecker(enemy);
	}

	public override void Init()
	{
		_bodyPart = base.Enemy.EnemyInfo._bodyPart;
		_headPart = base.Enemy.EnemyInfo._headPart;
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		Angles.Init();
		VisionChecker.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		VisionChecker.ManualUpdate();
		Angles.ManualUpdate();
		UpdateVision();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		Angles.Dispose();
		VisionChecker.Dispose();
		base.Dispose();
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known)
		{
			UpdateVisibleState(forceOff: true);
			UpdateCanShootState(forceOff: true);
		}
	}

	private void UpdateVision()
	{
		UpdateVisibleState(forceOff: false);
		UpdateCanShootState(forceOff: false);
	}

	private bool IsAnyPartVisible()
	{
		foreach (EnemyPartDataClass value in VisionChecker.EnemyParts.Parts.Values)
		{
			if (value != null && value.IsVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateVisibleState(bool forceOff)
	{
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		bool isVisible = IsVisible;
		if (forceOff)
		{
			IsVisible = false;
		}
		else if (!IsAnyPartVisible())
		{
			if (base.EnemyInfo.IsVisible)
			{
				try
				{
					base.EnemyInfo.SetVisible(false);
				}
				catch
				{
				}
			}
			IsVisible = false;
		}
		else
		{
			IsVisible = base.EnemyInfo.IsVisible;
		}
		if (base.Enemy.IsCurrentEnemy && !IsVisible && isVisible)
		{
			try
			{
				base.BotOwner.CalcGoal();
			}
			catch
			{
			}
		}
		else if (!base.Enemy.IsCurrentEnemy && IsVisible)
		{
			Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
			if (currentTargetEnemy == null || !currentTargetEnemy.IsVisible)
			{
				try
				{
					base.BotOwner.CalcGoal();
				}
				catch
				{
				}
			}
		}
		if (IsVisible)
		{
			if (!isVisible)
			{
				VisibleStartTime = Time.time;
				if (Seen && TimeSinceSeen >= 12f)
				{
					ShallReportRepeatContact = true;
				}
			}
			if (!Seen)
			{
				FirstContactOccured = true;
				TimeFirstSeen = Time.time;
				Seen = true;
				base.Enemy.Events.EnemyFirstSeen();
			}
			TimeLastSeen = Time.time;
			base.Enemy.UpdateCurrentEnemyPos(base.EnemyTransform.Position);
		}
		if (!IsVisible)
		{
			if (isVisible)
			{
				base.Enemy.UpdateLastSeenPosition(base.EnemyTransform.Position);
			}
			if (Seen && TimeSinceSeen > 12f && _nextReportLostVisualTime < Time.time)
			{
				_nextReportLostVisualTime = Time.time + 20f;
				ShallReportLostVisual = true;
			}
			VisibleStartTime = -1f;
		}
		base.Enemy.Events.OnVisionChange.CheckToggle(IsVisible);
		if (IsVisible != isVisible)
		{
			LastChangeVisionTime = Time.time;
		}
	}

	public void UpdateCanShootState(bool forceOff)
	{
		if (forceOff)
		{
			CanShoot = false;
		}
		else
		{
			CanShoot = VisionChecker.EnemyParts.CanShoot;
		}
	}
}
