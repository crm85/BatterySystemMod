using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonAIInfo
{
	public bool IsAI { get; private set; }

	public bool IsSAINBot { get; private set; }

	public BotOwner BotOwner { get; private set; }

	public BotComponent BotComponent { get; private set; }

	public void InitBot(BotOwner botOwner)
	{
		BotOwner = botOwner;
		IsAI = (Object)(object)botOwner != (Object)null;
	}

	public void InitBot(BotComponent bot)
	{
		BotComponent = bot;
		IsSAINBot = (Object)(object)bot != (Object)null;
	}
}
