using System.Collections.Generic;
using EFT;

namespace SAIN.Models.Structs;

public struct SAINBodyPart
{
	public readonly EBodyPart Type;

	public readonly BifacialTransform Transform;

	public readonly List<BodyPartCollider> Colliders;

	public SAINBodyPart(EBodyPart bodyPart, BifacialTransform transform, List<BodyPartCollider> colliders)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Type = bodyPart;
		Transform = transform;
		Colliders = colliders;
	}
}
