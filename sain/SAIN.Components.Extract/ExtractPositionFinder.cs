using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Components.Extract;

public class ExtractPositionFinder
{
	private static FieldInfo colliderField = AccessTools.Field(typeof(ExfiltrationPoint), "_collider");

	private static float initialColliderTestPointDensityFactor = 1f;

	private static float minColliderTestPointDensityFactor = 0.1f;

	private static int maxColliderTestPoints = 25;

	private static float defaultExtractNavMeshSearchRadius = 3f;

	private static float maxExtractNavMeshSearchRadius = 5f;

	private static float finalExtractNavMeshSearchRadiusAddition = 0.75f;

	private static float pathEndpointHeightDeprioritizationFactor = 2f;

	private static float minDistanceBetweenPathEndpoints = 75f;

	private static int maxPathEndpoints = 2;

	private ExfiltrationPoint ex = null;

	private readonly List<Vector3> pathEndpoints = new List<Vector3>();

	private readonly List<Vector3> sortedNavMeshPoints = new List<Vector3>();

	private readonly Stack<Vector3> navMeshTestPoints = new Stack<Vector3>();

	public bool ValidPathFound { get; private set; } = false;

	public Vector3? ExtractPosition { get; private set; } = null;

	public IReadOnlyCollection<Vector3> PathEndpoints => pathEndpoints.AsReadOnly();

	public ExtractPositionFinder(ExfiltrationPoint _ex)
	{
		ex = _ex;
	}

