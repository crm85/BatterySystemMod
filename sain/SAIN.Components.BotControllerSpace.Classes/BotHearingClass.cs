using System;
using System.Collections;
using EFT;
using EFT.HealthSystem;
using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes;

public class BotHearingClass : BotManagerBase
{
	public event Action<EPhraseTrigger, ETagStatus, Player> PlayerTalk;

	public event Action<SAINSoundType, Vector3, PlayerComponent, float, float> AISoundPlayed;

	public event Action<EftBulletClass> BulletImpact;

	public BotHearingClass(BotManagerComponent botController)
		: base(botController)
	{
	}

	public void BulletImpacted(EftBulletClass bullet)
	{
		this.BulletImpact?.Invoke(bullet);
	}

	public void PlayerTalked(EPhraseTrigger phrase, ETagStatus mask, Player player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Invalid comparison between Unknown and I4
		if ((int)phrase == 26 || (Object)(object)player == (Object)null || !player.HealthController.IsAlive)
		{
			return;
		}
		PlayerComponent playerComponent = base.SAINGameWorld.PlayerTracker.GetPlayerComponent((IPlayer)(object)player);
		if ((Object)(object)playerComponent != (Object)null)
		{
			if (1 == 0)
			{
			}
			float num = (((int)phrase == 9 || (int)phrase == 15) ? 70f : (((int)phrase != 29) ? ((float)(((int)mask == 1) ? 40 : 70)) : 35f));
			if (1 == 0)
			{
			}
			float inRange = num;
			playerComponent.PlayAISound(SAINSoundType.Conversation, player.Position, inRange, 1f, phrase, mask);
			this.PlayerTalk?.Invoke(phrase, mask, player);
		}
	}

	public void PlayAISound(string profileId, SAINSoundType soundType, Vector3 position, float range, float volume)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		PlayerComponent playerComponent = base.SAINGameWorld.PlayerTracker.GetPlayerComponent(profileId);
		PlayAISound(playerComponent, soundType, position, range, volume, limitFreq: true);
	}

	public void PlayAISound(IPlayer Player, SAINSoundType soundType, Vector3 position, float range, float volume)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		PlayerComponent playerComponent = base.SAINGameWorld.PlayerTracker.GetPlayerComponent(Player);
		PlayAISound(playerComponent, soundType, position, range, volume, limitFreq: true);
	}

	public void PlayAISound(PlayerComponent playerComponent, SAINSoundType soundType, Vector3 position, float range, float volume, bool limitFreq)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)playerComponent == (Object)null)
		{
			Logger.LogError("Player Component Null");
		}
		else if (playerComponent.IsActive && playerComponent.AIData.AISoundPlayer.ShallPlayAISound())
		{
			playerComponent.PlayAISound(soundType, position, range, volume, (EPhraseTrigger)0, (ETagStatus)1);
			this.AISoundPlayed?.Invoke(soundType, position, playerComponent, range, volume);
			((MonoBehaviour)base.BotController).StartCoroutine(WaitDelayThenPlayDefaultBotEvent(soundType, playerComponent, position, range, volume));
		}
	}

	private IEnumerator WaitDelayThenPlayDefaultBotEvent(SAINSoundType soundType, PlayerComponent playerComponent, Vector3 position, float range, float volume, float delay = 0.1f)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(delay);
		int num;
		if (playerComponent == null)
		{
			num = 0;
		}
		else
		{
			Player player = playerComponent.Player;
			bool? obj;
			if (player == null)
			{
				obj = null;
			}
			else
			{
				IHealthController healthController = player.HealthController;
				obj = ((healthController != null) ? new bool?(healthController.IsAlive) : ((bool?)null));
			}
			bool? flag = obj;
			num = ((flag == true) ? 1 : 0);
		}
		if (num != 0 && playerComponent.IsActive)
		{
			playBotEvent(playerComponent.Player, position, range * volume, soundType);
		}
	}

	private void playBotEvent(Player player, Vector3 position, float range, SAINSoundType soundType)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		AISoundType baseSoundType = getBaseSoundType(soundType);
		BotEventHandler botEventHandler = base.BotController.BotEventHandler;
		if (botEventHandler != null)
		{
			botEventHandler.PlaySound((IPlayer)(object)player, position, range, baseSoundType);
		}
	}

	private AISoundType getBaseSoundType(SAINSoundType soundType)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return (AISoundType)(soundType switch
		{
			SAINSoundType.Shot => 2, 
			SAINSoundType.SuppressedShot => 1, 
			_ => 0, 
		});
	}
}
