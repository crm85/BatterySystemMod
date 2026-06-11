using System;
using System.Collections.Generic;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using UnityEngine;

namespace SAIN.Classes.Coverfinder;

public class CoverPointClass
{
	private Vector3 _position;

	private static int _count;

	public Collider Collider { get; }

	public Vector3 ColliderPosition { get; }

	public int Id { get; }

	public float Height { get; }

	public float Value { get; }

	public Dictionary<BotComponent, CoverPointBotDataClass> BotData { get; } = new Dictionary<BotComponent, CoverPointBotDataClass>();

	public bool IsInUse { get; set; }

	public Vector3 ProtectionDirection { get; private set; }

	public float TimeLastUpdated { get; private set; }

	public float TimeSinceUpdated => Time.time - TimeLastUpdated;

	public Vector3 CoverPosition
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _position;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _position;
			Vector3 val = value - position;
			if (!((double)((Vector3)(ref val)).sqrMagnitude < 0.001))
			{
				_position = value;
				Vector3 val2 = ColliderPosition - value;
				val2.y = 0f;
				ProtectionDirection = ((Vector3)(ref val2)).normalized;
				TimeLastUpdated = Time.time;
				this.OnPositionUpdated?.Invoke(value);
			}
		}
	}

	public event Action<Vector3> OnPositionUpdated;

	public CoverPointClass(Collider collider, Vector3 colliderPos, Vector3 coverPosition)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Collider = collider;
		ColliderPosition = colliderPos;
		Bounds bounds = collider.bounds;
		Vector3 size = ((Bounds)(ref bounds)).size;
		Height = size.y;
		Value = (size.x + size.y + size.z).Round10();
		Id = _count;
		_count++;
		_position = coverPosition;
		Vector3 val = colliderPos - coverPosition;
		val.y = 0f;
		ProtectionDirection = ((Vector3)(ref val)).normalized;
		TimeLastUpdated = Time.time;
	}

	public CoverPointBotDataClass GetBotData(BotComponent bot)
	{
		return BotData.ContainsKey(bot) ? BotData[bot] : null;
	}

	public CoverPointBotDataClass CreateBotData(BotComponent bot, PathData pathData)
	{
		if (BotData.ContainsKey(bot))
		{
			BotData.Remove(bot);
		}
		CoverPointBotDataClass coverPointBotDataClass = new CoverPointBotDataClass(bot, this, pathData);
		BotData.Add(bot, coverPointBotDataClass);
		return coverPointBotDataClass;
	}
}
