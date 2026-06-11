using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public struct DirectionData
{
	public Vector3 Position;

	public Vector3 Direction;

	public Vector3 DirectionNormalized;

	public float Distance;

	public float Dot;

	public float HorizontalAngle;

	public float VerticalAngle;

	public float YDifference;

	public void Update(Vector3 Origin)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Direction = Position - Origin;
		DirectionNormalized = ((Vector3)(ref Direction)).normalized;
		Distance = ((Vector3)(ref Direction)).magnitude;
	}

	public void UpdateDotProductAndCalcNormal(Vector3 Origin, Vector3 LookDirection)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Position - Origin;
		UpdateDotProduct(((Vector3)(ref val)).normalized, LookDirection);
	}

	public void UpdateDotProduct(Vector3 DirectionNormal, Vector3 LookDirection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		HorizontalAngle = EnemyAnglesClass.CalcHorizontalAngle(DirectionNormal, LookDirection);
		VerticalAngle = EnemyAnglesClass.CalcVerticalAngle(DirectionNormal, LookDirection, out var yDiff);
		YDifference = yDiff;
		Dot = Vector3.Dot(LookDirection, DirectionNormal);
	}

	public void UpdateDotProduct(Vector3 LookDirection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		Dot = Vector3.Dot(LookDirection, DirectionNormalized);
	}
}
