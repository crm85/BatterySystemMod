using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EFT.InventoryLogic;
using HarmonyLib;
using SAIN.Components;
using SAIN.Preset.GlobalSettings;

namespace SAIN.SAINComponent.Classes;

public class BotWeightManagement : BotComponentClassBase
{
	private readonly List<Slot> _slots = new List<Slot>();

	public static readonly EquipmentSlot[] _botEquipmentSlots;

	public BotWeightManagement(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public override void Init()
	{
		if (GlobalSettingsClass.Instance.General.BotWeightEffects)
		{
			getSlots();
			Traverse.Create((object)base.Person.Player.InventoryController.Inventory).Field<GClass828<float>>("TotalWeight").Value = new GClass828<float>((Func<float>)getBotTotalWeight);
			base.Bot.Person.Player.Physical.EncumberDisabled = false;
		}
		base.Init();
	}

	private void getSlots()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_slots.Clear();
		EquipmentSlot[] botEquipmentSlots = _botEquipmentSlots;
		foreach (EquipmentSlot val in botEquipmentSlots)
		{
			_slots.Add(base.Player.Equipment.GetSlot(val));
		}
	}

	private float getBotTotalWeight()
	{
		float result = InventoryEquipment.smethod_1((IEnumerable<Slot>)_slots);
		_slots.Clear();
		return result;
	}

	static BotWeightManagement()
	{
		EquipmentSlot[] array = new EquipmentSlot[11];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		_botEquipmentSlots = (EquipmentSlot[])(object)array;
	}
}
