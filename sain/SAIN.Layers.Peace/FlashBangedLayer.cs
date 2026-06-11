using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Models.Enums;

namespace SAIN.Layers.Peace;

internal class FlashBangedLayer : SAINLayer
{
	public static readonly string Name = SAINLayer.BuildLayerName("FlashBanged");

	public FlashBangedLayer(BotOwner bot, int priority)
		: base(bot, priority, Name, ESAINLayer.Peace)
	{
	}

	public override Action GetNextAction()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		return new Action(typeof(ExtractAction), $"Extract : {base.Bot.Memory.Extract.ExtractReason}", (ActionData)null);
	}

	public override bool IsActive()
	{
		base.IsActive();
		setLayer(active: false);
		return false;
	}

	public override bool IsCurrentActionEnding()
	{
		return false;
	}
}
