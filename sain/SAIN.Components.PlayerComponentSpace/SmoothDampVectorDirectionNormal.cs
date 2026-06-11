using System;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class SmoothDampVectorDirectionNormal
{
	public Vector3 Current = Vector3.forward;

	public Vector3 Target = Vector3.forward;

	private float currentYaw;

	private float currentPitch;

	private float yawVelocity;

	private float pitchVelocity;

	public void Calculate(float deltaTime, float smoothing, float maxSpeed, float pitchClamp)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = ((Vector3)(ref Target)).normalized;
		float num = Mathf.Atan2(normalized.z, normalized.x) * 57.29578f;
		float num2 = Mathf.Clamp(Mathf.Asin(normalized.y) * 57.29578f, 0f - pitchClamp, pitchClamp);
		currentYaw = Mathf.SmoothDampAngle(currentYaw, num, ref yawVelocity, smoothing, maxSpeed, deltaTime);
		currentPitch = Mathf.SmoothDampAngle(currentPitch, num2, ref pitchVelocity, smoothing, maxSpeed, deltaTime);
		float num3 = currentYaw * ((float)Math.PI / 180f);
		float num4 = currentPitch * ((float)Math.PI / 180f);
		float num5 = Mathf.Cos(num4);
		Current = new Vector3(Mathf.Cos(num3) * num5, Mathf.Sin(num4), Mathf.Sin(num3) * num5);
	}
}
