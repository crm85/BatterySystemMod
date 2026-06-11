using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Mover;

public class SAINVaultClass : BotComponentClassBase
{
	public static readonly List<SAINVaultPoint> GlobalVaultPoints = new List<SAINVaultPoint>();

	private static float DebugTimer = 0f;

	public SAINVaultPoint CurrentVaultPoint;

	public List<SAINVaultPoint> VaultPointHistory = new List<SAINVaultPoint>();

	public float VaultMaxHeight => base.Player.VaultingParameters.VaultingHeight;

	public SAINVaultClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public static void DebugCheckObstacles(Player player)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SpherecastCheck(player, player.WeaponRoot.position, player.LookDirection, out var _, 5f);
	}

	public static void DebugVaultPointCount()
	{
		if (DebugTimer < Time.time && GlobalVaultPoints != null && GlobalVaultPoints.Count > 0)
		{
			DebugTimer = Time.time + 3f;
			Logger.LogDebug(GlobalVaultPoints.Count);
		}
	}

	public bool FindVaultPoint(NavMeshPath path, out SAINVaultPoint vaultPoint)
	{
		if (FindVaultPoint(base.Player, path, out vaultPoint))
		{
			CurrentVaultPoint = vaultPoint;
			VaultPointHistory.Add(vaultPoint);
			return true;
		}
		return false;
	}

	public static bool FindVaultPoint(Player player, NavMeshPath path, out SAINVaultPoint point)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		point = null;
		if (path.corners.Length < 3)
		{
			return false;
		}
		int num = 0;
		Vector3[] corners = path.corners;
		int num2 = corners.Length - 1;
		bool flag = false;
		for (int i = 0; i < num2 - 2; i++)
		{
			Vector3 start = corners[i];
			for (int j = i + 2; j < num2; j++)
			{
				num++;
				Vector3 end = corners[j];
				if (SpherecastCheck(player, start, end, out point))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogInfo($"{num} : {flag}");
		}
		return flag;
	}

	public static bool SpherecastCheck(Player player, Vector3 start, Vector3 end, out SAINVaultPoint result, float distance = 0f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		start.y += 0.33f;
		end.y += 0.33f;
		Vector3 val = end - start;
		if (distance == 0f)
		{
			distance = ((Vector3)(ref val)).magnitude;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.SphereCast(start, 0.1f, val, ref hit, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMaskClass.PlayerStaticCollisionsMask)) && CheckObstacleForVault(hit, player.VaultingParameters.VaultingHeight))
		{
			result = new SAINVaultPoint(((RaycastHit)(ref hit)).point);
			GlobalVaultPoints.Add(result);
			return true;
		}
		result = null;
		return false;
	}

	public static bool CheckObstacleForVault(RaycastHit hit, float maxHeight)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((RaycastHit)(ref hit)).collider == (Object)null)
		{
			return false;
		}
		Bounds bounds = ((RaycastHit)(ref hit)).collider.bounds;
		float y = ((Bounds)(ref bounds)).size.y;
		bool flag = y < maxHeight;
		Color color = (flag ? Color.green : Color.red);
		float size = (flag ? 1f : 0.5f);
		DebugGizmos.Sphere(((Component)((RaycastHit)(ref hit)).collider).transform.position, size, color, 60f);
		return flag;
	}

	public bool TryVaulting()
	{
		if (CanVault() && base.Player.VaultingComponent.TryVaulting())
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogWarning("Vault Success");
			}
			base.Player.OnVaulting();
			return true;
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning("Vault Fail");
		}
		return false;
	}

	public bool CanVault()
	{
		if ((Object)(object)base.Player == (Object)null || base.Player.VaultingComponent == null || base.Player.VaultingGameplayRestrictions == null)
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogWarning("Vault Fail, Something Null");
			}
			return false;
		}
		if (base.Player.VaultingGameplayRestrictions.CanVaulting())
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogWarning("Vault Success - Player Can Vault");
			}
			return true;
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning("Vault Fail - Player Can NOT Vault");
		}
		return false;
	}
}
