using System.Collections.Generic;
using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo.Cover;

internal class ShiftCoverAction : CombatAction, ISAINAction
{
	private readonly List<CoverPoint> UsedPoints = new List<CoverPoint>();

	private CoverPoint NewPoint;

	public ShiftCoverAction(BotOwner bot)
		: base(bot, "ShiftCoverAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		StartProfilingSample("Update");
		base.Shoot.ShootAnyVisibleEnemies(base.Bot.Enemy);
		base.Bot.Steering.SteerByPriority(base.Bot.Enemy);
		if (NewPoint == null && FindPointToGo())
		{
			base.Bot.Mover.SetTargetMoveSpeed(GetSpeed());
			base.Bot.Mover.SetTargetPose(GetPose());
		}
		else if (NewPoint != null && NewPoint.StraightDistanceStatus == CoverStatus.InCover)
		{
			base.Bot.Decision.EnemyDecisions.ShiftCoverComplete = true;
		}
		else if (NewPoint != null)
		{
			base.Bot.Mover.SetTargetMoveSpeed(GetSpeed());
			base.Bot.Mover.SetTargetPose(GetPose());
			base.Bot.Mover.GoToPoint(NewPoint.Position, out var _);
		}
		else
		{
			base.Bot.Decision.EnemyDecisions.ShiftCoverComplete = true;
		}
		EndProfilingSample();
	}

	private float GetSpeed()
	{
		PersonalityBehaviorSettings personalitySettings = base.Bot.Info.PersonalitySettings;
		return base.Bot.HasEnemy ? personalitySettings.Cover.MoveToCoverHasEnemySpeed : personalitySettings.Cover.MoveToCoverNoEnemySpeed;
	}

	private float GetPose()
	{
		PersonalityBehaviorSettings personalitySettings = base.Bot.Info.PersonalitySettings;
		return base.Bot.HasEnemy ? personalitySettings.Cover.MoveToCoverHasEnemyPose : personalitySettings.Cover.MoveToCoverNoEnemyPose;
	}

	private bool FindPointToGo()
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (NewPoint != null)
		{
			return true;
		}
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null)
		{
			if (NewPoint == null)
			{
				if (!UsedPoints.Contains(coverInUse))
				{
					UsedPoints.Add(coverInUse);
				}
				List<CoverPoint> coverPoints = base.Bot.Cover.CoverFinder.CoverPoints;
				for (int i = 0; i < coverPoints.Count; i++)
				{
					CoverPoint coverPoint = coverPoints[i];
					if (!(coverPoint.CoverHeight > coverInUse.CoverHeight) || UsedPoints.Contains(coverPoint))
					{
						continue;
					}
					for (int j = 0; j < UsedPoints.Count; j++)
					{
						Vector3 val = UsedPoints[j].Position - coverPoint.Position;
						if (((Vector3)(ref val)).sqrMagnitude > 5f && base.Bot.Mover.GoToPoint(coverPoint.Position, out var _))
						{
							base.Bot.Cover.CoverInUse = coverPoint;
							NewPoint = coverPoint;
							return true;
						}
					}
				}
			}
			if (NewPoint == null)
			{
				base.Bot.Decision.EnemyDecisions.ShiftCoverComplete = true;
			}
		}
		return false;
	}

	public override void Start()
	{
		Toggle(value: true);
		base.Bot.Decision.EnemyDecisions.ShiftCoverComplete = false;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Cover.CheckResetCoverInUse();
		NewPoint = null;
		UsedPoints.Clear();
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		stringBuilder.AppendLine("Shift Cover Info");
		SAINCoverClass cover = base.Bot.Cover;
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
		if (NewPoint != null)
		{
			stringBuilder.AppendLine("Cover In Use");
			GClass1437.AppendLabeledValue(stringBuilder, "Status", $"{NewPoint.StraightDistanceStatus}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Height / Value", $"{NewPoint.CoverHeight} {NewPoint.HardData.Value}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Length", $"{NewPoint.PathLength}", Color.white, Color.yellow, true);
			Vector3 val = NewPoint.Position - base.Bot.Position;
			GClass1437.AppendLabeledValue(stringBuilder, "Straight Distance", $"{((Vector3)(ref val)).magnitude}", Color.white, Color.yellow, true);
		}
	}
}
