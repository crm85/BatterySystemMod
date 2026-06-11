using System.Collections.Generic;
using EFT.InventoryLogic;
using SAIN.SAINComponent;

namespace SAIN.Components.BotComponentSpace.Classes;

public class ReloadClass : BotBase
{
	private readonly BotWeaponManager _weaponManager;

	public static bool RefillMagsOnEachWeapon(BotComponent bot, BotWeaponManager weaponManager, int count = -1, bool includeActiveMag = false, params EquipmentSlot[] slotsToIgnore)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		foreach (KeyValuePair<EquipmentSlot, BotWeaponInfo> item in weaponManager.info)
		{
			if (item.Value?.weapon == null || !IsMagFed(item.Value.weapon.ReloadMode))
			{
				continue;
			}
			bool flag = true;
			if (slotsToIgnore != null)
			{
				foreach (EquipmentSlot val in slotsToIgnore)
				{
					if (val == item.Key)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag && BotMagazineWeapon.RefillMags(bot, item.Value, count, includeActiveMag))
			{
				result = true;
			}
		}
		return result;
	}

	public static bool RefillMagsInSlot(EquipmentSlot slot, BotComponent bot, BotWeaponManager weaponManager, int count = -1, bool includeActiveMag = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (weaponManager.info.TryGetValue(slot, out var value))
		{
			return BotMagazineWeapon.RefillMags(bot, value, count, includeActiveMag);
		}
		return false;
	}

	public ReloadClass(BotComponent bot)
		: base(bot)
	{
		_weaponManager = base.Bot.BotOwner.WeaponManager;
	}

	private static bool IsMagFed(EReloadMode reloadMode)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if (1 == 0)
		{
		}
		bool result = (((int)reloadMode == 0 || (int)reloadMode == 3) ? true : false);
		if (1 == 0)
		{
		}
		return result;
	}
}
