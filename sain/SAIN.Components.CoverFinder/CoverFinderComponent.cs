using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EFT;
using EFT.Communications;
using SAIN.Classes.Coverfinder;
using SAIN.Components.BotControllerSpace.Classes.Raycasts;
using SAIN.Helpers;
using SAIN.Helpers.Events;
using SAIN.Models.Enums;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.CoverFinder;

public class CoverFinderComponent : BotComponentBase
{
	private const int COLLIDER_ARRAY_SIZE = 300;

	private const int TARGET_COVER_COUNT_AI = 4;

	private const int TARGET_COVER_COUNT_AI_PERF_MODE = 2;

	private const int TARGET_COVER_COUNT_HUMAN = 8;

	private const int TARGET_COVER_COUNT_HUMAN_PERF_MODE = 5;

	private const float UPDATE_TARGET_FREQUENCY = 0.25f;

	private const float SAMPLE_POINT_ORIGIN_RANGE = 1f;

	private const float SAMPLE_POINT_TARGET_RANGE = 1.5f;

	private const float RECHECK_POSITION_CHANGE = 0.5f;

	private const float RECHECK_POSITION_CHANGE_SQR = 0.25f;

	private const float RECHECK_POSITION_CHANGE_PERF_MODE = 1f;

	private const float RECHECK_POSITION_CHANGE_PERF_MODE_SQR = 1f;

	private const float FIND_COVER_DISTANCE_THRESHOLD = 5f;

	private const float FIND_COVER_DISTANCE_THRESHOLD_SQR = 25f;

	private const float FIND_COVER_WAIT_FREQ = 0.1f;

	private const float RECHECK_COVER_WAIT_FREQ = 0.1f;

	private const float RECHECK_COVER_WAIT_FOREACH_FREQ = 0.05f;

	private const float CLEAR_SPOTTED_FREQ = 0.5f;

	private const int COVERCOUNT_TO_START_DELAY = 1;

	private const int COLLIDERS_TO_CHECK_PER_FRAME = 3;

	private const int COLLIDERS_TO_CHECK_PER_FRAME_NO_COVER = 5;

	private readonly List<ColliderCoverData> ColliderCoverDataList = new List<ColliderCoverData>();

	private CheckCoverJob _coverJob;

	private JobHandle _coverJobHandle;

	private readonly WaitForSeconds _recheckWait = new WaitForSeconds(0.05f);

	private TargetData _targetData;

	private float _updateTargetTime;

	private readonly Collider[] _colliderArray = (Collider[])(object)new Collider[300];

	private Vector3 _lastPositionChecked = Vector3.zero;

	private Vector3 _lastRecheckTargetPosition;

	private Vector3 _lastRecheckBotPosition;

	private int _totalChecked;

	private float _debugLogTimer = 0f;

	private float _nextClearSpottedTime;

	private Coroutine _findCoverPointsCoroutine;

	private Coroutine _recheckCoverPointsCoroutine;

	private readonly List<CoverPoint> _tempRecheckList = new List<CoverPoint>();

	private static bool AllCollidersAnalyzed;

	private static float _debugTimer;

	private static float _debugTimer2;

	private static readonly List<string> _excludedColliderNames;

	public ECoverFinderStatus CurrentStatus { get; private set; }

	public TargetData TargetData
	{
		get
		{
			return _targetData;
		}
		private set
		{
			Enemy enemy = _targetData?.TargetEnemy;
			if (value == null)
			{
				if (_targetData != null)
				{
					subOrUnsub(value: false, enemy);
					_targetData = null;
				}
				return;
			}
			Enemy targetEnemy = value.TargetEnemy;
			if (_targetData == null)
			{
				_targetData = value;
				subOrUnsub(value: true, targetEnemy);
			}
			else if (enemy.IsDifferent(targetEnemy))
			{
				_targetData = value;
				subOrUnsub(value: false, enemy);
				subOrUnsub(value: true, targetEnemy);
			}
		}
	}

	public Vector3 OriginPoint => TargetData?.BotPosition ?? Vector3.zero;

	public Vector3 TargetPoint => TargetData?.TargetPosition ?? Vector3.zero;

	public BotComponent Bot { get; private set; }

	public List<CoverPoint> CoverPoints { get; } = new List<CoverPoint>();

