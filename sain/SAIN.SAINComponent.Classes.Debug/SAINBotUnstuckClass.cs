using System.Collections;
using System.Collections.Generic;
using Comfort.Common;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Debug;

public class SAINBotUnstuckClass : BotComponentClassBase
{
	private bool _botIsMoving;

	private float _timeStartMoving;

	private float _timeStopMoving;

	private float _timeOffMeshStart;

	private float _nextResetTime;

	private bool _isOnNavMesh;

	private float _nextCheckNavMeshTime;

	private bool _botStuckAfterVault;

	private Coroutine postVaultTracker;

	private float _nextVaultCheckTime;

	private bool DontUnstuckMe;

	private static readonly List<WildSpawnType> DontUnstuckTheseTypes = new List<WildSpawnType>
	{
		(WildSpawnType)0,
		(WildSpawnType)46
	};

	private bool _botVaulted;

	private float _botVaultedTime;

	private Coroutine botUnstuckCoroutine;

	private float TimeSinceTriedJumpOrVault;

	private bool HasTriedJumpOrVault;

	private const float MinDistance = 100f;

	private const float MaxDistance = 300f;

	private const float PathLengthCoef = 1.25f;

	private const float MinDistancePathLength = 125f;

	private Coroutine TeleportCoroutine;

	private bool IsTeleporting;

	private float teleportTimer;

	private static NavMeshPath PathToPlayer;

	private List<Player> HumanPlayers = new List<Player>();

	private RaycastHit StuckHit = default(RaycastHit);

	private float DebugStuckTimer = 0f;

	private float CheckStuckTimer = 0f;

	private float CheckPositionTimer = 0f;

	private Vector3 LastPos = Vector3.zero;

	private float JumpTimer = 0f;

	public bool BotIsMoving { get; private set; }

	public float TimeStartedMoving { get; private set; }

	public float TimeStoppedMoving { get; private set; }

	public float TimeSinceMovingStarted => TimeStartedMoving - Time.time;

	public PathControllerClass PathController { get; private set; }

	public float TimeSinceStuck => Time.time - TimeStuck;

	public float TimeStuck { get; private set; }

	public float TimeSpentNotMoving => Time.time - TimeStartedChangingPosition;

	public float TimeStartedChangingPosition { get; private set; }

	public bool BotIsStuck { get; private set; }

	public bool BotHasChangedPosition { get; private set; }

