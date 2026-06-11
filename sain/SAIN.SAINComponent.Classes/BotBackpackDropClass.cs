using System.Collections;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class BotBackpackDropClass : BotComponentClassBase
{
	private Coroutine _dropCoroutine;

	public EBackpackStatus BackpackStatus { get; private set; }

	private Item _currentBackpack => base.Bot.PlayerComponent.Equipment.GearInfo.GetItem((EquipmentSlot)4);

	public bool BackpackDropped => BackpackDropPosition.HasValue;

	public Item DroppedBackpack { get; private set; }

	public Vector3? BackpackDropPosition { get; private set; }

	public BotBackpackDropClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
	}

	public override void ManualUpdate()
	{
		if (BackpackDropPosition.HasValue && DroppedBackpack == null)
		{
			BackpackDropPosition = null;
		}
	}

	public bool DropBackpack()
	{
		return false;
	}

	private void executeDrop()
	{
		if (_dropCoroutine == null)
		{
			_dropCoroutine = ((MonoBehaviour)base.Bot).StartCoroutine(executeBackpackDrop());
		}
	}

	private IEnumerator executeBackpackDrop()
	{
		Item backpack = _currentBackpack;
		if (backpack != null)
		{
			if (DroppedBackpack == null)
			{
				DroppedBackpack = backpack;
			}
			base.Player.DropBackpack();
		}
		yield return (object)new WaitForSeconds(0.5f);
		if (_currentBackpack == null)
		{
			BackpackDropPosition = base.Bot.Position;
			BackpackStatus = EBackpackStatus.Dropped;
			Logger.LogInfo($"{((Object)base.BotOwner).name} Dropped Backpack at {base.Bot.Position} at {Time.time}");
		}
	}

	public bool RetreiveBackpack()
	{
		return false;
	}
}