	private CoverAnalyzer CoverAnalyzer { get; set; }

	private ColliderFinder ColliderFinder { get; set; }

	public bool ProcessingLimited { get; private set; }

	public CoverPoint FallBackPoint { get; private set; }

	public List<SpottedCoverPoint> SpottedCoverPoints { get; private set; } = new List<SpottedCoverPoint>();

	public static bool PerformanceMode { get; private set; }

	public static float CoverMinHeight { get; private set; }

	public static float CoverMinEnemyDist { get; private set; }

	public static float CoverMinEnemyDistSqr { get; private set; }

	public static bool DebugCoverFinder { get; private set; }

	public void Init(BotComponent bot)
	{
		base.Init(bot.Person);
		Bot = bot;
		ColliderFinder = new ColliderFinder(this);
		CoverAnalyzer = new CoverAnalyzer(bot, this);
		ToggleEvent botActiveToggle = bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Combine(botActiveToggle.OnToggle, new Action<bool>(botEnabled));
		ToggleEvent botStandByToggle = bot.BotActivation.BotStandByToggle;
		botStandByToggle.OnToggle = (Action<bool>)Delegate.Combine(botStandByToggle.OnToggle, new Action<bool>(botInStandBy));
		bot.OnDispose += botDisposed;
	}

	public void Update()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		updateTarget();
		if (DebugCoverFinder && CoverPoints.Count > 0)
		{
			DebugGizmos.Line(GClass1835.PickRandom<CoverPoint>((IReadOnlyList<CoverPoint>)CoverPoints).Position, Bot.Transform.HeadPosition, Color.yellow, 0.035f, 0.1f);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		StopLooking();
		((MonoBehaviour)this).StopAllCoroutines();
		DisposeJobs();
		if ((Object)(object)Bot != (Object)null)
		{
			Bot.OnDispose -= botDisposed;
			ToggleEvent botActiveToggle = Bot.BotActivation.BotActiveToggle;
			botActiveToggle.OnToggle = (Action<bool>)Delegate.Remove(botActiveToggle.OnToggle, new Action<bool>(botEnabled));
			ToggleEvent botStandByToggle = Bot.BotActivation.BotStandByToggle;
			botStandByToggle.OnToggle = (Action<bool>)Delegate.Remove(botStandByToggle.OnToggle, new Action<bool>(botInStandBy));
		}
		Object.Destroy((Object)(object)this);
	}

	private void DisposeJobs()
	{
		((JobHandle)(ref _coverJobHandle)).Complete();
		_coverJob.Dispose();
	}

