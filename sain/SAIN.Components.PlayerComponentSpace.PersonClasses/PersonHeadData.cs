using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonHeadData : PersonSubClass
{
	private const float TRANSFORM_UPDATE_HEADLOOK_FREQ = 1f / 30f;

	private float _nextUpdateHeadLookTime;

	public Vector3 LookDirection { get; private set; }

	public Vector3 Position { get; private set; }

	public BifacialTransform HeadTransform { get; }

	public void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Position = HeadTransform.position;
		updateHeadLook();
	}

	private void updateHeadLook()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (_nextUpdateHeadLookTime <= Time.time)
		{
			_nextUpdateHeadLookTime = Time.time + 1f / 30f;
			Vector3 lookDirection = Quaternion.Euler(0f, HeadTransform.rotation.x + 90f, 0f) * HeadTransform.forward;
			lookDirection.y = base.Person.Transform.LookDirection.y;
			LookDirection = lookDirection;
		}
	}

	public PersonHeadData(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
		HeadTransform = playerData.Player.PlayerBones.Head;
	}
}
