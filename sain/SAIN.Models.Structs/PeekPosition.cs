using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Models.Structs;

public struct PeekPosition
{
	public readonly Vector3 DangerDir;

	public readonly Vector3 DangerDirNormal;

	public readonly float DangerDistance;

	public readonly Vector3 Point;

	public PeekPosition(Vector3 point, Vector3 danger)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Point = point;
		DangerDirNormal = (DangerDir = danger - point).Normalize(out var magnitude);
		DangerDistance = magnitude;
	}
}
