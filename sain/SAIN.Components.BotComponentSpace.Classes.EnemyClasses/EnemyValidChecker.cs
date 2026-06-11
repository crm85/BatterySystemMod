using System;
using EFT;
using EFT.HealthSystem;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.BotComponentSpace.Classes.EnemyClasses;

public class EnemyValidChecker : EnemyBase, IBotClass, IDisposable
{
	public bool WasValid { get; private set; } = true;

	public bool CheckValid()
	{
		if (!WasValid)
		{
			return false;
		}
		WasValid = isValid();
		if (!WasValid)
		{
			base.Enemy.Events.SetEnemyAsInvalid();
		}
		return WasValid;
	}

	public EnemyValidChecker(Enemy enemy)
		: base(enemy)
	{
		base.CanEverTick = false;
	}

	private bool isValid()
	{
		PlayerComponent enemyPlayerComponent = base.EnemyPlayerComponent;
		if ((Object)(object)enemyPlayerComponent == (Object)null)
		{
			return false;
		}
		PersonClass person = enemyPlayerComponent.Person;
		if (person == null)
		{
			return false;
		}
		if (!person.ActivationClass.IsAlive)
		{
			return false;
		}
		Player player = person.Player;
		if (player != null)
		{
			IHealthController healthController = player.HealthController;
			if (((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null)) == true)
			{
				Player enemyPlayer = base.EnemyPlayer;
				object obj;
				if (enemyPlayer == null)
				{
					obj = null;
				}
				else
				{
					IAIData aIData = enemyPlayer.AIData;
					obj = ((aIData != null) ? aIData.BotOwner : null);
				}
				BotOwner val = (BotOwner)obj;
				PersonClass enemyPerson = base.EnemyPerson;
				if (enemyPerson != null && enemyPerson.AIInfo.IsAI && (Object)(object)val == (Object)null)
				{
					return false;
				}
				if ((Object)(object)val != (Object)null && val.ProfileId == base.BotOwner.ProfileId)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}
}
