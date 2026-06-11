using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.SubComponents;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class GrenadeReactionClass : BotSubClass<BotGrenadeManager>, IBotClass, IDisposable
{
	private const float MAX_ENEMY_GRENADE_DIST_TOCARE = 125f;

	public GrenadeTrackerClass DangerGrenade { get; private set; }

	public Vector3? GrenadeDangerPoint => DangerGrenade?.DangerPoint;

	public Dictionary<Throwable, GrenadeTrackerClass> EnemyGrenadesList { get; private set; } = new Dictionary<Throwable, GrenadeTrackerClass>();

	public GrenadeReactionClass(BotGrenadeManager ThrowWeapItemClass)
		: base(ThrowWeapItemClass)
	{
	}

	public override void Init()
	{
		GrenadeController grenadeController = BotManagerComponent.Instance.GrenadeController;
		grenadeController.OnGrenadeCollision += GrenadeCollision;
		grenadeController.OnGrenadeThrown += EnemyGrenadeThrown;
		grenadeController.OnGrenadeDangerUpdated += GrenadeDangerUpdated;
		base.Init();
	}

	public override void ManualUpdate()
	{
		foreach (GrenadeTrackerClass value in EnemyGrenadesList.Values)
		{
			value?.Update();
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		GrenadeController grenadeController = BotManagerComponent.Instance.GrenadeController;
		grenadeController.OnGrenadeCollision -= GrenadeCollision;
		grenadeController.OnGrenadeThrown -= EnemyGrenadeThrown;
		grenadeController.OnGrenadeDangerUpdated -= GrenadeDangerUpdated;
		foreach (GrenadeTrackerClass value in EnemyGrenadesList.Values)
		{
			if ((Object)(object)value?.Grenade != (Object)null)
			{
				((Throwable)value.Grenade).DestroyEvent -= RemoveGrenade;
			}
		}
		EnemyGrenadesList.Clear();
		base.Dispose();
	}

	public void EnemyGrenadeThrown(Grenade grenade, Vector3 dangerPoint, string profileId)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)base.Bot == (Object)null) && !(profileId == base.Bot.ProfileId) && base.Bot.BotActive)
		{
			Enemy enemy = base.Bot.EnemyController.GetEnemy(profileId, mustBeActive: false);
			if (enemy != null && enemy.RealDistance <= 125f)
			{
				EnemyGrenadesList.Add((Throwable)(object)grenade, new GrenadeTrackerClass(base.Bot, grenade, dangerPoint, GetReactionTime()));
				((Throwable)grenade).DestroyEvent += RemoveGrenade;
			}
			else
			{
				base.BotOwner.BewareGrenade.AddGrenadeDanger(dangerPoint, grenade);
			}
		}
	}

	private void GrenadeCollision(Grenade grenade, float maxRange)
	{
		if (EnemyGrenadesList.TryGetValue((Throwable)(object)grenade, out var value))
		{
			value?.CheckHeardGrenadeCollision(maxRange);
		}
	}

	private void GrenadeDangerUpdated(Grenade grenade, Vector3 Danger)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (EnemyGrenadesList.TryGetValue((Throwable)(object)grenade, out var value))
		{
			value.UpdateGrenadeDanger(Danger);
		}
	}

	private void RemoveGrenade(Throwable grenade)
	{
		if ((Object)(object)grenade != (Object)null)
		{
			grenade.DestroyEvent -= RemoveGrenade;
			EnemyGrenadesList.Remove(grenade);
		}
	}

	public float GetReactionTime()
	{
		float num = 0.25f;
		num /= base.Bot.Info.Profile.DifficultyModifier;
		num *= Random.Range(0.75f, 1.25f);
		return Mathf.Clamp(num, 0.2f, 1f);
	}
}
