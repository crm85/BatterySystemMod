using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonVelocityClass : PersonSubClass
{
	private const float TRANSFORM_UPDATE_VELOCITY_FREQ = 0.2f;

	private const float TRANSFORM_MIN_VELOCITY = 0.25f;

	private const float TRANSFORM_MAX_VELOCITY = 5f;

	private float _nextUpdateVelocityTime;

	public float MagnitudeNormal { get; private set; }

	public float Magnitude { get; private set; }

	public Vector3 Vector { get; private set; }

	public void Update()
	{
		updateVelocity();
	}

	private void updateVelocity()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (_nextUpdateVelocityTime <= Time.time)
		{
			_nextUpdateVelocityTime = Time.time + 0.2f;
			Vector = base.Person.Player.MovementContext.Velocity;
			Vector3 vector = Vector;
			getPlayerVelocity(((Vector3)(ref vector)).magnitude);
		}
	}

	private void getPlayerVelocity(float magnitude)
	{
		if (magnitude <= 0.25f)
		{
			Magnitude = 0f;
			MagnitudeNormal = 0f;
			return;
		}
		if (magnitude >= 5f)
		{
			Magnitude = 5f;
			MagnitudeNormal = 1f;
			return;
		}
		Magnitude = magnitude;
		float num = 4.75f;
		float num2 = magnitude - 0.25f;
		MagnitudeNormal = num2 / num;
	}

	public PersonVelocityClass(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
	}
}
