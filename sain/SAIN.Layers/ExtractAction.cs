using System;
using Systems.Effects;
using Comfort.Common;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using EFT.Interactive;
using SAIN.Components;
using SAIN.Components.BotController;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Memory;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers;

internal class ExtractAction : CombatAction, ISAINAction
{
	private float _sayExitLocatedTime;

	private bool shallSprint;

	private bool ExtractStarted = false;

	private float ReCalcPathTimer = 0f;

	private float ExtractTimer = -1f;

	public static float MinDistanceToStartExtract { get; } = 6f;

	private Vector3? Exfil => base.Bot.Memory.Extract.ExfilPosition;

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public ExtractAction(BotOwner bot)
		: base(bot, "Extract")
	{
	}

	public override void Start()
	{
		Toggle(value: true);
		base.Bot.Memory.Extract.ExtractStatus = EExtractStatus.Extracting;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Memory.Extract.ExtractStatus = EExtractStatus.None;
		((CustomLogic)this).BotOwner.Mover.MovementResume();
	}

	public override void Update(ActionData data)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		StartProfilingSample("Update");
		bool flag = IsFightingEnemy();
		updateShallSprint(flag);
		Vector3 value = Exfil.Value;
		Vector3 val = Exfil.Value - ((CustomLogic)this).BotOwner.Position;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		if (ExtractStarted)
		{
			SetStatus(EExtractStatus.ExtractingNow);
			StartExtract(value);
			base.Bot.Mover.SetTargetPose(0f);
			base.Bot.Mover.SetTargetMoveSpeed(0f);
			if (_sayExitLocatedTime < Time.time)
			{
				_sayExitLocatedTime = Time.time + 10f;
				base.Bot.Talk.GroupSay((EPhraseTrigger)91, null, withGroupDelay: true, 70f);
			}
		}
		else
		{
			if (flag)
			{
				SetStatus(EExtractStatus.Fighting);
			}
			else
			{
				SetStatus(EExtractStatus.MovingTo);
			}
			MoveToExtract(sqrMagnitude, value);
			base.Bot.Mover.SetTargetPose(1f);
			base.Bot.Mover.SetTargetMoveSpeed(1f);
		}
		updateSteering();
		EndProfilingSample();
	}

	private void SetStatus(EExtractStatus status)
	{
		base.Bot.Memory.Extract.ExtractStatus = status;
	}

	private bool IsFightingEnemy()
	{
		return base.Bot.Enemy != null && base.Bot.Enemy.Seen && (base.Bot.Enemy.Path.PathLength < 50f || base.Bot.Enemy.InLineOfSight);
	}

	private void updateShallSprint(bool fightingEnemy)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		float normalValue = base.Bot.Player.Physical.Stamina.NormalValue;
		if (base.Bot.Player.AIData.EnvironmentId != 0)
		{
			shallSprint = false;
		}
		else if (fightingEnemy)
		{
			shallSprint = false;
		}
		else if (normalValue > 0.75f)
		{
			shallSprint = true;
		}
		else if (normalValue < 0.2f)
		{
			shallSprint = false;
		}
		if (!((CustomLogic)this).BotOwner.GetPlayer.MovementContext.CanSprint)
		{
			shallSprint = false;
		}
		if (Exfil.HasValue)
		{
			Vector3 val = Exfil.Value - ((CustomLogic)this).BotOwner.Position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < 8f)
			{
				shallSprint = false;
			}
		}
	}

	private void updateSteering()
	{
		Enemy enemy = base.Bot.Enemy;
		if (!base.Shoot.ShootAnyVisibleEnemies(enemy) && !base.Bot.Suppression.TrySuppressEnemy(enemy) && !base.Bot.Steering.SteerByPriority(enemy))
		{
			base.Bot.Steering.LookToMovingDirection();
		}
	}

	private void MoveToExtract(float distance, Vector3 point)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((CustomLogic)this).BotOwner.Mover != null && !ExtractStarted)
		{
			ReCalcPath(point);
			if (distance > MinDistanceToStartExtract * 2f)
			{
				ExtractStarted = false;
			}
			if (distance < MinDistanceToStartExtract)
			{
				ExtractStarted = true;
			}
		}
	}

	private void ReCalcPath(Vector3 point)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Invalid comparison between Unknown and I4
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I4
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (ReCalcPathTimer > Time.time)
		{
			return;
		}
		ExtractTimer = -1f;
		ReCalcPathTimer = Time.time + 4f;
		base.Bot.Memory.Extract.ExtractStatus = EExtractStatus.MovingTo;
		if (shallSprint)
		{
			base.Bot.Mover.RunToPoint(point, ESprintUrgency.Low, stopSprintEnemyVisible: true, checkSameWay: true, mustHaveCompletePath: false);
		}
		else
		{
			base.Bot.Mover.GoToPoint(point, out var _, -1f, crawl: false, slowAtEnd: true, mustHaveCompletePath: false);
		}
		Vector3 position = base.Bot.Mover.PathFollower.MoveData.LastCorner.Position;
		bool flag = false;
		NavMeshPathStatus currentPathStatus = base.Bot.Mover.CurrentPathStatus;
		float num = Vector3.Distance(((CustomLogic)this).BotOwner.Position, position);
		bool flag2 = (int)currentPathStatus == 1 && num < BotExtractManager.MinDistanceToExtract;
		if ((int)currentPathStatus == 2 || flag2)
		{
			if (SAINPlugin.DebugSettings.Logs.DebugExtract)
			{
				Logger.LogWarning($"{((Object)((CustomLogic)this).BotOwner).name} has an invalid or incomplete path to extract. Status={currentPathStatus}, DistanceToEOP={num}");
			}
			BotManagerComponent.Instance.BotExtractManager.ResetExfilSearchTime(base.Bot);
			base.Bot.Memory.Extract.ExfilPoint = null;
			base.Bot.Memory.Extract.ExfilPosition = null;
		}
	}

	public void StartExtract(Vector3 point)
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		base.Bot.Memory.Extract.ExtractStatus = EExtractStatus.ExtractingNow;
		if (ExtractTimer == -1f)
		{
			ExtractTimer = BotManagerComponent.Instance.BotExtractManager.GetExfilTime(base.Bot.Memory.Extract.ExfilPoint);
			ActivateExfil(base.Bot.Memory.Extract.ExfilPoint);
			float num = ExtractTimer - Time.time;
			Logger.LogInfo($"{((Object)((CustomLogic)this).BotOwner).name} Starting Extract Timer of {num}");
			((CustomLogic)this).BotOwner.Mover.MovementPause(num, true);
		}
		if (ExtractTimer < Time.time)
		{
			Logger.LogInfo($"{((Object)((CustomLogic)this).BotOwner).name} Extracted at {point} for extract {base.Bot.Memory.Extract.ExfilPoint.Settings.Name} at {DateTime.UtcNow}");
			BotManagerComponent.Instance?.BotExtractManager?.LogExtractionOfBot(((CustomLogic)this).BotOwner, point, base.Bot.Memory.Extract.ExtractReason.ToString(), base.Bot.Memory.Extract.ExfilPoint);
			IBotGame instance = Singleton<IBotGame>.Instance;
			Player player = base.Bot.Player;
			Singleton<Effects>.Instance.EffectsCommutator.StopBleedingForPlayer((IPlayer)(object)player);
			((CustomLogic)this).BotOwner.Deactivate();
			((CustomLogic)this).BotOwner.Dispose();
			instance.BotsController.BotDied(((CustomLogic)this).BotOwner);
			instance.BotsController.DestroyInfo(player);
			Object.DestroyImmediate((Object)(object)((Component)((CustomLogic)this).BotOwner).gameObject);
			Object.Destroy((Object)(object)((CustomLogic)this).BotOwner);
		}
	}

	private void ActivateExfil(ExfiltrationPoint exfil)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		exfil.OnItemTransferred((IPlayer)(object)base.Bot.Player);
		if ((int)exfil.Status != 2)
		{
			return;
		}
		EExfiltrationType exfiltrationType = exfil.Settings.ExfiltrationType;
		EExfiltrationType val = exfiltrationType;
		switch ((int)val)
		{
		case 0:
			exfil.SetStatusLogged((EExfiltrationStatus)4, "Proceed-3");
			break;
		case 1:
			exfil.SetStatusLogged((EExfiltrationStatus)3, "Proceed-1");
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo("bot " + ((Object)base.Bot).name + " has started the VEX exfil");
			}
			break;
		case 2:
			exfil.SetStatusLogged((EExfiltrationStatus)6, "Proceed-2");
			break;
		}
	}
}
