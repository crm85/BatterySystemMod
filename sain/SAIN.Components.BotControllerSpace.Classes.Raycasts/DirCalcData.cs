using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct DirCalcData
{
	public Vector3 Point;

	public Vector3 Dir;

	public Vector3 DirNormal;

	public float Magnitude;

	public DirCalcData(Vector3 inPoint)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Dir = default(Vector3);
		DirNormal = default(Vector3);
		Magnitude = 0f;
		Point = inPoint;
	}
}
