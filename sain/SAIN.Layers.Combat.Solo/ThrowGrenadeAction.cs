using DrakiaXYZ.BigBrain.Brains;
using EFT;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo;

public class ThrowGrenadeAction : CombatAction, ISAINAction
{
	private float StartTime = 0f;

	private bool Stopped = false;

	public ThrowGrenadeAction(BotOwner bot)
		: base(bot, "ThrowGrenadeAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		if ((!Stopped && Time.time - StartTime > 1f) || base.Bot.Cover.CheckLimbsForCover())
		{
			Stopped = true;
			((CustomLogic)this).BotOwner.StopMove();
		}
		EndProfilingSample();
	}

	public override void Start()
	{
		StartTime = Time.time;
		Toggle(value: true);
		if (base.Bot.Squad.BotInGroup && base.Bot.Talk.GroupTalk.FriendIsClose)
		{
			base.Bot.Talk.Say((EPhraseTrigger)22);
		}
	}

	public override void Stop()
	{
		Toggle(value: false);
	}
}
