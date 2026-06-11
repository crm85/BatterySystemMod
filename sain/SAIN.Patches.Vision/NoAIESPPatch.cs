using System;
using System.Reflection;
using EFT;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Vision;

public class NoAIESPPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return typeof(BotOwner).GetMethod("IsEnemyLookingAtMe", BindingFlags.Instance | BindingFlags.Public, null, new Type[1] { typeof(IPlayer) }, null);
	}

	[PatchPrefix]
	public static bool PatchPrefix(ref bool __result)
	{
		__result = false;
		return false;
	}
}
