using System.Collections.Generic;
using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINFriendlyFireClass : BotComponentClassBase
{
	public bool ClearShot => FriendlyFireStatus != FriendlyFireStatus.FriendlyBlock;

	public FriendlyFireStatus FriendlyFireStatus { get; private set; }

	public SAINFriendlyFireClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyBotInCombat;
	}

	public override void ManualUpdate()
	{
		if (FriendlyFireStatus == FriendlyFireStatus.FriendlyBlock)
		{
			ShootData shootData = base.BotOwner.ShootData;
			if (shootData != null)
			{
				shootData.EndShoot();
			}
		}
		base.ManualUpdate();
	}

	public bool UpdateFriendlyFireStatus(Vector3 target, Vector3 weaponFirePort, Vector3 weaponPointDirection, BotComponent bot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		FriendlyFireStatus = CheckFriendlyFireStatus(target, weaponFirePort, weaponPointDirection, bot);
		return FriendlyFireStatus != FriendlyFireStatus.FriendlyBlock;
	}

	public bool UpdateFriendlyFireStatus(float distance, Vector3 weaponFirePort, Vector3 weaponPointDirection, BotComponent bot)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		FriendlyFireStatus = CheckFriendlyFireStatus(distance, weaponFirePort, weaponPointDirection, bot);
		return FriendlyFireStatus != FriendlyFireStatus.FriendlyBlock;
	}

	public static FriendlyFireStatus CheckFriendlyFireStatus(float distance, Vector3 weaponFirePort, Vector3 weaponPointDirection, BotComponent bot)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, BotComponent> dictionary = bot.Squad?.Members;
		if (dictionary == null || dictionary.Count <= 1)
		{
			return FriendlyFireStatus.None;
		}
		return CheckFriendlyFire(weaponFirePort, distance, weaponPointDirection, bot);
	}

	public static FriendlyFireStatus CheckFriendlyFireStatus(Vector3 target, Vector3 weaponFirePort, Vector3 weaponPointDirection, BotComponent bot)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, BotComponent> dictionary = bot.Squad?.Members;
		if (dictionary == null || dictionary.Count <= 1)
		{
			return FriendlyFireStatus.None;
		}
		Vector3 val = weaponFirePort - target;
		return CheckFriendlyFire(weaponFirePort, ((Vector3)(ref val)).magnitude, weaponPointDirection, bot);
	}

	public static FriendlyFireStatus CheckFriendlyFire(Vector3 weaponFirePort, float distance, Vector3 weaponPointDirection, BotComponent bot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit[] array = SphereCastAll(weaponFirePort, distance, weaponPointDirection);
		int num = array.Length;
		if (num == 0)
		{
			return FriendlyFireStatus.None;
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit val = array[i];
			if (!((Object)(object)((RaycastHit)(ref val)).collider == (Object)null))
			{
				Player playerByCollider = GameWorldComponent.Instance.GameWorld.GetPlayerByCollider(((RaycastHit)(ref val)).collider);
				if (!((Object)(object)playerByCollider == (Object)null) && !(playerByCollider.ProfileId == bot.ProfileId) && !bot.EnemyController.IsPlayerAnEnemy(playerByCollider.ProfileId))
				{
					return FriendlyFireStatus.FriendlyBlock;
				}
			}
		}
		return FriendlyFireStatus.Clear;
	}

	private static RaycastHit[] SphereCastAll(Vector3 weaponFirePort, float targetDistance, Vector3 weaponPointDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Physics.SphereCastAll(weaponFirePort, 0.2f, weaponPointDirection, targetDistance, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
	}
}
