using SAIN.Helpers;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Search;

public class SearchDeciderClass : BotSubClass<SAINSearchClass>
{
	private float _nextRecalcSearchTime;

	private float _nextCheckLootTime;

	private float _checkLootFreq = 1f;

	private float _searchLootChance = 40f;

	public SearchDeciderClass(SAINSearchClass searchClass)
		: base(searchClass)
	{
		base.CanEverTick = false;
	}

	public bool ShallStartSearch(Enemy enemy, out SearchReasonsStruct reasons)
	{
		calcSearchTime();
		reasons = default(SearchReasonsStruct);
		if (!WantToSearch(enemy, out reasons.WantSearchReasons))
		{
			reasons.NotSearchReason = SearchReasonsStruct.ENotSearchReason.DontWantTo;
			return false;
		}
		if (enemy.Events.OnSearch.Value)
		{
			if (base.BaseClass.PathFinder.TargetPlace == null)
			{
				reasons.NotSearchReason = SearchReasonsStruct.ENotSearchReason.NullTargetPlace;
				return false;
			}
			return true;
		}
		if (!base.BaseClass.PathFinder.HasPathToSearchTarget(enemy, out var failReason))
		{
			reasons.NotSearchReason = SearchReasonsStruct.ENotSearchReason.PathCalcFailed;
			reasons.PathCalcFailReason = failReason;
			return false;
		}
		return true;
	}

	private void calcSearchTime()
	{
		if (base.Bot.Decision.CurrentCombatDecision != ECombatDecision.Search && _nextRecalcSearchTime < Time.time)
		{
			_nextRecalcSearchTime = Time.time + 120f;
			base.Bot.Info.CalcTimeBeforeSearch();
		}
	}

