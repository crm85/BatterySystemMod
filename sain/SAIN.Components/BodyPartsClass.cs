using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EFT;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Models.Structs;
using SAIN.SAINComponent;
using UnityEngine;

namespace SAIN.Components;

public class BodyPartsClass : PlayerComponentBase
{
	private static class PartToBoneTypes
	{
		public static readonly EBodyPart[] PartTypes;

		private static readonly EBodyPartColliderType[] _headParts;

		private static readonly EBodyPartColliderType[] _upperBodyParts;

		private static readonly EBodyPartColliderType[] _lowerBodyParts;

		private static readonly EBodyPartColliderType[] _leftArmParts;

		private static readonly EBodyPartColliderType[] _rightArmParts;

		private static readonly EBodyPartColliderType[] _leftLegParts;

		private static readonly EBodyPartColliderType[] _rightLegParts;

		public static readonly Dictionary<EBodyPart, EBodyPartColliderType[]> PartsToCollidersTypes;

		static PartToBoneTypes()
		{
			EBodyPart[] array = new EBodyPart[7];
			RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			PartTypes = (EBodyPart[])(object)array;
			EBodyPartColliderType[] array2 = new EBodyPartColliderType[5];
			RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			_headParts = (EBodyPartColliderType[])(object)array2;
			EBodyPartColliderType[] array3 = new EBodyPartColliderType[4];
			RuntimeHelpers.InitializeArray(array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			_upperBodyParts = (EBodyPartColliderType[])(object)array3;
			EBodyPartColliderType[] array4 = new EBodyPartColliderType[5];
			RuntimeHelpers.InitializeArray(array4, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			_lowerBodyParts = (EBodyPartColliderType[])(object)array4;
			_leftArmParts = (EBodyPartColliderType[])(object)new EBodyPartColliderType[2]
			{
				(EBodyPartColliderType)4,
				(EBodyPartColliderType)5
			};
			_rightArmParts = (EBodyPartColliderType[])(object)new EBodyPartColliderType[2]
			{
				(EBodyPartColliderType)6,
				(EBodyPartColliderType)7
			};
			_leftLegParts = (EBodyPartColliderType[])(object)new EBodyPartColliderType[2]
			{
				(EBodyPartColliderType)9,
				(EBodyPartColliderType)8
			};
			_rightLegParts = (EBodyPartColliderType[])(object)new EBodyPartColliderType[2]
			{
				(EBodyPartColliderType)11,
				(EBodyPartColliderType)10
			};
			PartsToCollidersTypes = new Dictionary<EBodyPart, EBodyPartColliderType[]>
			{
				{
					(EBodyPart)0,
					_headParts
				},
				{
					(EBodyPart)1,
					_upperBodyParts
				},
				{
					(EBodyPart)2,
					_lowerBodyParts
				},
				{
					(EBodyPart)3,
					_leftArmParts
				},
				{
					(EBodyPart)5,
					_leftLegParts
				},
				{
					(EBodyPart)4,
					_rightArmParts
				},
				{
					(EBodyPart)6,
					_rightLegParts
				}
			};
		}
	}

	public PartDictionary Parts { get; } = new PartDictionary();

	public SAINBodyPart[] PartsArray { get; private set; }

	public BodyPartsClass(PlayerComponent component)
		: base(component)
	{
		createParts();
		PartsArray = Parts.Values.ToArray();
	}

	private void createParts()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		PlayerBones playerBones = base.Player.PlayerBones;
		foreach (KeyValuePair<EBodyPart, EBodyPartColliderType[]> partsToCollidersType in PartToBoneTypes.PartsToCollidersTypes)
		{
			EBodyPart key = partsToCollidersType.Key;
			SAINBodyPart value = createPart(key, playerBones, partsToCollidersType.Value);
			Parts.Add(key, value);
		}
	}

	private BifacialTransform getTransform(EBodyPart bodyPart, PlayerBones bones)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		return (BifacialTransform)((int)bodyPart switch
		{
			0 => bones.Head, 
			1 => bones.Ribcage, 
			2 => bones.Pelvis, 
			3 => bones.LeftShoulder, 
			4 => bones.RightShoulder, 
			5 => bones.LeftThigh1, 
			_ => bones.RightThigh1, 
		});
	}

	private SAINBodyPart createPart(EBodyPart bodyPartType, PlayerBones playerBones, EBodyPartColliderType[] colliderTypes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		BifacialTransform transform = getTransform(bodyPartType, playerBones);
		if (transform == null)
		{
			Logger.LogDebug($"{bodyPartType} has null bifacial transform");
		}
		List<BodyPartCollider> colliders = getColliders(playerBones, colliderTypes);
		if (colliders != null && colliders.Count == 0)
		{
			Logger.LogWarning($"No Colliders for {bodyPartType}!");
		}
		return new SAINBodyPart(bodyPartType, transform, colliders);
	}

	private List<BodyPartCollider> getColliders(PlayerBones playerBones, EBodyPartColliderType[] colliderTypes)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		List<BodyPartCollider> list = new List<BodyPartCollider>();
		if ((Object)(object)playerBones == (Object)null)
		{
			Logger.LogError("Player bones null");
			return list;
		}
		if (colliderTypes == null)
		{
			Logger.LogError("colliderTypes null");
			return list;
		}
		Dictionary<EBodyPartColliderType, BodyPartCollider> bodyPartCollidersDictionary = playerBones.BodyPartCollidersDictionary;
		foreach (EBodyPartColliderType val in colliderTypes)
		{
			if (!bodyPartCollidersDictionary.TryGetValue(val, out var value))
			{
				Logger.LogDebug($"{val} not in collider dictionary");
				continue;
			}
			if ((Object)(object)value == (Object)null || (Object)(object)value.Collider == (Object)null)
			{
				Logger.LogDebug($"{val} has null collider");
				continue;
			}
			if ((Object)(object)((Component)value).transform == (Object)null)
			{
				Logger.LogDebug($"{val} has null transform");
				continue;
			}
			if ((Object)(object)((Component)value.Collider).transform == (Object)null)
			{
				Logger.LogDebug($"{val} collider.Collider has null transform");
			}
			list.Add(value);
		}
		return list;
	}
}
