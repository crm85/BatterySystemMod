using System.Collections;
using System.Collections.Generic;
using EFT;
using EFT.Interactive;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class DoorOpener : BotComponentClassBase
{
	private struct linkObjects
	{
		public GameObject link;

		public GameObject midOpen;

		public GameObject midClose;
	}

	private readonly List<DoorData> _possibleInteractDoors = new List<DoorData>();

	private const float DOOR_SINGLE_INTERACTION_FREQ = 1f;

	private const float DOOR_INTERACTION_FREQ = 0.66f;

	private const float DOOR_CHECK_FREQ = 0.25f;

	private readonly List<NavMeshDoorLink> _doorsOnPath = new List<NavMeshDoorLink>();

	private DoorData _lastInteractedInfo;

	private static readonly Dictionary<NavMeshDoorLink, linkObjects> _debugObjects = new Dictionary<NavMeshDoorLink, linkObjects>();

	public bool _interactingWithDoor;

	private float _nextPosibleDoorInteractTime;

	private float _traversingEnd;

	public bool Interacting
	{
		get
		{
			return base.BotOwner.DoorOpener.Interacting;
		}
		private set
		{
			base.BotOwner.DoorOpener.Interacting = value;
		}
	}

	public bool NearDoor
	{
		get
		{
			return base.BotOwner.DoorOpener.NearDoor;
		}
		private set
		{
			base.BotOwner.DoorOpener.NearDoor = value;
		}
	}

	public bool BreachingDoor { get; private set; }

	public DoorFinder DoorFinder { get; }

	private static bool _debugMode => SAINPlugin.DebugSettings.Gizmos.DrawDoorLinks;

	public DoorOpener(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		DoorFinder = new DoorFinder(this);
	}

	public override void Init()
	{
		DoorFinder.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		DoorFinder.ManualUpdate();
		if (base.Bot.Mover.PathFollower.Moving || base.BotOwner.Mover.HasPathAndNoComplete)
		{
			CheckUseSAINOpener();
		}
		if (!_debugMode && _debugObjects.Count > 0)
		{
			foreach (linkObjects value in _debugObjects.Values)
			{
				Object.Destroy((Object)(object)value.link);
				Object.Destroy((Object)(object)value.midClose);
				Object.Destroy((Object)(object)value.midOpen);
			}
			_debugObjects.Clear();
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		DoorFinder.Dispose();
		base.Dispose();
	}

	public bool CheckUseSAINOpener()
	{
		if (!SAINPlugin.LoadedPreset.GlobalSettings.General.Doors.NewDoorOpening)
		{
			return base.BotOwner.DoorOpener.Update();
		}
		if (ModDetection.ProjectFikaLoaded)
		{
			return base.BotOwner.DoorOpener.Update();
		}
		if (!base.Bot.BotActivation.SAINLayersActive)
		{
			return base.BotOwner.DoorOpener.Update();
		}
		return FindDoorsToOpen();
	}

	private void checkEndDoorOpening()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (_traversingEnd < Time.time || (_lastInteractedInfo != null && (int)((WorldInteractiveObject)_lastInteractedInfo.Door).DoorState != 8))
		{
			endDoorInteraction();
		}
	}

	public bool FindDoorsToOpen()
	{
		if (Interacting)
		{
			if (!(_traversingEnd < Time.time))
			{
				return true;
			}
			NearDoor = false;
			BreachingDoor = false;
			Interacting = false;
			base.BotOwner.Mover.MovementResume();
			base.BotOwner.Mover.SprintPause(-1f);
			_interactingWithDoor = false;
		}
		if (Interacting)
		{
			return true;
		}
		if (_nextPosibleDoorInteractTime < Time.time)
		{
			_interactingWithDoor = findADoorToOpen();
			NearDoor = _interactingWithDoor;
		}
		return _interactingWithDoor;
	}

	private static IEnumerator SetDoorCollisionAfterDelay(Collider playerCollider, Collider doorCollider, bool value, float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		if ((Object)(object)playerCollider != (Object)null && (Object)(object)doorCollider != (Object)null)
		{
			EFTPhysicsClass.IgnoreCollision(playerCollider, doorCollider, value);
		}
	}

	private void drawLink(NavMeshDoorLink link)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (_debugMode && SAINPlugin.DebugSettings.Gizmos.DrawDebugGizmos && !_debugObjects.ContainsKey(link))
		{
			Vector3 val = ((Component)link).transform.position + Vector3.down;
			linkObjects value = new linkObjects
			{
				link = DebugGizmos.Line(val, val + Vector3.up * 2f, Color.white, 0.2f),
				midOpen = DebugGizmos.Line(val, link.MidOpen + Vector3.down, Color.blue, 0.2f),
				midClose = DebugGizmos.Line(val, link.MidClose + Vector3.down, Color.green, 0.2f)
			};
			_debugObjects.Add(link, value);
		}
	}

	private bool canInteract(NavMeshDoorLink link)
	{
		if (!link.ShallInteract())
		{
			return false;
		}
		if (!((Behaviour)link.Door).enabled || !((Component)link.Door).gameObject.activeInHierarchy)
		{
			return false;
		}
		if (checkIfDoorLast(link))
		{
			return false;
		}
		if (!((WorldInteractiveObject)link.Door).Operatable || !((Behaviour)link.Door).enabled)
		{
			return false;
		}
		return true;
	}

	private bool findADoorToOpen()
	{
		List<DoorData> interactionDoors = DoorFinder.InteractionDoors;
		if (interactionDoors.Count == 0)
		{
			return false;
		}
		_nextPosibleDoorInteractTime = Time.time + 0.25f;
		findPossibleInteractDoors(interactionDoors);
		List<DoorData> possibleInteractDoors = _possibleInteractDoors;
		if (possibleInteractDoors.Count == 0)
		{
			return false;
		}
		DoorData doorData = checkWantToOpenAnyDoors(possibleInteractDoors) ?? checkWantToCloseAnyDoors(possibleInteractDoors);
		if (doorData == null)
		{
			return false;
		}
		return interactWithDoor(doorData);
	}

	private bool interactWithDoor(DoorData data)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		data.LastInteractTime = Time.time;
		NavMeshDoorLink link = data.Link;
		Door door = link.Door;
		EDoorState doorState = ((WorldInteractiveObject)door).DoorState;
		EDoorState val = doorState;
		if ((int)val != 2)
		{
			if ((int)val == 4)
			{
				data.LastCloseTime = Time.time;
				_nextPosibleDoorInteractTime = Time.time + 1f;
				Interact(data, (EInteractionType)1);
				return true;
			}
			return false;
		}
		data.LastOpenTime = Time.time;
		_nextPosibleDoorInteractTime = Time.time + 1f;
		Interact(data, (EInteractionType)0);
		return true;
	}

	private DoorData checkWantToOpenAnyDoors(List<DoorData> doors)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		float num = -1f;
		DoorData doorData = null;
		foreach (DoorData door2 in doors)
		{
			if (door2.DoorInFront)
			{
				NavMeshDoorLink link = door2.Link;
				Door door = link.Door;
				if ((int)((WorldInteractiveObject)door).DoorState == 2 && door2.DotProduct > num)
				{
					num = door2.DotProduct;
					doorData = door2;
				}
			}
		}
		if (doorData != null && doorData.DotProduct > 0f)
		{
			return doorData;
		}
		return null;
	}

	private DoorData checkWantToCloseAnyDoors(List<DoorData> doors)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		DoorData result = null;
		foreach (DoorData door2 in doors)
		{
			NavMeshDoorLink link = door2.Link;
			Door door = link.Door;
			if ((int)((WorldInteractiveObject)door).DoorState != 4)
			{
				continue;
			}
			result = door2;
			break;
		}
		return result;
	}

	private void findPossibleInteractDoors(List<DoorData> list)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Invalid comparison between Unknown and I4
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Invalid comparison between Unknown and I4
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		_possibleInteractDoors.Clear();
		Vector3 val;
		if (base.BotOwner.Mover.HasPathAndNoComplete)
		{
			val = base.BotOwner.Mover.RealDestPoint;
		}
		else
		{
			if (!base.Bot.Mover.PathFollower.Moving)
			{
				return;
			}
			val = base.Bot.Mover.PathFollower.MoveData.CurrentCorner.Position;
		}
		Vector3 position = base.BotOwner.Transform.position;
		Vector3 val2 = val - position;
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		foreach (DoorData item in list)
		{
			NavMeshDoorLink link = item.Link;
			if (!canInteract(link) || !item.CanInteractByTime())
			{
				continue;
			}
			Door door = item.Door;
			drawLink(link);
			EDoorState doorState = ((WorldInteractiveObject)door).DoorState;
			EDoorState val3 = doorState;
			float num;
			if ((int)val3 != 2)
			{
				if ((int)val3 != 4)
				{
					continue;
				}
				num = 4f;
			}
			else
			{
				num = 4f;
			}
			item.CalcDirection(position);
			if (!(item.CurrentSqrMagnitude > num))
			{
				item.DotProduct = Vector3.Dot(item.DirectionNormal, normalized);
				if (CheckWantToInteract(item, position))
				{
					_possibleInteractDoors.Add(item);
				}
			}
		}
	}

	private void checkIfLastDoorExpire()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		DoorData lastInteractedInfo = _lastInteractedInfo;
		if (_lastInteractedInfo != null)
		{
			if ((int)((WorldInteractiveObject)lastInteractedInfo.Door).DoorState == 8)
			{
				lastInteractedInfo.LastInteractTime = Time.time;
			}
			else if (lastInteractedInfo.LastInteractTime + 1f < Time.time)
			{
				EFTPhysicsClass.IgnoreCollision(base.Player.CharacterController.GetCollider(), ((WorldInteractiveObject)lastInteractedInfo.Door).Collider, false);
				_lastInteractedInfo = null;
			}
		}
	}

	private bool checkIfDoorLast(NavMeshDoorLink link)
	{
		DoorData lastInteractedInfo = _lastInteractedInfo;
		if (lastInteractedInfo == null)
		{
			return false;
		}
		if (lastInteractedInfo.Link.Id != link.Id)
		{
			return false;
		}
		return lastInteractedInfo.CanInteractByTime();
	}

	private void endDoorInteraction()
	{
		NearDoor = false;
		BreachingDoor = false;
		Interacting = false;
		if (!base.Bot.Mover.PathFollower.Moving)
		{
			base.BotOwner.Mover.MovementResume();
			base.BotOwner.Mover.SprintPause(-1f);
		}
	}

	public bool ShallPauseSprintForOpening()
	{
		if (!Interacting || !BreachingDoor)
		{
			return false;
		}
		return true;
	}

	private bool shallKickOpen(Door door, EInteractionType Etype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Etype > 0)
		{
			return false;
		}
		if (!wantToKick())
		{
			return false;
		}
		GStruct425 breakInParameters = door.GetBreakInParameters(base.Bot.Position);
		return door.BreachSuccessRoll(breakInParameters.InteractionPosition);
	}

	private bool wantToKick()
	{
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
			if (base.Bot.Info.PersonalitySettings.General.KickOpenAllDoors)
			{
				return true;
			}
			if (base.BotOwner.Memory.IsUnderFire)
			{
				return true;
			}
			float? num = enemy.TimeSinceSeen;
			if (num.HasValue)
			{
				if (num.Value < 3f)
				{
					return true;
				}
				if (num.Value < 5f && enemy.InLineOfSight)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void IgnoreCollisionWithDoor(Door door, float resetDelay)
	{
		Player player = base.Player;
		object obj;
		if (player == null)
		{
			obj = null;
		}
		else
		{
			ICharacterController characterController = player.CharacterController;
			obj = ((characterController != null) ? characterController.GetCollider() : null);
		}
		Collider val = (Collider)obj;
		if ((Object)(object)val != (Object)null && (Object)(object)((WorldInteractiveObject)door).Collider != (Object)null)
		{
			EFTPhysicsClass.IgnoreCollision(val, ((WorldInteractiveObject)door).Collider, true);
			((MonoBehaviour)base.Player).StartCoroutine(SetDoorCollisionAfterDelay(val, ((WorldInteractiveObject)door).Collider, value: false, resetDelay));
		}
	}

	public void Interact(DoorData doorInfo, EInteractionType Etype)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected I4, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Invalid comparison between Unknown and I4
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Invalid comparison between Unknown and I4
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		doorInfo.LastInteractTime = Time.time;
		_lastInteractedInfo = doorInfo;
		Door door = doorInfo.Door;
		if (shallKickOpen(door, Etype) || (int)Etype == 3)
		{
			BreachingDoor = true;
			Etype = (EInteractionType)3;
		}
		else
		{
			BreachingDoor = false;
			((WorldInteractiveObject)door).Snap = (EDoorState)0;
		}
		_traversingEnd = Time.time;
		EInteractionType val = Etype;
		EInteractionType val2 = val;
		switch ((int)val2)
		{
		default:
			return;
		case 3:
			_traversingEnd += 1.5f;
			break;
		case 0:
			_traversingEnd += 0.35f;
			break;
		case 1:
			_traversingEnd += 0.1f;
			break;
		case 2:
			return;
		}
		if ((int)Etype == 3 || ModDetection.ProjectFikaLoaded || !GlobalSettingsClass.Instance.General.Doors.NoDoorAnimations)
		{
			base.BotOwner.Mover.SprintPause(2f);
			base.BotOwner.Mover.MovementPause(2f, true);
			base.BotOwner.DoorOpener.Interact(door, Etype);
			base.Bot.Steering.LookToPoint(((Component)door).transform.position);
			Interacting = true;
			return;
		}
		EInteractionType val3 = Etype;
		EInteractionType val4 = val3;
		EDoorState state;
		if ((int)val4 != 0)
		{
			if ((int)val4 != 1)
			{
				Logger.LogError($"Door open type set wrong! {Etype}");
				return;
			}
			state = (EDoorState)2;
		}
		else
		{
			state = (EDoorState)4;
		}
		bool shallInvert = ShallInvertDoorAngle(door);
		GameWorldComponent.Instance.Doors.ChangeDoorState(door, state, shallInvert);
		BotManagerComponent.Instance.BotHearing.PlayAISound(base.PlayerComponent, SAINSoundType.Door, ((Component)door).transform.position, 30f, 1f, limitFreq: true);
	}

	private bool ShallInvertDoorAngle(Door door)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!GlobalSettingsClass.Instance.General.Doors.InvertDoors)
		{
			return false;
		}
		GStruct425 interactionParameters = ((WorldInteractiveObject)door).GetInteractionParameters(base.BotOwner.Position);
		if (interactionParameters.AnimationId == (((int)((WorldInteractiveObject)door).DoorState != 1) ? ((WorldInteractiveObject)door).CalculateInteractionIndex(base.BotOwner.Position) : ((int)((WorldInteractiveObject)door).DoorKeyOpenInteraction)))
		{
			return false;
		}
		return true;
	}

	public bool CheckWantToInteract(DoorData data, Vector3 botPosition)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		NavMeshDoorLink link = data.Link;
		botPosition += Vector3.up;
		if (Mathf.Abs(botPosition.y - link.Open1.y) >= 0.5f)
		{
			return false;
		}
		EDoorState doorState = ((WorldInteractiveObject)data.Door).DoorState;
		EDoorState val = doorState;
		if ((int)val != 2)
		{
			if ((int)val == 4)
			{
				return data.DotProduct < 0f;
			}
			return false;
		}
		return data.DotProduct > 0f;
	}
}