	public SAINBotUnstuckClass(BotComponent sain)
		: base(sain)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.TickRequirement = ESAINTickState.OnlyBotActive;
	}

	public override void Init()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		PathController = base.BotOwner.Mover._pathController;
		DontUnstuckMe = DontUnstuckTheseTypes.Contains(base.Bot.Info.Profile.WildSpawnType);
		base.Init();
	}

	private void CheckIfMoving()
	{
		float time = Time.time;
		bool botIsMoving = _botIsMoving;
		_botIsMoving = base.BotOwner.Mover.IsMoving || base.Bot.Mover.PathFollower.Running;
		if (_botIsMoving && !botIsMoving)
		{
			_timeStartMoving = time;
		}
		else if (!_botIsMoving && botIsMoving)
		{
			_timeStopMoving = time;
		}
		if (_botIsMoving && time - _timeStartMoving > 0.25f)
		{
			if (!BotIsMoving)
			{
				TimeStartedMoving = time;
			}
			BotIsMoving = true;
		}
		else if (!_botIsMoving && time - _timeStopMoving > 0.25f)
		{
			if (BotIsMoving)
			{
				TimeStoppedMoving = time;
			}
			BotIsMoving = false;
		}
	}

	private bool checkFixOffMeshBot()
	{
		if (_nextCheckNavMeshTime < Time.time)
		{
			_nextCheckNavMeshTime = Time.time + 1f;
			bool isOnNavMesh = _isOnNavMesh;
			_isOnNavMesh = CheckBotIsOnNavMesh();
			if (!_isOnNavMesh)
			{
				if (isOnNavMesh)
				{
					_timeOffMeshStart = Time.time;
				}
				if (Time.time - _timeOffMeshStart > 3f && _nextResetTime < Time.time)
				{
					_nextResetTime = Time.time + 5f;
					base.Bot.Mover.ResetPath(0.33f);
				}
			}
			if (_isOnNavMesh && _timeOffMeshStart > 0f)
			{
				_timeOffMeshStart = -1f;
			}
		}
		return _isOnNavMesh;
	}

	public Vector2 findMoveDirection(Vector3 direction)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(direction.x, direction.z);
		Vector3 v = Quaternion.Euler(0f, 0f, base.Player.Rotation.x) * Vector2.op_Implicit(val);
		v = Vector.NormalizeFastSelf(v);
		return new Vector2(v.x, v.y);
	}

	private bool CheckBotIsOnNavMesh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		return NavMesh.SamplePosition(base.Bot.Position, ref val, 0.25f, -1);
	}

	private void CheckIfPositionChanged()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (CheckPositionTimer < Time.time)
		{
			CheckPositionTimer = Time.time + 0.5f;
			bool botHasChangedPosition = BotHasChangedPosition;
			Vector3 val = LastPos - base.Bot.Position;
			BotHasChangedPosition = ((Vector3)(ref val)).sqrMagnitude > 0.010000001f;
			if (botHasChangedPosition && !BotHasChangedPosition)
			{
				TimeStartedChangingPosition = Time.time;
			}
			else if (BotHasChangedPosition)
			{
				TimeStartedChangingPosition = 0f;
			}
			LastPos = base.Bot.Position;
		}
	}

	private IEnumerator trackPostVault(Vector3 preVaultPosition)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		WaitForSeconds wait = new WaitForSeconds(1f);
		yield return wait;
		if ((Object)(object)base.Bot == (Object)null || (Object)(object)base.BotOwner == (Object)null || (Object)(object)base.Player == (Object)null || !base.Player.HealthController.IsAlive)
		{
			yield break;
		}
		NavMeshHit hit1 = default(NavMeshHit);
		if (NavMesh.SamplePosition(preVaultPosition, ref hit1, 0.5f, -1))
		{
			preVaultPosition = ((NavMeshHit)(ref hit1)).position;
		}
		new NavMeshPath();
		_ = Time.time;
		bool botIsStuck = true;
		while (botIsStuck && !((Object)(object)base.Bot == (Object)null) && !((Object)(object)base.BotOwner == (Object)null) && !((Object)(object)base.Player == (Object)null) && base.Player.HealthController.IsAlive)
		{
			botIsStuck = isStuck(preVaultPosition);
			if (!botIsStuck)
			{
				break;
			}
			_botStuckAfterVault = botIsStuck;
			if (!isHumanVisible() && !isHumanClose())
			{
				teleport(preVaultPosition);
				break;
			}
			yield return wait;
		}
		_botStuckAfterVault = false;
	}

	private bool isStuck(Vector3 targetPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		NavMeshPath val = new NavMeshPath();
		NavMeshHit val2 = default(NavMeshHit);
		return !NavMesh.SamplePosition(base.Bot.Position, ref val2, 0.5f, -1) || !NavMesh.CalculatePath(((NavMeshHit)(ref val2)).position, targetPosition, -1, val) || (int)val.status > 0;
	}

	private void teleport(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Player.Teleport(position + Vector3.up * 0.25f, false);
		if (SAINPlugin.DebugMode)
		{
			Logger.LogWarning(((Object)base.BotOwner).name + " has teleported because they were stuck after vaulting, and no human players are visible to them, and no human players are close.");
		}
		BotMover mover = base.BotOwner.Mover;
		if (mover != null)
		{
			mover.Stop();
		}
		BotMover mover2 = base.BotOwner.Mover;
		if (mover2 != null)
		{
			mover2.RecalcWay();
		}
	}

	private bool isHumanVisible()
	{
		return base.Bot.EnemyController.HumanEnemyInLineofSight;
	}

	private bool isHumanClose()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		List<Player> allAlivePlayersList = Singleton<GameWorld>.Instance.AllAlivePlayersList;
		foreach (Player item in allAlivePlayersList)
		{
			if ((Object)(object)item != (Object)null && !item.IsAI && item.HealthController.IsAlive)
			{
				Vector3 val = item.Position - base.Bot.Position;
				if (((Vector3)(ref val)).sqrMagnitude < 2500f)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private bool tryVault()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.FileSettings.Move.VAULT_UNSTUCK_TOGGLE || !GlobalSettingsClass.Instance.Move.VAULT_UNSTUCK_TOGGLE)
		{
			return false;
		}
		Vector3 position = base.Bot.Position;
		if (base.Bot.Mover.TryVault())
		{
			_botVaultedTime = Time.time;
			if (postVaultTracker != null)
			{
				((MonoBehaviour)base.Bot).StopCoroutine(postVaultTracker);
				_botStuckAfterVault = false;
			}
			postVaultTracker = ((MonoBehaviour)base.Bot).StartCoroutine(trackPostVault(position));
			return true;
		}
		return false;
	}

	private void checkResetPathFromVault()
	{
		if (_botVaulted && !_botStuckAfterVault && _botVaultedTime + 1f < Time.time)
		{
			_botVaulted = false;
			base.Bot.Mover.ResetPath(0.1f);
		}
	}

	private void tryAutoVault()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Info.FileSettings.Move.VAULT_TOGGLE || !GlobalSettingsClass.Instance.Move.VAULT_TOGGLE || !(_nextVaultCheckTime < Time.time))
		{
			return;
		}
		BotOwner botOwner = base.BotOwner;
		if (botOwner != null)
		{
			BotMover mover = botOwner.Mover;
			if (((mover != null) ? new bool?(mover.IsMoving) : ((bool?)null)) == true)
			{
				goto IL_009d;
			}
		}
		if (!base.Bot.Mover.PathFollower.Moving)
		{
			return;
		}
		goto IL_009d;
		IL_009d:
		Vector3 lookDirection = base.Player.LookDirection;
		Vector3 normalized = ((Vector3)(ref lookDirection)).normalized;
		Vector3 normDirCurPoint = base.BotOwner.Mover.NormDirCurPoint;
		float num;
		if (Vector3.Dot(normalized, normDirCurPoint) > 0.85f && tryVault())
		{
			_botVaulted = true;
			num = 2f;
		}
		else
		{
			num = 0.5f;
		}
		_nextVaultCheckTime = Time.time + num;
	}

	public override void ManualUpdate()
	{
		if (!DontUnstuckMe && !base.Bot.BotActivation.BotInStandBy)
		{
			startCoroutine();
		}
		else if (botUnstuckCoroutine != null)
		{
			((MonoBehaviour)base.Bot).StopCoroutine(botUnstuckCoroutine);
		}
		base.ManualUpdate();
	}

	private void startCoroutine()
	{
		if (botUnstuckCoroutine == null)
		{
			botUnstuckCoroutine = ((MonoBehaviour)base.Bot).StartCoroutine(botUnstuck());
		}
	}

	private IEnumerator botUnstuck()
	{
		while (true)
		{
			if (base.Bot.BotActive && !base.Bot.GameEnding)
			{
				checkFixOffMeshBot();
				tryAutoVault();
				checkResetPathFromVault();
				doStuckChecks();
			}
			yield return null;
		}
	}

	private void doStuckChecks()
	{
		CheckIfMoving();
		CheckIfPositionChanged();
		if (CheckStuckTimer < Time.time)
		{
			checkIfBotStuck();
			checkCancelUnstuck();
			tryFixStuckBot();
		}
	}

	private void checkIfBotStuck()
	{
		if (!(CheckStuckTimer < Time.time))
		{
			return;
		}
		if (base.BotOwner.DoorOpener.Interacting)
		{
			CheckStuckTimer = Time.time + 1f;
			BotIsStuck = false;
			return;
		}
		CheckStuckTimer = Time.time + 0.5f;
		bool flag = _botStuckAfterVault || BotStuckGeneric() || BotStuckOnObject();
		if (!BotIsStuck && flag)
		{
			TimeStuck = Time.time;
		}
		BotIsStuck = flag;
	}

	private void checkCancelUnstuck()
	{
		if (!BotIsStuck && TeleportCoroutine != null)
		{
			((MonoBehaviour)base.Bot).StopCoroutine(TeleportCoroutine);
			HasTriedJumpOrVault = false;
			JumpTimer = Time.time + 1f;
			IsTeleporting = false;
		}
	}

	private void tryFixStuckBot()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (!BotIsStuck || !(TimeSinceStuck > 2f))
		{
			return;
		}
		if (SAINPlugin.DebugMode && DebugStuckTimer < Time.time && TimeSinceStuck > 5f)
		{
			DebugStuckTimer = Time.time + 10f;
			string[] obj = new string[6]
			{
				$"[{((Object)base.BotOwner).name}] has been stuck for [{TimeSinceStuck}] seconds ",
				"on [",
				null,
				null,
				null,
				null
			};
			Transform transform = ((RaycastHit)(ref StuckHit)).transform;
			obj[2] = ((transform != null) ? ((Object)transform).name : null);
			obj[3] = "] object ";
			Transform transform2 = ((RaycastHit)(ref StuckHit)).transform;
			obj[4] = $"at [{((transform2 != null) ? new Vector3?(transform2.position) : ((Vector3?)null))}] ";
			obj[5] = $"with Current Decision as [{base.Bot.Decision.CurrentCombatDecision}]";
			Logger.LogWarning(string.Concat(obj));
		}
		if (HasTriedJumpOrVault && TimeSinceStuck > 6f && TimeSinceTriedJumpOrVault + 2f < Time.time && !isHumanVisible() && !isHumanClose())
		{
			TeleportCoroutine = ((MonoBehaviour)base.Bot).StartCoroutine(CheckIfTeleport());
		}
		if (JumpTimer < Time.time && TimeSinceStuck > 2f)
		{
			JumpTimer = Time.time + 1f;
			if (!tryVault())
			{
				base.Bot.Mover.ResetPath(0.1f);
				HasTriedJumpOrVault = true;
				TimeSinceTriedJumpOrVault = Time.time;
			}
			else
			{
				_botVaulted = true;
			}
		}
	}

	private IEnumerator CheckIfTeleport()
	{
		bool shallTeleport = true;
		GetHumanPlayers();
		Vector3? teleportDestination = null;
		if (base.BotOwner.Mover.HasPathAndNoComplete)
		{
			for (int i = PathController.CurPath.CurIndex; i < PathController.CurPath.Length - 1; i++)
			{
				Vector3 corner = PathController.CurPath.GetPoint(i);
				Vector3 cornerDirection = corner - base.Bot.Position;
				_ = ((Vector3)(ref cornerDirection)).sqrMagnitude;
				if (((Vector3)(ref cornerDirection)).sqrMagnitude >= 1f)
				{
					teleportDestination = corner;
					break;
				}
				yield return null;
			}
		}
		Vector3 botPosition = base.Bot.Position;
		Vector3 val;
		if (teleportDestination.HasValue)
		{
			List<Player> allPlayers = Singleton<GameWorld>.Instance?.AllAlivePlayersList;
			if (allPlayers != null)
			{
				foreach (Player player in allPlayers)
				{
					if (!ShallCheckPlayer(player))
					{
						continue;
					}
					if (!BotIsStuck)
					{
						shallTeleport = false;
						yield break;
					}
					Vector3 playerPosition = player.Position;
					val = playerPosition - botPosition;
					float sqrMag = ((Vector3)(ref val)).sqrMagnitude;
					if (sqrMag < 10000f)
					{
						shallTeleport = false;
						break;
					}
					if (sqrMag < 90000f)
					{
						float pathLength;
						NavMeshPath path = CalcPath(botPosition, playerPosition, out pathLength);
						if (!CheckPathLength(playerPosition, path, pathLength))
						{
							shallTeleport = false;
							break;
						}
					}
					yield return null;
				}
			}
		}
		IsTeleporting = BotIsStuck && shallTeleport && teleportDestination.HasValue;
		if (IsTeleporting)
		{
			Teleport(teleportDestination.Value + Vector3.up * 0.25f);
			val = teleportDestination.Value - botPosition;
			Logger.LogDebug(string.Format(arg1: ((Vector3)(ref val)).magnitude, format: "Teleporting stuck bot: [{0}] [{1}] meters to the next corner they are trying to go to", arg0: ((Object)base.Player).name));
			_botStuckAfterVault = false;
			BotIsStuck = false;
		}
		yield return null;
	}

	private bool ShallCheckPlayer(Player player)
	{
		if ((Object)(object)base.Player == (Object)null || base.Player.HealthController == null || base.Player.AIData == null)
		{
			return false;
		}
		return base.Player.HealthController.IsAlive && !base.Player.AIData.IsAI;
	}

	private void Teleport(Vector3 position)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (teleportTimer < Time.time)
		{
			teleportTimer = Time.time + 3f;
			base.Player.Teleport(position, false);
		}
	}

	private static NavMeshPath CalcPath(Vector3 start, Vector3 end, out float pathLength)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (PathToPlayer == null)
		{
			PathToPlayer = new NavMeshPath();
		}
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(end, ref val, 1f, -1))
		{
			PathToPlayer.ClearCorners();
			if (NavMesh.CalculatePath(start, ((NavMeshHit)(ref val)).position, -1, PathToPlayer))
			{
				pathLength = GClass361.CalculatePathLength(PathToPlayer);
				return PathToPlayer;
			}
		}
		pathLength = 0f;
		return null;
	}

	private static bool CheckPathLength(Vector3 end, NavMeshPath path, float pathLength)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		if (path == null)
		{
			return false;
		}
		if ((int)path.status == 1)
		{
			Vector3 val = path.corners[path.corners.Length - 1];
			Vector3 val2 = val - end;
			float magnitude = ((Vector3)(ref val2)).magnitude;
			float num = magnitude + pathLength;
			if (num < 125f)
			{
				return false;
			}
		}
		if ((int)path.status == 0 && pathLength < 125f)
		{
			return false;
		}
		return (int)path.status != 2;
	}

	private List<Player> GetHumanPlayers()
	{
		HumanPlayers.Clear();
		List<Player> list = Singleton<GameWorld>.Instance?.AllAlivePlayersList;
		if (list != null)
		{
			foreach (Player item in list)
			{
				if ((Object)(object)item != (Object)null && !item.AIData.IsAI && item.HealthController.IsAlive)
				{
					HumanPlayers.Add(item);
				}
			}
		}
		return HumanPlayers;
	}

	private bool CanBeStuckDecisions(ECombatDecision decision)
	{
		return decision == ECombatDecision.Search || decision == ECombatDecision.MoveToCover || decision == ECombatDecision.DogFight || decision == ECombatDecision.RunToCover || decision == ECombatDecision.RunAway;
	}

	public bool BotStuckOnPlayer()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (!BotHasChangedPosition && CanBeStuckDecisions(base.Bot.Decision.CurrentCombatDecision))
		{
			if (base.BotOwner.Mover == null)
			{
				return false;
			}
			Vector3 position = base.BotOwner.Position;
			position.y += 0.4f;
			Vector3 dirCurPoint = base.BotOwner.Mover.DirCurPoint;
			dirCurPoint.y = 0f;
			Vector3 lookDirection = base.BotOwner.LookDirection;
			lookDirection.y = 0f;
			RaycastHit[] array = Physics.SphereCastAll(position, 0.15f, dirCurPoint, 0.5f, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
			if (array.Length != 0)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit stuckHit = array2[i];
					if (((Object)((RaycastHit)(ref stuckHit)).transform).name != ((Object)base.BotOwner).name)
					{
						StuckHit = stuckHit;
						return true;
					}
				}
			}
			RaycastHit[] array3 = Physics.SphereCastAll(position, 0.15f, lookDirection, 0.5f, LayerMask.op_Implicit(LayerMaskClass.PlayerMask));
			if (array3.Length != 0)
			{
				RaycastHit[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					RaycastHit stuckHit2 = array4[j];
					if (((Object)((RaycastHit)(ref stuckHit2)).transform).name != ((Object)base.BotOwner).name)
					{
						StuckHit = stuckHit2;
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool BotStuckGeneric()
	{
		return BotIsMoving && !BotHasChangedPosition && !base.BotOwner.DoorOpener.Interacting && TimeSpentNotMoving > 2f;
	}

	public bool BotStuckOnObject()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (CanBeStuckDecisions(base.Bot.Decision.CurrentCombatDecision) && !BotHasChangedPosition && !base.BotOwner.DoorOpener.Interacting && base.Bot.Decision.TimeSinceChangeDecision > 1f)
		{
			if (base.BotOwner.Mover == null)
			{
				return false;
			}
			Vector3 position = base.BotOwner.Position;
			position.y += 0.4f;
			Vector3 dirCurPoint = base.BotOwner.Mover.DirCurPoint;
			dirCurPoint.y = 0f;
			RaycastHit stuckHit = default(RaycastHit);
			if (Physics.SphereCast(position, 0.15f, dirCurPoint, ref stuckHit, 0.25f, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				StuckHit = stuckHit;
				return true;
			}
		}
		return false;
	}
}
