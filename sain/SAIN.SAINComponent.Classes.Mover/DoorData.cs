using EFT.Interactive;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class DoorData
{
	private const float DOOR_SINGLE_INTERACTION_FREQ = 1f;

	private const float UPDATE_SQRMAG_FREQ = 0.5f;

	private int _lastCalcFrame;

	private float _nextCheckSqrMagTime;

	public float LastInteractTime { get; set; }

	public float LastOpenTime { get; set; }

	public float LastCloseTime { get; set; }

	public NavMeshDoorLink Link { get; }

	public Vector3 LinkPosition { get; }

	public Door Door { get; }

	public float CurrentSqrMagnitude
	{
		get
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (_nextCheckSqrMagTime < Time.time)
			{
				_nextCheckSqrMagTime = Time.time + 0.5f;
				Vector3 direction = Direction;
				LastSqrMagnitude = ((Vector3)(ref direction)).sqrMagnitude;
			}
			return LastSqrMagnitude;
		}
	}

	public Vector3 CenterPoint
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			EDoorState doorState = ((WorldInteractiveObject)Door).DoorState;
			EDoorState val = doorState;
			if ((int)val == 4)
			{
				return Link.MidOpen;
			}
			return Link.MidClose;
		}
	}

	public float LastSqrMagnitude { get; private set; }

	public Vector3 Direction { get; private set; }

	public Vector3 DirectionNormal { get; private set; }

	public float DotProduct { get; set; }

	public bool DoorInFront => DotProduct > 0f;

	public DoorData(NavMeshDoorLink link)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Link = link;
		LinkPosition = ((Component)link).transform.position;
		Door = link.Door;
	}

	public bool CanInteractByTime()
	{
		return LastInteractTime + 1f < Time.time;
	}

	public void CalcDirection(Vector3 from)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (Time.frameCount != _lastCalcFrame)
		{
			_lastCalcFrame = Time.frameCount;
			Direction = CenterPoint - from;
			Vector3 direction = Direction;
			DirectionNormal = ((Vector3)(ref direction)).normalized;
		}
	}
}
