using System.Collections.Generic;
using Comfort.Common;
using EFT;
using SAIN.Classes.Coverfinder;
using SAIN.Components;
using SAIN.Components.CoverFinder;
using SAIN.Helpers;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class CoverAnalyzer : BotBase
{
	private readonly CoverFinderComponent CoverFinder;

	private const float POSITION_FINAL_MIN_DOT = 0.5f;

	private const float POSITION_EDGE_MIN_DOT = 0.5f;

	private const float POSTIION_EDGE_SAMPLE_RANGE = 0.5f;

	private const float POSITION_SAMPLE_RANGE = 1f;

	private const float PATH_SAME_DIST_MIN_RATIO = 0.66f;

	private const float PATH_SAME_CHECK_DIST = 0.1f;

	private const float PATH_NODE_MIN_DIST_SQR = 0.25f;

	private const float PATH_NODE_FIRST_DOT_MAX = 0.5f;

	private static Collider[] _playerColliderArray = (Collider[])(object)new Collider[5];

	private Vector3 OriginPoint => CoverFinder.OriginPoint;

	private Vector3 TargetPoint => CoverFinder.TargetPoint;

	private float CoverMinEnemyDistSqr => CoverFinderComponent.CoverMinEnemyDistSqr;

	private static bool DebugCoverFinder => CoverFinderComponent.DebugCoverFinder;

	private static float CoverMinHeight => CoverFinderComponent.CoverMinHeight;

	public CoverAnalyzer(BotComponent botOwner, CoverFinderComponent coverFinder)
		: base(botOwner)
	{
		CoverFinder = coverFinder;
	}

	public bool CheckCollider(Collider collider, TargetData targetData, out CoverPoint coverPoint, out string reason)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		coverPoint = null;
		SAINHardColliderData sAINHardColliderData = new SAINHardColliderData(collider);
		ColliderData colliderData = new ColliderData(sAINHardColliderData, targetData);
		if (!GetPlaceToMove(colliderData, sAINHardColliderData, targetData, out var place))
		{
			reason = "noPlaceToMove";
			return false;
		}
		if (!checkPositionVsOtherBots(place))
		{
			reason = "tooCloseToAnotherBot";
			return false;
		}
		if (isPositionSpotted(place))
		{
			reason = "tooCloseToSpottedPoint";
			return false;
		}
		if (!checkDistToTarget(place, targetData))
		{
			reason = "tooCloseToTarget";
			return false;
		}
		if (!visibilityCheck(place, targetData, colliderData, sAINHardColliderData))
		{
			reason = "pointVisibleToTarget";
			return false;
		}
		PathData pathData = new PathData(new NavMeshPath());
		if (!CheckPath(place, pathData, targetData))
		{
			reason = "badPath";
			return false;
		}
		reason = string.Empty;
		coverPoint = new CoverPoint(base.Bot, sAINHardColliderData, pathData, place);
		return true;
	}

	public bool RecheckCoverPoint(CoverPoint coverPoint, TargetData targetData, out string reason)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		SAINHardColliderData hardColliderData = coverPoint.HardColliderData;
		ColliderData colliderData = new ColliderData(hardColliderData, targetData);
		if (!GetPlaceToMove(colliderData, hardColliderData, targetData, out var place))
		{
			reason = "noPlaceToMove";
			return false;
		}
		if (!checkPositionVsOtherBots(place))
		{
			reason = "tooCloseToAnotherBot";
			return false;
		}
		if (coverPoint.StraightDistanceStatus == CoverStatus.InCover)
		{
			coverPoint.Position = place;
			reason = "inCover";
			return true;
		}
		if (isPositionSpotted(place))
		{
			reason = "tooCloseToSpottedPoint";
			return false;
		}
		if (!checkDistToTarget(place, targetData))
		{
			reason = "tooCloseToTarget";
			return false;
		}
		if (!visibilityCheck(place, targetData, colliderData, hardColliderData))
		{
			reason = "pointVisibleToTarget";
			return false;
		}
		if (!CheckPath(place, coverPoint.PathData, targetData))
		{
			reason = "badPath";
			return false;
		}
		coverPoint.Position = place;
		reason = string.Empty;
		return true;
	}

	public bool GetPlaceToMove(ColliderData colliderData, SAINHardColliderData hardData, TargetData targetDirections, out Vector3 place)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!checkColliderDirectionvsTargetDirection(colliderData, targetDirections))
		{
			place = Vector3.zero;
			return false;
		}
		if (!findSampledPosition(colliderData, hardData, 1f, out place))
		{
			place = Vector3.zero;
			return false;
		}
		return true;
	}

	private bool checkFinalPositionDirection(ColliderData colliderDirs, SAINHardColliderData hardData, TargetData targetDirs, Vector3 place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = place - hardData.Position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 dirTargetToColliderNormal = colliderDirs.dirTargetToColliderNormal;
		float num = Vector3.Dot(normalized, dirTargetToColliderNormal);
		return num > 0.5f;
	}

	private bool findSampledPosition(ColliderData colliderDirs, SAINHardColliderData hardData, float navSampleRange, out Vector3 coverPosition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hardData.Position + colliderDirs.dirTargetToColliderNormal;
		NavMeshHit val2 = default(NavMeshHit);
		if (!NavMesh.SamplePosition(val, ref val2, navSampleRange, -1))
		{
			coverPosition = Vector3.zero;
			return false;
		}
		coverPosition = findEdge(((NavMeshHit)(ref val2)).position, colliderDirs);
		return true;
	}

	private Vector3 findEdge(Vector3 navMeshHit, ColliderData colliderDirs)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.FindClosestEdge(navMeshHit, ref val, -1))
		{
			Vector3 normal = ((NavMeshHit)(ref val)).normal;
			Vector3 dirTargetToColliderNormal = colliderDirs.dirTargetToColliderNormal;
			if (Vector3.Dot(normal, dirTargetToColliderNormal) > 0.5f)
			{
				Vector3 val2 = ((NavMeshHit)(ref val)).position + colliderDirs.dirTargetToColliderNormal;
				NavMeshHit val3 = default(NavMeshHit);
				if (NavMesh.SamplePosition(val2, ref val3, 0.5f, -1))
				{
					return ((NavMeshHit)(ref val3)).position;
				}
			}
		}
		return navMeshHit;
	}

	private bool checkColliderDirectionvsTargetDirection(ColliderData colliderDirs, TargetData targetDirs)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(targetDirs.DirBotToTargetNormal, colliderDirs.dirBotToColliderNormal);
		if (num <= 0.33f)
		{
			return true;
		}
		float colliderDistanceToBot = colliderDirs.ColliderDistanceToBot;
		float targetDistance = targetDirs.TargetDistance;
		if (num <= 0.5f)
		{
			return colliderDistanceToBot < targetDistance * 0.75f;
		}
		if (num <= 0.66f)
		{
			return colliderDistanceToBot < targetDistance * 0.66f;
		}
		return colliderDistanceToBot < targetDistance * 0.5f;
	}

	private bool CheckPosition(Vector3 coverPosition, TargetData targetData, ColliderData colliderData, SAINHardColliderData hardData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = coverPosition - targetData.TargetPosition;
		return ((Vector3)(ref val)).sqrMagnitude > CoverMinEnemyDistSqr && !isPositionSpotted(coverPosition) && checkPositionVsOtherBots(coverPosition) && visibilityCheck(coverPosition, targetData, colliderData, hardData);
	}

	private bool checkDistToTarget(Vector3 coverPosition, TargetData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = coverPosition - data.TargetPosition;
		return ((Vector3)(ref val)).sqrMagnitude > CoverMinEnemyDistSqr;
	}

	private bool isPositionSpotted(Vector3 position)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		foreach (SpottedCoverPoint spottedCoverPoint in CoverFinder.SpottedCoverPoints)
		{
			Vector3 position2 = spottedCoverPoint.CoverPoint.Position;
			if (!spottedCoverPoint.IsValidAgain && spottedCoverPoint.TooClose(position2, position))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckPath(Vector3 position, PathData pathData, TargetData targetData)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		NavMeshPath path = pathData.Path;
		path.ClearCorners();
		NavMesh.CalculatePath(OriginPoint, position, -1, path);
		if ((int)path.status > 0)
		{
			return false;
		}
		float num = GClass361.CalculatePathLength(path);
		if (num > SAINPlugin.LoadedPreset.GlobalSettings.General.Cover.MaxCoverPathLength)
		{
			return false;
		}
		pathData.PathLength = num;
		if (!checkPathToEnemy(path, targetData))
		{
			return false;
		}
		return true;
	}

	private bool checkPathToEnemy(NavMeshPath path, TargetData targetData)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINBotSpaceAwareness.ArePathsDifferent(path, targetData.TargetEnemy.Path.PathToEnemy, 0.66f, 0.1f))
		{
			return false;
		}
		Vector3 dirBotToTargetNormal = targetData.DirBotToTargetNormal;
		for (int i = 1; i < path.corners.Length - 1; i++)
		{
			Vector3 val = path.corners[i];
			Vector3 val2 = TargetPoint - val;
			Vector3 val3 = val - OriginPoint;
			if (((Vector3)(ref val2)).sqrMagnitude < 0.25f)
			{
				if (DebugCoverFinder)
				{
				}
				return false;
			}
			if (i == 1)
			{
				if (Vector3.Dot(((Vector3)(ref val3)).normalized, dirBotToTargetNormal) > 0.5f)
				{
					if (DebugCoverFinder)
					{
					}
					return false;
				}
			}
			else
			{
				if (i >= path.corners.Length - 2)
				{
					continue;
				}
				Vector3 val4 = path.corners[i + 1];
				Vector3 val5 = val4 - val;
				if (Vector3.Dot(((Vector3)(ref val2)).normalized, ((Vector3)(ref val5)).normalized) > 0.5f && ((Vector3)(ref val5)).sqrMagnitude > ((Vector3)(ref val2)).sqrMagnitude)
				{
					if (DebugCoverFinder)
					{
					}
					return false;
				}
			}
		}
		return true;
	}

	private bool checkPositionVsOtherBots(Vector3 position)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		string profileId = base.Bot.ProfileId;
		if (checkIfPlayerCollidersNear(position, profileId, 0.5f))
		{
			return false;
		}
		Dictionary<string, BotComponent> members = base.Bot.Squad.Members;
		if (members != null)
		{
			foreach (BotComponent value in base.Bot.Squad.Members.Values)
			{
				if ((Object)(object)value == (Object)null || value.ProfileId == profileId)
				{
					continue;
				}
				List<CoverPoint> coverPoints = value.Cover.CoverPoints;
				foreach (CoverPoint item in coverPoints)
				{
					if (isDistanceTooClose(item, position))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private static bool checkIfPlayerCollidersNear(Vector3 point, string botProfileId, float radius)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _playerColliderArray.Length; i++)
		{
			_playerColliderArray[i] = null;
		}
		Physics.OverlapSphereNonAlloc(point, radius, _playerColliderArray, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
		int num = 0;
		Collider val = null;
		Collider[] playerColliderArray = _playerColliderArray;
		foreach (Collider val2 in playerColliderArray)
		{
			if (!((Object)(object)val2 == (Object)null))
			{
				num++;
				if (num > 1)
				{
					return true;
				}
				val = val2;
			}
		}
		if (num == 0)
		{
			return false;
		}
		GameWorld instance = Singleton<GameWorld>.Instance;
		Player val3 = ((instance != null) ? instance.GetPlayerByCollider(val) : null);
		if ((Object)(object)val3 == (Object)null)
		{
			return false;
		}
		if (val3.ProfileId == botProfileId)
		{
			return false;
		}
		return true;
	}

	private bool isDistanceTooClose(CoverPoint point, Vector3 position)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		int result;
		if (point != null)
		{
			Vector3 val = position - point.Position;
			result = ((((Vector3)(ref val)).sqrMagnitude < 0.25f) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private static bool visibilityCheck(Vector3 position, TargetData targetData, ColliderData colliderData, SAINHardColliderData hardColliderData)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = hardColliderData.Position - position;
		float distance = ((Vector3)(ref val)).magnitude * 1.25f;
		Vector3 targetPosition = targetData.TargetPosition;
		if (!checkRaycastToCoverCollider(position, targetPosition, out var hit, distance))
		{
			return false;
		}
		Vector3 val2 = targetData.DirBotToTargetNormal * 0.1f;
		Quaternion val3 = Quaternion.Euler(0f, 90f, 0f);
		Vector3 val4 = val3 * val2;
		if (!checkRaycastToCoverCollider(position + val4, targetPosition, out hit, distance))
		{
			return false;
		}
		if (!checkRaycastToCoverCollider(position - val4, targetPosition, out hit, distance))
		{
			return false;
		}
		return true;
	}

	private static bool checkRaycastToCoverCollider(Vector3 point, Vector3 target, out RaycastHit hit, float distance)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		point.y += 0.5f;
		target.y += 1.25f;
		Vector3 val = target - point;
		bool flag = Physics.Raycast(point, ((Vector3)(ref val)).normalized, ref hit, distance, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask));
		if (DebugCoverFinder)
		{
			if (flag)
			{
				DebugGizmos.Line(point, ((RaycastHit)(ref hit)).point, Color.white, 0.1f, 10f);
			}
			else
			{
				Vector3 endPoint = ((Vector3)(ref val)).normalized * distance + point;
				DebugGizmos.Line(point, endPoint, Color.red, 0.1f, 10f);
			}
		}
		return flag;
	}
}
