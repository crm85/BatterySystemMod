using System.Collections.Generic;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent;

public abstract class BotComponentClassBase : BotBase
{
	protected readonly List<IBotClass> SubClasses = new List<IBotClass>();

	protected BotComponentClassBase(BotComponent bot)
		: base(bot)
	{
		bot.AddBotClass(this);
	}

	public override void Init()
	{
		base.Bot.AddBotTickClass(this);
		foreach (IBotClass subClass in SubClasses)
		{
			subClass.Init();
		}
		base.Init();
	}

	public override void ManualUpdate()
	{
		float time = Time.time;
		foreach (IBotClass subClass in SubClasses)
		{
			subClass.ManualUpdate();
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		foreach (IBotClass subClass in SubClasses)
		{
			subClass?.Dispose();
		}
		base.Dispose();
	}

	protected void AddSubClass(IBotClass Class)
	{
		SubClasses.Add(Class);
	}
}
