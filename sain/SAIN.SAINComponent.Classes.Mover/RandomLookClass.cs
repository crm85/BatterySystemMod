using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class RandomLookClass : BotSubClass<SAINSteeringClass>
{
	private Vector3? _randomLookPoint;

	private float _randomLookTime = 0f;

	private bool _lookRandomToggle;

	public RandomLookClass(SAINSteeringClass steeringClass)
		: base(steeringClass)
	{
	}

	public Vector3? UpdateRandomLook()
	{
		if (_randomLookTime < Time.time)
		{
			_lookRandomToggle = !_lookRandomToggle;
			_randomLookPoint = FindRandomLookPos(out var isRandomLook);
			if (!_randomLookPoint.HasValue)
			{
				_randomLookTime = Time.time + 0.1f;
			}
			else
			{
				float num = (isRandomLook ? 2f : 4f);
				_randomLookTime = Time.time + num * Random.Range(0.66f, 1.33f);
			}
		}
		return _randomLookPoint;
	}

	private Vector3? FindRandomLookPos(out bool isRandomLook, int percentChancetoRandomLook = 40)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		_lookRandomToggle = EFTMath.RandomBool(percentChancetoRandomLook);
		if (_lookRandomToggle && GenerateRandomLookPos(out var result))
		{
			isRandomLook = true;
			return result;
		}
		isRandomLook = false;
		if (EFTMath.RandomBool() && base.BaseClass.FindLastKnownTarget(base.Bot.Enemy, out var Result))
		{
			return Result;
		}
		EnemyList knownEnemies = base.Bot.EnemyController.EnemyLists.KnownEnemies;
		int count = knownEnemies.Count;
		if (count > 0)
		{
			if (count == 1)
			{
				if (base.BaseClass.FindLastKnownTarget(knownEnemies[0], out Result))
				{
					return Result;
				}
				return null;
			}
			for (int i = 0; i < Mathf.Min(count, 4); i++)
			{
				if (base.BaseClass.FindLastKnownTarget(knownEnemies[Random.Range(0, count - 1)], out Result))
				{
					return Result;
				}
			}
		}
		return null;
	}

	private bool GenerateRandomLookPos(out Vector3 result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		LayerMask highPolyWithTerrainMaskAI = LayerMaskClass.HighPolyWithTerrainMaskAI;
		Vector3 headPosition = base.Bot.Transform.HeadPosition;
		bool result2 = false;
		float num = 0f;
		result = Vector3.zero;
		RaycastHit val = default(RaycastHit);
		for (int i = 0; i < 6; i++)
		{
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			((Vector3)(ref onUnitSphere)).Normalize();
			if (!Physics.Raycast(headPosition, onUnitSphere, ref val, 12f, LayerMask.op_Implicit(highPolyWithTerrainMaskAI)))
			{
				result = onUnitSphere + headPosition;
				return true;
			}
			if (((RaycastHit)(ref val)).distance > num)
			{
				num = ((RaycastHit)(ref val)).distance;
				result = ((RaycastHit)(ref val)).point;
				result2 = true;
			}
		}
		return result2;
	}
}
