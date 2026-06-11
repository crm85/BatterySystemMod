using EFT;
using EFT.InventoryLogic;
using SAIN.Components;

namespace SAIN.Patches.Generic.SetInHands;

public static class Helpers
{
	public static void SetItemEquiped(IPlayer Player, Item Item)
	{
		if (GameWorldComponent.TryGetPlayerComponent(Player, out var PlayerComponent))
		{
			PlayerComponent.SetItemEquippedInHands(Item);
		}
	}
}
