using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Generic;

public class GetAmmoForRefillPatch : ModulePatch
{
	private static readonly List<AmmoItemClass> _preallocatedAmmoList = new List<AmmoItemClass>(100);

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotReload), "method_3", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner ___botOwner_0, Weapon weapon, MagazineItemClass foundMag, ref AmmoItemClass __result)
	{
		__result = method_3(weapon, foundMag, ___botOwner_0.GetPlayer);
		return false;
	}

	private static AmmoItemClass method_3(Weapon weapon, MagazineItemClass foundMag, Player player)
	{
		Slot val = (weapon.HasChambers ? weapon.Chambers[0] : null);
		_preallocatedAmmoList.Clear();
		player.InventoryController.GetAcceptableItemsNonAlloc<AmmoItemClass>(BotReload._availableEquipmentSlots, (IList<AmmoItemClass>)_preallocatedAmmoList, (Predicate<AmmoItemClass>)null, (Predicate<GClass3050>)null);
		AmmoItemClass val2 = null;
		foreach (AmmoItemClass preallocatedAmmo in _preallocatedAmmoList)
		{
			bool flag = val != null && GClass2928.CanAccept((IContainer)(object)val, (Item)(object)preallocatedAmmo);
			bool flag2 = GClass2928.CheckItemFilter(foundMag.Cartridges.Filters, (Item)(object)preallocatedAmmo) && (val2 == null || ((Item)val2).StackObjectsCount < ((Item)preallocatedAmmo).StackObjectsCount);
			ModulePatch.Logger.LogDebug((object)$"Ammo [{((Item)preallocatedAmmo).Name}] Slot Accepts? [{flag}] filterAccepts? [{flag2}]");
			if (flag || flag2)
			{
				val2 = preallocatedAmmo;
			}
		}
		ModulePatch.Logger.LogDebug((object)$"Ammo [{((val2 != null) ? ((Item)val2).Name : null)}] Selected out of {_preallocatedAmmoList.Count}");
		_preallocatedAmmoList.Clear();
		return val2;
	}
}
