using System;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SAIN.Patches.Hearing;

public class SoundClipNameCheckerPatch2 : ModulePatch
{
	private const string FUSE = "SndFuse";

	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BaseSoundPlayer), "SoundAtPointEventHandler", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static void PatchPrefix(string soundName, BaseSoundPlayer __instance)
	{
		if (soundName == "SndFuse")
		{
			SoundElement val = __instance.AdditionalSounds.Find((SoundElement elem) => elem.EventName == "SndFuse" || elem.EventName == "SndSndFuse");
			if (val != null)
			{
				val.RollOff = 60;
				val.Volume = 1f;
			}
		}
	}
}
