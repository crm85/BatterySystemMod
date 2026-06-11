using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class PersonClass : PersonBase
{
	public string Name { get; private set; }

	public bool Active => ActivationClass.Active;

	public PersonAIInfo AIInfo { get; } = new PersonAIInfo();

	public PersonTransformClass Transform { get; }

	public PersonActiveClass ActivationClass { get; }

	public void Update()
	{
		ActivationClass.CheckActive();
		if (ActivationClass.PlayerActive)
		{
			Transform.Update();
		}
	}

	public void LateUpdate()
	{
		ActivationClass.CheckActive();
	}

	public void InitBot(BotOwner botOwner)
	{
		if ((Object)(object)botOwner == (Object)null)
		{
			Logger.LogWarning(Name + " : Null BotOwner, cannot Initialize!");
			return;
		}
		Name = ((Object)botOwner).name;
		AIInfo.InitBot(botOwner);
		ActivationClass.InitBot(botOwner);
	}

	public void InitBot(BotComponent bot)
	{
		AIInfo.InitBot(bot);
	}

	public PersonClass(PlayerData playerData)
		: base(playerData)
	{
		Transform = new PersonTransformClass(this, playerData);
		ActivationClass = new PersonActiveClass(this, playerData);
		Name = ((Object)playerData.Player).name;
	}
}