	private void updateTarget()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = Bot.CurrentTarget.CurrentTargetEnemy ?? Bot.Enemy;
		if (enemy == null)
		{
			TargetData = null;
		}
		else if (_updateTargetTime < Time.time && enemy.LastKnownPosition.HasValue)
		{
			CalcTargetPoint(enemy, enemy.LastKnownPosition.Value);
		}
	}

	public void ClearTarget()
	{
		TargetData = null;
		updateTarget();
	}

	public void CalcTargetPoint(Enemy enemy, Vector3 target)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		_updateTargetTime = Time.time + 0.25f;
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(target, ref val, 1.5f, -1))
		{
			target = ((NavMeshHit)(ref val)).position;
		}
		Vector3 position = Bot.Position;
		NavMeshHit val2 = default(NavMeshHit);
		if (NavMesh.SamplePosition(position, ref val2, 1f, -1))
		{
			position = ((NavMeshHit)(ref val2)).position;
		}
		if (TargetData == null || TargetData.TargetEnemy.IsDifferent(enemy))
		{
			TargetData = new TargetData(enemy);
		}
		TargetData.Update(target, position);
	}

	private int targetCoverCount(TargetData targetData)
	{
		bool isAI = targetData.TargetEnemy.IsAI;
		if (PerformanceMode)
		{
			return isAI ? 2 : 5;
		}
		return isAI ? 4 : 8;
	}

	private void targetEnemyPosUpdated(Enemy enemy, EnemyPlace place)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (place != null && enemy != null)
		{
			CalcTargetPoint(enemy, place.Position);
		}
	}

	private void botInStandBy(bool value)
	{
		if (value)
		{
			ToggleCoverFinder(value: false);
		}
	}

	private void botEnabled(bool value)
	{
		if (!value)
		{
			ToggleCoverFinder(value: false);
		}
	}

	public void ToggleCoverFinder(bool value)
	{
		if (value)
		{
			LookForCover();
		}
		else
		{
			StopLooking();
		}
	}

	public void LookForCover()
	{
		if (_findCoverPointsCoroutine == null)
		{
			_findCoverPointsCoroutine = ((MonoBehaviour)this).StartCoroutine(findCoverLoop());
		}
		if (_recheckCoverPointsCoroutine == null)
		{
			_recheckCoverPointsCoroutine = ((MonoBehaviour)this).StartCoroutine(recheckCoverLoop());
		}
	}

	public void StopLooking()
	{
		if (_findCoverPointsCoroutine != null)
		{
			CurrentStatus = ECoverFinderStatus.None;
			((MonoBehaviour)this).StopCoroutine(_findCoverPointsCoroutine);
			_findCoverPointsCoroutine = null;
			((MonoBehaviour)this).StopCoroutine(_recheckCoverPointsCoroutine);
			_recheckCoverPointsCoroutine = null;
			CoverPoints.Clear();
			if ((Object)(object)Bot != (Object)null)
			{
				Bot.Cover.CoverInUse = null;
			}
			FallBackPoint = null;
			ClearTarget();
			DisposeJobs();
		}
	}

	private IEnumerator findCoverLoop()
	{
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		while (true)
		{
			int coverCount = CoverPoints.Count;
			if (needToFindCover(coverCount, out var max))
			{
				CurrentStatus = ECoverFinderStatus.Idle;
				_lastPositionChecked = OriginPoint;
				bool debug = DebugCoverFinder;
				Stopwatch fullStopWatch = (debug ? Stopwatch.StartNew() : null);
				Stopwatch findFirstPointStopWatch = ((coverCount == 0 && debug) ? Stopwatch.StartNew() : null);
				Collider[] colliders = _colliderArray;
				yield return ColliderFinder.GetNewColliders(colliders);
				ColliderFinder.SortArrayBotDist(colliders);
				yield return findNewCoverPoints(colliders, ColliderFinder.HitCount, max, findFirstPointStopWatch);
				coverCount = CoverPoints.Count;
				sort(coverCount, CoverPoints);
				log(coverCount, findFirstPointStopWatch, fullStopWatch);
			}
			CurrentStatus = ECoverFinderStatus.None;
			yield return wait;
		}
	}

	private IEnumerator recheckCoverPoints(List<CoverPoint> tempList, bool limit = true)
	{
		if (!havePositionsChanged(TargetData))
		{
			yield break;
		}
		bool shallLimit = limit && shallLimitProcessing();
		WaitForSeconds wait = (shallLimit ? _recheckWait : null);
		ECoverFinderStatus lastStatus = CurrentStatus;
		CurrentStatus = (shallLimit ? ECoverFinderStatus.RecheckingPointsWithLimit : ECoverFinderStatus.RecheckingPointsNoLimit);
		foreach (CoverPoint coverPoint in tempList)
		{
			TargetData data = TargetData;
			if (data != null && coverPoint != null)
			{
				yield return checkCoverPoint(coverPoint, data, wait);
			}
		}
		CurrentStatus = lastStatus;
	}

	private IEnumerator checkCoverPoint(CoverPoint coverPoint, TargetData data, WaitForSeconds wait)
	{
		if (!PointStillGood(coverPoint, data, out var updated, out var _))
		{
			coverPoint.CoverData.IsBad = true;
		}
		if (updated)
		{
			yield return wait;
		}
	}

	private bool havePositionsChanged(TargetData targetData)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (targetData == null)
		{
			return false;
		}
		float num = (PerformanceMode ? 1f : 0.25f);
		Vector3 val = _lastRecheckTargetPosition - targetData.TargetPosition;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		val = _lastRecheckBotPosition - targetData.BotPosition;
		float sqrMagnitude2 = ((Vector3)(ref val)).sqrMagnitude;
		if (sqrMagnitude < num && sqrMagnitude2 < num)
		{
			return false;
		}
		_lastRecheckTargetPosition = targetData.TargetPosition;
		_lastRecheckBotPosition = targetData.BotPosition;
		return true;
	}

	private bool shallLimitProcessing()
	{
		Enemy enemy = Bot.Enemy;
		ProcessingLimited = (enemy != null && enemy.IsAI) || limitProcessingFromDecision(Bot.Decision.CurrentCombatDecision);
		return ProcessingLimited;
	}

	private static bool limitProcessingFromDecision(ECombatDecision decision)
	{
		switch (decision)
		{
		case ECombatDecision.Retreat:
		case ECombatDecision.RunToCover:
		case ECombatDecision.RunAway:
		case ECombatDecision.MoveToCover:
			return false;
		case ECombatDecision.Search:
		case ECombatDecision.HoldInCover:
			return true;
		default:
			return PerformanceMode;
		}
	}

	private bool colliderAlreadyUsed(Collider collider)
	{
		for (int i = 0; i < CoverPoints.Count; i++)
		{
			if ((Object)(object)collider == (Object)(object)CoverPoints[i].Collider)
			{
				return true;
			}
		}
		return false;
	}

	private bool filterColliderByName(Collider collider)
	{
		int result;
		if ((Object)(object)collider != (Object)null)
		{
			List<string> excludedColliderNames = _excludedColliderNames;
			Transform transform = ((Component)collider).transform;
			object item;
			if (transform == null)
			{
				item = null;
			}
			else
			{
				Transform parent = transform.parent;
				item = ((parent != null) ? ((Object)parent).name : null);
			}
			result = (excludedColliderNames.Contains((string)item) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private IEnumerator recheckCoverLoop()
	{
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		while (true)
		{
			clearSpotted();
			if (TargetData != null)
			{
				_tempRecheckList.AddRange(CoverPoints);
				yield return ((MonoBehaviour)this).StartCoroutine(recheckCoverPoints(_tempRecheckList, limit: false));
				yield return ((MonoBehaviour)this).StartCoroutine(clearAndSortPoints(_tempRecheckList));
				_tempRecheckList.Clear();
			}
			yield return wait;
		}
	}

	private IEnumerator clearAndSortPoints(List<CoverPoint> tempList)
	{
		foreach (CoverPoint point in tempList)
		{
			if (point?.CoverData.IsBad ?? true)
			{
				CoverPoints.Remove(point);
			}
		}
		OrderPointsByPathDist(CoverPoints);
		yield return null;
	}

	private bool needToFindCover(int coverCount, out int max)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		max = 0;
		if (!isTargetValid(out var data))
		{
			return false;
		}
		max = targetCoverCount(data);
		if (coverCount == 0)
		{
			return true;
		}
		if (coverCount < max / 2)
		{
			return true;
		}
		if (coverCount <= 1 && coverCount < max)
		{
			return true;
		}
		Vector3 val = _lastPositionChecked - OriginPoint;
		if (((Vector3)(ref val)).sqrMagnitude >= 25f)
		{
			return true;
		}
		return false;
	}

	private IEnumerator CheckDistanceToAllColliders(Collider[] colliders)
	{
		if (colliders == null || colliders.Length == 0 || TargetData == null)
		{
			yield break;
		}
		ColliderCoverDataList.Clear();
		DirCalcData botToTargetData = new DirCalcData
		{
			Point = TargetData.TargetPosition,
			Dir = TargetData.DirBotToTarget,
			DirNormal = TargetData.DirBotToTargetNormal,
			Magnitude = TargetData.TargetDistance
		};
		for (int i = 0; i < colliders.Length; i++)
		{
			Collider collider = colliders[i];
			if ((Object)(object)collider != (Object)null)
			{
				ColliderCoverData coverData = new ColliderCoverData(i, collider, TargetData.TargetPosition, TargetData.BotPosition, botToTargetData);
				ColliderCoverDataList.Add(coverData);
			}
		}
		int count = ColliderCoverDataList.Count;
		if (count > 0)
		{
			_coverJob = new CheckCoverJob
			{
				Input = new NativeArray<ColliderCoverData>(count, (Allocator)3, (NativeArrayOptions)1),
				Output = new NativeArray<ColliderCoverData>(count, (Allocator)3, (NativeArrayOptions)1)
			};
			for (int j = 0; j < count; j++)
			{
				_coverJob.Input[j] = ColliderCoverDataList[j];
			}
			_coverJobHandle = IJobForExtensions.Schedule<CheckCoverJob>(_coverJob, count, default(JobHandle));
			yield return null;
			((JobHandle)(ref _coverJobHandle)).Complete();
			NativeArray<ColliderCoverData> outputData = _coverJob.Output;
			StringBuilder stringBuilder = new StringBuilder();
			BotOwner botOwner = base.BotOwner;
			stringBuilder.AppendLine($"[{((botOwner != null) ? ((Object)botOwner).name : null)}] Completed Cover Job Count: [{count}]");
			for (int k = 0; k < count; k++)
			{
				ColliderCoverData coverData2 = outputData[k];
				GameWorldComponent.Instance.CoverManager.CreateCover(coverData2.Collider);
				stringBuilder.AppendLine($"[{k}:{coverData2.Index}]:[{coverData2.BotToCoverDirectionData.Magnitude}]:[{coverData2.TargetToCoverDirectionData.Magnitude}]");
				ColliderCoverDataList[k] = coverData2;
			}
			Logger.LogDebug(stringBuilder.ToString());
			_coverJob.Dispose();
		}
	}

	private void sort(int coverCount, List<CoverPoint> points)
	{
		if (coverCount == 0)
		{
			FallBackPoint = null;
			return;
		}
		if (coverCount < 2)
		{
			FallBackPoint = points.First();
			return;
		}
		FallBackPoint = FindFallbackPoint(points);
		OrderPointsByPathDist(points);
	}

	private void log(int coverCount, params Stopwatch[] watches)
	{
		for (int i = 0; i < watches.Length; i++)
		{
			watches[i]?.Stop();
		}
		if (!DebugCoverFinder)
		{
			return;
		}
		if (_debugLogTimer < Time.time)
		{
			_debugLogTimer = Time.time + 1f;
			if (coverCount > 0)
			{
				Logger.LogInfo($"[{((Object)base.BotOwner).name}] - Found [{coverCount}] CoverPoints. Colliders checked: [{_totalChecked}] Collider Array Size = [{ColliderFinder.HitCount}]");
			}
			else
			{
				Logger.LogWarning($"[{((Object)base.BotOwner).name}] - No Cover Found! Valid Colliders checked: [{_totalChecked}] Collider Array Size = [{ColliderFinder.HitCount}]");
			}
		}
		if (_debugTimer2 < Time.time)
		{
			_debugTimer2 = Time.time + 5f;
		}
	}

	private IEnumerator findNewCoverPoints(Collider[] colliders, int hits, int max, Stopwatch debugStopWatch)
	{
		_totalChecked = 0;
		int waitCount = 0;
		int coverCount = CoverPoints.Count;
		for (int i = 0; i < hits; i++)
		{
			if (coverCount >= max)
			{
				break;
			}
			Collider collider = colliders[i];
			if ((Object)(object)collider == (Object)null)
			{
				continue;
			}
			if (coverCount >= 1)
			{
				yield return null;
			}
			else if (coverCount > 0)
			{
				endStopWatch(debugStopWatch);
				if (waitCount >= 3 || shallLimitProcessing())
				{
					waitCount = 0;
					yield return null;
				}
			}
			else if (waitCount >= 5)
			{
				waitCount = 0;
				yield return null;
			}
			_totalChecked++;
			if (!isTargetValid(out var data))
			{
				break;
			}
			if (!filterColliderByName(collider) && !colliderAlreadyUsed(collider))
			{
				if (CoverAnalyzer.CheckCollider(collider, data, out var newPoint, out var _))
				{
					CoverPoints.Add(newPoint);
					coverCount++;
				}
				waitCount++;
				data = null;
				newPoint = null;
			}
		}
	}

	private bool isTargetValid(out TargetData data)
	{
		data = TargetData;
		return data != null && data.TargetEnemy.WasValid;
	}

	private void endStopWatch(Stopwatch debugStopWatch)
	{
		if (debugStopWatch != null && debugStopWatch.IsRunning)
		{
			debugStopWatch.Stop();
			if (_debugTimer < Time.time)
			{
				_debugTimer = Time.time + 5f;
				Logger.LogAndNotifyDebug($"Time to Find First CoverPoint: [{debugStopWatch.ElapsedMilliseconds}ms]", (ENotificationDurationType)0);
			}
		}
	}

	public static void OrderPointsByPathDist(List<CoverPoint> points)
	{
		points.Sort((CoverPoint x, CoverPoint y) => x.PathData.RoundedPathLength.CompareTo(y.PathData.RoundedPathLength));
	}

	private CoverPoint FindFallbackPoint(List<CoverPoint> points)
	{
		points.Sort((CoverPoint x, CoverPoint y) => x.HardData.Height.CompareTo(y.HardData.Height));
		return points.Last();
	}

	public bool PointStillGood(CoverPoint coverPoint, TargetData targetData, out bool updated, out string reason)
	{
		updated = false;
		if (coverPoint.CoverData.IsBad)
		{
			reason = "badPoint";
			return false;
		}
		if (!coverPoint.ShallUpdate(targetData.TargetProfileID))
		{
			reason = "notTimeToUpdate";
			return true;
		}
		if (PointIsSpotted(coverPoint))
		{
			reason = "spotted";
			return false;
		}
		updated = true;
		if (!CoverAnalyzer.RecheckCoverPoint(coverPoint, targetData, out reason))
		{
			return false;
		}
		return true;
	}

	private void subOrUnsub(bool value, Enemy enemy)
	{
		if (value)
		{
			enemy.Events.OnPositionUpdated += targetEnemyPosUpdated;
		}
		else
		{
			enemy.Events.OnPositionUpdated -= targetEnemyPosUpdated;
		}
	}

	private void clearSpotted()
	{
		if (_nextClearSpottedTime < Time.time)
		{
			_nextClearSpottedTime = Time.time + 0.5f;
			SpottedCoverPoints.RemoveAll((SpottedCoverPoint x) => x.IsValidAgain);
		}
	}

	private bool PointIsSpotted(CoverPoint point)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (point == null)
		{
			return true;
		}
		clearSpotted();
		foreach (SpottedCoverPoint spottedCoverPoint in SpottedCoverPoints)
		{
			Vector3 position = spottedCoverPoint.CoverPoint.Position;
			if (spottedCoverPoint.TooClose(position, point.Position))
			{
				return true;
			}
		}
		if (point.Spotted)
		{
			SpottedCoverPoints.Add(new SpottedCoverPoint(point));
		}
		return point.Spotted;
	}

	public void OnDestroy()
	{
		StopLooking();
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private void botDisposed()
	{
		Dispose();
	}

	static CoverFinderComponent()
	{
		PerformanceMode = false;
		CoverMinHeight = 0.5f;
		CoverMinEnemyDist = 5f;
		CoverMinEnemyDistSqr = 25f;
		DebugCoverFinder = false;
		_excludedColliderNames = new List<string>
		{
			"metall_fence_2", "metallstolb", "stolb", "fonar_stolb", "fence_grid", "metall_fence_new", "ladder_platform", "frame_L", "frame_small_collider", "bump2x_p3_set4x",
			"bytovka_ladder", "sign", "sign17_lod", "ograda1", "ladder_metal"
		};
		PresetHandler.OnPresetUpdated += updateSettings;
		updateSettings(SAINPresetClass.Instance);
	}

	private static void updateSettings(SAINPresetClass preset)
	{
		PerformanceMode = SAINPlugin.LoadedPreset.GlobalSettings.General.Performance.PerformanceMode;
		CoverMinHeight = SAINPlugin.LoadedPreset.GlobalSettings.General.Cover.CoverMinHeight;
		CoverMinEnemyDist = SAINPlugin.LoadedPreset.GlobalSettings.General.Cover.CoverMinEnemyDistance;
		CoverMinEnemyDistSqr = CoverMinEnemyDist * CoverMinEnemyDist;
		DebugCoverFinder = SAINPlugin.LoadedPreset.GlobalSettings.General.Cover.DebugCoverFinder;
	}

	private static void AnalyzeAllColliders()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (AllCollidersAnalyzed)
		{
			return;
		}
		AllCollidersAnalyzed = true;
		float coverMinHeight = CoverMinHeight;
		Collider[] array = (Collider[])(object)new Collider[500000];
		int num = Physics.OverlapSphereNonAlloc(Vector3.zero, 1000f, array);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Bounds bounds = array[i].bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			if (size.y < CoverMinHeight || (size.x < 0.1f && size.z < 0.1f))
			{
				array[i] = null;
				num2++;
			}
		}
		Logger.LogError($"All Colliders Analyzed. [{num - num2}] are suitable out of [{num}] colliders");
	}
}
