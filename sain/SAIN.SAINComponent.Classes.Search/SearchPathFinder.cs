using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Search;

public class SearchPathFinder : BotSubClass<SAINSearchClass>
{
	private string _failReason;

	private bool _canStartSearch;

	private float _nextCheckSearchTime;

	private float _nextCheckPosTime;

	private NavMeshPath _searchPath;

	public Vector3 FinalDestination { get; private set; }

	public EnemyPlace TargetPlace { get; private set; }

	public BotPeekPlan? PeekPoints { get; private set; }

	public bool SearchedTargetPosition { get; private set; }

	public bool FinishedPeeking { get; set; }

	public SearchPathFinder(SAINSearchClass searchClass)
		: base(searchClass)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		base.CanEverTick = false;
		_searchPath = new NavMeshPath();
	}

	public bool HasPathToSearchTarget(Enemy enemy, out string failReason)
	{
		if (_nextCheckSearchTime < Time.time)
		{
			_nextCheckSearchTime = Time.time + 1f;
			_canStartSearch = CalculatePath(enemy, out failReason);
			_failReason = failReason;
		}
		failReason = _failReason;
		return _canStartSearch;
	}

	public void UpdateSearchDestination(Enemy enemy)
	{
		checkFinishedSearch(enemy);
		if (_nextCheckPosTime < Time.time || SearchedTargetPosition || FinishedPeeking || TargetPlace == null)
		{
			_nextCheckPosTime = Time.time + 4f;
			if (CalculatePath(enemy, out var _))
			{
			}
		}
	}

	private void checkFinishedSearch(Enemy enemy)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (SearchedTargetPosition)
		{
			return;
		}
		EnemyPlace lastKnownPlace = enemy.KnownPlaces.LastKnownPlace;
		if (lastKnownPlace == null)
		{
			Reset();
			return;
		}
		if (lastKnownPlace.HasArrivedPersonal || lastKnownPlace.HasArrivedSquad)
		{
			Reset();
			return;
		}
		NavMeshPath pathToEnemy = enemy.Path.PathToEnemy;
		if (pathToEnemy.corners.Length > 2)
		{
			return;
		}
		Vector3 val = FinalDestination - base.Bot.Position;
		if ((double)((Vector3)(ref val)).sqrMagnitude > 0.75)
		{
			return;
		}
		Vector3? val2 = pathToEnemy.LastCorner();
		if (!val2.HasValue)
		{
			Reset();
			return;
		}
		val = val2.Value - FinalDestination;
		string failReason;
		if (((Vector3)(ref val)).sqrMagnitude < 1f)
		{
			SearchedTargetPosition = true;
			enemy.KnownPlaces.SetPlaceAsSearched(lastKnownPlace);
			Reset();
		}
		else if (!CalculatePath(enemy, out failReason))
		{
			Logger.LogDebug("Failed to calc path during search for reason: [" + failReason + "]");
			Reset();
		}
	}

	public void Reset()
	{
		_searchPath.ClearCorners();
		PeekPoints?.DisposeDebug();
		PeekPoints = null;
		TargetPlace = null;
		FinishedPeeking = false;
		SearchedTargetPosition = false;
	}

	public bool CalculatePath(Enemy enemy, out string failReason)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = enemy.Path.PathToEnemy.LastCorner() ?? enemy.KnownPlaces.LastKnownPlace?.Position;
		if (!val.HasValue)
		{
			failReason = "lastPathPoint Null";
			return false;
		}
		Vector3 value = val.Value;
		Vector3 position = base.Bot.Position;
		Vector3 val2 = value - position;
		if (((Vector3)(ref val2)).sqrMagnitude <= 0.25f)
		{
			failReason = "tooClose";
			return false;
		}
		_searchPath.ClearCorners();
		if (!NavMesh.CalculatePath(position, value, -1, _searchPath))
		{
			failReason = "pathInvalid";
			return false;
		}
		Vector3? val3 = _searchPath.LastCorner();
		if (!val3.HasValue)
		{
			failReason = "lastCornerNull";
			return false;
		}
		base.BaseClass.Reset();
		FinalDestination = val3.Value;
		PeekPoints = findPeekPosition(enemy);
		TargetPlace = enemy.KnownPlaces.LastKnownPlace;
		failReason = string.Empty;
		return true;
	}

	public bool CalculatePath(EnemyPlace place, out EPathCalcFailReason failReason)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		if (place == null)
		{
			failReason = EPathCalcFailReason.NullPlace;
			return false;
		}
		Vector3 position = place.Position;
		Vector3 position2 = base.Bot.Position;
		Vector3 val = position - position2;
		if (((Vector3)(ref val)).sqrMagnitude <= 0.5f)
		{
			failReason = EPathCalcFailReason.TooClose;
			return false;
		}
		NavMeshHit val2 = default(NavMeshHit);
		if (!NavMesh.SamplePosition(position, ref val2, 1f, -1))
		{
			failReason = EPathCalcFailReason.SampleEnd;
			return false;
		}
		NavMeshHit val3 = default(NavMeshHit);
		if (!NavMesh.SamplePosition(position2, ref val3, 1f, -1))
		{
			failReason = EPathCalcFailReason.SampleStart;
			return false;
		}
		_searchPath.ClearCorners();
		if (!NavMesh.CalculatePath(((NavMeshHit)(ref val3)).position, ((NavMeshHit)(ref val2)).position, -1, _searchPath) || (int)_searchPath.status == 1)
		{
			failReason = EPathCalcFailReason.CalcPath;
			return false;
		}
		if (!_searchPath.LastCorner().HasValue)
		{
			failReason = EPathCalcFailReason.LastCorner;
			return false;
		}
		base.BaseClass.Reset();
		TargetPlace = place;
		failReason = EPathCalcFailReason.None;
		return true;
	}

	private BotPeekPlan? findPeekPosition(Enemy enemy)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (!enemy.Path.EnemyCorners.TryGetValue(ECornerType.Blind, out var value))
		{
			return null;
		}
		Vector3[] corners = enemy.Path.PathToEnemy.corners;
		int num = corners.Length;
		int pathIndex = value.PathIndex;
		Vector3 groundPosition = value.GroundPosition;
		Vector3 position = base.Bot.Position;
		Vector3 val = groundPosition - position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 start = groundPosition - normalized * 3f;
		NavMeshHit val5 = default(NavMeshHit);
		for (int i = pathIndex; i < num; i++)
		{
			Vector3 val2 = corners[i];
			Vector3 val3 = val2 - groundPosition;
			Vector3 normalized2 = ((Vector3)(ref val3)).normalized;
			float num2 = findHorizSignedAngle(normalized, normalized2);
			if (!(Mathf.Abs(num2) < 5f))
			{
				Vector3 val4 = groundPosition - normalized2 * 3f;
				if (NavMesh.Raycast(groundPosition, val4, ref val5, -1))
				{
					val4 = ((NavMeshHit)(ref val5)).position;
				}
				return new BotPeekPlan(start, val4, val2);
			}
		}
		return null;
	}

	private float findHorizSignedAngle(Vector3 dirA, Vector3 dirB)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		dirA.y = 0f;
		dirB.y = 0f;
		return Vector3.SignedAngle(dirA, dirB, Vector3.up);
	}
}
