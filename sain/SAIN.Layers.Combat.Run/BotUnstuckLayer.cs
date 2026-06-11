using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Layers.Combat.Run;

internal class BotUnstuckLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("Unstuck");

	public BotUnstuckLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.Unstuck)
	{
	}

	public override Action GetNextAction()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		return new Action(typeof(GetUnstuckAction), "Getting Unstuck", (ActionData)null);
	}

	public override bool IsActive()
	{
		base.IsActive();
		setLayer(active: false);
		return false;
	}

	public override bool IsCurrentActionEnding()
	{
		if ((Object)(object)base.Bot == (Object)null)
		{
			return true;
		}
		return false;
	}
}
