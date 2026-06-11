using System;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using SAIN.SAINComponent;
using SAIN.SAINComponent.Classes.Info;

namespace SAIN.Components.PlayerComponentSpace.Classes.Equipment;

public class SAINEquipmentClass : PlayerComponentBase
{
	private static readonly EquipmentSlot[] _weaponSlots = (EquipmentSlot[])(object)new EquipmentSlot[3]
	{
		default(EquipmentSlot),
		(EquipmentSlot)1,
		(EquipmentSlot)2
	};

	public InventoryEquipment EquipmentClass { get; private set; }

	public Action<float> OnPowerRecalced { get; set; }

	public GearInfo GearInfo { get; private set; }

	public WeaponInfo CurrentWeaponInfo { get; private set; }

	public WeaponInfo WeaponInInventory => PrimaryWeapon ?? SecondaryWeapon ?? HolsterWeapon;

	public WeaponInfo PrimaryWeapon => GetWeaponInfo((EquipmentSlot)0);

	public WeaponInfo SecondaryWeapon => GetWeaponInfo((EquipmentSlot)1);

	public WeaponInfo HolsterWeapon => GetWeaponInfo((EquipmentSlot)2);

	public Dictionary<EquipmentSlot, WeaponInfo> WeaponInfos { get; } = new Dictionary<EquipmentSlot, WeaponInfo>();

	public SAINEquipmentClass(PlayerComponent playerComponent)
		: base(playerComponent)
	{
		EquipmentClass = playerComponent.Player.Equipment;
		GearInfo = new GearInfo(this);
	}

	public void Init()
	{
		getAllWeapons();
		base.PlayerComponent.OnWeaponEquipped += OnWeaponEquiped;
	}

	public override void Dispose()
	{
		foreach (KeyValuePair<EquipmentSlot, WeaponInfo> weaponInfo in WeaponInfos)
		{
			weaponInfo.Value?.Dispose();
		}
		WeaponInfos.Clear();
		base.PlayerComponent.OnWeaponEquipped -= OnWeaponEquiped;
	}

	public void WeaponModified(Weapon Weapon)
	{
		foreach (KeyValuePair<EquipmentSlot, WeaponInfo> weaponInfo in WeaponInfos)
		{
			if (weaponInfo.Value.Weapon == Weapon)
			{
				weaponInfo.Value.WeaponModified(base.Player);
				return;
			}
		}
		getAllWeapons();
		foreach (KeyValuePair<EquipmentSlot, WeaponInfo> weaponInfo2 in WeaponInfos)
		{
			if (weaponInfo2.Value.Weapon == Weapon)
			{
				weaponInfo2.Value.WeaponModified(base.Player);
				break;
			}
		}
	}

	protected void OnWeaponEquiped(Weapon weapon, Weapon lastWeapon)
	{
		if (weapon != null)
		{
			GetCurrentWeaponInfo(weapon);
			if (CurrentWeaponInfo == null)
			{
				getAllWeapons();
				GetCurrentWeaponInfo(weapon);
			}
		}
	}

	private void ReCalcPowerOfEquipment()
	{
		if (SAINPlugin.LoadedPreset.GlobalSettings.PowerCalc.CalcPower(base.PlayerComponent, out var power))
		{
			OnPowerRecalced?.Invoke(power);
		}
	}

	public void Update()
	{
		if (CurrentWeaponInfo != null)
		{
			return;
		}
		AbstractHandsController handsController = base.Player.HandsController;
		Item obj = ((handsController != null) ? handsController.Item : null);
		Weapon val = (Weapon)(object)((obj is Weapon) ? obj : null);
		if (val == null)
		{
			return;
		}
		foreach (WeaponInfo value in WeaponInfos.Values)
		{
			if (value.Weapon == val)
			{
				SetCurrentWeaponInfo(value);
			}
		}
	}

	private void SetCurrentWeaponInfo(WeaponInfo WeaponInfo)
	{
		WeaponInfo.WeaponEquiped(base.Player);
		CurrentWeaponInfo = WeaponInfo;
		ReCalcPowerOfEquipment();
	}

	private void getAllWeapons()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		EquipmentSlot[] weaponSlots = _weaponSlots;
		foreach (EquipmentSlot val in weaponSlots)
		{
			Item containedItem = EquipmentClass.GetSlot(val).ContainedItem;
			Weapon val2 = (Weapon)(object)((containedItem is Weapon) ? containedItem : null);
			if (val2 != null)
			{
				WeaponInfo value;
				if (!WeaponInfos.ContainsKey(val))
				{
					WeaponInfos.Add(val, new WeaponInfo(val2));
				}
				else if (WeaponInfos.TryGetValue(val, out value) && value.Weapon != val2)
				{
					value.Dispose();
					WeaponInfos[val] = new WeaponInfo(val2);
				}
			}
		}
	}

	private void GetCurrentWeaponInfo(Weapon weapon)
	{
		foreach (WeaponInfo value in WeaponInfos.Values)
		{
			if (weapon == value.Weapon)
			{
				SetCurrentWeaponInfo(value);
				return;
			}
		}
		CurrentWeaponInfo = null;
	}

	public WeaponInfo GetWeaponInfo(EquipmentSlot slot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (WeaponInfos.TryGetValue(slot, out var value))
		{
			return value;
		}
		return null;
	}
}
