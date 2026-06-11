using EFT;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINNotLooking
{
	private static LookSettings Settings => SAINPlugin.LoadedPreset.GlobalSettings.Look;

	public static float GetSpreadIncrease(IPlayer person, BotOwner botOwner)
	{
		if (Settings.NotLooking.NotLookingToggle && CheckIfPlayerNotLooking(person, botOwner))
		{
			return Settings.NotLooking.NotLookingAccuracyAmount;
		}
		return 0f;
	}

	public static float GetVisionSpeedDecrease(EnemyInfo enemyInfo)
	{
		if (CheckIfPlayerNotLooking(enemyInfo))
		{
			return Settings.NotLooking.NotLookingVisionSpeedModifier;
		}
		return 1f;
	}

	private static bool CheckIfPlayerNotLooking(IPlayer player, BotOwner botOwner)
	{
		if (player == null || (Object)(object)botOwner == (Object)null)
		{
			return false;
		}
		if (botOwner.EnemiesController.EnemyInfos.TryGetValue(player, out var value))
		{
			return CheckIfPlayerNotLooking(value);
		}
		return false;
	}

	private static bool CheckIfPlayerNotLooking(EnemyInfo enemyInfo)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (enemyInfo == null || (Object)(object)enemyInfo.Owner == (Object)null)
		{
			return false;
		}
		IPlayer person = enemyInfo.Person;
		if (person == null)
		{
			return false;
		}
		if (!enemyInfo.HaveSeenPersonal || Time.time - enemyInfo.PersonalSeenTime <= Settings.NotLooking.NotLookingTimeLimit || !enemyInfo.IsVisible)
		{
			Vector3 val = person.LookDirection;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Vector3 position = person.Position;
			Vector3 position2 = enemyInfo.Owner.Position;
			val = position2 - position;
			Vector3 normalized2 = ((Vector3)(ref val)).normalized;
			float num = Vector3.Angle(normalized2, normalized);
			return num >= Settings.NotLooking.NotLookingAngle;
		}
		return false;
	}
}
