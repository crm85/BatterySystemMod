using System;
using System.Collections.Generic;
using EFT.Interactive;
using SAIN.Components;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class DoorFinder : BotSubClass<DoorOpener>, IBotClass, IDisposable
{
	private const float DOORS_UPDATE_FREQ = 0.5f;

	private const float DOORS_FIND_CLOSE_FREQ = 2f;

	private const float DOORS_CLOSE_DISTANCE = 400f;

	private const float DOORS_INTERACTION_DISTANCE = 64f;

	private const float DOORS_FIND_INTERACTION_FREQ = 1f;

	private const float DOORS_UPDATE_VOXEL_FREQ = 0.5f;

	private float _nextUpdateDoorTime;

	private float _nextCheckDistanceTime;

	private float _nextUpdateVoxelTime;

	private float _nextUpdateInteractTime;

	public List<DoorData> InteractionDoors { get; } = new List<DoorData>();

	public List<DoorData> CloseDoors { get; } = new List<DoorData>();

	public List<DoorData> AllDoors { get; } = new List<DoorData>();

	public NavGraphVoxelSimple CurrentVoxel { get; private set; }

	private bool _moving => base.BotOwner.Mover.HasPathAndNoComplete || base.Bot.Mover.PathFollower.Moving;

	public event Action<NavGraphVoxelSimple, NavGraphVoxelSimple> OnNewVoxel;

	public event Action<List<DoorData>> OnNewCloseDoorsFound;

	static DoorFinder()
	{
	}

	public DoorFinder(DoorOpener opener)
		: base(opener)
	{
	}

	public override void Init()
	{
		base.Init();
	}

	public override void ManualUpdate()
	{
		updateVoxel();
		updateCurrentDoors();
		base.ManualUpdate();
	}

	private void newMove(Vector3 currentCorner, Vector3 destination)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		findDotProducts(base.Bot.Position, currentCorner);
	}

	private static void debugFindDoors()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(Vector3.zero, 1000f, LayerMaskClass.DoorLayer);
		Logger.LogDebug($"Found {array.Length} total doors");
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			DebugGizmos.Sphere(((Component)val).transform.position, -1f);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	private void updateCurrentDoors()
	{
		if (_nextUpdateDoorTime < Time.time)
		{
			_nextUpdateDoorTime = Time.time + 0.5f;
			updateAllDoors(force: false);
		}
	}

	private void updateVoxel()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		float time = Time.time;
		if (_nextUpdateVoxelTime < time && _moving)
		{
			_nextUpdateVoxelTime = time + 0.5f;
			base.BotOwner.AIData.SetPosToVoxel(base.Bot.Position);
			NavGraphVoxelSimple currentVoxel = CurrentVoxel;
			CurrentVoxel = base.BotOwner.VoxelesPersonalData.CurVoxel;
			if (currentVoxel != CurrentVoxel)
			{
				findAllDoors(CurrentVoxel);
				this.OnNewVoxel?.Invoke(CurrentVoxel, currentVoxel);
			}
		}
	}

	private void findAllDoors(NavGraphVoxelSimple voxel)
	{
		AllDoors.Clear();
		if (voxel == null)
		{
			return;
		}
		_nextUpdateDoorTime = Time.time + 0.5f;
		_nextCheckDistanceTime = Time.time + 2f;
		foreach (NavMeshDoorLink doorLink in voxel.DoorLinks)
		{
			if (isDoorOpenable(doorLink.Door))
			{
				AllDoors.Add(new DoorData(doorLink));
			}
		}
		updateAllDoors(force: true);
	}

	private bool isDoorOpenable(Door door)
	{
		if (!((Behaviour)door).enabled || !((Component)door).gameObject.activeInHierarchy || !((WorldInteractiveObject)door).Operatable)
		{
			return false;
		}
		if (BotBase.GlobalSettings.General.Doors.DisableAllDoors && GameWorldComponent.Instance.Doors.DisableDoor(door))
		{
			return false;
		}
		return true;
	}

	private void updateAllDoors(bool force)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = base.Bot.Position;
		foreach (DoorData allDoor in AllDoors)
		{
			allDoor.CalcDirection(position);
		}
		findCloseDoors(force);
		findDoorsToInteract(position, force);
	}

	private void findDoorsToInteract(Vector3 botPosition, bool force)
	{
		if (force || _nextUpdateInteractTime < Time.time)
		{
			_nextUpdateInteractTime = Time.time + 1f;
			findDoorsInRange(64f, CloseDoors, InteractionDoors);
		}
	}

	private void findDotProducts(Vector3 botPosition, Vector3 currentCornerDestination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = currentCornerDestination - botPosition;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		foreach (DoorData interactionDoor in InteractionDoors)
		{
			interactionDoor.CalcDirection(botPosition);
			interactionDoor.DotProduct = Vector3.Dot(interactionDoor.DirectionNormal, normalized);
		}
	}

	private void findCloseDoors(bool force)
	{
		if (force || _nextCheckDistanceTime < Time.time)
		{
			findDoorsInRange(400f, AllDoors, CloseDoors);
			this.OnNewCloseDoorsFound?.Invoke(CloseDoors);
		}
	}

	private static void findDoorsInRange(float range, List<DoorData> doorsToCheck, List<DoorData> result)
	{
		result.Clear();
		foreach (DoorData item in doorsToCheck)
		{
			if (item.CurrentSqrMagnitude <= range)
			{
				result.Add(item);
			}
		}
	}
}
