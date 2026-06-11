using System;
using EFT;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using UnityEngine;

namespace SAIN.Components;

public abstract class BotComponentBase : MonoBehaviour, IDisposable
{
	public string ProfileId { get; private set; }

	public PersonClass Person { get; private set; }

	public PlayerComponent PlayerComponent => Person.PlayerComponent;

	public BotOwner BotOwner => Person.AIInfo.BotOwner;

	public Player Player => Person.Player;

	public PersonTransformClass Transform => Person.Transform;

	public Vector3 Position => Person.Transform.Position;

	public Vector3 LookDirection => Person.Transform.LookDirection;

	public event Action OnDispose;

	public virtual bool Init(PersonClass person)
	{
		if (person == null || (Object)(object)person.Player == (Object)null)
		{
			return false;
		}
		Person = person;
		ProfileId = person.ProfileId;
		person.Player.ActiveHealthController.SetDamageCoeff(1f);
		return true;
	}

	public virtual void Dispose()
	{
		this.OnDispose?.Invoke();
	}
}
