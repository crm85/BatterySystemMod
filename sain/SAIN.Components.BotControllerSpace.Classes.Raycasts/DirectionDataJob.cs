using System.Collections;
using System.Collections.Generic;
using SAIN.Components.PlayerComponentSpace;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public class DirectionDataJob : BotManagerBase
{
	private JobHandle _PlayerTickJobHandle;

	private PlayerTickJob _PlayerTickJob;

	private readonly List<PlayerTickData> _playerTickData = new List<PlayerTickData>();

	public DirectionDataJob(BotManagerComponent botController)
		: base(botController)
	{
		((MonoBehaviour)botController).StartCoroutine(DirectionDataJobLoop());
	}

	private IEnumerator DirectionDataJobLoop()
	{
		yield return null;
		while ((Object)(object)GameWorldComponent.Instance != (Object)null)
		{
			HashSet<PlayerComponent> players = GameWorldComponent.Instance.PlayerTracker?.AlivePlayerArray;
			if (players == null || players.Count <= 1)
			{
				yield return null;
				continue;
			}
			foreach (PlayerComponent playerComp in players)
			{
				if ((Object)(object)playerComp != (Object)null && playerComp.OtherPlayersData != null)
				{
					_playerTickData.Add(playerComp.GetPreparedTickData());
				}
			}
			int jobCount = _playerTickData.Count;
			if (jobCount > 0)
			{
				_PlayerTickJob = new PlayerTickJob
				{
					Input = new NativeArray<PlayerTickData>(jobCount, (Allocator)3, (NativeArrayOptions)1),
					Output = new NativeArray<PlayerTickData>(jobCount, (Allocator)3, (NativeArrayOptions)1)
				};
				for (int i = 0; i < jobCount; i++)
				{
					_PlayerTickJob.Input[i] = _playerTickData[i];
				}
				_PlayerTickJobHandle = IJobForExtensions.Schedule<PlayerTickJob>(_PlayerTickJob, jobCount, default(JobHandle));
				yield return null;
				((JobHandle)(ref _PlayerTickJobHandle)).Complete();
				for (int j = 0; j < jobCount; j++)
				{
					PlayerTickData data = _PlayerTickJob.Output[j];
					data.ReadData();
					data.Owner.SetTickData(data);
				}
				_PlayerTickJob.Dispose();
				_playerTickData.Clear();
			}
			yield return null;
		}
	}

	public void Dispose()
	{
		((JobHandle)(ref _PlayerTickJobHandle)).Complete();
		_PlayerTickJob.Dispose();
	}
}
