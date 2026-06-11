using System.Collections.Generic;
using SAIN.Classes.Coverfinder;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.CoverFinder;

public class ColliderCoverComponent : MonoBehaviour
{
	private const int DIRECTION_COUNT = 60;

	private static readonly Vector3[] StaticDirections;

	private bool _generated;

	public HashSet<CoverPointClass> CoverPoints { get; } = new HashSet<CoverPointClass>();

	public Vector3 ColliderPosition { get; private set; }

	public Vector3 Size { get; private set; }

	public Collider Collider { get; private set; }

	static ColliderCoverComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = new List<Vector3>();
		float num = 6f;
		Vector3 forward = Vector3.forward;
		for (int i = 0; i < 60; i++)
		{
			Quaternion val = Quaternion.Euler(0f, num * (float)i, 0f);
			list.Add(val * forward);
		}
		StaticDirections = list.ToArray();
	}

	public void Initialize(Collider collider)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Collider = collider;
		Bounds bounds = collider.bounds;
		Size = ((Bounds)(ref bounds)).size;
		ColliderPosition = ((Component)collider).transform.position;
	}

	private static void CheckCoverRealSize(Collider collider, out List<RaycastHit> Hits, Vector3 heightOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)collider).transform.position;
		Bounds bounds = collider.bounds;
		Vector3 size = ((Bounds)(ref bounds)).size;
		float magnitude = ((Vector3)(ref size)).magnitude;
		Hits = new List<RaycastHit>();
		RaycastHit item = default(RaycastHit);
		for (int i = 0; i < StaticDirections.Length; i++)
		{
			Vector3 val = StaticDirections[i];
			Vector3 val2 = val * magnitude;
			Vector3 val3 = val2 + position;
			DebugGizmos.Sphere(val3, 0.1f, Color.white);
			Ray val4 = default(Ray);
			((Ray)(ref val4)).direction = -val2;
			((Ray)(ref val4)).origin = val3 + heightOffset;
			Ray val5 = val4;
			if (collider.Raycast(val5, ref item, magnitude))
			{
				Hits.Add(item);
			}
		}
	}

	public void Generate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Size;
		float magnitude = ((Vector3)(ref val)).magnitude;
		Vector3 val2 = Vector3.up * Mathf.Min(Size.y * 0.5f, 0.5f);
		CheckCoverRealSize(Collider, out var Hits, val2);
		float num = FindExtent(Hits);
		if (num > 0f)
		{
			Logger.LogDebug($"Extent Found [{num}]");
			if (num > 0.2f)
			{
				DebugGizmos.Ray(ColliderPosition, Vector3.up, Color.white, magnitude);
				List<NavMeshHit> list = new List<NavMeshHit>();
				NavMeshHit item = default(NavMeshHit);
				for (int i = 0; i < Hits.Count; i++)
				{
					RaycastHit val3 = Hits[i];
					Vector3 val4 = ((RaycastHit)(ref val3)).point - val2;
					Vector3 normal = ((RaycastHit)(ref val3)).normal;
					normal.y = 0f;
					((Vector3)(ref normal)).Normalize();
					if (NavMesh.SamplePosition(val4 + normal, ref item, 0.5f, -1))
					{
						list.Add(item);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					bool flag = true;
					NavMeshHit val5;
					for (int k = j + 1; k < list.Count; k++)
					{
						val5 = list[j];
						Vector3 position = ((NavMeshHit)(ref val5)).position;
						val5 = list[k];
						val = position - ((NavMeshHit)(ref val5)).position;
						if (((Vector3)(ref val)).sqrMagnitude < 0.1f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						HashSet<CoverPointClass> coverPoints = CoverPoints;
						Collider collider = Collider;
						Vector3 colliderPosition = ColliderPosition;
						val5 = list[j];
						coverPoints.Add(new CoverPointClass(collider, colliderPosition, ((NavMeshHit)(ref val5)).position));
						val5 = list[j];
						DebugGizmos.Sphere(((NavMeshHit)(ref val5)).position, 0.25f, Color.red);
						val5 = list[j];
						DebugGizmos.Line(((NavMeshHit)(ref val5)).position, ColliderPosition, Color.yellow, 0.02f);
					}
				}
			}
		}
		_generated = true;
		Logger.LogDebug($"Generated Points [{CoverPoints.Count}]");
	}

	private static float FindExtent(List<RaycastHit> hits)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int num2 = hits.Count - 1; num2 >= 0; num2--)
		{
			RaycastHit val = hits[num2];
			for (int num3 = hits.Count - 2; num3 >= 0; num3--)
			{
				Vector3 point = ((RaycastHit)(ref val)).point;
				RaycastHit val2 = hits[num3];
				Vector3 val3 = point - ((RaycastHit)(ref val2)).point;
				float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
				if (sqrMagnitude > num)
				{
					num = sqrMagnitude;
				}
			}
		}
		return num;
	}
}
