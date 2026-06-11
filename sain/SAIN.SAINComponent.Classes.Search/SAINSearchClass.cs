using SAIN.Components;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Search;

public class SAINSearchClass : BotComponentClassBase
{
	private float _waitAtPointTimer = -1f;

	private float _advanceTime;

	public bool SearchActive { get; private set; }

	public Enemy SearchTarget { get; private set; }

	public ESearchMove NextState { get; private set; }

	public ESearchMove CurrentState { get; private set; }

	public ESearchMove LastState { get; private set; }

	public Vector3? FinalDestination => PathFinder.FinalDestination;

	public BotPeekPlan? PeekPoints => PathFinder.PeekPoints;

	public SearchDeciderClass SearchDecider { get; private set; }

	public SearchPathFinder PathFinder { get; private set; }

	private bool _Running => base.Bot.Mover.PathFollower.Running;

	private PersonalitySearchSettings _searchSettings => base.Bot.Info.PersonalitySettings.Search;

	public SAINSearchClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
		SearchDecider = new SearchDeciderClass(this);
		PathFinder = new SearchPathFinder(this);
	}

	public void ToggleSearch(bool value, Enemy target)
	{
		if (value)
		{
			if (SearchTarget != null)
			{
				if (SearchTarget == target)
				{
					SearchActive = true;
					return;
				}
				SearchTarget.Events.OnSearch.CheckToggle(value: false);
				SearchTarget = null;
			}
			if (target != null)
			{
				target.Events.OnSearch.CheckToggle(value: true);
				SearchTarget = target;
			}
			SearchActive = true;
		}
		else
		{
			if (SearchTarget != null)
			{
				SearchTarget.Events.OnSearch.CheckToggle(value: false);
				SearchTarget = null;
			}
			SearchActive = false;
			Reset();
		}
	}

	public void Search(bool shallSprint, Enemy enemy)
	{
		PathFinder.UpdateSearchDestination(enemy);
		SwitchSearchModes(shallSprint, enemy);
		PeekPoints?.DrawDebug();
	}

	private bool WaitAtPoint()
	{
		if (_waitAtPointTimer < 0f)
		{
			float num = 3f;
			num *= base.Bot.Info.PersonalitySettings.Search.SearchWaitMultiplier;
			float num2 = num * Random.Range(0.25f, 1.25f);
			_waitAtPointTimer = Time.time + num2;
			base.Bot.Mover.PathFollower.Pause(num2);
		}
		if (_waitAtPointTimer < Time.time)
		{
			base.Bot.Mover.PathFollower.Unpause();
			_waitAtPointTimer = -1f;
			return false;
		}
		return true;
	}

	private bool MoveToPoint(Vector3 destination, bool shallSprint)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (shallSprint && base.Bot.Mover.RunToPoint(destination, ESprintUrgency.Middle, stopSprintEnemyVisible: true))
		{
			return true;
		}
		if (base.Bot.Mover.GoToPoint(destination, out var _))
		{
			return true;
		}
		return false;
	}

	private void HandleLight(bool stealthy)
	{
		if (!_Running && !base.Bot.Mover.PathFollower.Running)
		{
			if (stealthy || _searchSettings.Sneaky)
			{
				base.Bot.BotLight.ToggleLight(value: false);
			}
			else if (base.Bot.Mover.PathFollower.Moving)
			{
				base.Bot.BotLight.HandleLightForSearch(base.Bot.Mover.PathFollower.MoveData.CurrentCornerDistanceSqr);
			}
		}
	}

	private void SwitchSearchModes(bool shallSprint, Enemy enemy)
	{
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		if (!FinalDestination.HasValue)
		{
			return;
		}
		if (CheckEndPeek())
		{
			LastState = CurrentState;
			CurrentState = ESearchMove.None;
		}
		bool stealthy = SearchDecider.ShallBeStealthyDuringSearch(enemy);
		GetSpeedandPose(out var speed, out var pose, shallSprint, stealthy);
		HandleLight(stealthy);
		CheckShallWaitandReload();
		if (ShallSwapToSprint(shallSprint, speed, pose))
		{
			return;
		}
		ESteerPriority currentSteerPriority = base.Bot.Steering.CurrentSteerPriority;
		ESteerPriority eSteerPriority = currentSteerPriority;
		if (eSteerPriority == ESteerPriority.HeardThreat || eSteerPriority == ESteerPriority.LastHit)
		{
			base.Bot.Mover.PathFollower.Pause(0.33f);
		}
		else if (CurrentState != ESearchMove.Wait)
		{
			base.Bot.Mover.PathFollower.Unpause();
		}
		ESearchMove currentState = CurrentState;
		switch (CurrentState)
		{
		case ESearchMove.None:
			if (ShallStartPeek(shallSprint))
			{
				CurrentState = ESearchMove.MoveToStartPeek;
			}
			else if (MoveToPoint(FinalDestination.Value, shallSprint))
			{
				CurrentState = ESearchMove.DirectMove;
			}
			break;
		case ESearchMove.DirectMove:
			SetSpeedPose(speed, pose);
			MoveToPoint(FinalDestination.Value, shallSprint);
			break;
		case ESearchMove.Advance:
			if (_advanceTime < 0f)
			{
				_advanceTime = Time.time + 5f;
			}
			if (_advanceTime < Time.time)
			{
				_advanceTime = -1f;
				CurrentState = ESearchMove.None;
				PathFinder.FinishedPeeking = true;
			}
			else
			{
				SetSpeedPose(speed, pose);
				MoveToPoint(FinalDestination.Value, shallSprint);
			}
			break;
		case ESearchMove.MoveToStartPeek:
		{
			PeekPosition? peekPosition = PeekPoints?.PeekStart;
			if (peekPosition.HasValue && !BotIsAtPoint(peekPosition.Value.Point))
			{
				SetSpeedPose(speed, pose);
				if (MoveToPoint(peekPosition.Value.Point, shallSprint))
				{
					break;
				}
			}
			CurrentState = ESearchMove.MoveToEndPeek;
			break;
		}
		case ESearchMove.MoveToEndPeek:
		{
			PeekPosition? peekPosition = PeekPoints?.PeekEnd;
			if (peekPosition.HasValue && !BotIsAtPoint(peekPosition.Value.Point))
			{
				SetSpeedPose(speed, pose);
				if (MoveToPoint(peekPosition.Value.Point, shallSprint))
				{
					break;
				}
			}
			CurrentState = ESearchMove.Wait;
			NextState = ESearchMove.MoveToDangerPoint;
			break;
		}
		case ESearchMove.MoveToDangerPoint:
		{
			Vector3? val = PeekPoints?.DangerPoint;
			if (val.HasValue && !BotIsAtPoint(val.Value))
			{
				SetSpeedPose(speed, pose);
				if (MoveToPoint(val.Value, shallSprint))
				{
					break;
				}
			}
			CurrentState = ESearchMove.Advance;
			break;
		}
		case ESearchMove.Wait:
			if (WaitAtPoint())
			{
				base.Bot.Mover.SetTargetMoveSpeed(0f);
				base.Bot.Mover.SetTargetPose(0.75f);
			}
			else
			{
				base.Bot.Mover.SetTargetMoveSpeed(speed);
				base.Bot.Mover.SetTargetPose(pose);
				CurrentState = NextState;
			}
			break;
		}
		if (currentState != CurrentState)
		{
			LastState = currentState;
		}
	}

	private bool ShallSwapToSprint(bool shallSprint, float speed, float pose)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!shallSprint)
		{
			return false;
		}
		ESearchMove currentState = CurrentState;
		ESearchMove eSearchMove = currentState;
		if ((uint)eSearchMove <= 1u)
		{
			return false;
		}
		if (!MoveToPoint(FinalDestination.Value, shallSprint: true))
		{
			return false;
		}
		LastState = CurrentState;
		CurrentState = ESearchMove.DirectMove;
		SetSpeedPose(speed, pose);
		return true;
	}

	private bool CheckEndPeek()
	{
		ESearchMove currentState = CurrentState;
		ESearchMove eSearchMove = currentState;
		if ((uint)eSearchMove <= 1u || (uint)(eSearchMove - 5) <= 1u)
		{
			return false;
		}
		if (!PeekPoints.HasValue)
		{
			return true;
		}
		if (PathFinder.FinishedPeeking)
		{
			return true;
		}
		return false;
	}

	private void GetSpeedandPose(out float speed, out float pose, bool sprinting, bool stealthy)
	{
		speed = 1f;
		pose = 1f;
		if (!sprinting && !base.Player.IsSprintEnabled && !_Running && !base.Bot.Mover.PathFollower.Running && !GetIndoorsSpeedPose(stealthy, out speed, out pose))
		{
			if (_searchSettings.Sneaky && base.Bot.Cover.CoverPoints.Count > 2 && Time.time - base.BotOwner.Memory.UnderFireTime > 30f)
			{
				speed = 0.25f;
				pose = 0.6f;
			}
			else if (stealthy)
			{
				speed = 0.5f;
				pose = 0.7f;
			}
		}
	}

	private bool GetIndoorsSpeedPose(bool stealthy, out float speed, out float pose)
	{
		speed = 1f;
		pose = 1f;
		if (!base.Bot.Memory.Location.IsIndoors)
		{
			return false;
		}
		PersonalitySearchSettings searchSettings = _searchSettings;
		if (searchSettings.Sneaky)
		{
			speed = searchSettings.SneakySpeed;
			pose = searchSettings.SneakyPose;
		}
		else if (stealthy)
		{
			speed = 0.33f;
			pose = 1f;
		}
		return true;
	}

	private void CheckShallWaitandReload()
	{
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null)
		{
			BotReload reload = weaponManager.Reload;
			if (((reload != null) ? new bool?(reload.Reloading) : ((bool?)null)) == true && CurrentState != ESearchMove.Wait)
			{
				NextState = CurrentState;
				CurrentState = ESearchMove.Wait;
			}
		}
	}

	private bool ShallStartPeek(bool shallSprint)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (shallSprint)
		{
			return false;
		}
		if (PeekPoints.HasValue && MoveToPoint(PeekPoints.Value.PeekStart.Point, shallSprint))
		{
			return true;
		}
		return false;
	}

	private bool ShallDirectMove(bool shallSprint)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (shallSprint)
		{
			return true;
		}
		if (!PeekPoints.HasValue)
		{
			return true;
		}
		if (MoveToPoint(FinalDestination.Value, shallSprint))
		{
			return true;
		}
		return false;
	}

	private void SetSpeedPose(float speed, float pose)
	{
		base.Bot.Mover.SetTargetMoveSpeed(speed);
		base.Bot.Mover.SetTargetPose(pose);
	}

	public void Reset()
	{
		ResetStates();
		PathFinder.Reset();
	}

	public void ResetStates()
	{
		CurrentState = ESearchMove.None;
		LastState = ESearchMove.None;
		NextState = ESearchMove.None;
	}

	public bool BotIsAtPoint(Vector3 point, float reachDist = 0.5f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return DistanceToDestination(point) < reachDist;
	}

	public float DistanceToDestination(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = point - base.Bot.Position;
		return ((Vector3)(ref val)).magnitude;
	}
}
