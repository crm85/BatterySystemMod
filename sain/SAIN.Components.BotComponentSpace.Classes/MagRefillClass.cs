using EFT.InventoryLogic;

namespace SAIN.Components.BotComponentSpace.Classes;

public class MagRefillClass
{
	public Slot magazineSlot;

	public bool canAccept(MagazineItemClass mag)
	{
		return GClass2928.CanAccept((IContainer)(object)magazineSlot, (Item)(object)mag);
	}
}
