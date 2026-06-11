using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyCorner
{
	public int PathIndex { get; }

	public Vector3 GroundPosition { get; }

	public EnemyCorner(Vector3 groundPoint, int pathIndex)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		PathIndex = pathIndex;
		GroundPosition = groundPoint;
		base._002Ector();
	}
}
