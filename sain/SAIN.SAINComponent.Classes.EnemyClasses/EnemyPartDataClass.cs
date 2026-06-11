using System.Collections.Generic;
using EFT;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyPartDataClass
{
	private readonly Dictionary<EBodyPartColliderType, BodyPartCollider> _colliderDictionary = new Dictionary<EBodyPartColliderType, BodyPartCollider>();

	public readonly EBodyPart BodyPart;

	public readonly List<BodyPartCollider> Colliders;

	public readonly BifacialTransform Transform;

	private int _index;

	private readonly int _indexMax;

	public Dictionary<ERaycastCheck, RaycastResult> RaycastResults { get; private set; } = new Dictionary<ERaycastCheck, RaycastResult>();

	public float TimeSeen { get; private set; }

	public bool IsVisible { get; private set; }

	public float TimeSinceLastVisionCheck => Mathf.Max(RaycastResults[ERaycastCheck.LineofSight].TimeSinceChecked, RaycastResults[ERaycastCheck.Vision].TimeSinceChecked);

	public float TimeSinceLastVisionSuccess => Mathf.Max(RaycastResults[ERaycastCheck.LineofSight].TimeSinceSuccess, RaycastResults[ERaycastCheck.Vision].TimeSinceSuccess);

	public bool LineOfSight => RaycastResults[ERaycastCheck.LineofSight].InSight;

	public bool CanShoot => RaycastResults[ERaycastCheck.Shoot].InSight;

	public EnemyPartDataClass(EBodyPart bodyPart, BifacialTransform transform, List<BodyPartCollider> colliders)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		BodyPart = bodyPart;
		Transform = transform;
		Colliders = colliders;
		_indexMax = colliders.Count - 1;
		foreach (BodyPartCollider collider in colliders)
		{
			if (!_colliderDictionary.ContainsKey(collider.BodyPartColliderType))
			{
				_colliderDictionary.Add(collider.BodyPartColliderType, collider);
			}
		}
		RaycastResults.Add(ERaycastCheck.LineofSight, new RaycastResult());
		RaycastResults.Add(ERaycastCheck.Shoot, new RaycastResult());
		RaycastResults.Add(ERaycastCheck.Vision, new RaycastResult());
	}

	public void Update(Enemy enemy)
	{
		IsVisible = enemy.Vision.Angles.CanBeSeen && TimeSinceLastVisionSuccess < 0.25f;
		if (!IsVisible)
		{
			TimeSeen = 0f;
		}
		else if (TimeSeen <= 0f)
		{
			TimeSeen = Time.time;
		}
	}

	public void SetLineOfSight(Vector3 castPoint, EBodyPartColliderType colliderType, RaycastHit raycastHit, ERaycastCheck type, float time)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		RaycastResults[type].Update(castPoint, _colliderDictionary[colliderType], raycastHit, time);
	}

	public SAINBodyPartRaycast GetRaycast()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		BodyPartCollider collider = GetCollider();
		return new SAINBodyPartRaycast
		{
			CastPoint = GetCastPoint(collider),
			PartType = BodyPart,
			ColliderType = collider.BodyPartColliderType
		};
	}

	private BodyPartCollider GetCollider()
	{
		BodyPartCollider result = Colliders[_index];
		_index++;
		if (_index > _indexMax)
		{
			_index = 0;
		}
		return result;
	}

	private Vector3 GetCastPoint(BodyPartCollider collider)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		float colliderMinSize = GetColliderMinSize(collider);
		Vector3 val = Random.insideUnitSphere * colliderMinSize;
		return collider.Collider.ClosestPoint(((Component)collider).transform.position + val);
	}

	private float GetColliderMinSize(BodyPartCollider collider)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)collider.Collider == (Object)null)
		{
			return 0f;
		}
		Bounds bounds = collider.Collider.bounds;
		Vector3 size = ((Bounds)(ref bounds)).size;
		float num = size.x;
		if (size.y < num)
		{
			num = size.y;
		}
		if (size.z < num)
		{
			num = size.z;
		}
		return num;
	}
}
