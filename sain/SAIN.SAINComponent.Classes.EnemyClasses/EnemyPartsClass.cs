using System.Collections.Generic;
using System.Linq;
using SAIN.Components;
using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyPartsClass : EnemyBase
{
	private const float LINEOFSIGHT_TIME = 0.25f;

	private const float CANSHOOT_TIME = 0.25f;

	private float _timeLastInSight;

	private float _timeLastCanShoot;

	private int _index;

	private readonly int _indexMax;

	public bool LineOfSight => TimeSinceInLineOfSight < 0.25f;

	public float TimeSinceInLineOfSight => Time.time - _timeLastInSight;

	public bool CanShoot => TimeSinceCanShoot < 0.25f;

	public float TimeSinceCanShoot => Time.time - _timeLastCanShoot;

	public Dictionary<EBodyPart, EnemyPartDataClass> Parts { get; } = new Dictionary<EBodyPart, EnemyPartDataClass>();

	public EnemyPartDataClass[] PartsArray { get; private set; }

	public EnemyPartsClass(Enemy enemy)
		: base(enemy)
	{
		createPartDatas(enemy.Player.PlayerBones);
		PartsArray = Parts.Values.ToArray();
		_indexMax = Parts.Count;
	}

	public void Update()
	{
		UpdateParts();
	}

	private void UpdateParts()
	{
		bool flag = false;
		bool flag2 = false;
		float time = Time.time;
		foreach (EnemyPartDataClass value in Parts.Values)
		{
			value.Update(base.Enemy);
			if (!flag2 && value.CanShoot)
			{
				flag2 = true;
				_timeLastCanShoot = time;
			}
			if (!flag && value.LineOfSight)
			{
				flag = true;
				_timeLastInSight = time;
			}
		}
	}

	public EnemyPartDataClass GetNextPart()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		EBodyPart key = (EBodyPart)_index;
		if (!Parts.TryGetValue(key, out var value))
		{
			_index = 0;
			value = Parts[(EBodyPart)1];
		}
		_index++;
		if (_index > _indexMax)
		{
			_index = 0;
		}
		if (value == null)
		{
			value = GClass1835.PickRandom<KeyValuePair<EBodyPart, EnemyPartDataClass>>((IEnumerable<KeyValuePair<EBodyPart, EnemyPartDataClass>>)Parts).Value;
		}
		return value;
	}

	private void createPartDatas(PlayerBones bones)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		PartDictionary parts = base.Enemy.EnemyPlayerComponent.BodyParts.Parts;
		foreach (KeyValuePair<EBodyPart, SAINBodyPart> item in parts)
		{
			Parts.Add(item.Key, new EnemyPartDataClass(item.Key, item.Value.Transform, item.Value.Colliders));
		}
	}
}
