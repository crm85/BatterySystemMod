using System.Collections.Generic;
using EFT;
using SAIN.BotController.Classes;
using SAIN.Helpers;
using SAIN.SAINComponent;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class LightDetectionClass : PlayerComponentBase
{
	public List<Vector3> LightPoints { get; } = new List<Vector3>();

	public LightDetectionClass(PlayerComponent component)
		: base(component)
	{
	}

	public bool CheckIsBeamVisible(FlashLightClass EnemyFlashlight)
	{
		if (!EnemyFlashlight.WhiteLight && !EnemyFlashlight.Laser)
		{
			IAIData aIData = base.Player.AIData;
			if (aIData != null)
			{
				BotOwner botOwner = aIData.BotOwner;
				bool? obj;
				if (botOwner == null)
				{
					obj = null;
				}
				else
				{
					BotNightVisionData nightVision = botOwner.NightVision;
					obj = ((nightVision != null) ? new bool?(nightVision.UsingNow) : ((bool?)null));
				}
				if (obj == false)
				{
					return false;
				}
			}
		}
		if (EnemyFlashlight.LightDetection.LightPoints.Count <= 0)
		{
			return false;
		}
		return true;
	}

	public void TryToInvestigate(IPlayer Player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = EstimatePosition(Player.Position, base.PlayerComponent.GetDistanceToPlayer(Player.ProfileId), 10f);
		BotComponent botComponent = base.PlayerComponent.BotComponent;
		if ((Object)(object)botComponent != (Object)null)
		{
			botComponent.Squad.SquadInfo.AddPointToSearch(val, 25f, botComponent, (AISoundType)0, Player, Squad.ESearchPointType.Flashlight);
			return;
		}
		BotOwner botOwner = base.PlayerComponent.BotOwner;
		if (botOwner != null)
		{
			botOwner.BotsGroup.AddPointToSearch(val, 20f, base.PlayerComponent.BotOwner, true, false);
		}
	}

	public static Vector3 EstimatePosition(Vector3 playerPos, float distance, float dispersion)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(distance, 0f, 50f);
		float num2 = num / dispersion;
		float num3 = EFTMath.Random(0f - num2, num2);
		float num4 = EFTMath.Random(0f - num2, num2);
		return new Vector3(playerPos.x + num3, playerPos.y, playerPos.z + num4);
	}
}
