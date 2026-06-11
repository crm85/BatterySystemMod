using UnityEngine;

namespace SAIN.Models.Structs;

public struct SAINHardColliderData
{
	public Collider Collider { get; }

	public Vector3 Position { get; }

	public SAINHardColliderData(Collider collider)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Collider = collider;
		Position = ((Component)collider).transform.position;
	}
}
