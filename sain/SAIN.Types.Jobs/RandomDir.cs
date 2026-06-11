using UnityEngine;

namespace SAIN.Types.Jobs;

public readonly struct RandomDir
{
	public readonly Vector3 Direction;

	public readonly Vector3 DirectionNormal;

	public readonly float Magnitude;

	public RandomDir(float RandomMin, float RandomMax)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Magnitude = Random.Range(RandomMin, RandomMax);
		DirectionNormal = Random.onUnitSphere;
		Direction = DirectionNormal * Magnitude;
	}

	public RandomDir(float magnitude)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Magnitude = magnitude;
		DirectionNormal = Random.onUnitSphere;
		Direction = DirectionNormal * Magnitude;
	}

	public RandomDir(float magnitude, Vector3 directionNormal)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Magnitude = magnitude;
		DirectionNormal = directionNormal;
		Direction = DirectionNormal * Magnitude;
	}
}
