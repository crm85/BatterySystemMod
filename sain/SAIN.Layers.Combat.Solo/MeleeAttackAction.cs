using DrakiaXYZ.BigBrain.Brains;
using EFT;

namespace SAIN.Layers.Combat.Solo;

internal class MeleeAttackAction : CombatAction, ISAINAction
{
	public MeleeAttackAction(BotOwner bot)
		: base(bot, "Melee Attack")
	{
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		((CustomLogic)this).BotOwner.WeaponManager.Melee.RunToEnemyUpdate();
		EndProfilingSample();
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}
}
