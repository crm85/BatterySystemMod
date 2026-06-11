using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public struct PlayerDirectionData
{
	public Vector3 OwnerViewPosition;

	public Vector3 OwnerPosition;

	public Vector3 OwnerLookDirection;

	public DirectionData MainData;

	public BodyPartDirectionData[] BodyParts;
}
