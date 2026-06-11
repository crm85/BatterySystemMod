using System;
using SAIN.Components;

namespace SAIN;

public interface IBotClass : IDisposable
{
	BotComponent Bot { get; }

	ESAINTickState TickRequirement { get; }

	bool CanEverTick { get; }

	float TickInterval { get; }

	float LastTickTime { get; }

	void Init();

	void ManualUpdate();

	bool ShallTick(float CurrentTime);
}
