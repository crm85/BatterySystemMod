using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class RaycastResult
{
	private const float SIGHT_PERIOD_SEC = 0.25f;

	private float _lastCheckTime;

	private float _lastSuccessTime;

	public bool InSight => TimeSinceSuccess <= 0.25f;

	public float TimeSinceChecked => Time.time - _lastCheckTime;

	public float TimeSinceSuccess => Time.time - _lastSuccessTime;

	public RaycastHit LastRaycastHit { get; private set; }

	public BodyPartCollider LastSuccessBodyPart { get; private set; }

	public Vector3? LastSuccessPoint { get; private set; }

	public void Update(Vector3 castPoint, BodyPartCollider bodyPartCollider, RaycastHit raycastHit, float time)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_lastCheckTime = time;
		LastRaycastHit = raycastHit;
		if ((Object)(object)((RaycastHit)(ref raycastHit)).collider == (Object)null)
		{
			LastSuccessBodyPart = bodyPartCollider;
			LastSuccessPoint = castPoint;
			_lastSuccessTime = time;
		}
		else
		{
			LastSuccessBodyPart = null;
			LastSuccessPoint = null;
		}
	}
}
