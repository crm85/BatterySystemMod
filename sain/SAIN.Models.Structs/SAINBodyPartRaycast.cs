using UnityEngine;

namespace SAIN.Models.Structs;

public struct SAINBodyPartRaycast
{
	public EBodyPart PartType;

	public EBodyPartColliderType ColliderType;

	public Vector3 CastPoint;
}
