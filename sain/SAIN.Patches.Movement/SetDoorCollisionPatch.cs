using System;
using System.Collections;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Movement;

public class SetDoorCollisionPatch : ModulePatch
{
	private const float NO_COLLISION_INTERVAL = 3f;

	private static Collider[] _preAllocArray = (Collider[])(object)new Collider[10];

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(Door), "SetDoorState", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void Patch(Door __instance, EDoorState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (((int)state != 2 && (int)state != 4) || !((Object)(object)((__instance != null) ? ((WorldInteractiveObject)__instance).Collider : null) != (Object)null))
		{
			return;
		}
		for (int i = 0; i < _preAllocArray.Length; i++)
		{
			_preAllocArray[i] = null;
		}
		Physics.OverlapSphereNonAlloc(((Component)__instance).transform.position, 20f, _preAllocArray, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
		Collider[] preAllocArray = _preAllocArray;
		foreach (Collider val in preAllocArray)
		{
			if (!((Object)(object)val != (Object)null))
			{
				continue;
			}
			GameWorld instance = Singleton<GameWorld>.Instance;
			Player val2 = ((instance != null) ? instance.GetPlayerByCollider(val) : null);
			if ((Object)(object)val2 != (Object)null && val2.IsAI)
			{
				Collider collider = val2.CharacterController.GetCollider();
				if ((Object)(object)collider != (Object)null)
				{
					((MonoBehaviour)val2).StartCoroutine(SetDoorCollisionAfterDelay(val2, collider, ((WorldInteractiveObject)__instance).Collider, 3f));
				}
			}
		}
	}

	private static IEnumerator SetDoorCollisionAfterDelay(Player player, Collider playerCollider, Collider doorCollider, float delay)
	{
		if ((Object)(object)playerCollider != (Object)null && (Object)(object)doorCollider != (Object)null && (Object)(object)player != (Object)null)
		{
			player.POM.IgnoreCollider(doorCollider, true);
			player.MovementContext.IgnoreInteractionCollision(doorCollider, true);
			EFTPhysicsClass.IgnoreCollision(playerCollider, doorCollider, true);
		}
		yield return (object)new WaitForSeconds(delay);
		if ((Object)(object)playerCollider != (Object)null && (Object)(object)doorCollider != (Object)null && (Object)(object)player != (Object)null)
		{
			player.POM.IgnoreCollider(doorCollider, false);
			player.MovementContext.IgnoreInteractionCollision(doorCollider, false);
			EFTPhysicsClass.IgnoreCollision(playerCollider, doorCollider, false);
		}
	}
}
