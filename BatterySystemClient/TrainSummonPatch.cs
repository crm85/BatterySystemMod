using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BatterySystem.Configs;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.MovingPlatforms;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace BatterySystem
{
	public class TrainSummonPatch : ModulePatch
	{
		private static readonly HashSet<string> WhiteFlareTemplateIds = new HashSet<string>
		{
			"62389bc9423ed1685422dc57", // 26x75mm flare cartridge (White)
			"624c09da2cec124eb67c1046"  // Signal flare (White)
		};

		private static readonly FieldInfo DepartField = AccessTools.Field(typeof(MovingPlatform), "_depart");

		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(BotEventHandler), nameof(BotEventHandler.SuccessFlare));
		}

		[PatchPostfix]
		private static void Postfix(Player player, Vector3 position, AmmoTemplate ammoTemplate)
		{
			if (!BatterySystemConfig.EnableWhiteFlareTrainSummon.Value) return;
			if (!BatterySystemPlugin.InGame()) return;
			if (player == null || !player.IsYourPlayer) return;
			if (!IsWhiteFlare(ammoTemplate)) return;

			SummonNotStartedTrains();
		}

		private static bool IsWhiteFlare(AmmoTemplate ammoTemplate)
		{
			if (ammoTemplate == null) return false;
			if (WhiteFlareTemplateIds.Contains(ammoTemplate.StringId)) return true;

			string name = ammoTemplate.Name ?? string.Empty;
			string shortName = ammoTemplate.ShortName ?? string.Empty;
			string templateName = ammoTemplate._name ?? string.Empty;

			return (name.IndexOf("white", StringComparison.OrdinalIgnoreCase) >= 0
				|| shortName.IndexOf("white", StringComparison.OrdinalIgnoreCase) >= 0
				|| templateName.IndexOf("white", StringComparison.OrdinalIgnoreCase) >= 0)
				&& (name.IndexOf("flare", StringComparison.OrdinalIgnoreCase) >= 0
					|| shortName.IndexOf("flare", StringComparison.OrdinalIgnoreCase) >= 0
					|| templateName.IndexOf("flare", StringComparison.OrdinalIgnoreCase) >= 0
					|| templateName.IndexOf("rsp", StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private static void SummonNotStartedTrains()
		{
			MovingPlatform[] platforms = Singleton<GameWorld>.Instance?.Platforms
				?? LocationScene.GetAllObjectsAndWhenISayAllIActuallyMeanIt<MovingPlatform>().ToArray();

			Locomotive[] trains = platforms.OfType<Locomotive>().Where(train => train != null).ToArray();
			if (trains.Length == 0)
			{
				Debug.Log("[BatterySystem] White flare train summon requested, but this location has no train.");
				return;
			}

			int summonedCount = 0;
			foreach (Locomotive train in trains)
			{
				if (TrySummonTrain(train))
				{
					summonedCount++;
				}
			}

			Debug.Log($"[BatterySystem] White flare train summon started {summonedCount}/{trains.Length} train(s).");
		}

		private static bool TrySummonTrain(Locomotive train)
		{
			if (train.TravelState == null) return false;
			if (train.TravelState.Value != Locomotive.ETravelState.NotStarted) return false;

			DateTime summonTime = EFTDateTimeClass.UtcNow;
			if (train.Initialized)
			{
				SetDepartTime(train, summonTime);
			}
			else
			{
				train.Init(summonTime);
			}

			train.enabled = true;
			train.OnRoute = true;
			train.TravelState.Value = Locomotive.ETravelState.OnRouteToDestination;
			train.Move(true);

			foreach (Carriage carriage in train.Carriage ?? Array.Empty<Carriage>())
			{
				if (carriage != null)
				{
					SetDepartTime(carriage, summonTime);
					carriage.enabled = true;
					carriage.Move(true);
				}
			}

			return true;
		}

		private static void SetDepartTime(MovingPlatform platform, DateTime departTime)
		{
			DepartField?.SetValue(platform, departTime);
		}
	}
}
