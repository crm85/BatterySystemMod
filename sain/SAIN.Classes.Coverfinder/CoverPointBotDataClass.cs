using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Classes.Coverfinder;

public class CoverPointBotDataClass
{
	private const int SPOTTED_HITINCOVER_COUNT_TOTAL = 3;

	private const int SPOTTED_HITINCOVER_COUNT_THIRDPARTY = 1;

	private const int SPOTTED_HITINCOVER_COUNT_CANTSEE = 2;

	private const int SPOTTED_HITINCOVER_COUNT_LEGS = 2;

	private const int SPOTTED_HITINCOVER_COUNT_UNKNOWN = 1;

	private const float HITINCOVER_MAX_DAMAGE = 120f;

	private const float HITINCOVER_MIN_DAMAGE = 40f;

	private const float HITINCOVER_DAMAGE_COEF = 3f;

	private const float DIST_COVER_INCOVER = 1f;

	private const float DIST_COVER_INCOVER_STAY = 1.25f;

	private const float DIST_COVER_CLOSE = 10f;

	private const float DIST_COVER_MID = 20f;

	private CoverStatus _straightDistStatus;

	private CoverStatus _pathLengthStatus;

	private readonly BotComponent Bot;

	public CoverPointClass CoverPoint { get; }

	public Vector3 Position
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return CoverPoint.CoverPosition;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			CoverPoint.CoverPosition = value;
		}
	}

	public float BotDistance { get; set; }

	public float PathLength
	{
		get
		{
			return PathData.PathLength;
		}
		set
		{
			PathData.PathLength = value;
		}
	}

	public bool Spotted
	{
		get
		{
			CoverHitCounts hitsInCover = _hitsInCover;
			if (hitsInCover.Spotted)
			{
				if (Time.time - hitsInCover.TimeSpotted > 2f)
				{
					ResetGetHit();
				}
				return hitsInCover.Spotted;
			}
			hitsInCover.Spotted = CheckSpotted();
			if (hitsInCover.Spotted)
			{
				hitsInCover.TimeSpotted = Time.time;
			}
			return hitsInCover.Spotted;
		}
	}

	public CoverStatus StraightDistanceStatus
	{
		get
		{
			float botDistance = BotDistance;
			if (_straightDistStatus == CoverStatus.InCover && botDistance <= 1.25f)
			{
				return _straightDistStatus;
			}
			_straightDistStatus = CheckStatus(botDistance);
			return _straightDistStatus;
		}
	}

	public CoverStatus PathDistanceStatus
	{
		get
		{
			float pathLength = PathLength;
			if (_pathLengthStatus == CoverStatus.InCover && pathLength <= 1.25f)
			{
				return _pathLengthStatus;
			}
			_pathLengthStatus = CheckStatus(pathLength);
			return _pathLengthStatus;
		}
	}

	public NavMeshPath PathToPoint => PathData.Path;

	public float CoverHeight => CoverPoint.Height;

	public Collider Collider => CoverPoint.Collider;

	public float LastHitInCoverTime { get; private set; }

	public int RoundedPathLength => PathData.RoundedPathLength;

	public bool BotInThisCover => StraightDistanceStatus == CoverStatus.InCover || PathDistanceStatus == CoverStatus.InCover;

	public PathData PathData { get; }

	private CoverHitCounts _hitsInCover { get; } = new CoverHitCounts();

	public void SetInUse(bool inInUse)
	{
		CoverPoint.IsInUse = inInUse;
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart partHit, Enemy currentEnemy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		int num = CalcHitCount(DamageInfoStruct);
		bool flag = partHit.IsLegs();
		CoverHitCounts hitsInCover = _hitsInCover;
		LastHitInCoverTime = Time.time;
		hitsInCover.Total += num;
		IPlayerOwner player = DamageInfoStruct.Player;
		IPlayer val = ((player != null) ? player.iPlayer : null);
		if (currentEnemy == null || val == null)
		{
			hitsInCover.Unknown += num;
		}
		else if (!(currentEnemy.EnemyPlayer.ProfileId == val.ProfileId))
		{
			Enemy enemy = Bot.EnemyController.GetEnemy(val.ProfileId, mustBeActive: false);
			if (enemy == null)
			{
				hitsInCover.Unknown += num;
				return;
			}
			if (flag && !enemy.IsVisible)
			{
				hitsInCover.Legs += num;
			}
			if (Vector3.Dot(enemy.EnemyDirectionNormal, CoverPoint.ProtectionDirection) < 0.25f)
			{
				hitsInCover.ThirdParty += num;
			}
		}
		else if (!currentEnemy.IsVisible)
		{
			if (flag)
			{
				hitsInCover.Legs += num;
			}
			hitsInCover.CantSee += num;
		}
	}

	public void ResetGetHit()
	{
		_hitsInCover.Reset();
	}

	public CoverPointBotDataClass(BotComponent bot, CoverPointClass coverPoint, PathData pathData)
	{
		Bot = bot;
		CoverPoint = coverPoint;
		PathData = pathData;
	}

	private static CoverStatus CheckStatus(float distance)
	{
		if (distance <= 1f)
		{
			return CoverStatus.InCover;
		}
		if (distance <= 10f)
		{
			return CoverStatus.CloseToCover;
		}
		if (distance <= 20f)
		{
			return CoverStatus.MidRangeToCover;
		}
		return CoverStatus.FarFromCover;
	}

	private bool CheckSpotted()
	{
		CoverHitCounts hitsInCover = _hitsInCover;
		int total = hitsInCover.Total;
		if (total == 0)
		{
			return false;
		}
		if (total >= 3)
		{
			return true;
		}
		if (hitsInCover.CantSee >= 2)
		{
			return true;
		}
		if (hitsInCover.Unknown >= 1)
		{
			return true;
		}
		if (hitsInCover.ThirdParty >= 1)
		{
			return true;
		}
		if (hitsInCover.Legs >= 2)
		{
			return true;
		}
		return false;
	}

	private static int CalcHitCount(DamageInfoStruct DamageInfoStruct)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		float damage = DamageInfoStruct.Damage;
		float num = 120f;
		float num2 = 3f;
		if (damage >= num)
		{
			return Mathf.RoundToInt(num2);
		}
		float num3 = 40f;
		float num4 = 1f;
		if (damage <= num3)
		{
			return Mathf.RoundToInt(num4);
		}
		float num5 = num - num3;
		float num6 = damage - num3;
		float num7 = Mathf.Lerp(num3, num, num6 / num5);
		return Mathf.RoundToInt(num7);
	}
}
