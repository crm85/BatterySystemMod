using System.Collections.Generic;
using EFT;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyCornerDictionary : Dictionary<ECornerType, EnemyCorner>
{
	private BifacialTransform _weaponRoot;

	private PersonTransformClass _transform;

	public EnemyCornerDictionary(PersonTransformClass transform, BifacialTransform weaponRoot)
	{
		_transform = transform;
		_weaponRoot = weaponRoot;
	}

	public Vector3? GroundPosition(ECornerType type)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return GetCorner(type)?.GroundPosition;
	}

	public EnemyCorner GetCorner(ECornerType cornerType)
	{
		if (TryGetValue(cornerType, out var value))
		{
			return value;
		}
		return null;
	}

	public void AddOrReplace(ECornerType type, EnemyCorner corner)
	{
		if (corner == null)
		{
			Remove(type);
		}
		else if (!ContainsKey(type))
		{
			Add(type, corner);
		}
		else
		{
			base[type] = corner;
		}
	}
}
