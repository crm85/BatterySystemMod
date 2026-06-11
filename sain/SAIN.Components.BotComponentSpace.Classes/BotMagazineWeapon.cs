using System;
using System.Collections.Generic;
using EFT.InventoryLogic;

namespace SAIN.Components.BotComponentSpace.Classes;

public class BotMagazineWeapon
{
	private static readonly List<MagazineItemClass> _preAllocMagList = new List<MagazineItemClass>(20);

	public static bool RefillMags(BotComponent bot, BotWeaponInfo weapon, int numberToRefill = -1, bool includeActiveMag = false)
	{
		Slot magazineSlot = weapon.weapon.GetMagazineSlot();
		if (magazineSlot == null)
		{
			Logger.LogError("slot null");
			return false;
		}
		Item containedItem = magazineSlot.ContainedItem;
		MagazineItemClass val = (MagazineItemClass)(object)((containedItem is MagazineItemClass) ? containedItem : null);
		if (val == null)
		{
			Item containedItem2 = magazineSlot.ContainedItem;
			string obj = ((containedItem2 != null) ? containedItem2.Name : null);
			Item containedItem3 = magazineSlot.ContainedItem;
			Logger.LogError("mag null :: " + obj + " :: " + ((containedItem3 != null) ? containedItem3.ShortName : null));
			return false;
		}
		_preAllocMagList.Clear();
		bot.Player.InventoryController.GetReachableItemsOfTypeNonAlloc<MagazineItemClass>((IList<MagazineItemClass>)_preAllocMagList, (Predicate<MagazineItemClass>)null);
		if (_preAllocMagList.Count == 0)
		{
			_preAllocMagList.Clear();
			Logger.LogDebug("[" + bot.Info.Profile.NickName + "] no mags");
			return false;
		}
		int refilled = 0;
		int full = 0;
		if (includeActiveMag)
		{
			CheckMag(weapon, ref refilled, ref full, val);
		}
		foreach (MagazineItemClass preAllocMag in _preAllocMagList)
		{
			if (GClass2928.CanAccept((IContainer)(object)magazineSlot, (Item)(object)preAllocMag))
			{
				CheckMag(weapon, ref refilled, ref full, preAllocMag);
				if (numberToRefill >= 0 && refilled >= numberToRefill)
				{
					break;
				}
			}
		}
		_preAllocMagList.Clear();
		if (refilled > 0 || full >= numberToRefill)
		{
			Logger.LogDebug($"[{bot.Info.Profile.NickName}] success mags {refilled} : {full}");
			return true;
		}
		Logger.LogDebug($"[{bot.Info.Profile.NickName}] failed mags {refilled} : {full}");
		return false;
	}

	private static void CheckMag(BotWeaponInfo weapon, ref int refilled, ref int full, MagazineItemClass mag)
	{
		if (mag != null)
		{
			if (mag.Count == mag.MaxCount)
			{
				full++;
				return;
			}
			weapon.Reload.method_2(weapon.weapon, mag);
			refilled++;
		}
	}

	public static float GetAmmoRatio(MagazineItemClass magazine)
	{
		if (magazine == null)
		{
			return 0f;
		}
		return (float)magazine.Count / (float)magazine.MaxCount;
	}
}