	public IEnumerator SearchForExfilPosition()
	{
		if (ValidPathFound)
		{
			yield break;
		}
		if ((Object)(object)ex == (Object)null)
		{
			Logger.LogError("Cannot find a position for a null exfil point");
			yield break;
		}
		FindExtractPositionsOnNavMesh();
		if (!navMeshTestPoints.Any())
		{
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning("Cannot find any NavMesh positions for " + ex.Settings.Name);
			}
			yield break;
		}
		ExtractPosition = navMeshTestPoints.Pop();
		if (ExtractFinderComponent.DebugMode)
		{
			Logger.LogInfo($"Testing point {ExtractPosition} for {ex.Settings.Name}. {navMeshTestPoints.Count} test points remaining.");
		}
		FindPathEndPoints(ExtractPosition.Value);
		if (pathEndpoints.Count == 0)
		{
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning("Could not find any path endpoints near " + ex.Settings.Name);
			}
			yield break;
		}
		foreach (Vector3 pathEndPoint in pathEndpoints)
		{
			if (NavMeshHelpers.DoesCompletePathExist(pathEndPoint, ExtractPosition.Value))
			{
				ValidPathFound = true;
				if (ExtractFinderComponent.DebugMode)
				{
					Logger.LogInfo("Found complete path to " + ex.Settings.Name);
				}
				yield break;
			}
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning(string.Format(arg2: Vector3.Distance(ExtractPosition.Value, pathEndPoint), format: "Could not find a complete path to {0} from {1} ({2}m away).", arg0: ex.Settings.Name, arg1: pathEndPoint));
			}
			yield return null;
		}
		if (ExtractFinderComponent.DebugMode)
		{
			Logger.LogWarning("Could not find a complete path to " + ex.Settings.Name);
		}
	}

	private float GetColliderTestPointSearchRadius(BoxCollider collider)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float val = Math.Min(Math.Min(collider.size.x, collider.size.y), collider.size.z) / 2f;
		val = Math.Min(val, maxExtractNavMeshSearchRadius);
		if (val == 0f)
		{
			val = defaultExtractNavMeshSearchRadius;
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning($"Collider size of {ex.Settings.Name} is (0, 0, 0). Using {val}m to check accessibility.");
			}
		}
		return val;
	}

	private void FindExtractPositionsOnNavMesh()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (navMeshTestPoints.Any())
		{
			return;
		}
		if (sortedNavMeshPoints.Any())
		{
			CreateNavMeshTestPointStack();
			return;
		}
		BoxCollider val = (BoxCollider)colliderField.GetValue(ex);
		if ((Object)(object)val == (Object)null)
		{
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning("Could not find collider for " + ex.Settings.Name);
			}
			return;
		}
		float colliderTestPointSearchRadius = GetColliderTestPointSearchRadius(val);
		IEnumerable<Vector3> colliderTestPoints = GetColliderTestPoints(val, colliderTestPointSearchRadius);
		IList<Vector3> colliderTestPointsOnNavMesh = GetColliderTestPointsOnNavMesh(colliderTestPoints, colliderTestPointSearchRadius + finalExtractNavMeshSearchRadiusAddition);
		Vector3 referencePoint = ((Component)ex).transform.position;
		bool flag = true;
		while (colliderTestPointsOnNavMesh.Count > 0)
		{
			IEnumerable<Vector3> source = colliderTestPointsOnNavMesh.OrderBy((Vector3 x) => Vector3.Distance(x, referencePoint));
			referencePoint = (flag ? source.First() : source.Last());
			flag = false;
			sortedNavMeshPoints.Add(referencePoint);
			colliderTestPointsOnNavMesh.Remove(referencePoint);
		}
		sortedNavMeshPoints.Reverse();
		CreateNavMeshTestPointStack();
	}

	private void CreateNavMeshTestPointStack()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector3 sortedNavMeshPoint in sortedNavMeshPoints)
		{
			navMeshTestPoints.Push(sortedNavMeshPoint);
		}
		if (ExtractFinderComponent.DebugMode)
		{
			Logger.LogInfo($"Found {navMeshTestPoints.Count} extract postions for {ex.Settings.Name}");
		}
	}

	private IEnumerable<Vector3> GetColliderTestPoints(BoxCollider collider, float searchRadius)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		float num = initialColliderTestPointDensityFactor;
		IEnumerable<Vector3> enumerable = Enumerable.Repeat<Vector3>(Vector3.positiveInfinity, maxColliderTestPoints + 1);
		int num2 = enumerable.Count();
		while (num2 > maxColliderTestPoints && num >= minColliderTestPointDensityFactor)
		{
			enumerable = collider.GetNavMeshTestPoints(searchRadius, num);
			if (enumerable.Count() == num2)
			{
				if (ExtractFinderComponent.DebugMode)
				{
					Logger.LogWarning("Could not minimize collider test point count for " + ex.Settings.Name);
				}
				break;
			}
			num2 = enumerable.Count();
			num /= 2f;
		}
		num *= 2f;
		if (!enumerable.Any())
		{
			enumerable = Enumerable.Repeat<Vector3>(((Component)collider).transform.position, 1);
			if (ExtractFinderComponent.DebugMode)
			{
				Logger.LogWarning("Could not create test points. Using collider position instead");
			}
		}
		else if (ExtractFinderComponent.DebugMode)
		{
			Logger.LogInfo($"Generated {enumerable.Count()} collider test points using a density factor of {Math.Round(num, 3)} and a search radius of {searchRadius}m");
			Logger.LogInfo($"Extract collider: center={((Component)collider).transform.position}, size={collider.size}.");
		}
		return enumerable;
	}

	private IList<Vector3> GetColliderTestPointsOnNavMesh(IEnumerable<Vector3> colliderTestPoints, float searchRadius)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = new List<Vector3>();
		foreach (Vector3 colliderTestPoint in colliderTestPoints)
		{
			Vector3? navMeshPoint = NavMeshHelpers.GetNearbyNavMeshPoint(colliderTestPoint, searchRadius);
			if (navMeshPoint.HasValue && !list.Any(delegate(Vector3 x)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				Vector3? val = navMeshPoint;
				return val.HasValue && x == val.GetValueOrDefault();
			}))
			{
				list.Add(navMeshPoint.Value);
			}
		}
		if (ExtractFinderComponent.DebugMode && !list.Any())
		{
			Logger.LogWarning($"Could not find any NavMesh points for {ex.Settings.Name} from {colliderTestPoints.Count()} test points using radius {searchRadius}m");
			Logger.LogWarning("Test points: " + string.Join(",", colliderTestPoints));
		}
		return list;
	}

	private void FindPathEndPoints(Vector3 testPoint)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		if (pathEndpoints.Count > 0)
		{
			return;
		}
		List<Vector3> list = GameWorldHandler.SAINGameWorld.GetAllSpawnPointPositionsOnNavMesh()?.ToList();
		if (list == null || list.Count == 0)
		{
			return;
		}
		Dictionary<Vector3, float> dictionary = new Dictionary<Vector3, float>(list.Count);
		foreach (Vector3 item in list)
		{
			float num = Vector3.Distance(item, testPoint);
			float num2 = Math.Abs(item.y - testPoint.y) * pathEndpointHeightDeprioritizationFactor;
			dictionary[item] = num + num2;
		}
		for (int i = 0; i < maxPathEndpoints; i++)
		{
			Vector3? val = null;
			float num3 = float.MaxValue;
			foreach (Vector3 item2 in list)
			{
				bool flag = false;
				foreach (Vector3 pathEndpoint in pathEndpoints)
				{
					if (Vector3.Distance(item2, pathEndpoint) <= minDistanceBetweenPathEndpoints)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					float num4 = dictionary[item2];
					if (num4 < num3)
					{
						num3 = num4;
						val = item2;
					}
				}
			}
			if (val.HasValue)
			{
				pathEndpoints.Add(val.Value);
				continue;
			}
			break;
		}
	}
}
