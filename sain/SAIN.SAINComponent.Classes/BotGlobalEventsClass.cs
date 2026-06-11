using System;
using SAIN.Components;
using SAIN.Helpers.Events;

namespace SAIN.SAINComponent.Classes;

public class BotGlobalEventsClass : BotComponentClassBase
{
	public event Action<BotComponent> OnEnterPeace;

	public event Action<BotComponent> OnExitPeace;

	public event Action<BotComponent, NavGraphVoxelSimple, NavGraphVoxelSimple> OnVoxelChanged;

	public BotGlobalEventsClass(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public override void Init()
	{
		ToggleEventTimeTracked onPeaceChanged = base.Bot.EnemyController.Events.OnPeaceChanged;
		onPeaceChanged.OnToggle = (Action<bool>)Delegate.Combine(onPeaceChanged.OnToggle, new Action<bool>(PeaceChanged));
		base.Bot.DoorOpener.DoorFinder.OnNewVoxel += onVoxelChange;
		base.Init();
	}

	public override void Dispose()
	{
		ToggleEventTimeTracked onPeaceChanged = base.Bot.EnemyController.Events.OnPeaceChanged;
		onPeaceChanged.OnToggle = (Action<bool>)Delegate.Remove(onPeaceChanged.OnToggle, new Action<bool>(PeaceChanged));
		base.Bot.DoorOpener.DoorFinder.OnNewVoxel -= onVoxelChange;
		base.Dispose();
	}

	private void onVoxelChange(NavGraphVoxelSimple newVoxel, NavGraphVoxelSimple oldVoxel)
	{
		this.OnVoxelChanged?.Invoke(base.Bot, newVoxel, oldVoxel);
	}

	public void PeaceChanged(bool value)
	{
		if (value)
		{
			this.OnEnterPeace?.Invoke(base.Bot);
		}
		else
		{
			this.OnExitPeace?.Invoke(base.Bot);
		}
	}
}
