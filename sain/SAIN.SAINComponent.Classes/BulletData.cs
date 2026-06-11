using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class BulletData
{
	public bool BulletFelt;

	public bool BulletFiredAtMe;

	public Vector3 ProjectionPoint;

	public float ProjectionPointDistance;

	public bool Suppressed;

	public BulletData(bool defaults = false)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BulletFelt = defaults;
		BulletFiredAtMe = defaults;
		ProjectionPoint = Vector3.zero;
		ProjectionPointDistance = float.MaxValue;
		Suppressed = defaults;
	}
}
