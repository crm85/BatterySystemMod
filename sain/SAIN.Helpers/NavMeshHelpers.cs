using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Helpers;

public static class NavMeshHelpers
{
	public static bool DoesCompletePathExist(Vector3 sourcePosition, Vector3 targetPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		NavMeshPath val = new NavMeshPath();
		return NavMesh.CalculatePath(sourcePosition, targetPosition, -1, val) && (int)val.status == 0;
	}

	public static Vector3? GetNearbyNavMeshPoint(Vector3 testPoint, float radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(testPoint, ref val, radius, -1))
		{
			return ((NavMeshHit)(ref val)).position;
		}
		return null;
	}

	public static IEnumerable<Vector3> GetNavMeshTestPoints(this BoxCollider collider, float radius, float densityFactor)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(((Component)collider).transform.position, collider.size);
		return bounds.GetNavMeshTestPoints(radius, densityFactor);
	}

	public static IEnumerable<Vector3> GetNavMeshTestPoints(this Bounds bounds, float radius, float densityFactor)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		float num = Math.Min(Math.Min(((Bounds)(ref bounds)).size.x, ((Bounds)(ref bounds)).size.x), ((Bounds)(ref bounds)).size.x) / 2f;
		if (num < radius)
		{
			Logger.LogError($"Radius {radius} is smaller than min bounds extent {num} of size {((Bounds)(ref bounds)).size}");
			return Enumerable.Empty<Vector3>();
		}
		int num2 = (int)Math.Max(1.0, Math.Ceiling((((Bounds)(ref bounds)).size.x - radius * 2f) * densityFactor / (2f * radius)));
		int num3 = (int)Math.Max(1.0, Math.Ceiling((((Bounds)(ref bounds)).size.z - radius * 2f) * densityFactor / (2f * radius)));
		int num4 = (int)Math.Max(1.0, Math.Ceiling((((Bounds)(ref bounds)).size.y - radius * 2f) * densityFactor / (2f * radius)));
		float num5 = Math.Max(0f, (((Bounds)(ref bounds)).size.x - radius * 2f) / (float)num2);
		float num6 = Math.Max(0f, (((Bounds)(ref bounds)).size.z - radius * 2f) / (float)num3);
		float num7 = Math.Max(0f, (((Bounds)(ref bounds)).size.y - radius * 2f) / (float)num4);
		List<Vector3> list = new List<Vector3>();
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(((Bounds)(ref bounds)).min.x + radius, ((Bounds)(ref bounds)).min.y + radius, ((Bounds)(ref bounds)).min.z + radius);
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num4; j++)
			{
				for (int k = 0; k <= num3; k++)
				{
					list.Add(new Vector3(val.x + num5 * (float)i, val.y + num7 * (float)j, val.z + num6 * (float)k));
				}
			}
		}
		return list;
	}
}
