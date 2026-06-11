using System;
using System.Collections.Generic;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyPlace : IDisposable
{
	private const float ENEMY_DIST_TO_PLACE_CHECK_FREQ = 10f;

	private const float ENEMY_DIST_TO_PLACE_FOR_LEAVE = 150f;

	private const float ENEMY_DIST_TO_PLACE_FOR_LEAVE_AI = 100f;

	private const float ENEMY_DIST_RECHECK_MIN_SQRMAG = 0.25f;

	private Vector3 _position;

	private float _nextCheckLeaveTime;

	public float _timeLastUpdated;

	private bool _hasArrivedPers;

	public float _timeArrivedPers;

	private bool _hasArrivedSquad;

	public float _timeArrivedSquad;

	private bool _hasSeenPers;

	public float _timeSeenPers;

	private bool _hasSquadSeen;

	public float _timeSquadSeen;

	public PlaceData PlaceData { get; }

	public EEnemyPlaceType PlaceType { get; }

	public SAINSoundType? SoundType { get; set; }

	public bool Visible { get; private set; }

	public bool IsDanger { get; set; }

	public Vector3 BotPositionWhenUpdated { get; protected set; }

	public Vector3 EnemyRealPositionWhenUpdated { get; protected set; }

	public Dictionary<EBodyPart, Vector3> BodyPartPositions { get; } = new Dictionary<EBodyPart, Vector3>();

	public bool ShallClear
	{
		get
		{
			PersonClass personClass = PlaceData.Enemy?.EnemyPerson;
			if (personClass == null)
			{
				return true;
			}
			PersonActiveClass activationClass = personClass.ActivationClass;
			if (!activationClass.Active || !activationClass.IsAlive)
			{
				return true;
			}
			if (PlayerLeftArea)
			{
				return true;
			}
			return false;
		}
	}

	private bool PlayerLeftArea
	{
		get
		{
			if (_nextCheckLeaveTime < Time.time)
			{
				_nextCheckLeaveTime = Time.time + 10f;
				float distanceToEnemyRealPosition = DistanceToEnemyRealPosition;
				if (PlaceData.IsAI)
				{
					return distanceToEnemyRealPosition > 100f;
				}
				return distanceToEnemyRealPosition > 150f;
			}
			return false;
		}
	}

	public Vector3 Position => _position;

	public float TimeSincePositionUpdated => Time.time - _timeLastUpdated;

	public float DistanceToBot { get; private set; }

	public float DistanceToEnemyRealPosition { get; private set; }

	public bool HasArrivedPersonal
	{
		get
		{
			return _hasArrivedPers;
		}
		set
		{
			if (value)
			{
				_timeArrivedPers = Time.time;
				HasSeenPersonal = true;
			}
			_hasArrivedPers = value;
		}
	}

	public bool HasArrivedSquad
	{
		get
		{
			return _hasArrivedSquad;
		}
		set
		{
			if (value)
			{
				_timeArrivedSquad = Time.time;
			}
			_hasArrivedSquad = value;
		}
	}

	public bool HasSeenPersonal
	{
		get
		{
			return _hasSeenPers;
		}
		set
		{
			if (value)
			{
				_timeSeenPers = Time.time;
			}
			_hasSeenPers = value;
		}
	}

	public bool HasSeenSquad
	{
		get
		{
			return _hasSquadSeen;
		}
		set
		{
			if (value)
			{
				_timeSquadSeen = Time.time;
			}
			_hasSquadSeen = value;
		}
	}

	public event Action<EnemyPlace> OnPositionUpdated;

	public event Action<EnemyPlace> OnDispose;

	public Vector3 EnemyHeadAtPosition()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (BodyPartPositions.ContainsKey((EBodyPart)0))
		{
			return BodyPartPositions[(EBodyPart)0];
		}
		return Position + Vector3.up * 1.5f;
	}

	public void SetDistances(float BotDistanceToPlace, float EnemyDistanceToPlace, BotComponent bot)
	{
		if ((Object)(object)bot == (Object)(object)PlaceData.Owner)
		{
			DistanceToBot = BotDistanceToPlace;
			if (BotDistanceToPlace < 1f)
			{
				HasArrivedPersonal = true;
			}
		}
		else if (BotDistanceToPlace < 1f)
		{
			HasArrivedSquad = true;
		}
		DistanceToEnemyRealPosition = EnemyDistanceToPlace;
	}

	public void SetVisibilityOfPlace(bool Value, BotComponent bot)
	{
		if ((Object)(object)bot == (Object)(object)PlaceData.Owner)
		{
			Visible = Value;
			if (Value)
			{
				HasSeenPersonal = true;
			}
		}
		else if (Value)
		{
			HasSeenSquad = true;
		}
	}

	public EnemyPlace(PlaceData placeData, Vector3 position, bool isDanger, EEnemyPlaceType placeType, SAINSoundType? soundType)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PlaceData = placeData;
		IsDanger = isDanger;
		PlaceType = placeType;
		SoundType = soundType;
		SetPosition(position);
	}

	public EnemyPlace(PlaceData placeData, SAINHearingReport report)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		PlaceData = placeData;
		IsDanger = report.isDanger;
		PlaceType = report.placeType;
		SoundType = report.soundType;
		SetPosition(report.position);
	}

	private void SetPosition(Vector3 position)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		BotPositionWhenUpdated = PlaceData.Owner.Position;
		EnemyRealPositionWhenUpdated = PlaceData.Enemy.EnemyPosition;
		_position = position;
		updateDistancesNow(position);
		SetLastKnownPartPositions(PlaceData.Enemy);
		_timeLastUpdated = Time.time;
	}

	private void SetLastKnownPartPositions(Enemy enemy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		BodyPartPositions.Clear();
		Vector3 position = _position;
		Vector3 enemyPosition = enemy.EnemyPosition;
		EnemyPartDataClass[] partsArray = enemy.Vision.VisionChecker.EnemyParts.PartsArray;
		foreach (EnemyPartDataClass enemyPartDataClass in partsArray)
		{
			Vector3 value = enemyPartDataClass.Transform.position - enemyPosition + position;
			BodyPartPositions.Add(enemyPartDataClass.BodyPart, value);
		}
	}

	public void Dispose()
	{
		this.OnDispose?.Invoke(this);
	}

	public void UpdatePosition(Vector3 value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		checkNewValue(value, _position);
		_position = value;
		_timeLastUpdated = Time.time;
		BotPositionWhenUpdated = PlaceData.Owner.Position;
		Visible = false;
		SetLastKnownPartPositions(PlaceData.Enemy);
		this.OnPositionUpdated?.Invoke(this);
		HasArrivedPersonal = false;
		HasArrivedSquad = false;
		HasSeenPersonal = false;
		HasSeenSquad = false;
	}

	private void checkNewValue(Vector3 value, Vector3 oldValue)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = value - oldValue;
		if (((Vector3)(ref val)).sqrMagnitude > 0.25f)
		{
			updateDistancesNow(value);
		}
	}

	private void updateDistancesNow(Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = position - PlaceData.Owner.Position;
		DistanceToBot = ((Vector3)(ref val)).magnitude;
		val = position - PlaceData.Enemy.EnemyTransform.Position;
		DistanceToEnemyRealPosition = ((Vector3)(ref val)).magnitude;
	}

	public float Distance(Vector3 point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _position - point;
		return ((Vector3)(ref val)).magnitude;
	}

	public float DistanceSqr(Vector3 toPoint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _position - toPoint;
		return ((Vector3)(ref val)).sqrMagnitude;
	}
}
