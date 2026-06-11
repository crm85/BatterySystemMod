using System.Collections.Generic;
using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo.Cover;

internal class WalkToCoverAction : CombatAction, ISAINAction
{
	private float _nextUpdateCoverTime;

	private bool _wasCrawling;

	private float RecalcPathTimer = 0f;

	private CoverPoint _coverDestination;

	private float _timeStart;

	public WalkToCoverAction(BotOwner bot)
		: base(bot, "WalkToCoverAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		base.Bot.Mover.SetTargetPose(1f);
		Enemy currentTargetEnemy = base.Bot.CurrentTarget.CurrentTargetEnemy;
		if (_nextUpdateCoverTime < Time.time)
		{
			_nextUpdateCoverTime = Time.time + 0.1f;
			findCover(currentTargetEnemy);
			reCheckCover(currentTargetEnemy);
		}
		EngageEnemy(currentTargetEnemy);
		if (_coverDestination == null)
		{
			base.Bot.Mover.DogFight.DogFightMove(aggressive: true, currentTargetEnemy);
		}
		else if (base.Bot.Cover.CoverInUse == null)
		{
			base.Bot.Mover.DogFight.DogFightMove(aggressive: false, currentTargetEnemy);
		}
		EndProfilingSample();
	}

	private void findCover(Enemy enemy)
	{
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null && !coverInUse.CoverData.IsBad)
		{
			return;
		}
		base.Bot.Cover.SortPointsByPathDist();
		List<CoverPoint> coverPoints = base.Bot.Cover.CoverPoints;
		for (int i = 0; i < coverPoints.Count; i++)
		{
			CoverPoint coverPoint = coverPoints[i];
			if (checkMoveToCover(coverPoint, enemy))
			{
				RecalcPathTimer = Time.time + 1f;
				break;
			}
		}
	}

	private void reCheckCover(Enemy enemy)
	{
		CoverPoint coverInUse = base.Bot.Cover.CoverInUse;
		if (coverInUse != null && RecalcPathTimer < Time.time)
		{
			RecalcPathTimer = Time.time + 1f;
			if (!checkMoveToCover(coverInUse, enemy))
			{
				base.Bot.Cover.CoverInUse = null;
				_nextUpdateCoverTime = -1f;
			}
		}
	}

	private bool checkMoveToCover(CoverPoint coverPoint, Enemy enemy)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (coverPoint != null && !coverPoint.Spotted && !coverPoint.CoverData.IsBad)
		{
			bool crawl = base.Bot.Decision.CurrentSelfDecision != ESelfDecision.None && base.Bot.Player.MovementContext.CanProne && (_wasCrawling || (coverPoint.StraightDistanceStatus == CoverStatus.FarFromCover && base.Bot.Mover.Prone.ShallProneHide(enemy)));
			if (base.Bot.Mover.GoToPoint(coverPoint.Position, out var _, -1f, crawl))
			{
				_wasCrawling = base.Bot.Mover.Crawling;
				base.Bot.Cover.CoverInUse = coverPoint;
				_coverDestination = coverPoint;
				return true;
			}
		}
		return false;
	}

	private void EngageEnemy(Enemy Enemy)
	{
		if (Enemy == null)
		{
			base.Bot.Steering.SteerByPriority();
		}
		else if (!base.Shoot.ShootAnyVisibleEnemies(Enemy) && !base.Bot.Suppression.TrySuppressAnyEnemy(Enemy, base.Bot.EnemyController.EnemyLists.KnownEnemies) && !base.Bot.Steering.SteerByPriority(Enemy, lookRandom: false))
		{
			base.Bot.Steering.LookToLastKnownEnemyPosition(Enemy);
		}
	}

	public override void Start()
	{
		Toggle(value: true);
		_timeStart = Time.time;
	}

	public override void Stop()
	{
		Toggle(value: false);
		base.Bot.Mover.DogFight.ResetDogFightStatus();
		base.Bot.Cover.CheckResetCoverInUse();
		base.Bot.Suppression.ResetSuppressing();
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		stringBuilder.AppendLine("Walk To Cover Info");
		SAINCoverClass cover = base.Bot.Cover;
		GClass1437.AppendLabeledValue(stringBuilder, "CoverFinder State", $"{cover.CurrentCoverFinderState}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Cover Count", $"{cover.CoverPoints.Count}", Color.white, Color.yellow, true);
		DebugOverlay.AddMoveData(base.Bot, stringBuilder);
		if (base.Bot.CurrentTargetPosition.HasValue)
		{
			GClass1437.AppendLabeledValue(stringBuilder, "Current Target Position", $"{base.Bot.CurrentTargetPosition.Value}", Color.white, Color.yellow, true);
		}
		else
		{
			GClass1437.AppendLabeledValue(stringBuilder, "Current Target Position", (string)null, Color.white, Color.yellow, true);
		}
		if (_coverDestination != null)
		{
			stringBuilder.AppendLine("Cover Destination");
			GClass1437.AppendLabeledValue(stringBuilder, "Status", $"{_coverDestination.StraightDistanceStatus}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Height / Value", $"{_coverDestination.CoverHeight} {_coverDestination.HardData.Value}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Path Length", $"{_coverDestination.PathLength}", Color.white, Color.yellow, true);
			Vector3 val = _coverDestination.Position - base.Bot.Position;
			GClass1437.AppendLabeledValue(stringBuilder, "Straight Distance", $"{((Vector3)(ref val)).magnitude}", Color.white, Color.yellow, true);
		}
	}
}
