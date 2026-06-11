using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo.Cover;

internal class HoldinCoverAction : CombatAction, ISAINAction
{
	private float _nextCheckPosTime;

	private Vector3 _position;

	private const float RAYCAST_LEAN_HITOBJECT_DIST = 0.5f;

	private LeanSetting CurrentLean;

	private float ChangeLeanTimer;

	private CoverPoint CoverInUse;

	public HoldinCoverAction(BotOwner bot)
		: base(bot, "HoldinCoverAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		checkPositionAdjustments();
		Enemy enemy = base.Bot.Enemy;
		if (!base.Shoot.ShootAnyVisibleEnemies(enemy) && !base.Bot.Suppression.TrySuppressEnemy(enemy))
		{
			base.Bot.Steering.SteerByPriority(enemy);
		}
		EndProfilingSample();
	}

	private void checkPositionAdjustments()
	{
		CoverPoint coverInUse = CoverInUse;
		if (coverInUse == null)
		{
			base.Bot.Mover.DogFight.DogFightMove(aggressive: true, base.Bot.Enemy);
			return;
		}
		adjustMyPosition();
		base.Bot.Cover.DuckInCover(base.Bot.Enemy);
		checkSetProne();
		checkSetLean();
	}

	private void adjustMyPosition()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!(_nextCheckPosTime < Time.time))
		{
			return;
		}
		_nextCheckPosTime = Time.time + 1f;
		Vector3 position = CoverInUse.Position;
		if (!base.Bot.Player.IsInPronePose)
		{
			Vector3 val = position - _position;
			if (((Vector3)(ref val)).sqrMagnitude > 0.25f)
			{
				_position = position;
				base.Bot.Mover.GoToPoint(position, out var _);
				return;
			}
		}
		base.Bot.Mover.StopMove();
	}

	private void checkSetProne()
	{
		if (base.Bot.Info.FileSettings.Move.PRONE_TOGGLE && GlobalSettingsClass.Instance.Move.PRONE_TOGGLE && base.Bot.Enemy != null && base.Bot.Player.MovementContext.CanProne && (double)base.Bot.Player.PoseLevel <= 0.1 && base.Bot.Enemy.IsVisible && ((CustomLogic)this).BotOwner.WeaponManager.Reload.Reloading)
		{
			base.Bot.Mover.Prone.SetProne(value: true);
		}
	}

	private void checkSetLean()
	{
		if (!base.Bot.Info.FileSettings.Move.LEAN_INCOVER_TOGGLE || !GlobalSettingsClass.Instance.Move.LEAN_INCOVER_TOGGLE || base.Bot.Suppression.IsSuppressed || base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None)
		{
			base.Bot.Mover.Lean.FastLean(LeanSetting.None);
			CurrentLean = LeanSetting.None;
		}
		else if (CurrentLean != LeanSetting.None && ShallHoldLean())
		{
			base.Bot.Mover.Lean.FastLean(CurrentLean);
			ChangeLeanTimer = Time.time + 0.66f;
		}
		else if (ChangeLeanTimer < Time.time)
		{
			setLean();
		}
	}

	private void setLean()
	{
		LeanSetting currentLean = CurrentLean;
		LeanSetting leanSetting = currentLean;
		LeanSetting leanSetting2;
		if ((uint)(leanSetting - 1) <= 1u)
		{
			leanSetting2 = LeanSetting.None;
			ChangeLeanTimer = Time.time + Random.Range(0.75f, 4f);
		}
		else
		{
			leanSetting2 = base.Bot.Mover.Lean.FindLeanFromBlindCornerAngle(base.Bot.Enemy);
			if (leanSetting2 == LeanSetting.None)
			{
				leanSetting2 = (EFTMath.RandomBool() ? LeanSetting.Left : LeanSetting.Right);
			}
			ChangeLeanTimer = Time.time + Random.Range(0.5f, 2f);
		}
		if (!checkLeanIntoObject(leanSetting2))
		{
			CurrentLean = leanSetting2;
			base.Bot.Mover.Lean.FastLean(leanSetting2);
		}
	}

	private bool checkLeanIntoObject(LeanSetting lean)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Vector3 headPosition = base.Bot.Transform.HeadPosition;
		Vector3 val = ((lean == LeanSetting.Right) ? base.Bot.Transform.DirectionData.Right() : base.Bot.Transform.DirectionData.Left());
		if ((uint)(lean - 1) <= 1u)
		{
			return Vector.Raycast(headPosition, headPosition + val * 0.5f, LayerMaskClass.HighPolyWithTerrainMask);
		}
		return false;
	}

	private bool ShallHoldLean()
	{
		if (base.Bot.Suppression.IsSuppressed)
		{
			return false;
		}
		Enemy enemy = base.Bot.Enemy;
		if (enemy == null || !enemy.Seen)
		{
			return false;
		}
		if (enemy.IsVisible && enemy.CanShoot)
		{
			return true;
		}
		if (enemy.TimeSinceSeen < 3f)
		{
			return true;
		}
		return false;
	}

	public override void Start()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Toggle(value: true);
		ChangeLeanTimer = Time.time + 2f;
		CoverInUse = base.Bot.Cover.CoverInUse;
		if (CoverInUse != null)
		{
			_position = CoverInUse.Position;
		}
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Cover.CheckResetCoverInUse();
		base.Bot.Mover.Prone.SetProne(value: false);
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		stringBuilder.AppendLine("Hold In Cover Info");
		SAINCoverClass cover = base.Bot.Cover;
		GClass1437.AppendLabeledValue(stringBuilder, "CoverFinder State", $"{cover.CurrentCoverFinderState}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Cover Count", $"{cover.CoverPoints.Count}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Current Cover Status", $"{CoverInUse?.StraightDistanceStatus}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Current Cover Height", $"{CoverInUse?.HardData.Height}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Current Cover Value", $"{CoverInUse?.HardData.Value}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "CoverFinder State", $"{cover.CurrentCoverFinderState}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Cover Count", $"{cover.CoverPoints.Count}", Color.white, Color.yellow, true);
		if (base.Bot.CurrentTargetPosition.HasValue)
		{
			GClass1437.AppendLabeledValue(stringBuilder, "Current Target Position", $"{base.Bot.CurrentTargetPosition.Value}", Color.white, Color.yellow, true);
		}
		else
		{
			GClass1437.AppendLabeledValue(stringBuilder, "Current Target Position", (string)null, Color.white, Color.yellow, true);
		}
		if (CoverInUse != null)
		{
			stringBuilder.AppendLine("Cover In Use");
			GClass1437.AppendLabeledValue(stringBuilder, "Status", $"{CoverInUse.StraightDistanceStatus}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Height / Value", $"{CoverInUse.CoverHeight} {CoverInUse.HardData.Value}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Length", $"{CoverInUse.PathLength}", Color.white, Color.yellow, true);
			Vector3 val = CoverInUse.Position - base.Bot.Position;
			GClass1437.AppendLabeledValue(stringBuilder, "Straight Distance", $"{((Vector3)(ref val)).magnitude}", Color.white, Color.yellow, true);
		}
	}
}
