using System;
using System.Collections;
using System.Collections.Generic;
using EFT;
using SAIN.Components;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Helpers;

public static class Vector
{
	public struct CrossPoint
	{
		public float x;

		public float y;

		public CrossPoint(float dx, float dy)
		{
			x = dx;
			y = dy;
		}

		public CrossPoint(Vector3 v)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			x = v.x;
			y = v.z;
		}
	}

	public struct VectorPair
	{
		public Vector3 a;

		public Vector3 b;

		public VectorPair(Vector3 a, Vector3 b)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			this.a = a;
			this.b = b;
		}
	}

	private static readonly Dictionary<Vector3, Vector3[]> dictionary_0 = new Dictionary<Vector3, Vector3[]>();

	public static List<Vector3> GeneratePointsAlongDirection(Vector3 start, Vector3 direction, float distance, float spacing)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = new List<Vector3>();
		Vector3 val = ((Vector3)(ref direction)).normalized * spacing;
		int num = Mathf.FloorToInt(distance / spacing);
		for (int i = 1; i <= num; i++)
		{
			Vector3 item = start + val * (float)i;
			list.Add(item);
		}
		return list;
	}

	public static void GeneratePointsAlongDirection(List<Vector3> points, Vector3 start, Vector3 direction, float distance, float spacing)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Vector3)(ref direction)).normalized * spacing;
		int num = Mathf.FloorToInt(distance / spacing);
		for (int i = 1; i <= num; i++)
		{
			Vector3 item = start + val * (float)i;
			points.Add(item);
		}
	}

	public static float FindFlatSignedAngle(Vector3 a, Vector3 b, Vector3 origin)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		a.y = 0f;
		b.y = 0f;
		origin.y = 0f;
		return Vector3.SignedAngle(a - origin, b - origin, Vector3.up);
	}

	public static float FindFlatAngle(Vector3 a, Vector3 b, Vector3 origin)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		a.y = 0f;
		b.y = 0f;
		origin.y = 0f;
		return Vector3.Angle(a - origin, b - origin);
	}

	public static Vector3? FindFirstBlindCorner(BotOwner botOwner, NavMeshPath path)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)botOwner == (Object)null || path == null)
		{
			return null;
		}
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		Vector3 headPoint = botOwner.LookSensor._headPoint;
		Vector3 position = botOwner.Position;
		Vector3 offset = headPoint - position;
		Vector3[] corners = path.corners;
		if (corners.Length > 2)
		{
			for (int i = 0; i < corners.Length - 2; i++)
			{
				Vector3 cornerA = corners[i];
				Vector3 val = corners[i + 1];
				Vector3 val2 = corners[i + 2];
				if (CheckIfBlindCorner(headPoint, val2, offset))
				{
					if (SAINPlugin.DebugMode)
					{
						DebugGizmos.Sphere(val, 0.025f, 5f);
					}
					Vector3 val3 = AdjustCornerPosition(cornerA, val, val2, 0.5f);
					if (SAINPlugin.DebugMode)
					{
						DebugGizmos.Sphere(val3, 0.05f, 5f);
					}
					return val3 + (botOwner.WeaponRoot.position - botOwner.Position);
				}
			}
		}
		return null;
	}

	public static IEnumerator FindBlindCorner(BotComponent bot, NavMeshPath path)
	{
		yield return null;
	}

	public static bool CheckIfBlindCorner(Vector3 lookSensor, Vector3 corner, Vector3 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = corner + offset;
		Vector3 val2 = val - lookSensor;
		return Physics.Raycast(lookSensor, val2, ((Vector3)(ref val2)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask));
	}

	public static Vector3 AdjustCornerPosition(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC, float amount = 0.1f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		cornerA.y = cornerB.y;
		cornerC.y = cornerB.y;
		Vector3 val = (cornerA + cornerC) / 2f;
		Vector3 val2 = val - cornerB;
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		Vector3 val3 = normalized * amount;
		return cornerB + val3;
	}

	public static bool Raycast(Vector3 start, Vector3 end, LayerMask mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		return Physics.Raycast(start, ((Vector3)(ref val)).normalized, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(mask));
	}

	public static bool Raycast(Vector3 start, Vector3 end, out RaycastHit hitInfo, LayerMask mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		return Physics.Raycast(start, ((Vector3)(ref val)).normalized, ref hitInfo, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(mask));
	}

	public static float DistanceBetween(Vector3 A, Vector3 B)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = A - B;
		return ((Vector3)(ref val)).magnitude;
	}

	public static float DistanceBetweenSqr(Vector3 A, Vector3 B)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = A - B;
		return ((Vector3)(ref val)).sqrMagnitude;
	}

	public static Vector3 DangerPoint(Vector3 position, Vector3 force, float mass)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		force /= mass;
		Vector3 val = CalculateForce(position, force);
		Vector3 midPoint = (position + val) / 2f;
		CheckThreePoints(position, midPoint, val, out var hitPos);
		return hitPos;
	}

	private static bool CheckThreePoints(Vector3 from, Vector3 midPoint, Vector3 target, out Vector3 hitPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = midPoint - from;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(new Ray(from, val), ref val2, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
		{
			hitPos = ((RaycastHit)(ref val2)).point;
			return false;
		}
		Vector3 val3 = midPoint - target;
		if (Physics.Raycast(new Ray(midPoint, val3), ref val2, ((Vector3)(ref val3)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
		{
			hitPos = ((RaycastHit)(ref val2)).point;
			return false;
		}
		hitPos = target;
		return true;
	}

	private static Vector3 CalculateForce(Vector3 from, Vector3 force)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Vector3 v = default(Vector3);
		((Vector3)(ref v))._002Ector(force.x, 0f, force.z);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Vector3)(ref v)).magnitude, force.y);
		float num = 2f * val.x * val.y / HelpersGClass.Gravity;
		if (val.y < 0f)
		{
			num = 0f - num;
		}
		return NormalizeFastSelf(v) * num + from;
	}

	public static bool CanShootToTarget(ShootPointClass shootToPoint, Vector3 firePos, LayerMask mask, bool doubleSide = false)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (shootToPoint == null)
		{
			return false;
		}
		bool result = false;
		Vector3 val = shootToPoint.Point - firePos;
		Ray val2 = default(Ray);
		((Ray)(ref val2))._002Ector(firePos, val);
		float magnitude = ((Vector3)(ref val)).magnitude;
		RaycastHit val3 = default(RaycastHit);
		if (!Physics.Raycast(val2, ref val3, magnitude * shootToPoint.DistCoef, LayerMask.op_Implicit(mask)))
		{
			if (doubleSide)
			{
				if (!Physics.Raycast(new Ray(shootToPoint.Point, -val), ref val3, magnitude, LayerMask.op_Implicit(mask)))
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	public static Vector3 Rotate(Vector3 direction, float degX, float degY, float degZ)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(degX, degY, degZ) * direction;
	}

	public static Vector3 Offset(Vector3 targetDirection, Vector3 offsetDirection, float magnitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return targetDirection + ((Vector3)(ref offsetDirection)).normalized * magnitude;
	}

	public static float SignedAngle(Vector3 from, Vector3 to, bool normalize = false)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (normalize)
		{
			((Vector3)(ref to)).Normalize();
			((Vector3)(ref from)).Normalize();
		}
		float value = Vector3.SignedAngle(from, to, Vector3.up);
		return value.Round10();
	}

	public static List<Vector3> NavMeshPointsFromSampledPoint(Vector3 point, Vector3 start, List<Vector3> list, int count = 5, float magnitude = 4f, float sampleDistance = 0.25f, int maxIterations = 15)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (list == null)
		{
			return null;
		}
		list.Clear();
		NavMeshHit val3 = default(NavMeshHit);
		for (int i = 0; i < maxIterations; i++)
		{
			Vector3 val = RandomVector3(1f, 0f, 1f);
			Vector3 val2 = ((Vector3)(ref val)).normalized * magnitude;
			if (NavMesh.SamplePosition(val2 + point, ref val3, sampleDistance, -1))
			{
				NavMeshPath val4 = new NavMeshPath();
				if (NavMesh.CalculatePath(point, ((NavMeshHit)(ref val3)).position, -1, val4))
				{
					if ((int)val4.status == 1)
					{
						list.Add(val4.corners[val4.corners.Length - 1]);
					}
					else
					{
						list.Add(((NavMeshHit)(ref val3)).position);
					}
				}
			}
			if (list.Count >= count)
			{
				break;
			}
		}
		return list;
	}

	public static Vector3 RandomVector3(float x, float y, float z)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(RandomRange(x), RandomRange(y), RandomRange(z));
	}

	public static float RandomRange(float magnitude)
	{
		return Random.Range(0f - magnitude, magnitude);
	}

	public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return Angle * (Point - Pivot) + Pivot;
	}

	public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Vector3 Euler)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Point.RotateAroundPivot(Pivot, Quaternion.Euler(Euler));
	}

	public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Quaternion Angle)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Me.position = Me.position.RotateAroundPivot(Pivot, Angle);
	}

	public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Vector3 Euler)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Me.position = Me.position.RotateAroundPivot(Pivot, Quaternion.Euler(Euler));
	}

	public static Vector3 Multiply(this Vector3 multiplier1, Vector3 multiplier2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(multiplier1.x * multiplier2.x, multiplier1.y * multiplier2.y, multiplier1.z * multiplier2.z);
	}

	public static Vector2 Multiply(this Vector2 multiplier1, Vector2 multiplier2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(multiplier1.x * multiplier2.x, multiplier1.y * multiplier2.y);
	}

	public static Vector3 Divide(this Vector3 divisible, Vector3 divisor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(divisible.x / divisor.x, divisible.y / divisor.y, divisible.z / divisor.z);
	}

	public static Vector2 Divide(this Vector2 divisible, Vector2 divisor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(divisible.x / divisor.x, divisible.y / divisor.y);
	}

	public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.Clamp(vector.x, Mathf.Min(min.x, max.x), Mathf.Max(min.x, max.x)), Mathf.Clamp(vector.y, Mathf.Min(min.y, max.y), Mathf.Max(min.y, max.y)), Mathf.Clamp(vector.z, Mathf.Min(min.z, max.z), Mathf.Max(min.z, max.z)));
	}

	public static Vector3 DeltaAngle(this Vector3 from, Vector3 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.DeltaAngle(from.x, to.x), Mathf.DeltaAngle(from.y, to.y), Mathf.DeltaAngle(from.z, to.z));
	}

	public static float AngOfNormazedVectors(Vector3 a, Vector3 b)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Acos(a.x * b.x + a.y * b.y + a.z * b.z) * 57.29578f;
	}

	public static float AngOfNormazedVectorsCoef(Vector3 a, Vector3 b)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static bool IsAngLessNormalized(Vector3 a, Vector3 b, float cos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return a.x * b.x + a.y * b.y + a.z * b.z > cos;
	}

	public static Vector3 NormalizeFast(Vector3 v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		return new Vector3(v.x / num, v.y / num, v.z / num);
	}

	public static Vector3 NormalizeFastSelf(Vector3 v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		v.x /= num;
		v.y /= num;
		v.z /= num;
		return v;
	}

	public static Vector3 Rotate90(Vector3 n, SideTurn side)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (side == SideTurn.left)
		{
			return new Vector3(0f - n.z, n.y, n.x);
		}
		return new Vector3(n.z, n.y, 0f - n.x);
	}

	public static Vector3 RotateVectorOnAngToZ(Vector3 d, float angDegree)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = NormalizeFastSelf(d);
		float num = (float)Math.PI / 180f * angDegree;
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		return new Vector3(val.x * num2, num3, val.z * num2);
	}

	public static Vector3 RotateOnAngUp(Vector3 b, float angDegree)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float num = angDegree * ((float)Math.PI / 180f);
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		float num4 = b.x * num3 - b.z * num2;
		float num5 = b.z * num3 + b.x * num2;
		return new Vector3(num4, 0f, num5);
	}

	public static Vector2 RotateOnAng(Vector2 b, float a)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float num = a * ((float)Math.PI / 180f);
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Cos(num);
		float num4 = b.x * num3 - b.y * num2;
		float num5 = b.y * num3 + b.x * num2;
		return new Vector2(num4, num5);
	}

	public static float Length(this Quaternion quaternion)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
	}

	public static void Normalize(this Quaternion quaternion)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		float num = quaternion.Length();
		if (!Mathf.Approximately(num, 1f))
		{
			if (Mathf.Approximately(num, 0f))
			{
				((Quaternion)(ref quaternion)).Set(0f, 0f, 0f, 1f);
			}
			else
			{
				((Quaternion)(ref quaternion)).Set(quaternion.x / num, quaternion.y / num, quaternion.z / num, quaternion.w / num);
			}
		}
	}

	public static bool IsOnNavMesh(Vector3 v, float dist = 0.04f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		NavMesh.SamplePosition(v, ref val, dist, -1);
		return ((NavMeshHit)(ref val)).hit;
	}

	public static Vector3 GetProjectionPoint(Vector3 p, Vector3 p1, Vector3 p2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		float num = p1.z - p2.z;
		if (num == 0f)
		{
			return new Vector3(p.x, p1.y, p1.z);
		}
		float num2 = p2.x - p1.x;
		if (num2 == 0f)
		{
			return new Vector3(p1.x, p1.y, p.z);
		}
		float num3 = p1.x * p2.z - p2.x * p1.z;
		float num4 = num2 * p.x - num * p.z;
		float num5 = (0f - (num2 * num3 + num * num4)) / (num2 * num2 + num * num);
		return new Vector3((0f - (num3 + num2 * num5)) / num, p1.y, num5);
	}

	public static Vector3 Test4Sides(Vector3 dir, Vector3 headPos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		dir.y = 0f;
		Vector3[] array = FindAngleFromDir(dir);
		if (array == null)
		{
			Console.WriteLine("can' find posible dirs");
			return dir;
		}
		Vector3[] array2 = array;
		foreach (Vector3 val in array2)
		{
			if (TestDir(headPos, val, 8f))
			{
				return val;
			}
		}
		return Vector3.one;
	}

	public static bool TestDir(Vector3 headPos, Vector3 dir, float dist)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Vector3? outPos;
		return TestDir(headPos, dir, dist, out outPos);
	}

	public static bool TestDir(Vector3 headPos, Vector3 dir, float dist, out Vector3? outPos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		outPos = null;
		RaycastHit val = default(RaycastHit);
		bool flag = Physics.Raycast(new Ray(headPos, dir), ref val, dist, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask));
		bool result = !flag;
		if (flag)
		{
			outPos = ((RaycastHit)(ref val)).point;
		}
		return result;
	}

	public static bool InBounds(Vector3 pos, BoxCollider[] colliders)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		foreach (BoxCollider box in colliders)
		{
			if (PointInOABB(pos, box))
			{
				return true;
			}
		}
		return false;
	}

	public static bool PointInOABB(Vector3 point, BoxCollider box)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		point = ((Component)box).transform.InverseTransformPoint(point) - box.center;
		float num = box.size.x * 0.5f;
		float num2 = box.size.y * 0.5f;
		float num3 = box.size.z * 0.5f;
		return point.x < num && point.x > 0f - num && point.y < num2 && point.y > 0f - num2 && point.z < num3 && point.z > 0f - num3;
	}

	public static float SqrDistance(this Vector3 a, Vector3 b)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(a.x - b.x, a.y - b.y, a.z - b.z);
		return val.x * val.x + val.y * val.y + val.z * val.z;
	}

	public static bool IsZero(this Vector2 vector)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return vector.x.IsZero() && vector.y.IsZero();
	}

	private static void CreateVectorArray8Dir(Vector3 startDir, int[] indexOfDirs)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[8];
		for (int i = 0; i < 8; i++)
		{
			int num = indexOfDirs[i];
			Vector3 val = RotateOnAngUp(startDir, EFTMath.GreateRandom(num, 0.1f));
			array[i] = val;
		}
		dictionary_0.Add(startDir, array);
	}

	private static Vector3[] FindAngleFromDir(Vector3 dir)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		Vector3[] result = null;
		foreach (KeyValuePair<Vector3, Vector3[]> item in dictionary_0)
		{
			float num2 = Vector3.Angle(dir, item.Key);
			if (num2 < num)
			{
				num = num2;
				result = item.Value;
			}
		}
		return result;
	}

	public static void Init()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[8];
		int num = 45;
		int num2 = 4;
		int num3 = 1;
		for (int i = 0; i < num2; i++)
		{
			if (i == 0)
			{
				array[0] = 0;
				array[7] = 180;
				continue;
			}
			int num4 = i * num;
			int num5 = 360 - num4;
			array[num3] = num4;
			array[num3 + 1] = num5;
			num3 += 2;
		}
		CreateVectorArray8Dir(Vector3.forward, array);
		CreateVectorArray8Dir(Vector3.left, array);
		CreateVectorArray8Dir(Vector3.right, array);
		CreateVectorArray8Dir(Vector3.back, array);
	}

	public static Vector3? GetCrossPoint(VectorPair p1, VectorPair p2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return GetCrossPoint(p1.a, p1.b, p2.a, p2.b);
	}

	public static Vector3? GetCrossPoint(Vector3 a1, Vector3 b1, Vector3 a2, Vector3 b2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		CrossPoint? crossPoint = Cross(new CrossPoint(a1), new CrossPoint(b1), new CrossPoint(a2), new CrossPoint(b2));
		if (crossPoint.HasValue)
		{
			return new Vector3(crossPoint.Value.x, a1.y, crossPoint.Value.y);
		}
		return null;
	}

	public static CrossPoint? Cross(CrossPoint p1, CrossPoint p2, CrossPoint p3, CrossPoint p4)
	{
		float num = (p1.x - p2.x) * (p4.y - p3.y) - (p1.y - p2.y) * (p4.x - p3.x);
		if (num == 0f)
		{
			return null;
		}
		float num2 = (p1.x - p3.x) * (p4.y - p3.y) - (p1.y - p3.y) * (p4.x - p3.x);
		float num3 = (p1.x - p2.x) * (p1.y - p3.y) - (p1.y - p2.y) * (p1.x - p3.x);
		float num4 = num2 / num;
		float num5 = num3 / num;
		if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
		{
			float dx = p1.x + num4 * (p2.x - p1.x);
			float dy = p1.y + num4 * (p2.y - p1.y);
			return new CrossPoint(dx, dy);
		}
		return null;
	}
}
