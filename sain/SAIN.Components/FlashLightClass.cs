using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SAIN.Components.PlayerComponentSpace;
using SAIN.SAINComponent;
using UnityEngine;

namespace SAIN.Components;

public class FlashLightClass : PlayerComponentBase
{
	private readonly List<DeviceMode> activeModes = new List<DeviceMode>();

	private static readonly FieldInfo _tacticalModesField = AccessTools.Field(typeof(TacticalComboVisualController), "list_0");

	public List<TacticalComboVisualController> TacticalDevices { get; private set; }

	public bool UsingLight { get; private set; }

	public bool UsingLaser { get; private set; }

	public bool LaserOnly => !WhiteLight && !IRLight && (Laser || IRLaser);

	public bool DeviceActive => ActiveModes.Count > 0;

	public bool IRLaser => ActiveModes.Contains(DeviceMode.IRLaser);

	public bool IRLight => ActiveModes.Contains(DeviceMode.IRLight);

	public bool Laser => ActiveModes.Contains(DeviceMode.VisibleLaser);

	public bool WhiteLight => ActiveModes.Contains(DeviceMode.WhiteLight);

	public LightDetectionClass LightDetection { get; } = new LightDetectionClass(component);

	private static bool _debugMode => SAINPlugin.LoadedPreset.GlobalSettings.General.Flashlight.DebugFlash;

	public List<DeviceMode> ActiveModes => activeModes;

	public event Action<bool> OnLightToggle;

	public event Action<bool> OnLaserToggle;

	public FlashLightClass(PlayerComponent component)
		: base(component)
	{
	}

	public void Update()
	{
	}

	public void CheckDevice()
	{
		CheckUsingLightModes();
		bool usingLight = UsingLight;
		UsingLight = ActiveModes.Contains(DeviceMode.WhiteLight) || ActiveModes.Contains(DeviceMode.IRLight);
		if (usingLight != UsingLight)
		{
			this.OnLightToggle?.Invoke(UsingLight);
		}
		bool usingLaser = UsingLaser;
		UsingLaser = ActiveModes.Contains(DeviceMode.VisibleLaser) || ActiveModes.Contains(DeviceMode.IRLaser);
		if (usingLaser != UsingLaser)
		{
			this.OnLaserToggle?.Invoke(UsingLaser);
		}
	}

	private void CheckUsingLightModes()
	{
		ActiveModes.Clear();
		Player player = base.Player;
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		if (_tacticalModesField == null)
		{
			Logger.LogError("Could find not find _tacticalModesField");
			return;
		}
		AbstractHandsController handsController = player.HandsController;
		FirearmController val = (FirearmController)(object)((handsController is FirearmController) ? handsController : null);
		if ((Object)(object)val == (Object)null)
		{
			Logger.LogError("Could find not find firearmController");
			return;
		}
		Transform weaponRoot = ((AbstractHandsController)val).WeaponRoot;
		TacticalDevices = GClass6.GetComponentsInChildrenActiveIgnoreFirstLevel<TacticalComboVisualController>(weaponRoot);
		if (TacticalDevices == null)
		{
			Logger.LogError("Could find not find tacticalComboVisualControllers");
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		foreach (TacticalComboVisualController tacticalDevice in TacticalDevices)
		{
			List<Transform> list = _tacticalModesField.GetValue(tacticalDevice) as List<Transform>;
			foreach (Transform item in list)
			{
				if (!((Component)item).gameObject.activeInHierarchy)
				{
					continue;
				}
				foreach (Transform child in GClass6.GetChildren(item))
				{
					string text = ((Object)child).name.ToLower();
					if (!flag && text.StartsWith("light_0"))
					{
						flag = true;
						if (_debugMode)
						{
							Logger.LogDebug("Found Light! Name:" + text);
						}
						ActiveModes.Add(DeviceMode.WhiteLight);
					}
					if (!flag2 && text.StartsWith("vis_0"))
					{
						flag2 = true;
						if (_debugMode)
						{
							Logger.LogDebug("Found Visible Laser! Name:" + text);
						}
						ActiveModes.Add(DeviceMode.VisibleLaser);
					}
					if (!flag3 && text.StartsWith("il_0"))
					{
						flag3 = true;
						if (_debugMode)
						{
							Logger.LogDebug("Found IR Light! Name:" + text);
						}
						ActiveModes.Add(DeviceMode.IRLight);
					}
					if (!flag4 && text.StartsWith("ir_0"))
					{
						if (_debugMode)
						{
							Logger.LogDebug("Found IR Laser! Name:" + text);
						}
						flag4 = true;
						ActiveModes.Add(DeviceMode.IRLaser);
					}
				}
			}
		}
	}
}
