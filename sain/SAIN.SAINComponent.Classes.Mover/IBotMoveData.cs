using System;
using System.Collections.Generic;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public interface IBotMoveData : IDisposable
{
	bool Active { get; }

	EBotMoveStatus CurrentMoveStatus { get; }

	BotCornerDetails Destination { get; }

	EBotSprintStatus CurrentSprintStatus { get; }

	ESprintUrgency SprintUrgency { get; }

	List<BotCornerDetails> PathCornerDetails { get; }

	List<Vector3> PathCorners { get; }

	int CornerCount { get; }

	int CurrentIndex { get; }

	bool OnLastCorner { get; }

	float CurrentCornerDistanceSqr { get; }

	BotCornerDetails CurrentCorner { get; }

	BotCornerDetails LastCorner { get; }

	bool WantToSprint { get; }

	bool ShallSprintNow { get; }

	bool ShallStopSprintWhenSeeEnemy { get; }

	float PauseTime { get; }

	float CancelTime { get; }

	float PathLength { get; }

	float TimeStarted { get; }

	bool Canceling { get; }
}
