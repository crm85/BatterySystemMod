using System;
using System.Collections.Generic;
using SAIN.Helpers;
using SAIN.Models.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class SAINEnemyPath : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private const float ENEMY_DISTANCE_VERYCLOSE = 10f;

	private const float ENEMY_DISTANCE_CLOSE = 20f;

	private const float ENEMY_DISTANCE_MID = 80f;

	private const float ENEMY_DISTANCE_FAR = 150f;

	protected List<PathSegment> PathVisionSegments = new List<PathSegment>();

	private float _nextLogTime;

	private const float ACTIVE_SEARCH_COEF = 0.5f;

	private const float MAX_FREQ_CALCPATH = 2f;

	private const float MAX_FREQ_CALCPATH_AI = 4f;

	private const float MAX_FREQ_CALCPATH_DISTANCE = 250f;

	private const float MIN_FREQ_CALCPATH = 0.33f;

	private const float MIN_FREQ_CALCPATH_AI = 0.66f;

	private const float MIN_FREQ_CALCPATH_DISTANCE = 50f;

	private const float DISTANCE_DIFFERENCE = 200f;

	private const float PERFORMANCE_MODE_COEF = 1.5f;

	private const float CURRENTENEMY_COEF = 0.5f;

	private const float MAX_CALCPATH_RANGE = 500f;

	private const float MAX_CALCPATH_RANGE_AI = 300f;

	private Vector3? _enemyLastPosChecked;

	private Vector3 _botLastPosChecked;

	private float _calcPathTime = 0f;

	public EPathDistance EPathDistance
	{
		get
		{
			float pathLength = PathLength;
			if (pathLength <= 10f)
			{
				return EPathDistance.VeryClose;
			}
			if (pathLength <= 20f)
			{
				return EPathDistance.Close;
			}
			if (pathLength <= 80f)
			{
				return EPathDistance.Mid;
			}
			if (pathLength <= 150f)
			{
				return EPathDistance.Far;
			}
			return EPathDistance.VeryFar;
		}
	}

	public float PathLength { get; private set; } = float.MaxValue;

	public float DistanceToEnemyPositionFromLastCorner { get; private set; }

	public EnemyCornerDictionary EnemyCorners { get; private set; } = new EnemyCornerDictionary(enemy.Bot.Transform, enemy.BotOwner.WeaponRoot);

	public NavMeshPath PathToEnemy { get; } = new NavMeshPath();

	public NavMeshPathStatus PathToEnemyStatus { get; private set; }

	public Vector3[] PathCorners { get; private set; }

	public List<Vector3> VisionPathCheckPoints { get; } = new List<Vector3>();

	public List<Vector3> VisionPathPoints { get; } = new List<Vector3>();

	public List<Vector3> VisionPathPoints_Cache { get; } = new List<Vector3>();

	public SAINEnemyPath(Enemy enemy)
		: base(enemy)
	{
	}//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0037: Expected O, but got Unknown


	public override void Init()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		base.Init();
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known)
		{
			Clear();
		}
	}

	public override void ManualUpdate()
	{
		CheckCalcPath();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		base.Dispose();
	}

	public void CheckCalcPath()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Invalid comparison between Unknown and I4
		if (!ShallCalcNewPath())
		{
			return;
		}
		Vector3 value = base.Enemy.KnownPlaces.LastKnownPosition.Value;
		PathToEnemy.ClearCorners();
		NavMesh.CalculatePath(base.Bot.Position, value, -1, PathToEnemy);
		PathToEnemyStatus = PathToEnemy.status;
		PathCorners = PathToEnemy.corners;
		CalcPathDistanceAndCreateVisionCheckSegments();
		NavMeshPathStatus pathToEnemyStatus = PathToEnemyStatus;
		NavMeshPathStatus val = pathToEnemyStatus;
		if ((int)val > 1)
		{
			if ((int)val == 2)
			{
				EnemyCorners.Clear();
			}
		}
		else
		{
			findCorners(value, PathToEnemyStatus, PathCorners);
		}
		base.Enemy.Events.PathUpdated(PathToEnemyStatus);
	}

	public bool ShallCalcNewPath()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Enemy;
		if (enemy != null && !enemy.EnemyKnown)
		{
			return false;
		}
		Vector3? lastKnownPosition = base.Enemy.KnownPlaces.LastKnownPosition;
		if (!lastKnownPosition.HasValue)
		{
			return false;
		}
		if (!base.Enemy.IsCurrentEnemy && !isEnemyInRange())
		{
			return false;
		}
		if (!checkPositionsChanged(base.Bot.Position, lastKnownPosition.Value))
		{
			return false;
		}
		if (_calcPathTime + calcDelayOnDistance() > Time.time)
		{
			return false;
		}
		_calcPathTime = Time.time;
		return true;
	}

	private void CalcPathDistanceAndCreateVisionCheckSegments()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		float num = (base.Enemy.IsAI ? 0.5f : 0.25f);
		PathVisionSegments.Clear();
		VisionPathCheckPoints.Clear();
		PathLength = 0f;
		int num2 = PathCorners.Length;
		for (int i = 0; i < num2 - 1; i++)
		{
			Vector3 val = PathCorners[i];
			Vector3 val2 = PathCorners[i + 1];
			Vector3 val3 = val2 - val;
			float magnitude = ((Vector3)(ref val3)).magnitude;
			PathLength += magnitude;
			if (i <= 0)
			{
				continue;
			}
			if (i == 1)
			{
				VisionPathCheckPoints.Add(val);
			}
			bool flag = i == num2 - 2;
			if (PathLength <= 50f)
			{
				if (magnitude > num)
				{
					if (magnitude > num * 2f)
					{
						Vector.GeneratePointsAlongDirection(VisionPathCheckPoints, val, val3, magnitude, num);
					}
					else
					{
						VisionPathCheckPoints.Add(val + val3 * 0.5f);
					}
				}
				VisionPathCheckPoints.Add(val2);
			}
			else if (flag)
			{
				VisionPathCheckPoints.Add(val2);
			}
		}
		int num3 = (base.Enemy.IsAI ? 128 : 512);
		VisionPathPoints.Clear();
		for (int j = 0; j < VisionPathCheckPoints.Count; j++)
		{
			Vector.GeneratePointsAlongDirection(VisionPathPoints, VisionPathCheckPoints[j], Vector3.up, 1.5f, 0.375f);
			if (VisionPathPoints.Count >= num3)
			{
				break;
			}
		}
		if (num2 > 0)
		{
			Vector3 val4 = base.Enemy.LastKnownPosition.Value - PathCorners[num2 - 1];
			DistanceToEnemyPositionFromLastCorner = ((Vector3)(ref val4)).magnitude;
		}
		else
		{
			DistanceToEnemyPositionFromLastCorner = 0f;
		}
	}

	private void findCorners(Vector3 enemyPosition, NavMeshPathStatus status, Vector3[] corners)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EnemyCorner corner = findFirstCorner(enemyPosition, corners);
		EnemyCorners.AddOrReplace(ECornerType.First, corner);
		EnemyCorner corner2 = findLastCorner(enemyPosition, status, corners);
		EnemyCorners.AddOrReplace(ECornerType.Last, corner2);
		EnemyCorner corner3 = createLastKnownCorner(enemyPosition, corners.Length - 1);
		EnemyCorners.AddOrReplace(ECornerType.LastKnown, corner3);
	}

	public void Clear()
	{
		_calcPathTime = 0f;
		PathToEnemy.ClearCorners();
		PathToEnemyStatus = (NavMeshPathStatus)2;
		EnemyCorners.Clear();
		PathLength = float.MaxValue;
		PathCorners = null;
		DistanceToEnemyPositionFromLastCorner = 0f;
		PathVisionSegments.Clear();
		VisionPathCheckPoints.Clear();
	}

	private float calcDelayOnDistance()
	{
		bool performanceMode = SAINPlugin.LoadedPreset.GlobalSettings.General.Performance.PerformanceMode;
		bool isCurrentEnemy = base.Enemy.IsCurrentEnemy;
		bool isAI = base.Enemy.IsAI;
		bool value = base.Enemy.Events.OnSearch.Value;
		float realDistance = base.Enemy.RealDistance;
		float num = (isAI ? 4f : 2f);
		if (isCurrentEnemy)
		{
			num *= 0.5f;
		}
		if (performanceMode)
		{
			num *= 1.5f;
		}
		if (value)
		{
			num *= 0.5f;
		}
		if (realDistance > 250f)
		{
			return num;
		}
		float num2 = (isAI ? 0.66f : 0.33f);
		if (isCurrentEnemy)
		{
			num2 *= 0.5f;
		}
		if (performanceMode)
		{
			num2 *= 1.5f;
		}
		if (value)
		{
			num2 *= 0.5f;
		}
		if (realDistance < 50f)
		{
			return num2;
		}
		float num3 = realDistance - 50f;
		float num4 = num3 / 200f;
		float num5 = num - num2;
		float num6 = num4 * num5 + num2;
		float result = Mathf.Clamp(num6, num2, num);
		if (_nextLogTime < Time.time)
		{
			_nextLogTime = Time.time + 10f;
		}
		return result;
	}

	private bool isEnemyInRange()
	{
		return (base.Enemy.IsAI && base.Enemy.RealDistance <= 300f) || (!base.Enemy.IsAI && base.Enemy.RealDistance <= 500f);
	}

	private bool checkPositionsChanged(Vector3 botPosition, Vector3 enemyPosition)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (base.Enemy.Events.OnSearch.Value)
		{
			return true;
		}
		if (_enemyLastPosChecked.HasValue)
		{
			Vector3 val = _enemyLastPosChecked.Value - enemyPosition;
			if (((Vector3)(ref val)).sqrMagnitude < 0.025f)
			{
				val = _botLastPosChecked - botPosition;
				if (((Vector3)(ref val)).sqrMagnitude < 0.025f)
				{
					return false;
				}
			}
		}
		_enemyLastPosChecked = enemyPosition;
		_botLastPosChecked = botPosition;
		return true;
	}

	public float CalculatePathLength(Vector3[] corners)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (corners == null)
		{
			return float.MaxValue;
		}
		float num = 0f;
		for (int i = 0; i < corners.Length - 1; i++)
		{
			Vector3 val = corners[i];
			Vector3 val2 = corners[i + 1];
			float num2 = num;
			Vector3 val3 = val - val2;
			num = num2 + ((Vector3)(ref val3)).magnitude;
		}
		return num;
	}

	private EnemyCorner findLastCorner(Vector3 enemyPosition, NavMeshPathStatus pathStatus, Vector3[] corners)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		int num = corners.Length;
		int num2;
		Vector3 groundPoint;
		if ((int)pathStatus == 0 && num > 2)
		{
			num2 = num - 2;
			groundPoint = corners[num2];
		}
		else
		{
			num2 = num - 1;
			groundPoint = corners[num2];
		}
		return new EnemyCorner(groundPoint, num2);
	}

	private EnemyCorner findFirstCorner(Vector3 enemyPosition, Vector3[] corners)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (corners.Length < 2)
		{
			return null;
		}
		Vector3 groundPoint = corners[1];
		return new EnemyCorner(groundPoint, 1);
	}

	private EnemyCorner createLastKnownCorner(Vector3 enemyPosition, int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new EnemyCorner(enemyPosition, index);
	}
}
