using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using UnityEngine;

namespace SAIN.Components.Helpers;

public class SAINSoundTypeHandler
{
	public static void AISoundFileChecker(string sound, Player player)
	{
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		IHealthController healthController = player.HealthController;
		if (healthController != null && !healthController.IsAlive)
		{
			return;
		}
		SAINSoundType soundType = SAINSoundType.None;
		Item item = player.HandsController.Item;
		float num = 20f;
		if (item != null)
		{
			if (item is ThrowWeapItemClass)
			{
				if (sound == "Pin")
				{
					soundType = SAINSoundType.GrenadePin;
					num = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_GrenadePinDraw;
				}
				if (sound == "Draw")
				{
					soundType = SAINSoundType.GrenadeDraw;
					num = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_GrenadePinDraw;
				}
			}
			else if (item is MedsItemClass)
			{
				soundType = SAINSoundType.Heal;
				num = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Healing;
				if (sound == "CapRemove" || sound == "Inject")
				{
					num *= 0.5f;
				}
			}
			else
			{
				soundType = SAINSoundType.Reload;
				num = SAINPlugin.LoadedPreset.GlobalSettings.Hearing.BaseSoundRange_Reload;
			}
		}
		BotManagerComponent.Instance?.BotHearing.PlayAISound(player.ProfileId, soundType, player.Position + Vector3.up, num, 1f);
	}
}
