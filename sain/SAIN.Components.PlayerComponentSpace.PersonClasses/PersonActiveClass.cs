using System;
using EFT;
using EFT.HealthSystem;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonActiveClass : PersonSubClass
{
	private bool _playerNullOrDead;

	public bool Active => PlayerActive && (!base.Person.AIInfo.IsAI || BotActive);

	public bool PlayerActive { get; private set; }

	public bool BotActive { get; private set; }

	public bool GameObjectActive { get; private set; }

	public bool IsAlive { get; private set; } = true;

	public event Action<bool> OnGameObjectActiveChanged;

	public event Action<bool> OnPlayerActiveChanged;

	public event Action<bool> OnBotActiveChanged;

	public event Action<PersonClass> OnPersonDeadOrDespawned;

	public void CheckActive()
	{
		if (IsAlive)
		{
			IsAlive = checkAlive();
		}
		if (!IsAlive)
		{
			playerKilledOrNull();
		}
		bool gameObjectActive = GameObjectActive;
		GameObjectActive = IsAlive && checkGameObjectActive();
		if (gameObjectActive != GameObjectActive)
		{
			this.OnGameObjectActiveChanged?.Invoke(GameObjectActive);
		}
		bool playerActive = PlayerActive;
		PlayerActive = IsAlive && GameObjectActive && checkPlayerExists();
		if (playerActive != PlayerActive)
		{
			this.OnPlayerActiveChanged?.Invoke(PlayerActive);
		}
		bool botActive = BotActive;
		BotActive = PlayerActive && checkBotActive();
		if (botActive != BotActive)
		{
			this.OnBotActiveChanged?.Invoke(BotActive);
		}
	}

	public void Disable()
	{
		bool gameObjectActive = GameObjectActive;
		GameObjectActive = false;
		if (gameObjectActive != GameObjectActive)
		{
			this.OnGameObjectActiveChanged?.Invoke(GameObjectActive);
		}
		bool playerActive = PlayerActive;
		PlayerActive = false;
		if (playerActive != PlayerActive)
		{
			this.OnPlayerActiveChanged?.Invoke(PlayerActive);
		}
		bool botActive = BotActive;
		BotActive = false;
		if (botActive != BotActive)
		{
			this.OnBotActiveChanged?.Invoke(BotActive);
		}
	}

	private void botStateChanged(EBotState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)state == 4)
		{
			BotOwner botOwner = base.Person.AIInfo.BotOwner;
			if ((Object)(object)botOwner != (Object)null)
			{
				botOwner.OnBotStateChange -= botStateChanged;
			}
			IsAlive = false;
			playerKilledOrNull();
		}
	}

	public PersonActiveClass(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		person.Player.OnPlayerDeadOrUnspawn += new GDelegate70(playerDeadOrUnspawn);
		IsAlive = true;
	}

	private bool checkAlive()
	{
		IPlayer iPlayer = base.IPlayer;
		if (iPlayer == null)
		{
			return false;
		}
		IHealthController healthController = iPlayer.HealthController;
		if (healthController != null && !healthController.IsAlive)
		{
			return false;
		}
		if (base.Person.AIInfo.IsAI)
		{
			BotOwner botOwner = base.Person.AIInfo.BotOwner;
			if ((Object)(object)botOwner == (Object)null || (Object)(object)((Component)botOwner).gameObject == (Object)null || (Object)(object)botOwner.Transform?.Original == (Object)null)
			{
				return false;
			}
		}
		return true;
	}

	private bool checkGameObjectActive()
	{
		Player player = base.Player;
		GameObject val = ((player != null) ? ((Component)player).gameObject : null);
		if ((Object)(object)val == (Object)null)
		{
			return false;
		}
		if (!((Behaviour)base.Player).isActiveAndEnabled)
		{
			return false;
		}
		return val.activeInHierarchy;
	}

	private void playerKilledOrNull()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (!_playerNullOrDead)
		{
			_playerNullOrDead = true;
			Player player = base.Player;
			if ((Object)(object)player != (Object)null)
			{
				player.OnPlayerDeadOrUnspawn -= new GDelegate70(playerDeadOrUnspawn);
			}
			this.OnPersonDeadOrDespawned?.Invoke(base.Person);
		}
	}

	private void playerDeadOrUnspawn(Player player)
	{
		IsAlive = false;
		playerKilledOrNull();
	}

	private bool checkBotActive()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		if (!IsAlive)
		{
			return false;
		}
		if (base.Person.AIInfo.IsAI)
		{
			BotOwner botOwner = base.Person.AIInfo.BotOwner;
			if ((Object)(object)botOwner == (Object)null)
			{
				return false;
			}
			if ((int)botOwner.BotState != 2)
			{
				return false;
			}
		}
		return true;
	}

	private bool checkPlayerExists()
	{
		Player player = base.Player;
		return (Object)(object)player != (Object)null && (Object)(object)((Component)player).gameObject != (Object)null && (Object)(object)player.Transform?.Original != (Object)null;
	}

	public void InitBot(BotOwner botOwner)
	{
		botOwner.OnBotStateChange += botStateChanged;
	}
}
