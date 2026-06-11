using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonBaseTransformClass : PersonSubClass
{
	private readonly BifacialTransform _transform;

	private readonly BifacialTransform _bodyPart;

	private readonly BodyPartCollider _eyePart;

	public Vector3 Position { get; private set; }

	public Vector3 EyePosition { get; private set; }

	public Vector3 BodyPosition { get; private set; }

	public void Update()
	{
		updateTransform();
	}

	private void updateTransform()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Position = _transform.position;
		EyePosition = _eyePart.Center;
		BodyPosition = _bodyPart.position;
	}

	public PersonBaseTransformClass(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		_transform = playerData.Player.Transform;
		PlayerBones playerBones = playerData.Player.PlayerBones;
		_bodyPart = playerBones.Ribcage;
		EBodyPartColliderType key = (EBodyPartColliderType)(playerBones.BodyPartCollidersDictionary.ContainsKey((EBodyPartColliderType)15) ? 15 : 0);
		_eyePart = playerBones.BodyPartCollidersDictionary[key];
	}
}
