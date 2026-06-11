using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT.Interactive;
using SAIN.Preset.GlobalSettings;
using UnityEngine;

namespace SAIN.Components;

public class DoorHandler : GameWorldBase, IGameWorldClass
{
	private bool _doorsDisabled;

	private readonly Dictionary<string, Door> _disabledDoors = new Dictionary<string, Door>();

	private readonly Dictionary<int, GameObject> _doorsWithTriggers = new Dictionary<int, GameObject>();

	private bool _doorsDisabledByHost;

	public event Action<Door, EDoorState, bool> OnDoorStateChanged;

	public event Action<bool> OnDoorsDisabled;

	public DoorHandler(GameWorldComponent component)
		: base(component)
	{
	}

	public void Init()
	{
	}

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		checkDoors();
	}

	public void Dispose()
	{
		foreach (KeyValuePair<int, GameObject> doorsWithTrigger in _doorsWithTriggers)
		{
			GameObject value = doorsWithTrigger.Value;
			object obj;
			if (value == null)
			{
				obj = null;
			}
			else
			{
				GameObject gameObject = value.gameObject;
				obj = ((gameObject != null) ? gameObject.GetComponent<SphereCollider>() : null);
			}
			SphereCollider val = (SphereCollider)obj;
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)val);
			}
			Object.Destroy((Object)(object)doorsWithTrigger.Value);
		}
		_doorsWithTriggers.Clear();
	}

	public void ChangeDoorState(Door door, EDoorState state, bool shallInvert)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (shallInvert)
		{
			((WorldInteractiveObject)door).OpenAngle = 0f - ((WorldInteractiveObject)door).OpenAngle;
		}
		((WorldInteractiveObject)door).SetDoorState(state, false);
		this.OnDoorStateChanged?.Invoke(door, state, shallInvert);
		if (shallInvert)
		{
			((WorldInteractiveObject)door).OpenAngle = 0f - ((WorldInteractiveObject)door).OpenAngle;
		}
	}

	public void HostDisabledDoors(bool value)
	{
		_doorsDisabledByHost = value;
	}

	private void checkDoors()
	{
		if (Singleton<IBotGame>.Instance != null)
		{
			bool flag = _doorsDisabledByHost || GlobalSettingsClass.Instance.General.Doors.DisableAllDoors;
			if (!_doorsDisabled && flag)
			{
				this.OnDoorsDisabled?.Invoke(obj: true);
				_doorsDisabled = true;
				disableDoors();
			}
			else if (_doorsDisabled && !flag)
			{
				this.OnDoorsDisabled?.Invoke(obj: false);
				_doorsDisabled = false;
				enableDoors();
			}
		}
	}

	public bool DisableDoor(Door door)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((WorldInteractiveObject)door).DoorState != 4 && (int)((WorldInteractiveObject)door).DoorState != 2)
		{
			return false;
		}
		if (!((WorldInteractiveObject)door).Operatable || !((Behaviour)door).enabled)
		{
			return false;
		}
		if (((Component)door).gameObject.layer != LayerMask.op_Implicit(LayerMaskClass.InteractiveLayer))
		{
			return false;
		}
		GClass6.SmartDisable(((Component)door).gameObject);
		((Behaviour)door).enabled = false;
		_disabledDoors.Add(((WorldInteractiveObject)door).Id, door);
		return true;
	}

	private void disableDoors()
	{
		int doorCount = 0;
		GClass835.ExecuteForEach<Door>((IEnumerable<Door>)Object.FindObjectsOfType<Door>(), (Action<Door>)delegate(Door door)
		{
			if (DisableDoor(door))
			{
				doorCount++;
			}
		});
		_doorsDisabled = true;
		Logger.LogDebug($"Disabled Doors: {doorCount}");
	}

	private void enableDoors()
	{
		int num = 0;
		foreach (KeyValuePair<string, Door> disabledDoor in _disabledDoors)
		{
			GClass6.SmartEnable(((Component)disabledDoor.Value).gameObject);
			((Behaviour)disabledDoor.Value).enabled = true;
			num++;
		}
		_disabledDoors.Clear();
		_doorsDisabled = false;
		Logger.LogDebug($"Enabled Doors: {num}");
	}
}
