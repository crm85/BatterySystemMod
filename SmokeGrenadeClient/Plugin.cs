using System;
using BepInEx;
using Comfort.Common;
using EFT;
using SmokeGrenadeClient.Configs;

namespace SmokeGrenadeClient
{
	[BepInPlugin("com.jiro.smokegrenades", "Jiro-SmokeGrenades", "1.0.0")]
	[BepInDependency("me.sol.sain", BepInDependency.DependencyFlags.SoftDependency)]
	public class SmokeGrenadePlugin : BaseUnityPlugin
	{
		public void Awake()
		{
			SmokeGrenadeConfig.Init(Config);
			if (!SmokeGrenadeConfig.EnableMod.Value) return;

			EnablePatchSafe("SmokeGrenadeExplosionPatch", () => new SmokeGrenadeExplosionPatch().Enable());
			EnablePatchSafe("M18SmokeEffectSuppressionPatch", () => new M18SmokeEffectSuppressionPatch().Enable());
			EnablePatchSafe("SainRetreatDecisionSmokePatch", SainRetreatDecisionSmokePatch.TryEnable);
		}

		private void EnablePatchSafe(string patchName, Action enable)
		{
			try
			{
				enable();
			}
			catch (Exception ex)
			{
				Logger.LogWarning($"SmokeGrenades patch {patchName} failed to enable; continuing without it. {ex}");
			}
		}

		public static bool InGame()
		{
			return Singleton<GameWorld>.Instance?.MainPlayer?.HealthController.IsAlive == true
				&& !(Singleton<GameWorld>.Instance.MainPlayer is HideoutPlayer);
		}
	}
}
