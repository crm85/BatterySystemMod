using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class SmoothDampVector
{
	public Vector3 Current = Vector3.forward;

	public Vector3 Target = Vector3.forward;

	public Vector3 Velocity = Vector3.zero;

	public float CapLengthDistance;

	public SmoothDampVector(float capDistance = -1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		CapLengthDistance = capDistance;
		base._002Ector();
	}

	public void Calculate(float deltaTime, float smoothing, float maxSpeed, float xCoef = 1f, float yCoef = 1f, float zCoef = 1f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 target = Target;
		if (CapLengthDistance > 0f && ((Vector3)(ref Target)).sqrMagnitude > 1f)
		{
			((Vector3)(ref target)).Normalize();
		}
		Current = new Vector3(Mathf.SmoothDamp(Current.x, target.x, ref Velocity.x, smoothing * xCoef, maxSpeed, deltaTime), Mathf.SmoothDamp(Current.y, target.y, ref Velocity.y, smoothing * yCoef, maxSpeed, deltaTime), Mathf.SmoothDamp(Current.z, target.z, ref Velocity.z, smoothing * zCoef, maxSpeed, deltaTime));
	}
}
