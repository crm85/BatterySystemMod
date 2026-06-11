using System;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision;

public class SAINSelfActionClass : BotComponentClassBase
{
	private float _handsBusyTimer;

	private float _nextCheckTime;

	private float _firstAidTimer;

	private float _trySurgeryTime;

	private float _stimTimer;

	private float _nextHealTime = 0f;

	private bool UsingMeds
	{
		get
		{
			BotMedecine medecine = base.BotOwner.Medecine;
			return medecine != null && medecine.Using;
		}
	}

	public SAINSelfActionClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		if (!base.Bot.SAINLayersActive || _nextCheckTime > Time.time || _nextHealTime > Time.time)
		{
			return;
		}
		ESelfDecision currentSelfDecision = base.Bot.Decision.CurrentSelfDecision;
		switch (currentSelfDecision)
		{
		case ESelfDecision.None:
			_nextCheckTime = Time.time + 1f;
			return;
		case ESelfDecision.Reload:
			_nextCheckTime = Time.time + 1f;
			return;
		}
		if (UsingMeds)
		{
			_nextCheckTime = Time.time + 1f;
			return;
		}
		if (base.BotOwner.WeaponManager.Reload.Reloading)
		{
			_nextCheckTime = Time.time + 1f;
			return;
		}
		if (base.Bot.Medical.TimeSinceShot < 0.5f)
		{
			return;
		}
		_nextCheckTime = Time.time + 0.2f;
		if (_handsBusyTimer > Time.time)
		{
			return;
		}
		if (base.Player.HandsController.IsInInteractionStrictCheck())
		{
			_handsBusyTimer = Time.time + 0.5f;
			return;
		}
		bool flag = false;
		switch (currentSelfDecision)
		{
		case ESelfDecision.FirstAid:
			flag = DoFirstAid();
			break;
		case ESelfDecision.Surgery:
			flag = true;
			break;
		case ESelfDecision.Stims:
			flag = DoStims();
			break;
		}
		if (flag)
		{
			_nextHealTime = Time.time + 5f;
		}
	}

	public bool DoFirstAid()
	{
		BotFirstAidClass val = base.BotOwner.Medecine?.FirstAid;
		if (val == null || base.BotOwner.IsDead)
		{
			return false;
		}
		if (_firstAidTimer < Time.time && val.ShallStartUse())
		{
			_firstAidTimer = Time.time + 5f;
			val.TryApplyToCurrentPart((int?)null, (Action)null);
			return true;
		}
		return false;
	}

	public bool DoSurgery()
	{
		GClass473 val = base.BotOwner.Medecine?.SurgicalKit;
		if (val == null || base.BotOwner.IsDead)
		{
			return false;
		}
		if (_trySurgeryTime < Time.time && val.ShallStartUse())
		{
			_trySurgeryTime = Time.time + 5f;
			val.ApplyToCurrentPart((Action)null);
			return true;
		}
		return false;
	}

	public bool DoStims()
	{
		GClass475 val = base.BotOwner.Medecine?.Stimulators;
		if (val == null || base.BotOwner.IsDead)
		{
			return false;
		}
		if (_stimTimer < Time.time && val.CanUseNow())
		{
			_stimTimer = Time.time + 3f;
			try
			{
				val.TryApply(false, (int?)null, (Action<bool>)null);
			}
			catch
			{
			}
			return true;
		}
		return false;
	}

	private bool HaveStimsToHelp()
	{
		return false;
	}

	public void BotCancelReload()
	{
		if (base.BotOwner.WeaponManager.Reload.Reloading)
		{
			base.BotOwner.WeaponManager.Reload.TryStopReload();
		}
	}
}
