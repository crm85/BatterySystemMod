using System;
using System.Collections;
using System.Collections.Generic;
using Comfort.Common;
using EFT;
using SAIN.Components;
using SAIN.Components.BotController;
using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.Types.Jobs;

public abstract class SainJobTemplate : ISainJob
{
	protected readonly string Name;

	protected readonly bool Looping;

	protected readonly float LoopInterval;

	protected readonly MonoBehaviour Owner;

	protected Coroutine Coroutine;

	public bool Active => Coroutine != null;

	protected static GameWorld GameWorld => Singleton<GameWorld>.Instance;

	protected static IBotGame BotGame => Singleton<IBotGame>.Instance;

	protected static GameWorldComponent SAINGameWorld => GameWorldComponent.Instance;

	protected static BotManagerComponent SAINBotController => BotManagerComponent.Instance;

	protected static PlayerDictionary AlivePlayers => GameWorldComponent.Instance?.PlayerTracker?.AlivePlayersDictionary;

	protected static List<IPlayer> DeadPlayers => GameWorldComponent.Instance?.PlayerTracker?.DeadPlayers;

	protected static BotDictionary AliveBots => BotSpawnController.Instance?.BotDictionary;

	protected static bool GameActive
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			GameStatus gameStatus = GameStatus;
			if (1 == 0)
			{
			}
			bool result = gameStatus - 1 <= 3;
			if (1 == 0)
			{
			}
			return result;
		}
	}

	protected static GameStatus GameStatus
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (BotGame == null)
			{
				return (GameStatus)0;
			}
			return BotGame.Status;
		}
	}

	public event Action OnExecutionFinished;

	public event Action OnExecutionStarted;

	protected SainJobTemplate(string InName, MonoBehaviour InOwner, bool InLooping = true, float InLoopInterval = 1f / 30f)
	{
		Name = InName;
		Looping = InLooping;
		LoopInterval = InLoopInterval;
		Owner = InOwner;
		base._002Ector();
	}

	protected virtual IEnumerator Loop()
	{
		Logger.LogDebug("Starting Job: [" + Name + "]");
		WaitForSeconds Wait = new WaitForSeconds(LoopInterval);
		while (LoopCondition())
		{
			if (!CanProceed())
			{
				yield return null;
				continue;
			}
			this.OnExecutionStarted?.Invoke();
			yield return PrimaryFunction();
			this.OnExecutionFinished?.Invoke();
			yield return Wait;
		}
		Logger.LogDebug("Job Ended [" + Name + "]");
	}

	protected virtual IEnumerator PrimaryFunction()
	{
		yield return null;
	}

	protected virtual bool LoopCondition()
	{
		return true;
	}

	protected virtual bool CanProceed()
	{
		return true;
	}

	public void Start()
	{
		if (!Active)
		{
			if ((Object)(object)Owner == (Object)null)
			{
				Logger.LogError("Owner Null. Cannot Start.");
			}
			else
			{
				Coroutine = Owner.StartCoroutine(Loop());
			}
		}
	}

	public void Stop()
	{
		if (Active)
		{
			if ((Object)(object)Owner == (Object)null)
			{
				Logger.LogError("Owner Null. Cannot Stop Coroutine.");
			}
			else
			{
				Owner.StopCoroutine(Coroutine);
			}
		}
	}
}
