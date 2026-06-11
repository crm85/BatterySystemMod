using System;
using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo.Cover;

internal class DoSurgeryAction : CombatAction, ISAINAction
{
	private float _startSurgeryTime;

	private float _actionStartedTime;

	public DoSurgeryAction(BotOwner bot)
		: base(bot, "Surgery")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		checkDoSurgery();
		handleSteering();
		EndProfilingSample();
	}

	private void checkDoSurgery()
	{
		if (base.Bot.Medical.Surgery.AreaClearForSurgery)
		{
			base.Bot.Mover.PauseMovement(30f);
			base.Bot.Mover.PathFollower.Cancel();
			base.Bot.Mover.SetTargetMoveSpeed(0f);
			base.Bot.Cover.DuckInCover(base.Bot.Enemy);
			tryStartSurgery();
		}
		else
		{
			((CustomLogic)this).BotOwner.Mover.MovementResume();
			base.Bot.Mover.SetTargetPose(1f);
			base.Bot.Medical.Surgery.SurgeryStarted = false;
			base.Bot.Medical.TryCancelHeal();
			base.Bot.Mover.DogFight.DogFightMove(aggressive: false, base.Bot.Enemy);
		}
	}

	private void handleSteering()
	{
		if (!base.Bot.Steering.SteerByPriority(null, lookRandom: false) && !base.Bot.Steering.LookToLastKnownEnemyPosition(base.Bot.Enemy))
		{
			base.Bot.Steering.LookToRandomPosition();
		}
	}

	private bool tryStartSurgery()
	{
		if (tryStart())
		{
			return true;
		}
		if (checkFullHeal())
		{
			return true;
		}
		return false;
	}

	private bool tryStart()
	{
		GClass473 surgicalKit = ((CustomLogic)this).BotOwner.Medecine.SurgicalKit;
		if (_startSurgeryTime < Time.time && !((GClass468)surgicalKit).Using && surgicalKit.ShallStartUse())
		{
			base.Bot.Medical.Surgery.SurgeryStarted = true;
			surgicalKit.ApplyToCurrentPart((Action)onSurgeryDone);
			return true;
		}
		return false;
	}

	private bool checkFullHeal()
	{
		if (base.Bot.Medical.Surgery.SurgeryStarted = _actionStartedTime + 30f < Time.time)
		{
			Player player = base.Bot.Player;
			if (player != null)
			{
				ActiveHealthController activeHealthController = player.ActiveHealthController;
				if (activeHealthController != null)
				{
					activeHealthController.RestoreFullHealth();
				}
			}
			base.Bot.Decision.ResetDecisions(active: true);
			return true;
		}
		return false;
	}

	private void onSurgeryDone()
	{
		base.Bot.Medical.Surgery.SurgeryStarted = false;
		_actionStartedTime = Time.time;
		_startSurgeryTime = Time.time + 1f;
		if (((CustomLogic)this).BotOwner.Medecine.SurgicalKit.HaveWork)
		{
			if (base.Bot.Enemy != null && !(base.Bot.Enemy.TimeSinceSeen > 90f))
			{
				return;
			}
			Player player = base.Bot.Player;
			if (player != null)
			{
				ActiveHealthController activeHealthController = player.ActiveHealthController;
				if (activeHealthController != null)
				{
					activeHealthController.RestoreFullHealth();
				}
			}
			base.Bot.Decision.ResetDecisions(active: true);
		}
		else
		{
			base.Bot.Decision.ResetDecisions(active: true);
		}
	}

	public override void Start()
	{
		Toggle(value: true);
		base.Bot.Mover.PauseMovement(3f);
		_startSurgeryTime = Time.time + 1f;
		_actionStartedTime = Time.time;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Cover.CheckResetCoverInUse();
		base.Bot.Medical.Surgery.SurgeryStarted = false;
		((CustomLogic)this).BotOwner.MovementResume();
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		stringBuilder.AppendLine($"Health Status {base.Bot.Memory.Health.HealthStatus}");
		stringBuilder.AppendLine($"Surgery Started? {base.Bot.Medical.Surgery.SurgeryStarted}");
		stringBuilder.AppendLine($"Time Since Surgery Started {Time.time - base.Bot.Medical.Surgery.SurgeryStartTime}");
		stringBuilder.AppendLine($"Area Clear? {base.Bot.Medical.Surgery.AreaClearForSurgery}");
		stringBuilder.AppendLine($"ShallStartUse Surgery? {((CustomLogic)this).BotOwner.Medecine.SurgicalKit.ShallStartUse()}");
		stringBuilder.AppendLine($"IsBleeding? {((CustomLogic)this).BotOwner.Medecine.FirstAid.IsBleeding}");
	}
}
