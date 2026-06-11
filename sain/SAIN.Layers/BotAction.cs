using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.Layers;

public abstract class BotAction : CustomLogic
{
	private BotComponent _bot;

	public string Name { get; private set; }

	public BotComponent Bot
	{
		get
		{
			if ((Object)(object)((CustomLogic)this).BotOwner == (Object)null)
			{
				return null;
			}
			if ((Object)(object)_bot == (Object)null && BotManagerComponent.Instance.GetSAIN(((CustomLogic)this).BotOwner, out var bot))
			{
				_bot = bot;
			}
			if ((Object)(object)_bot == (Object)null)
			{
				_bot = ((Component)((CustomLogic)this).BotOwner).GetComponent<BotComponent>();
			}
			return _bot;
		}
	}

	public BotAction(BotOwner botOwner, string name)
		: base(botOwner)
	{
		Name = name;
	}

	protected void ToggleAction(bool value)
	{
		if (value)
		{
			PatrollingData patrollingData = ((CustomLogic)this).BotOwner.PatrollingData;
			if (patrollingData != null)
			{
				patrollingData.Pause();
			}
		}
		else
		{
			PatrollingData patrollingData2 = ((CustomLogic)this).BotOwner.PatrollingData;
			if (patrollingData2 != null)
			{
				patrollingData2.Unpause();
			}
		}
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
		DebugOverlay.AddBaseInfo(Bot, ((CustomLogic)this).BotOwner, stringBuilder);
	}

	protected void StartProfilingSample(string functionName)
	{
		if (!SAINPlugin.ProfilingMode)
		{
		}
	}

	protected void EndProfilingSample()
	{
		if (!SAINPlugin.ProfilingMode)
		{
		}
	}
}
