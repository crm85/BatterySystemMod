using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class SmoothDampenedFloat(float smoothing, float maxVelocity = 50f)
{
	private readonly float _smoothingValue = smoothing;

	private readonly float _maxVelocity = maxVelocity;

	private float _velocity = 0f;

	public float LastSmoothedValue { get; private set; }

	public float TargetValue { get; private set; }

	public float Get(float deltaTime)
	{
		if (Mathf.Abs(LastSmoothedValue - TargetValue) < 0.001f)
		{
			LastSmoothedValue = TargetValue;
			_velocity = 0f;
			return LastSmoothedValue;
		}
		LastSmoothedValue = Mathf.SmoothDamp(LastSmoothedValue, TargetValue, ref _velocity, smoothing, maxVelocity, deltaTime);
		return LastSmoothedValue;
	}

	public void Set(float targetValue)
	{
		TargetValue = targetValue;
	}
}
