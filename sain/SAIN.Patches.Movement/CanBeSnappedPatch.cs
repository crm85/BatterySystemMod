using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Movement;

public class CanBeSnappedPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.PropertyGetter(typeof(Player), "CanBeSnapped");
	}

	[PatchPrefix]
	public static bool Patch(ref bool __result)
	{
		__result = false;
		return false;
	}
}
