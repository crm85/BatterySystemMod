using System;
using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Memory;

public class HealthTracker : BotBase
{
	private float _nextHealthUpdateTime = 0f;

	public bool Healthy => (int)HealthStatus == 1024;

	public bool Injured => (int)HealthStatus == 2048;

	public bool BadlyInjured => (int)HealthStatus == 4096;

	public bool Dying => (int)HealthStatus == 8192;

	public ETagStatus HealthStatus { get; private set; }

	public bool OnPainKillers { get; private set; }

	public event Action<ETagStatus> HealthStatusChanged;

	public HealthTracker(BotComponent sain)
		: base(sain)
	{
	}

	public override void ManualUpdate()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_nextHealthUpdateTime < Time.time)
		{
			_nextHealthUpdateTime = Time.time + 0.5f;
			ETagStatus healthStatus = HealthStatus;
			HealthStatus = base.Player.HealthStatus;
			if (HealthStatus != healthStatus)
			{
				this.HealthStatusChanged?.Invoke(HealthStatus);
			}
			MovementContext movementContext = base.Player.MovementContext;
			OnPainKillers = movementContext != null && movementContext.PhysicalConditionIs((EPhysicalCondition)1);
		}
		base.ManualUpdate();
	}
}
