using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public struct FlashLightPoint
{
	private readonly float ExpireTime;

	public readonly Vector3 Point;

	public readonly float TimeCreated;

	public bool ShallExpire => Time.time - TimeCreated > ExpireTime;

	public FlashLightPoint(Vector3 point, float expireTime = 0.25f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Point = point;
		TimeCreated = Time.time;
		ExpireTime = expireTime;
	}
}
