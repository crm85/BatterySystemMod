using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public sealed class SAINVaultPoint
{
	private static int PointCount;

	public readonly Vector3 Position;

	public readonly float TimeCreated;

	public readonly int ID;

	public SAINVaultPoint(Vector3 pos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Position = pos;
		TimeCreated = Time.time;
		ID = PointCount;
		PointCount++;
	}
}
