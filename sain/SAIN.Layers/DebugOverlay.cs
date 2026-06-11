using System;
using System.Collections.Generic;
using System.Text;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.Decision;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using SAIN.SAINComponent.Classes.Mover;
using SAIN.SAINComponent.Classes.Search;
using UnityEngine;

namespace SAIN.Layers;

public static class DebugOverlay
{
	private static bool _expandedEnemyInfo => SAINPlugin.DebugSettings.Overlay.Overlay_EnemyInfo_Expanded;

	public static void AddBaseInfo(BotComponent bot, BotOwner botOwner, StringBuilder stringBuilder)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			DebugOverlaySettings overlay = SAINPlugin.DebugSettings.Overlay;
			SAINBotInfoClass info = bot.Info;
			if (overlay.Overlay_Info)
			{
				stringBuilder.AppendLine($"Name: [{bot.Person.Name}] Nickname: [{bot.Player.Profile.Nickname}] Personality: [{info.Personality}] Type: [{info.Profile.WildSpawnType}] PowerLevel: [{info.Profile.PowerLevel}]");
				GClass1437.AppendLabeledValue(stringBuilder, "In Combat", $"{bot.IsInCombat}", Color.white, Color.yellow, true);
				GClass1437.AppendLabeledValue(stringBuilder, "Target Enemy", bot.CurrentTarget?.CurrentTargetEnemy?.EnemyName ?? "", Color.white, Color.yellow, true);
				GClass1437.AppendLabeledValue(stringBuilder, "Goal Enemy", bot.Enemy?.EnemyName ?? "", Color.white, Color.yellow, true);
				stringBuilder.AppendLine(decisionInfo(bot));
				GClass1437.AppendLabeledValue(stringBuilder, "Steering", $"{bot.Steering.CurrentSteerPriority} : {bot.Steering.EnemySteerDir}", Color.white, Color.yellow, true);
				GClass1437.AppendLabeledValue(stringBuilder, "DogFight Status", $"{bot.Mover.DogFight.Status}", Color.white, Color.yellow, true);
				string text = $"Pose [{bot.Mover.Pose.PoseValue.LastSmoothedValue}:{bot.Mover.Pose.PoseValue.TargetValue}]";
				string text2 = $"Speed [{bot.Mover.Pose.SpeedValue.LastSmoothedValue} : {bot.Mover.Pose.SpeedValue.TargetValue}]";
				GClass1437.AppendLine(stringBuilder, "[" + text + "] [" + text2 + "]", Color.white);
				if (overlay.Overlay_Info_Expanded)
				{
					AddMoveData(bot, stringBuilder);
					GClass1437.AppendLine(stringBuilder, $"Lean [{bot.Mover.Lean.LeanAngleValue.LastSmoothedValue}:{bot.Mover.Lean.LeanAngleValue.TargetValue}] " + $"CanLeanByState:{bot.Mover.Lean.CanLeanByState} " + $"CurrentLeanSetting:{bot.Mover.Lean.LeanDirection}", Color.white);
					stringBuilder.AppendLine($"Suppression Num: [{bot.Suppression?.SuppressionNumber}] State: [{bot.Suppression?.CurrentState}] Last State: [{bot.Suppression?.LastState}]");
					stringBuilder.AppendLine($"CoverPoints: [{bot.Cover.CoverPoints.Count}] : StartSearchDelay [{info.TimeBeforeSearch}] : Hold Ground Time [{info.HoldGroundDelay}]");
					object arg = bot.Memory.Location.IsIndoors;
					Player player = bot.Player;
					stringBuilder.AppendLine($"Indoors? {arg} EnvironmentID: {((player != null) ? new int?(player.AIData.EnvironmentId) : ((int?)null))} In Bunker? {bot.PlayerComponent.AIData.PlayerLocation.InBunker}");
					Dictionary<string, BotComponent> dictionary = bot.Squad.SquadInfo?.Members;
					if (dictionary != null && dictionary.Count > 1)
					{
						stringBuilder.AppendLine($"Squad Personality: [{bot.Squad.SquadInfo.SquadPersonality}]");
					}
				}
			}
			if (overlay.Overlay_Decisions)
			{
				stringBuilder.AppendLine($"Main Decisn [{bot.Decision.CurrentCombatDecision}] : Last [{bot.Decision.PreviousCombatDecision}]");
				stringBuilder.AppendLine($"Squad Decisn [{bot.Decision.CurrentSquadDecision}] : Last [{bot.Decision.PreviousSquadDecision}]");
				stringBuilder.AppendLine($"Self Decisn [{bot.Decision.CurrentSelfDecision}] : Last [{bot.Decision.PreviousSelfDecision}]");
				stringBuilder.AppendLine("DecisionReasons");
				StringBuilder decisionReasons = bot.Decision.EnemyDecisions.DecisionReasons;
				stringBuilder.Append(decisionReasons);
			}
			if (overlay.Overlay_EnemyLists)
			{
				EnemyListsClass enemyLists = bot.EnemyController.EnemyLists;
				EnemyList enemyList = enemyLists.GetEnemyList(EEnemyListType.Known);
				EnemyList enemyList2 = enemyLists.GetEnemyList(EEnemyListType.Visible);
				EnemyList enemyList3 = enemyLists.GetEnemyList(EEnemyListType.InLineOfSight);
				EnemyList enemyList4 = enemyLists.GetEnemyList(EEnemyListType.ActiveThreats);
				stringBuilder.AppendLine("EnemyList[bots/human]: " + $"Known[{enemyList.Bots}/{enemyList.Humans}] " + $"Visible[{enemyList2.Bots}/{enemyList2.Humans}] " + $"InLOS[{enemyList3.Bots}/{enemyList3.Humans}] " + $"ActvThreat [{enemyList4.Bots}/{enemyList4.Humans}]");
			}
			if (overlay.OverLay_AimInfo)
			{
				IBotAiming currentAiming = bot.BotOwner.AimingManager.CurrentAiming;
				BotAimingClass val = (BotAimingClass)(object)((currentAiming is BotAimingClass) ? currentAiming : null);
				if (val != null)
				{
					stringBuilder.AppendLine($"AimData: Status [{bot.Aim.AimStatus}] " + $"Last Aim Time: [{bot.Aim.LastAimTime}] " + $"AimingTime [{val.float_7}] " + $"TimeToFnsh: [{val.float_5}]");
					Vector3 val2 = bot.BotOwner.AimingManager.CurrentAiming.RealTargetPoint - bot.BotOwner.AimingManager.CurrentAiming.EndTargetPoint;
					stringBuilder.AppendLine($"AimOffsetMagnitude [{((Vector3)(ref val2)).magnitude.Round100()}] " + $"Friendly Fire Status [{bot.FriendlyFire.FriendlyFireStatus}] " + $"No Bush ESP Status: [{bot.NoBushESP.NoBushESPActive}]");
				}
			}
			if (overlay.Overlay_EnemyInfo)
			{
				Enemy enemy2Show = getEnemy2Show(bot);
				if (enemy2Show != null)
				{
					CreateEnemyInfo(stringBuilder, enemy2Show);
				}
			}
			if (!overlay.Overlay_Search)
			{
				return;
			}
			EnemyDecisionClass enemyDecisions = bot.Decision.EnemyDecisions;
			bool? debugShallSearch = enemyDecisions.DebugShallSearch;
			if (debugShallSearch.HasValue)
			{
				if (debugShallSearch == true)
				{
					GClass1437.AppendLabeledValue(stringBuilder, "Searching", $"Current State: {bot.Search.CurrentState} " + $"Next: {bot.Search.NextState} " + $"Last: {bot.Search.LastState}", Color.white, Color.yellow, true);
				}
				SearchReasonsStruct debugSearchReasons = enemyDecisions.DebugSearchReasons;
				SearchReasonsStruct.WantSearchReasonsStruct wantSearchReasons = debugSearchReasons.WantSearchReasons;
				GClass1437.AppendLabeledValue(stringBuilder, "Want Search Reasons", $"[WantToSearchReason : {wantSearchReasons.WantToSearchReason}] " + $"[NotWantToSearchReason: {wantSearchReasons.NotWantToSearchReason}] " + $"[CantStartReason: {wantSearchReasons.CantStartReason}]", Color.white, Color.yellow, true);
				if (debugSearchReasons.NotSearchReason != SearchReasonsStruct.ENotSearchReason.None)
				{
					GClass1437.AppendLabeledValue(stringBuilder, "Not Search Reason", $"{debugSearchReasons.NotSearchReason}", Color.white, Color.yellow, true);
				}
				if (!GClass1437.IsNullOrEmpty(debugSearchReasons.PathCalcFailReason))
				{
					GClass1437.AppendLabeledValue(stringBuilder, "CalcPath Fail Reason", debugSearchReasons.PathCalcFailReason ?? "", Color.white, Color.yellow, true);
				}
			}
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
	}

	public static void AddMoveData(BotComponent bot, StringBuilder stringBuilder)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		IBotMoveData moveData = bot.Mover.PathFollower.MoveData;
		GClass1437.AppendLabeledValue(stringBuilder, "Move Status", $"{moveData.CurrentMoveStatus}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move Sprint Status", $"{moveData.CurrentSprintStatus}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move Active", $"{moveData.Active}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move Canceling", $"{moveData.Canceling}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move CornerCount", $"{moveData.CornerCount}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move CurrentIndex", $"{moveData.CurrentIndex}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "Move Corner Distance", $"{moveData.CurrentCornerDistanceSqr.Sqrt()}", Color.white, Color.yellow, true);
	}

	private static Enemy getEnemy2Show(BotComponent bot)
	{
		DebugOverlaySettings overlay = SAINPlugin.DebugSettings.Overlay;
		Enemy enemy = null;
		if (overlay.OverLay_AlwaysShowMainPlayerInfo)
		{
			foreach (Enemy value in bot.EnemyController.Enemies.Values)
			{
				if (value != null && value.EnemyPlayer.IsYourPlayer)
				{
					enemy = value;
				}
			}
		}
		Enemy enemy2 = null;
		if (overlay.OverLay_AlwaysShowClosestHumanInfo)
		{
			float num = float.MaxValue;
			foreach (Enemy value2 in bot.EnemyController.Enemies.Values)
			{
				if (value2 != null && !value2.IsAI && value2.RealDistance < num)
				{
					num = value2.RealDistance;
					enemy2 = value2;
				}
			}
		}
		return enemy ?? enemy2 ?? bot.Enemy;
	}

	private static string decisionInfo(BotComponent sain)
	{
		string result = string.Empty;
		switch (sain.ActiveLayer)
		{
		case ESAINLayer.Combat:
		case ESAINLayer.AvoidThreat:
			result = $"MainDcsn: [{sain.Decision.CurrentCombatDecision}] : Layer [{sain.ActiveLayer}]";
			break;
		case ESAINLayer.Squad:
			result = $"SqdDcsn: [{sain.Decision.CurrentSquadDecision}] : Layer [{sain.ActiveLayer}]";
			break;
		case ESAINLayer.Extract:
			result = $"Extract: [{sain.Memory.Extract.ExtractReason}][{sain.Memory.Extract.ExtractStatus}] : Layer [{sain.ActiveLayer}]";
			break;
		}
		return result;
	}

	private static void CreateEnemyInfo(StringBuilder stringBuilder, Enemy enemy)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		if (enemy == null)
		{
			return;
		}
		string[] obj = new string[5] { "EnemyData: Name [", null, null, null, null };
		Player enemyPlayer = enemy.EnemyPlayer;
		obj[1] = ((enemyPlayer != null) ? enemyPlayer.Profile.Nickname : null);
		obj[2] = "] ";
		obj[3] = $"RealDistance [{enemy.RealDistance}] ";
		IPlayer enemyIPlayer = enemy.EnemyIPlayer;
		float? obj2;
		if (enemyIPlayer == null)
		{
			obj2 = null;
		}
		else
		{
			IAIData aIData = enemyIPlayer.AIData;
			obj2 = ((aIData != null) ? new float?(aIData.PowerOfEquipment) : ((float?)null));
		}
		obj[4] = $"Power [{obj2}]";
		stringBuilder.AppendLine(string.Concat(obj));
		stringBuilder.AppendLine($"Visible [{enemy.IsVisible}] Seen [{enemy.Seen}]");
		stringBuilder.AppendLine($"Aim/Scatter Multi [{enemy.Aim.AimAndScatterMultiplier}]");
		GClass1437.AppendLabeledValue(stringBuilder, "Time To Spot", $"{(1f / enemy.Vision.LastGainSightResult).Round100()}", Color.white, Color.yellow, true);
		BodyPartType partType;
		float percentSpotted = getPercentSpotted(enemy, out partType);
		if (percentSpotted > 0f)
		{
			GClass1437.AppendLabeledValue(stringBuilder, "Percent Spotted", $"{partType} : {percentSpotted}", Color.white, Color.yellow, true);
		}
		addPlaceInfo(stringBuilder, enemy.KnownPlaces.LastKnownPlace, "Last Known Position");
		if (enemy.Seen && _expandedEnemyInfo)
		{
			addPlaceInfo(stringBuilder, enemy.KnownPlaces.LastSeenPlace, "Last Seen");
		}
		if (_expandedEnemyInfo)
		{
			stringBuilder.AppendLine($"HorizAngle [{enemy.Vision.Angles.AngleToEnemyHorizontalSigned.Round100()}] VertiAngle [{enemy.Vision.Angles.AngleToEnemyVerticalSigned.Round100()}]");
			stringBuilder.AppendLine($"GainSightMod [{enemy.Vision.GainSightCoef.Round100()}] VisionDistance [{(enemy.Bot.BotOwner.Settings.FileSettings.Core.VisibleDistance + enemy.Vision.VisionDistance).Round100()}]");
		}
		stringBuilder.AppendLine();
		GClass1437.AppendLabeledValue(stringBuilder, "Can Shoot", $"{enemy.Vision.VisionChecker.EnemyParts.CanShoot}", Color.white, Color.yellow, true);
		GClass1437.AppendLabeledValue(stringBuilder, "In Line of Sight", $"{enemy.InLineOfSight}", Color.white, Color.yellow, true);
		if (_expandedEnemyInfo)
		{
			Dictionary<EBodyPart, EnemyPartDataClass>.ValueCollection values = enemy.Vision.VisionChecker.EnemyParts.Parts.Values;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (EnemyPartDataClass item in values)
			{
				if (item.TimeSinceLastVisionCheck > 2f)
				{
					num3++;
					continue;
				}
				num2++;
				if (item.LineOfSight)
				{
					num++;
				}
			}
			GClass1437.AppendLabeledValue(stringBuilder, "Body Parts", $"In LOS: {num} : Checked: {num2} : Not Checked: {num3}", Color.white, Color.yellow, true);
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine($"Heard [{enemy.Heard}] Recently? [{enemy.Status.HeardRecently}]");
		if (enemy.Heard && _expandedEnemyInfo)
		{
			addPlaceInfo(stringBuilder, enemy.KnownPlaces.LastHeardPlace, "Last Heard");
		}
	}

	private static float getPercentSpotted(Enemy enemy, out BodyPartType partType)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected I4, but got Unknown
		EnemyPartData value = enemy.EnemyInfo.BodyData().Value;
		float num = ((value != null) ? value.GetVisibilityLevel() : 0f);
		partType = (BodyPartType)1;
		foreach (KeyValuePair<EnemyPart, EnemyPartData> allActivePart in enemy.EnemyInfo.AllActiveParts)
		{
			float visibilityLevel = allActivePart.Value.GetVisibilityLevel();
			if (visibilityLevel > num)
			{
				num = visibilityLevel;
				partType = (BodyPartType)(int)allActivePart.Key.BodyPartType;
			}
		}
		return Mathf.Clamp(num.Round100(), 0f, 100f);
	}

	private static void addPlaceInfo(StringBuilder stringBuilder, EnemyPlace place, string name)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (place != null)
		{
			stringBuilder.AppendLine(name + " Data");
			GClass1437.AppendLabeledValue(stringBuilder, "Time Since Updated", $"{place.TimeSincePositionUpdated.Round100()}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Enemy Distance", $"{place.DistanceToEnemyRealPosition.Round100()}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Bot Distance", $"{place.DistanceToBot.Round100()}", Color.white, Color.yellow, true);
			GClass1437.AppendLabeledValue(stringBuilder, "Searched", $"Personal: {place.HasArrivedPersonal} / Squad: {place.HasArrivedSquad}", Color.white, Color.yellow, true);
			stringBuilder.AppendLine();
		}
	}
}
