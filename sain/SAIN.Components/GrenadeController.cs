using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Helpers;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components;

public class GrenadeController : BotManagerBase
{
	public readonly Dictionary<Throwable, List<PlayerComponent>> ActiveGrenades = new Dictionary<Throwable, List<PlayerComponent>>();

	private static FieldInfo _rigidBodyField;

	public event Action<Grenade, float> OnGrenadeCollision;

	public event Action<Grenade, Vector3, string> OnGrenadeThrown;

	public event Action<Grenade, Vector3> OnGrenadeDangerUpdated;

	public GrenadeController(BotManagerComponent controller)
		: base(controller)
	{
	}

	public void Init()
	{
	}

	public void Update()
	{
	}

	public void Dispose()
	{
	}

	public void Subscribe(BotEventHandler eventHandler)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		eventHandler.OnGrenadeThrow += new GDelegate19(GrenadeThrown);
		eventHandler.OnGrenadeExplosive += new GDelegate17(GrenadeExplosion);
	}

	public void UnSubscribe(BotEventHandler eventHandler)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		eventHandler.OnGrenadeThrow -= new GDelegate19(GrenadeThrown);
		eventHandler.OnGrenadeExplosive -= new GDelegate17(GrenadeExplosion);
	}

	public void GrenadeCollided(Grenade grenade, float maxRange)
	{
		this.OnGrenadeCollision?.Invoke(grenade, maxRange);
	}

	private void GrenadeExplosion(Vector3 explosionPosition, string playerProfileID, bool isSmoke, float smokeRadius, float smokeLifeTime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!Singleton<BotEventHandler>.Instantiated || playerProfileID == null)
		{
			return;
		}
		Player alivePlayer = GameWorldInfo.GetAlivePlayer(playerProfileID);
		if (!((Object)(object)alivePlayer != (Object)null))
		{
			return;
		}
		if (!isSmoke)
		{
			RegisterGrenadeExplosionForSAINBots(explosionPosition, alivePlayer, playerProfileID, 200f);
			return;
		}
		RegisterGrenadeExplosionForSAINBots(explosionPosition, alivePlayer, playerProfileID, 50f);
		float num = smokeRadius * HelpersGClass.SMOKE_GRENADE_RADIUS_COEF;
		Vector3 position = alivePlayer.Position;
		if (base.BotController.DefaultController == null)
		{
			return;
		}
		foreach (KeyValuePair<BotZone, GClass555> item in (Dictionary<BotZone, GClass555>)(object)base.BotController.DefaultController.Groups())
		{
			foreach (BotsGroup group in item.Value.GetGroups(true))
			{
				group.AddSmokePlace(explosionPosition, smokeLifeTime, num, position);
			}
		}
	}

	private void RegisterGrenadeExplosionForSAINBots(Vector3 explosionPosition, Player player, string playerProfileID, float range)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		BotEventHandler instance = Singleton<BotEventHandler>.Instance;
		if (instance != null)
		{
			instance.PlaySound((IPlayer)(object)player, explosionPosition, range, (AISoundType)2);
		}
		foreach (BotComponent value in base.Bots.Values)
		{
			if (value == null || !value.BotActive)
			{
				continue;
			}
			Vector3 val = value.Position - explosionPosition;
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (magnitude < range)
			{
				Enemy enemy = value.EnemyController.GetEnemy(playerProfileID, mustBeActive: true);
				if (enemy != null)
				{
					float num = magnitude / 10f;
					Vector3 val2 = Random.onUnitSphere * num;
					val2.y = 0f;
					Vector3 position = enemy.EnemyPosition + val2;
					SAINHearingReport heard = new SAINHearingReport
					{
						position = position,
						soundType = SAINSoundType.GrenadeExplosion,
						placeType = EEnemyPlaceType.Hearing,
						isDanger = (magnitude < 100f || enemy.InLineOfSight),
						shallReportToSquad = true
					};
					enemy.Hearing.SetHeard(heard);
				}
			}
		}
	}

	private void GrenadeThrown(Grenade grenade, Vector3 position, Vector3 force, float mass)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)grenade == (Object)null)
		{
			return;
		}
		Player alivePlayer = GameWorldInfo.GetAlivePlayer(grenade.ProfileId);
		if ((Object)(object)alivePlayer == (Object)null)
		{
			Logger.LogError("Player Null from ID " + grenade.ProfileId);
		}
		else
		{
			if (!alivePlayer.HealthController.IsAlive)
			{
				return;
			}
			Vector3 val = Vector.DangerPoint(position, force, mass);
			((Throwable)grenade).DestroyEvent += grenadeDestroyed;
			BotEventHandler instance = Singleton<BotEventHandler>.Instance;
			if (instance != null)
			{
				instance.PlaySound((IPlayer)(object)alivePlayer, ((Component)grenade).transform.position, 20f, (AISoundType)2);
			}
			this.OnGrenadeThrown?.Invoke(grenade, val, grenade.ProfileId);
			if (!GameWorldComponent.TryGetPlayerComponent((IPlayer)(object)alivePlayer, out var PlayerComponent))
			{
				return;
			}
			List<PlayerComponent> list = new List<PlayerComponent>();
			foreach (OtherPlayerData value in PlayerComponent.OtherPlayersData.DataDictionary.Values)
			{
				if (value.DistanceData.Distance < 125f && value.PlayerComponent.IsSAINBot)
				{
					list.Add(value.PlayerComponent);
				}
			}
			ActiveGrenades.Add((Throwable)(object)grenade, list);
			((MonoBehaviour)base.BotController).StartCoroutine(GrenadeTracker(grenade, PlayerComponent, list, val));
		}
	}

	private void grenadeDestroyed(Throwable Grenade)
	{
		ActiveGrenades.Remove(Grenade);
	}

	private IEnumerator GrenadeTracker(Grenade Grenade, PlayerComponent Thrower, List<PlayerComponent> RelevantPlayers, Vector3 DangerPoint)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody Rigidbody = (Rigidbody)_rigidBodyField.GetValue(Grenade);
		if ((Object)(object)Rigidbody == (Object)null)
		{
			Logger.LogError("RigidBody Null");
			yield break;
		}
		RaycastHit Hit = default(RaycastHit);
		while ((Object)(object)Grenade != (Object)null && (Object)(object)base.BotController != (Object)null && (Object)(object)Rigidbody != (Object)null)
		{
			Vector3 Velocity = Rigidbody.velocity;
			if (((Vector3)(ref Velocity)).magnitude < 0.1f)
			{
				this.OnGrenadeDangerUpdated?.Invoke(Grenade, ((Component)Grenade).transform.position);
			}
			else if (Velocity.y < 0f)
			{
				Vector3 VelocityNormal = ((Vector3)(ref Velocity)).normalized;
				if (Vector3.Dot(VelocityNormal, Vector3.down) > 0.5f && Physics.Raycast(((Component)Grenade).transform.position, VelocityNormal, ref Hit, 5f, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
				{
					this.OnGrenadeDangerUpdated?.Invoke(Grenade, ((RaycastHit)(ref Hit)).point);
				}
			}
			yield return null;
		}
	}

	static GrenadeController()
	{
		_rigidBodyField = AccessTools.Field(typeof(Throwable), "Rigidbody");
	}
}
