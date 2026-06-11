using System.Reflection;
using EFT;
using HarmonyLib;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class GrenadeVelocityTracker : MonoBehaviour
{
	private const float GRENADE_UPDATE_FREQUENCY = 0.5f;

	private Rigidbody _rigidBody;

	private Grenade _grenade;

	private static FieldInfo _rigidBodyField;

	private float _nextUpdateTime;

	public Vector3 Velocity { get; private set; }

	public float VelocityMagnitude { get; private set; }

	public void Awake()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		_grenade = ((Component)this).GetComponent<Grenade>();
		((Throwable)_grenade).DestroyEvent += GrenadeDestroyed;
		_rigidBody = (Rigidbody)_rigidBodyField.GetValue(_grenade);
	}

	public void Update()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_grenade == (Object)null))
		{
			if ((Object)(object)_rigidBody == (Object)null)
			{
				GrenadeDestroyed((Throwable)(object)_grenade);
			}
			else if (_nextUpdateTime < Time.time)
			{
				_nextUpdateTime = Time.time + 0.5f;
				Velocity = _rigidBody.velocity;
				Vector3 velocity = Velocity;
				VelocityMagnitude = ((Vector3)(ref velocity)).magnitude;
			}
		}
	}

	static GrenadeVelocityTracker()
	{
		_rigidBodyField = AccessTools.Field(typeof(Throwable), "Rigidbody");
	}

	private void GrenadeDestroyed(Throwable grenade)
	{
		if ((Object)(object)grenade != (Object)null)
		{
			grenade.DestroyEvent -= GrenadeDestroyed;
		}
		Object.Destroy((Object)(object)this);
	}
}
