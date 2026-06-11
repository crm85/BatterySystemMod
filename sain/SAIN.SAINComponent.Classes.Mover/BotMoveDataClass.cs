using System;
using System.Collections.Generic;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes.Mover;

public class BotMoveDataClass : IBotMoveData, IDisposable
{
	public bool Active => CurrentMoveStatus != EBotMoveStatus.None;

	public EBotMoveStatus CurrentMoveStatus { get; set; }

	public BotCornerDetails Destination { get; set; } = default(BotCornerDetails);

	public BotCornerDetails LastCorner { get; set; } = default(BotCornerDetails);

	public BotCornerDetails CurrentCorner { get; set; } = default(BotCornerDetails);

	public int CurrentIndex { get; set; }

	public List<BotCornerDetails> PathCornerDetails { get; } = new List<BotCornerDetails>();

	public List<Vector3> PathCorners { get; } = new List<Vector3>();

	public int CornerCount => PathCorners.Count;

	public bool OnLastCorner => CurrentIndex == CornerCount - 1;

	public float CurrentCornerDistanceSqr { get; set; }

	public EBotSprintStatus CurrentSprintStatus { get; set; }

	public ESprintUrgency SprintUrgency { get; set; }

	public bool WantToSprint { get; set; }

	public bool ShallSprintNow { get; set; }

	public bool ShallStopSprintWhenSeeEnemy { get; set; }

	public float PauseTime { get; set; }

	public bool Paused => CurrentMoveStatus == EBotMoveStatus.Paused;

	public float CancelTime { get; set; }

	public bool Canceling => CurrentMoveStatus == EBotMoveStatus.Canceling;

	public float TimeStarted { get; set; }

	public float PathLength { get; set; }

	private NavMeshPath _destinationPath { get; } = new NavMeshPath();

	public bool CheckPaused()
	{
		if (CurrentMoveStatus == EBotMoveStatus.Paused)
		{
			if (PauseTime > Time.time)
			{
				return true;
			}
			CurrentMoveStatus = (WantToSprint ? EBotMoveStatus.Running : EBotMoveStatus.Walking);
		}
		return false;
	}

	public void ActivateNewPath(Vector3 destination, bool shallSprint, ESprintUrgency urgency, Vector3[] corners, float shortCornerConfigDistance)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Dispose();
		CurrentMoveStatus = (shallSprint ? EBotMoveStatus.Running : EBotMoveStatus.Walking);
		TimeStarted = Time.time;
		WantToSprint = shallSprint;
		SprintUrgency = urgency;
		AnalyzePath(corners, shortCornerConfigDistance);
		SetNewDestination(destination);
	}

	public bool TryUpdatePath(Vector3 possibleDestination)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			Vector3 val = Destination.Position - possibleDestination;
			if (((Vector3)(ref val)).sqrMagnitude < 0.025f)
			{
				return true;
			}
			val = LastCorner.Position - possibleDestination;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < 0.5f)
			{
				SetNewDestination(possibleDestination);
				return true;
			}
			if (sqrMagnitude < 1f && TryCalcPathToNewDestination(possibleDestination, out var newCorners) && newCorners.Length != 0)
			{
				AddCornersToPath(newCorners);
				SetNewDestination(possibleDestination);
				return true;
			}
		}
		return false;
	}

	private void AddCornersToPath(Vector3[] newCorners)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		int num = PathCorners.Count - 1;
		bool flag = CurrentCorner.Index == num;
		if (num >= 0)
		{
			PathCorners.RemoveAt(num);
			PathCornerDetails.RemoveAt(num);
		}
		int num2 = newCorners.Length;
		for (int i = 0; i < num2; i++)
		{
			PathCorners.Add(newCorners[i]);
			Vector3? nextCorner = ((i < num2 - 1) ? new Vector3?(newCorners[i + 1]) : ((Vector3?)null));
			PathCornerDetails.AddCornerToPath(newCorners[i], nextCorner, EBotCornerType.PathTurn, EBotCornerType.PathEndApproach, EBotCornerType.PathEnd);
		}
		PathLength = PathCornerDetails.CalcPathLength();
		int num3 = PathCorners.Count - 1;
		LastCorner = PathCornerDetails[num3];
		if (flag && num3 >= num && num >= 0)
		{
			CurrentCorner = PathCornerDetails[num];
		}
	}

	private void SetNewDestination(Vector3 destination)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int count = PathCorners.Count;
		Destination = BotCornerDetails.Create(destination, EBotCornerType.Destination, count);
		BotCornerDetails botCornerDetails = PathCornerDetails[count - 1];
		botCornerDetails.SetDirection(destination - botCornerDetails.Position);
		LastCorner = botCornerDetails;
		PathCornerDetails[count - 1] = botCornerDetails;
	}

	private bool TryCalcPathToNewDestination(Vector3 destination, out Vector3[] newCorners)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		Vector3 val = PathCorners[CornerCount - 2];
		_destinationPath.ClearCorners();
		if (NavMesh.CalculatePath(val, destination, -1, _destinationPath) && (int)_destinationPath.status == 0)
		{
			newCorners = _destinationPath.corners;
			return true;
		}
		newCorners = null;
		return false;
	}

	private void AnalyzePath(Vector3[] corners, float shortCornerConfigDistance)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		PathCornerDetails.Clear();
		PathCorners.Clear();
		for (int i = 0; i < corners.Length; i++)
		{
			PathCorners.Add(corners[i]);
			PathCornerDetails.Add(BotCornerDetails.Create(ref corners, shortCornerConfigDistance, CornerCount, i));
		}
		PathLength = PathCornerDetails.CalcPathLength();
		LastCorner = PathCornerDetails[PathCornerDetails.Count - 1];
	}

	public void Dispose()
	{
		CurrentMoveStatus = EBotMoveStatus.None;
		CurrentSprintStatus = EBotSprintStatus.None;
		SprintUrgency = ESprintUrgency.None;
		Destination = default(BotCornerDetails);
		LastCorner = default(BotCornerDetails);
		CurrentCorner = default(BotCornerDetails);
		WantToSprint = false;
		ShallSprintNow = false;
		ShallStopSprintWhenSeeEnemy = false;
		PathCornerDetails.Clear();
		PathCorners.Clear();
		_destinationPath.ClearCorners();
		CurrentIndex = 0;
		CurrentCornerDistanceSqr = 0f;
		PauseTime = -1f;
		CancelTime = -1f;
		PathLength = 0f;
		TimeStarted = 0f;
	}
}
