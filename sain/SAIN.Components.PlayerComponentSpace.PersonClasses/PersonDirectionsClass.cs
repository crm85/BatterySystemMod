using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonDirectionsClass : PersonSubClass
{
	public Vector3 LookDirection { get; private set; }

	public Vector3 Right()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return AngledLookDirection(0f, 90f, 0f);
	}

	public Vector3 Left()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return AngledLookDirection(0f, -90f, 0f);
	}

	public Vector3 Back()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return -LookDirection;
	}

	public Vector3 AngledLookDirection(float x, float y, float z)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(x, y, z) * LookDirection;
	}

	public void Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		LookDirection = base.Player.MovementContext.LookDirection;
	}

	public PersonDirectionsClass(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
	}
}