	public bool WantToSearch(Enemy enemy, out SearchReasonsStruct.WantSearchReasonsStruct reasons)
	{
		reasons = default(SearchReasonsStruct.WantSearchReasonsStruct);
		if (enemy == null)
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.NullEnemy;
			return false;
		}
		EnemyPlace lastKnownPlace = enemy.KnownPlaces.LastKnownPlace;
		if (lastKnownPlace == null)
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.NullLastKnown;
			return false;
		}
		if (lastKnownPlace.HasArrivedPersonal || lastKnownPlace.HasArrivedSquad)
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.AlreadySearchedLastKnown;
			return false;
		}
		if (!enemy.Seen && !base.Bot.Info.PersonalitySettings.Search.WillSearchFromAudio)
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.WontSearchFromAudio;
			return false;
		}
		if (!canStartSearch(enemy, out reasons.CantStartReason))
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.CantStart;
			return false;
		}
		if (!shallSearch(enemy, out reasons.WantToSearchReason))
		{
			reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.ShallNotSearch;
			return false;
		}
		reasons.NotWantToSearchReason = SearchReasonsStruct.ENotWantToSearchReason.None;
		return true;
	}

	private bool shallSearch(Enemy enemy, out SearchReasonsStruct.EWantToSearchReason reason)
	{
		if (enemy.Hearing.EnemyHeardFromPeace && base.Bot.Info.PersonalitySettings.Search.HeardFromPeaceBehavior == EHeardFromPeaceBehavior.SearchNow)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.HeardFromPeaceSearchNow;
			return true;
		}
		if (ShallBeStealthyDuringSearch(enemy) && base.Bot.Decision.EnemyDecisions.TimeToUnfreeze > Time.time && enemy.TimeSinceLastKnownUpdated > 10f)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.BeingStealthy;
			return true;
		}
		float timeBeforeSearch = base.Bot.Info.TimeBeforeSearch;
		if (enemy.Events.OnSearch.Value)
		{
			return shallContinueSearch(enemy, timeBeforeSearch, out reason);
		}
		return shallBeginSearch(enemy, timeBeforeSearch, out reason);
	}

	public bool ShallBeStealthyDuringSearch(Enemy enemy)
	{
		if (!SAINPlugin.LoadedPreset.GlobalSettings.Mind.SneakyBots)
		{
			return false;
		}
		if (SAINPlugin.LoadedPreset.GlobalSettings.Mind.OnlySneakyPersonalitiesSneaky && !base.Bot.Info.PersonalitySettings.Search.Sneaky)
		{
			return false;
		}
		if (!enemy.Hearing.EnemyHeardFromPeace)
		{
			return false;
		}
		if (base.Bot.Info.PersonalitySettings.Search.HeardFromPeaceBehavior == EHeardFromPeaceBehavior.SearchNow)
		{
			return false;
		}
		float maximumDistanceToBeSneaky = SAINPlugin.LoadedPreset.GlobalSettings.Mind.MaximumDistanceToBeSneaky;
		return enemy.RealDistance < maximumDistanceToBeSneaky;
	}

	private bool shallBeginSearchCauseLooting(Enemy enemy)
	{
		if (!enemy.Status.EnemyIsLooting)
		{
			return false;
		}
		if (_nextCheckLootTime < Time.time)
		{
			_nextCheckLootTime = Time.time + _checkLootFreq;
			return EFTMath.RandomBool(_searchLootChance);
		}
		return false;
	}

	private bool shallBeginSearch(Enemy enemy, float timeBeforeSearch, out SearchReasonsStruct.EWantToSearchReason reason)
	{
		if (shallBeginSearchCauseLooting(enemy))
		{
			enemy.Status.SearchingBecauseLooting = true;
			reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_Looting;
			return true;
		}
		float powerLevel = base.Bot.Info.Profile.PowerLevel;
		if (enemy.EnemyPlayer.AIData.PowerOfEquipment < powerLevel * 0.5f)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_PowerLevel;
			return true;
		}
		if (enemy.Seen && enemy.TimeSinceSeen >= timeBeforeSearch)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_EnemyNotSeen;
			return true;
		}
		EnemyPlace lastSquadSeenPlace = enemy.KnownPlaces.LastSquadSeenPlace;
		if (lastSquadSeenPlace != null && lastSquadSeenPlace.TimeSincePositionUpdated >= timeBeforeSearch)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_EnemyNotSeen_Squad;
			return true;
		}
		if (base.Bot.Info.PersonalitySettings.Search.WillSearchFromAudio)
		{
			if (enemy.Heard && enemy.TimeSinceHeard >= timeBeforeSearch)
			{
				reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_EnemyNotHeard;
				return true;
			}
			EnemyPlace lastSquadHeardPlace = enemy.KnownPlaces.LastSquadHeardPlace;
			if (lastSquadHeardPlace != null && lastSquadHeardPlace.TimeSincePositionUpdated >= timeBeforeSearch)
			{
				reason = SearchReasonsStruct.EWantToSearchReason.NewSearch_EnemyNotHeard_Squad;
				return true;
			}
		}
		reason = SearchReasonsStruct.EWantToSearchReason.None;
		return false;
	}

	private bool canStartSearch(Enemy enemy, out SearchReasonsStruct.ECantStartReason reason)
	{
		PersonalitySearchSettings search = base.Bot.Info.PersonalitySettings.Search;
		if (!search.WillSearchForEnemy)
		{
			reason = SearchReasonsStruct.ECantStartReason.WontSearchForEnemy;
			return false;
		}
		if (base.Bot.Suppression.IsHeavySuppressed)
		{
			reason = SearchReasonsStruct.ECantStartReason.Suppressed;
			return false;
		}
		if (enemy.IsVisible)
		{
			reason = SearchReasonsStruct.ECantStartReason.EnemyVisible;
			return false;
		}
		reason = SearchReasonsStruct.ECantStartReason.None;
		return true;
	}

	private bool shallContinueSearch(Enemy enemy, float timeBeforeSearch, out SearchReasonsStruct.EWantToSearchReason reason)
	{
		if (enemy.Status.SearchingBecauseLooting)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_Looting;
			return true;
		}
		float powerLevel = base.Bot.Info.Profile.PowerLevel;
		if (enemy.EnemyPlayer.AIData.PowerOfEquipment < powerLevel * 0.5f)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_PowerLevel;
			return true;
		}
		timeBeforeSearch = Mathf.Clamp(timeBeforeSearch / 3f, 0f, 120f);
		if (enemy.Seen && enemy.TimeSinceSeen >= timeBeforeSearch)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_EnemyNotSeen_Personal;
			return true;
		}
		EnemyPlace lastSquadSeenPlace = enemy.KnownPlaces.LastSquadSeenPlace;
		if (lastSquadSeenPlace != null && lastSquadSeenPlace.TimeSincePositionUpdated >= timeBeforeSearch)
		{
			reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_EnemyNotSeen_Squad;
			return true;
		}
		if (base.Bot.Info.PersonalitySettings.Search.WillSearchFromAudio)
		{
			if (enemy.Heard)
			{
				reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_EnemyNotHeard;
				return true;
			}
			EnemyPlace lastSquadHeardPlace = enemy.KnownPlaces.LastSquadHeardPlace;
			if (lastSquadHeardPlace != null)
			{
				reason = SearchReasonsStruct.EWantToSearchReason.ContinueSearch_EnemyNotHeard_Squad;
				return true;
			}
		}
		reason = SearchReasonsStruct.EWantToSearchReason.None;
		return false;
	}
}
