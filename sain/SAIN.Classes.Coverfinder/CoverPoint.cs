using System;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Classes.Coverfinder;

public class CoverPoint
{
	private const int SPOTTED_HITINCOVER_COUNT_TOTAL = 3;

	private const int SPOTTED_HITINCOVER_COUNT_THIRDPARTY = 1;

	private const int SPOTTED_HITINCOVER_COUNT_CANTSEE = 2;

	private const int SPOTTED_HITINCOVER_COUNT_LEGS = 2;

	private const int SPOTTED_HITINCOVER_COUNT_UNKNOWN = 1;

	private const float HITINCOVER_MAX_DAMAGE = 120f;

	private const float HITINCOVER_MIN_DAMAGE = 40f;

	private const float HITINCOVER_DAMAGE_COEF = 3f;

	private const float CHECKDIST_MAX_DIST = 50f;

	private const float CHECKDIST_MIN_DIST = 10f;

	private const float CHECKDIST_MAX_DELAY = 1f;

	private const float CHECKDIST_MIN_DELAY = 0.1f;

	private const float DIST_COVER_INCOVER = 1f;

	private const float DIST_COVER_INCOVER_STAY = 1.25f;

	private const float DIST_COVER_CLOSE = 10f;

	private const float DIST_COVER_MID = 20f;

	private string _lastCheckedProfileId;

	private float _nextGetDistTime;

	private static int _count;

	private readonly BotComponent Bot;

	public CoverData CoverData { get; } = new CoverData();

	public Vector3 Position
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return CoverData.Position;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = CoverData.Position;
			Vector3 val = value - position;
			if (!((double)((Vector3)(ref val)).sqrMagnitude < 0.001))
			{
				updateDirAndPos(value);
				this.OnPositionUpdated?.Invoke(value);
			}
		}
	}

	public float Distance
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (_nextGetDistTime < Time.time)
			{
				Vector3 val = Position - Bot.Position;
				float magnitude = ((Vector3)(ref val)).magnitude;
				CoverData.BotDistance = magnitude;
				_nextGetDistTime = Time.time + calcMagnitudeDelay(magnitude);
			}
			return CoverData.BotDistance;
		}
	}

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
			hitsInCover.Spotted = checkSpotted();
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
			float distance = Distance;
			if (CoverData.StraightLengthStatus == CoverStatus.InCover && distance <= 1.25f)
			{
				return CoverStatus.InCover;
			}
			CoverData.StraightLengthStatus = checkStatus(distance);
			return CoverData.StraightLengthStatus;
		}
	}

	public CoverStatus PathDistanceStatus
	{
		get
		{
			float pathLength = PathLength;
			if (CoverData.PathLengthStatus == CoverStatus.InCover && pathLength <= 1.25f)
			{
				return CoverStatus.InCover;
			}
			CoverData.PathLengthStatus = checkStatus(pathLength);
			return CoverData.PathLengthStatus;
		}
	}

	public NavMeshPath PathToPoint => PathData.Path;

	public float CoverHeight => HardData.Height;

	public Collider Collider => HardColliderData.Collider;

	public SAINHardColliderData HardColliderData { get; }

	public float LastHitInCoverTime { get; private set; }

	public bool IsCurrent => Bot.Cover.CoverInUse == this;

	public int RoundedPathLength => PathData.RoundedPathLength;

	public bool BotInThisCover => IsCurrent && (StraightDistanceStatus == CoverStatus.InCover || PathDistanceStatus == CoverStatus.InCover);

	public SAINHardCoverData HardData { get; }

	public PathData PathData { get; }

	private CoverHitCounts _hitsInCover { get; } = new CoverHitCounts();

	public event Action<Vector3> OnPositionUpdated;

	public bool ShallUpdate(string targetProfileId)
	{
		if (_lastCheckedProfileId != targetProfileId)
		{
			_lastCheckedProfileId = targetProfileId;
			return true;
		}
		float num = (IsCurrent ? 0.2f : 0.5f);
		if (CoverData.TimeSinceUpdated >= num)
		{
			return true;
		}
		return false;
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart partHit, Enemy currentEnemy)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		int num = calcHitCount(DamageInfoStruct);
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
			if (Vector3.Dot(enemy.EnemyDirectionNormal, CoverData.ProtectionDirection) < 0.25f)
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

	public CoverPoint(BotComponent bot, SAINHardColliderData colliderData, PathData pathData, Vector3 coverPosition)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Bot = bot;
		HardColliderData = colliderData;
		PathData = pathData;
		Bounds bounds = colliderData.Collider.bounds;
		Vector3 size = ((Bounds)(ref bounds)).size;
		HardData = new SAINHardCoverData
		{
			Id = _count,
			Height = size.y,
			Value = (size.x + size.y + size.z).Round10()
		};
		updateDirAndPos(coverPosition);
		_count++;
	}

	private void updateDirAndPos(Vector3 coverPosition)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		CoverData.Position = coverPosition;
		Vector3 val = HardColliderData.Position - coverPosition;
		val.y = 0f;
		CoverData.ProtectionDirection = ((Vector3)(ref val)).normalized;
		CoverData.TimeLastUpdated = Time.time;
	}

	private CoverStatus checkStatus(float distance)
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

	private bool checkSpotted()
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

	private int calcHitCount(DamageInfoStruct DamageInfoStruct)
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

	private float calcMagnitudeDelay(float dist)
	{
		float num = 1f;
		float num2 = 50f;
		if (dist >= num2)
		{
			return num;
		}
		float num3 = 0.1f;
		float num4 = 10f;
		if (dist <= num4)
		{
			return num3;
		}
		float num5 = num2 - num4;
		float num6 = dist - num4;
		return Mathf.Lerp(num3, num, num6 / num5);
	}
}
