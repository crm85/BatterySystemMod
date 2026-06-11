using System.Collections;
using System.Collections.Generic;
using EFT.InventoryLogic;
using SAIN.Components.PlayerComponentSpace.Classes.Equipment;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Info;

public class GearInfo
{
	private const float GEAR_UPDATE_FREQ = 2f;

	private readonly List<FaceShieldComponent> _faceShieldComponents = new List<FaceShieldComponent>();

	private readonly List<ArmorComponent> _armorList = new List<ArmorComponent>();

	public bool HasEarPiece { get; private set; }

	public bool HasHelmet => HelmetArmorClass > 0;

	public bool HasHeavyHelmet { get; private set; }

	public int HelmetArmorClass { get; private set; }

	public bool HasFaceShield { get; private set; }

	public bool HasArmor => BodyArmorClass != 0;

	public int BodyArmorClass { get; private set; }

	protected SAINEquipmentClass Equipment { get; private set; }

	protected InventoryEquipment _equipment => Equipment.EquipmentClass;

	public GearInfo(SAINEquipmentClass equipment)
	{
		Equipment = equipment;
	}

	public IEnumerator GearUpdateLoop()
	{
		WaitForSeconds wait = new WaitForSeconds(2f);
		while (true)
		{
			HasEarPiece = GetItem((EquipmentSlot)12) != null;
			yield return null;
			HasFaceShield = false;
			Item helmetItem = GetItem((EquipmentSlot)11);
			if (helmetItem != null)
			{
				GClass3176.GetItemComponentsInChildrenNonAlloc<FaceShieldComponent>(helmetItem, _faceShieldComponents, true);
				yield return null;
				foreach (FaceShieldComponent faceComponent in _faceShieldComponents)
				{
					if (((GClass3175)faceComponent).Item.IsArmorMod())
					{
						HasFaceShield = true;
						break;
					}
				}
				_faceShieldComponents.Clear();
			}
			yield return null;
			HasHeavyHelmet = false;
			HelmetArmorClass = findMaxAC(helmetItem);
			foreach (ArmorComponent armor in _armorList)
			{
				if ((int)armor.Deaf == 2)
				{
					HasHeavyHelmet = true;
					break;
				}
			}
			_armorList.Clear();
			yield return null;
			int vestAC = findMaxAC((EquipmentSlot)7);
			_armorList.Clear();
			yield return null;
			int bodyAC = findMaxAC((EquipmentSlot)6);
			_armorList.Clear();
			BodyArmorClass = Mathf.Max(vestAC, bodyAC);
			yield return wait;
		}
	}

	public Item GetItem(EquipmentSlot slot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _equipment.GetSlot(slot).ContainedItem;
	}

	private int findMaxAC(Item item)
	{
		if (item == null)
		{
			return 0;
		}
		GClass3176.GetItemComponentsInChildrenNonAlloc<ArmorComponent>(item, _armorList, true);
		int num = 0;
		for (int i = 0; i < _armorList.Count; i++)
		{
			ArmorComponent val = _armorList[i];
			if (val.ArmorClass > num)
			{
				num = val.ArmorClass;
			}
		}
		return num;
	}

	private int findMaxAC(EquipmentSlot slot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Item containedItem = _equipment.GetSlot(slot).ContainedItem;
		return findMaxAC(containedItem);
	}
}
