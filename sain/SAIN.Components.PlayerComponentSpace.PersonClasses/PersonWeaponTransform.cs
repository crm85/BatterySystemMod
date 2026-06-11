using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonWeaponTransform : PersonSubClass
{
	private FirearmController _fireArmController;

	private readonly BifacialTransform _weaponRootTransform;

	public Vector3 FirePort { get; private set; }

	public Vector3 PointDirection { get; private set; }

	public Vector3 Root { get; private set; }

	public FirearmController FirearmController
	{
		get
		{
			if ((Object)(object)_fireArmController == (Object)null)
			{
				ref FirearmController fireArmController = ref _fireArmController;
				AbstractHandsController handsController = base.Player.HandsController;
				fireArmController = (FirearmController)(object)((handsController is FirearmController) ? handsController : null);
			}
			return _fireArmController;
		}
	}

	public void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Root = _weaponRootTransform.position;
		getWeaponTransforms();
	}

	private void getWeaponTransforms()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		FirearmController firearmController = FirearmController;
		if ((Object)(object)firearmController != (Object)null)
		{
			BifacialTransform currentFireport = firearmController.CurrentFireport;
			if (currentFireport != null)
			{
				Vector3 position = currentFireport.position;
				Vector3 val = currentFireport.Original.TransformDirection(base.Player.LocalShotDirection);
				firearmController.AdjustShotVectors(ref position, ref val);
				FirePort = position;
				PointDirection = ((Vector3)(ref val)).normalized;
				return;
			}
		}
		FirePort = Root;
		PointDirection = base.Player.LookDirection;
	}

	public PersonWeaponTransform(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
		_weaponRootTransform = playerData.Player.WeaponRoot;
	}
}
