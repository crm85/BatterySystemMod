using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class BotBusyHandsDetector : BotComponentClassBase
{
	private const float CHECK_FREQ = 0.1f;

	private const float TIME_TO_RESET_GENERIC = 5f;

	private const float TIME_TO_RESET_HEAL_FIRSTAID = 10f;

	private const float TIME_TO_RESET_HEAL_STIMS = 3f;

	private const float TIME_TO_RESET_HEAL_SURGERY = 40f;

	private const float TIME_TO_RESET_WEAPONS_RELOAD = 10f;

	private const float TIME_TO_RESET_WEAPONS_SWAP = 3f;

	private const float TIME_TO_RESET_WEAPONS_GRENADE = 3f;

	private Dictionary<GEventArgs1, float> _OngoingEvents = new Dictionary<GEventArgs1, float>();

	private List<GEventArgs1> _eventsToRemove = new List<GEventArgs1>();

	private List<GEventArgs1> _events = new List<GEventArgs1>();

	private float _timeStartInteraction = -1f;

	private bool _isInInteraction;

	private bool _isInInteractionStrictCheck;

	private float _nextCheckTime;

	public BotBusyHandsDetector(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void ManualUpdate()
	{
		checkShallFix();
		base.ManualUpdate();
	}

	private void checkShallFix()
	{
		if (_nextCheckTime < Time.time)
		{
			_nextCheckTime = Time.time + 0.1f;
		}
	}

	private void checkBusyHands()
	{
		AbstractHandsController handsController = base.Player.HandsController;
		if (!((Object)(object)handsController != (Object)null))
		{
			return;
		}
		_isInInteraction = handsController.IsInInteraction();
		_isInInteractionStrictCheck = _isInInteraction || handsController.IsInInteractionStrictCheck();
		bool flag = _isInInteraction || _isInInteractionStrictCheck;
		if (flag)
		{
			logTimeSince();
			collectQueEvents();
			if (_timeStartInteraction <= 0f)
			{
				_timeStartInteraction = Time.time;
			}
		}
		else if (!flag)
		{
			_OngoingEvents.Clear();
			if (_timeStartInteraction > 0f)
			{
				_timeStartInteraction = -1f;
			}
		}
	}

	private void checkBusyTooLong()
	{
		float timeStartInteraction = _timeStartInteraction;
		if (!(timeStartInteraction <= 0f) && botHasBusyHands(timeStartInteraction, out var reason))
		{
			resetHands(reason);
		}
	}

	private bool botHasBusyHands(float startTime, out string reason)
	{
		float num = Time.time - startTime;
		BotMedecine medecine = base.BotOwner.Medecine;
		if (medecine != null)
		{
			GClass475 stimulators = medecine.Stimulators;
			if (stimulators != null && ((GClass468)stimulators).Using)
			{
				reason = "stims";
				return num > 3f;
			}
			BotFirstAidClass firstAid = medecine.FirstAid;
			if (firstAid != null && ((GClass468)firstAid).Using)
			{
				reason = "firstAid";
				return num > 10f;
			}
			GClass473 surgicalKit = medecine.SurgicalKit;
			if (surgicalKit != null && ((GClass468)surgicalKit).Using)
			{
				reason = "surgery";
				return num > 40f;
			}
		}
		BotWeaponManager weaponManager = base.BotOwner.WeaponManager;
		if (weaponManager != null)
		{
			if (weaponManager.Reload.Reloading)
			{
				reason = "reloading";
				return num > 10f;
			}
			if (weaponManager.Selector.IsChanging)
			{
				reason = "changingWeapon";
				return num > 3f;
			}
			if (weaponManager.Grenades.ThrowindNow)
			{
				reason = "throwingGrenade";
				return num > 3f;
			}
		}
		reason = "generic";
		return num > 5f;
	}

	private void resetHands(string reason)
	{
		Logger.LogWarning("[" + ((Object)base.BotOwner).name + "] is resetting hands because [" + reason + "] too long!");
		resetHandsController(base.Player);
	}

	private void collectQueEvents()
	{
		InventoryController inventoryController = base.Player.InventoryController;
		if (inventoryController == null)
		{
			Logger.LogError("FixHandsController: could not find '_inventoryController'");
		}
		else
		{
			if (((TraderControllerClass)inventoryController).List_0.Count <= 0)
			{
				return;
			}
			_events.Clear();
			_events.AddRange(((TraderControllerClass)inventoryController).List_0);
			float time = Time.time;
			foreach (GEventArgs1 @event in _events)
			{
				if (!_OngoingEvents.ContainsKey(@event))
				{
					_OngoingEvents.Add(@event, time);
				}
			}
			_eventsToRemove.Clear();
			foreach (KeyValuePair<GEventArgs1, float> ongoingEvent in _OngoingEvents)
			{
				if (!_events.Contains(ongoingEvent.Key))
				{
					_eventsToRemove.Add(ongoingEvent.Key);
				}
			}
			foreach (GEventArgs1 item in _eventsToRemove)
			{
				_OngoingEvents.Remove(item);
			}
			_events.Clear();
			_eventsToRemove.Clear();
		}
	}

	private void logTimeSince()
	{
		float time = Time.time;
		foreach (KeyValuePair<GEventArgs1, float> ongoingEvent in _OngoingEvents)
		{
			Logger.LogDebug($"[{ongoingEvent.Key.EventId}] : [{time - ongoingEvent.Value}]");
		}
	}

	private static void resetHandsController(Player player)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		InventoryController inventoryController = player.InventoryController;
		if (inventoryController == null)
		{
			Logger.LogError("FixHandsController: could not find '_inventoryController'");
			return;
		}
		int count = ((TraderControllerClass)inventoryController).List_0.Count;
		if (count > 0)
		{
			GEventArgs1[] array = (GEventArgs1[])(object)new GEventArgs1[count];
			((TraderControllerClass)inventoryController).List_0.CopyTo(array);
			GEventArgs1[] array2 = array;
			foreach (GEventArgs1 val in array2)
			{
				((TraderControllerClass)inventoryController).RemoveActiveEvent(val);
			}
			Logger.LogInfo($"Cleared {count} stuck inventory operations.");
		}
		AbstractHandsController handsController = player.HandsController;
		FirearmController val2 = (FirearmController)(object)((handsController is FirearmController) ? handsController : null);
		if (val2 != null)
		{
			player.MovementContext.OnStateChanged -= new GDelegate71(val2.method_17);
			player.Physical.OnSprintStateChangedEvent -= val2.method_16;
			val2.RemoveBallisticCalculator();
		}
		try
		{
			player.SpawnController((AbstractHandsController)(object)player.method_156(), (Action)null);
		}
		catch (Exception ex)
		{
			Logger.LogWarning("Stopped exception when spawning controller. InnerException: " + ex.InnerException);
		}
		if (player.LastEquippedWeaponOrKnifeItem != null)
		{
			InteractionsHandlerClass.Discard(player.LastEquippedWeaponOrKnifeItem, (TraderControllerClass)(object)inventoryController, true);
			player.ProcessStatus = (EProcessStatus)0;
			player.TrySetLastEquippedWeapon(true);
		}
		else
		{
			player.ProcessStatus = (EProcessStatus)0;
			player.SetFirstAvailableItem((Callback<IHandsController>)Class1667.class1667_0.method_0);
		}
		player.SetInventoryOpened(false);
		if (handsController != null)
		{
			handsController.Destroy();
		}
		if ((Object)(object)handsController != (Object)null)
		{
			Object.Destroy((Object)(object)handsController);
		}
		AbstractHandsController handsController2 = player.HandsController;
		FirearmController val3 = (FirearmController)(object)((handsController2 is FirearmController) ? handsController2 : null);
		if (val3 != null && val3.Weapon != null)
		{
			Traverse.Create((object)player.ProceduralWeaponAnimation).Field("_firearmAnimationData").SetValue((object)val3);
		}
	}
}
