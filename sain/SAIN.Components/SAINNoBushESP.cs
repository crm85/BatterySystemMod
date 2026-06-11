using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components;

public class SAINNoBushESP : MonoBehaviour
{
	private static readonly PropertyInfo GoalEnemyProp;

	private static readonly PropertyInfo IsVisibleProp;

	private static readonly MethodInfo CanShootByState;

	private BotOwner BotOwner;

	private BotComponent SAIN;

	private float NoBushTimer = 0f;

	private static LayerMask NoBushMask;

	private static readonly List<string> ExclusionList;

	private static NoBushESPSettings Settings => SAINPlugin.LoadedPreset.GlobalSettings.Look.NoBushESP;

	private static bool UserToggle => Settings.NoBushESPToggle;

	private static bool EnhancedChecks => Settings.NoBushESPEnhanced;

	private static float EnhancedRatio => Settings.NoBushESPEnhancedRatio;

	private static float Frequency => Settings.NoBushESPFrequency;

	private static bool DebugMode => Settings.NoBushESPDebugMode;

	public bool NoBushESPActive { get; private set; } = false;

	private Vector3 HeadPosition => BotOwner.LookSensor._headPoint;

	static SAINNoBushESP()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		NoBushMask = LayerMask.op_Implicit(0);
		ExclusionList = new List<string>
		{
			"filbert", "fibert", "tree", "pine", "plant", "birch", "collider", "timber", "spruce", "bush",
			"metal", "wood", "grass"
		};
		Type typeFromHandle = typeof(BotOwner);
		Type fieldType = AccessTools.Field(typeFromHandle, PropertyNames.Memory).FieldType;
		GoalEnemyProp = AccessTools.Property(fieldType, PropertyNames.GoalEnemy);
		IsVisibleProp = AccessTools.Property(GoalEnemyProp.PropertyType, PropertyNames.IsVisible);
		Type propertyType = AccessTools.Property(typeFromHandle, PropertyNames.ShootData).PropertyType;
		CanShootByState = AccessTools.PropertySetter(propertyType, PropertyNames.CanShootByState);
	}

	public void Init(BotOwner botOwner, BotComponent sain = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (LayerMask.op_Implicit(NoBushMask) == 0)
		{
			NoBushMask = LayerMask.op_Implicit(LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMaskAI) | (1 << LayerMask.NameToLayer(PropertyNames.PlayerSpirit)));
		}
		BotOwner = botOwner;
		SAIN = sain;
	}

	public void Update()
	{
		if ((Object)(object)BotOwner == (Object)null || !UserToggle)
		{
			NoBushESPActive = false;
		}
		else if (NoBushTimer < Time.time)
		{
			NoBushTimer = Time.time + Frequency;
			bool canShoot = NoBushESPCheck();
			SetCanShoot(canShoot);
		}
	}

	public bool NoBushESPCheck()
	{
		object obj = (SAIN?.Enemy)?.EnemyInfo;
		if (obj == null)
		{
			BotOwner botOwner = BotOwner;
			if (botOwner == null)
			{
				obj = null;
			}
			else
			{
				BotMemoryClass memory = botOwner.Memory;
				obj = ((memory != null) ? memory.GoalEnemy : null);
			}
		}
		EnemyInfo val = (EnemyInfo)obj;
		if (val != null && (val.IsVisible || val.CanShoot))
		{
			IPlayer person = val.Person;
			if (person != null && !person.IsAI)
			{
				if (EnhancedChecks)
				{
					return NoBushESPCheckEnhanced(person);
				}
				return NoBushESPCheck(person);
			}
		}
		return false;
	}

	public bool NoBushESPCheck(IPlayer player)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = player.MainParts[(BodyPartType)1].Position;
		return RayCast(position, HeadPosition);
	}

	public bool NoBushESPCheckEnhanced(IPlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int count = player.MainParts.Count;
		Vector3 headPosition = HeadPosition;
		foreach (KeyValuePair<BodyPartType, EnemyPart> mainPart in player.MainParts)
		{
			if (RayCast(mainPart.Value.Position, headPosition))
			{
				num++;
			}
		}
		float num2 = (float)num / (float)count;
		bool flag = num2 >= EnhancedRatio;
		if (flag && DebugMode)
		{
			Logger.LogDebug($"Enhanced Active: [{num2}] visible from hit count: [{num}] / [{count}]. Config Value: [{EnhancedRatio}]");
		}
		return flag;
	}

	private static bool RayCast(Vector3 end, Vector3 start)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(start, ((Vector3)(ref val)).normalized, ref val2, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(NoBushMask)))
		{
			Transform transform = ((RaycastHit)(ref val2)).transform;
			object obj;
			if (transform == null)
			{
				obj = null;
			}
			else
			{
				Transform parent = transform.parent;
				obj = ((parent != null) ? ((Component)parent).gameObject : null);
			}
			GameObject val3 = (GameObject)obj;
			if ((Object)(object)val3 != (Object)null)
			{
				string text = ((val3 == null) ? null : ((Object)val3).name?.ToLower());
				foreach (string exclusion in ExclusionList)
				{
					if (text.Contains(exclusion))
					{
						if (DebugMode)
						{
							Logger.LogDebug(exclusion);
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	public void SetCanShoot(bool blockShoot)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		NoBushESPActive = blockShoot;
		if (!blockShoot)
		{
			return;
		}
		BotOwner botOwner = BotOwner;
		object obj;
		if (botOwner == null)
		{
			obj = null;
		}
		else
		{
			BotMemoryClass memory = botOwner.Memory;
			obj = ((memory != null) ? memory.GoalEnemy : null);
		}
		EnemyInfo val = (EnemyInfo)obj;
		if (val != null)
		{
			if (DebugMode)
			{
				Logger.LogDebug("No Bush ESP active");
			}
			val.SetCanShoot(false);
			val.SetVisible(false);
			IBotAiming currentAiming = BotOwner.AimingManager.CurrentAiming;
			BotAimingClass val2 = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
			if (val2 != null && (int)val2.aimStatus_0 != 1)
			{
				val2.aimStatus_0 = (AimStatus)1;
			}
			EnemyVisionClass enemyVisionClass = SAIN?.EnemyController.GetEnemy(val.ProfileId, mustBeActive: false)?.Vision;
			if (enemyVisionClass != null)
			{
				bool forceOff = true;
				enemyVisionClass.UpdateCanShootState(forceOff);
				enemyVisionClass.UpdateVisibleState(forceOff);
			}
		}
	}
}
