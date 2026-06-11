using System;
using System.Collections.Generic;
using System.Text;
using SAIN.Helpers;
using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyKnownPlaces : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private readonly PlaceData _placeData;

	private float _nextTalkClearTime;

	private float _nextCheckSearchTime;

	private float _nextSortPlacesTime;

	private GUIObject debugLastKnown;

	private readonly Dictionary<EnemyPlace, GUIObject> _guiObjects = new Dictionary<EnemyPlace, GUIObject>();

	public EnemyPlace LastKnownPlace { get; private set; }

	public EnemyPlace LastSeenPlace { get; private set; }

	public EnemyPlace LastHeardPlace { get; private set; }

	public EnemyPlace LastSquadSeenPlace { get; private set; }

	public EnemyPlace LastSquadHeardPlace { get; private set; }

	public float TimeSinceLastKnownUpdated => (LastKnownPlace == null) ? float.MaxValue : (Time.time - TimeLastKnownUpdated);

	public Vector3? LastKnownPosition => LastKnownPlace?.Position;

	public Vector3? LastSeenPosition => LastSeenPlace?.Position;

	public Vector3? LastHeardPosition => LastHeardPlace?.Position;

	public float EnemyDistanceFromLastKnown
	{
		get
		{
			if (LastKnownPlace == null)
			{
				return float.MaxValue;
			}
			return LastKnownPlace.DistanceToEnemyRealPosition;
		}
	}

	public float BotDistanceFromLastKnown
	{
		get
		{
			if (LastKnownPlace == null)
			{
				return float.MaxValue;
			}
			return LastKnownPlace.DistanceToBot;
		}
	}

	public float EnemyDistanceFromLastSeen
	{
		get
		{
			if (LastSeenPlace == null)
			{
				return float.MaxValue;
			}
			return LastSeenPlace.DistanceToEnemyRealPosition;
		}
	}

	public float EnemyDistanceFromLastHeard
	{
		get
		{
			if (LastHeardPlace == null)
			{
				return float.MaxValue;
			}
			return LastHeardPlace.DistanceToEnemyRealPosition;
		}
	}

	public bool SearchedAllKnownLocations { get; private set; }

	public List<EnemyPlace> AllEnemyPlaces { get; } = new List<EnemyPlace>();

	public float TimeLastKnownUpdated { get; private set; } = -1000f;

	private void checkSearched()
	{
		if (!(_nextCheckSearchTime > Time.time))
		{
			_nextCheckSearchTime = Time.time + 0.25f;
			bool flag = true;
			if (LastKnownPlace != null && !LastKnownPlace.HasArrivedPersonal && !LastKnownPlace.HasArrivedSquad)
			{
				flag = false;
			}
			if (flag && !SearchedAllKnownLocations)
			{
				base.Enemy.Events.EnemyLocationsSearched();
			}
			SearchedAllKnownLocations = flag;
		}
	}

	public EnemyKnownPlaces(Enemy enemy)
		: base(enemy)
	{
		_placeData = new PlaceData
		{
			Enemy = enemy,
			Owner = enemy.Bot,
			IsAI = enemy.IsAI,
			OwnerID = enemy.Bot.ProfileId
		};
	}

	public override void Init()
	{
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		base.Init();
	}

	public override void ManualUpdate()
	{
		UpdatePlaces();
		if (base.Enemy.EnemyKnown)
		{
			checkSearched();
			if (base.Enemy.IsCurrentEnemy)
			{
				createDebug();
			}
		}
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		clearAllPlaces();
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = base.Enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(OnEnemyKnownChanged));
		foreach (KeyValuePair<EnemyPlace, GUIObject> guiObject in _guiObjects)
		{
			DebugGizmos.DestroyLabel(guiObject.Value);
		}
		_guiObjects?.Clear();
		base.Dispose();
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
		if (!known)
		{
			clearAllPlaces();
		}
	}

	private void clearAllPlaces()
	{
		AllEnemyPlaces.Clear();
		LastSeenPlace = null;
		LastHeardPlace = null;
		LastSquadSeenPlace = null;
		LastSquadHeardPlace = null;
		LastKnownPlace = null;
		TimeLastKnownUpdated = -1000f;
	}

	private void createDebug()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode)
		{
			EnemyPlace lastKnownPlace = LastKnownPlace;
			if (lastKnownPlace != null)
			{
				if (debugLastKnown == null)
				{
					debugLastKnown = DebugGizmos.CreateLabel(lastKnownPlace.Position, string.Empty);
					_guiObjects.Add(lastKnownPlace, debugLastKnown);
				}
				updateDebugString(lastKnownPlace, debugLastKnown);
			}
		}
		else if (debugLastKnown != null)
		{
			DebugGizmos.DestroyLabel(debugLastKnown);
			debugLastKnown = null;
		}
	}

	private void tryTalk()
	{
		if (_nextTalkClearTime < Time.time && base.Bot.Talk.GroupSay((EPhraseTrigger)(EFTMath.RandomBool(75f) ? 58 : 77), null, withGroupDelay: true, 75f))
		{
			_nextTalkClearTime = Time.time + 10f;
		}
	}

	public void SetPlaceAsSearched(EnemyPlace place)
	{
		tryTalk();
		if (place.PlaceData.OwnerID == base.Bot.ProfileId)
		{
			place.HasArrivedPersonal = true;
		}
		else
		{
			place.HasArrivedSquad = true;
		}
	}

	private void updateDebugString(EnemyPlace place, GUIObject obj)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		obj.WorldPos = place.Position;
		StringBuilder stringBuilder = obj.StringBuilder;
		stringBuilder.Clear();
		stringBuilder.AppendLine("Bot: " + ((Object)base.BotOwner).name);
		stringBuilder.AppendLine("Known Location of " + base.EnemyPlayer.Profile.Nickname);
		if (LastKnownPlace == place)
		{
			stringBuilder.AppendLine("Last Known Location.");
		}
		stringBuilder.AppendLine($"Time Since Position Updated: {place.TimeSincePositionUpdated}");
		stringBuilder.AppendLine($"Arrived? [{place.HasArrivedPersonal}]" + (place.HasArrivedPersonal ? $"Time Since Arrived: [{Time.time - place._timeArrivedPers}]" : string.Empty));
		stringBuilder.AppendLine($"Seen? [{place.HasSeenPersonal}]" + (place.HasSeenPersonal ? $"Time Since Seen: [{Time.time - place._timeSeenPers}]" : string.Empty));
	}

	public EnemyPlace UpdateSeenPlace(Vector3 position)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (LastSeenPlace == null)
		{
			LastSeenPlace = new EnemyPlace(_placeData, position, isDanger: true, EEnemyPlaceType.Vision, null)
			{
				HasSeenPersonal = true
			};
			addPlace(LastSeenPlace);
		}
		else
		{
			LastSeenPlace.UpdatePosition(position);
		}
		return LastSeenPlace;
	}

	private void addPlace(EnemyPlace place)
	{
		if (place != null)
		{
			SearchedAllKnownLocations = false;
			place.OnPositionUpdated += LastKnownPosUpdated;
			LastKnownPosUpdated(place);
			AllEnemyPlaces.Add(place);
		}
	}

	public void UpdateSquadSeenPlace(EnemyPlace place)
	{
		if (place != null && LastSquadSeenPlace != place)
		{
			RemovePlace(LastSquadSeenPlace);
			LastSquadSeenPlace = place;
			addPlace(place);
		}
	}

	public EnemyPlace UpdatePersonalHeardPosition(SAINHearingReport report)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (base.Enemy.IsVisible)
		{
		}
		EnemyPlace lastHeardPlace = LastHeardPlace;
		if (lastHeardPlace != null)
		{
			lastHeardPlace.IsDanger = report.isDanger;
			lastHeardPlace.SoundType = report.soundType;
			lastHeardPlace.HasArrivedPersonal = false;
			lastHeardPlace.HasArrivedSquad = false;
			lastHeardPlace.HasSeenPersonal = false;
			lastHeardPlace.HasSeenSquad = false;
			lastHeardPlace.UpdatePosition(report.position);
			return lastHeardPlace;
		}
		LastHeardPlace = new EnemyPlace(_placeData, report);
		addPlace(LastHeardPlace);
		return LastHeardPlace;
	}

	public void UpdateSquadHeardPlace(EnemyPlace place)
	{
		if (place != null && LastSquadHeardPlace != place)
		{
			RemovePlace(LastSquadHeardPlace);
			LastSquadHeardPlace = place;
			addPlace(place);
		}
	}

	private void UpdatePlaces()
	{
		if (_nextSortPlacesTime < Time.time)
		{
			_nextSortPlacesTime = Time.time + 0.5f;
			SortAndClearPlaces();
		}
	}

	private void RemovePlace(EnemyPlace place)
	{
		if (place != null)
		{
			place.OnPositionUpdated -= LastKnownPosUpdated;
			AllEnemyPlaces.Remove(place);
			if (LastKnownPlace != null && LastKnownPlace == place)
			{
				LastKnownPlace = null;
			}
			if (_guiObjects.ContainsKey(place))
			{
				DebugGizmos.DestroyLabel(_guiObjects[place]);
				_guiObjects.Remove(place);
			}
			place.Dispose();
		}
	}

	private void SortAndClearPlaces()
	{
		EnemyPlace lastSeenPlace = LastSeenPlace;
		if (lastSeenPlace != null && lastSeenPlace.ShallClear)
		{
			RemovePlace(LastSeenPlace);
			LastSeenPlace = null;
		}
		EnemyPlace lastHeardPlace = LastHeardPlace;
		if (lastHeardPlace != null && lastHeardPlace.ShallClear)
		{
			RemovePlace(LastHeardPlace);
			LastHeardPlace = null;
		}
		EnemyPlace lastSquadHeardPlace = LastSquadHeardPlace;
		if (lastSquadHeardPlace != null && lastSquadHeardPlace.ShallClear)
		{
			RemovePlace(LastSquadHeardPlace);
			LastSquadHeardPlace = null;
		}
		EnemyPlace lastSquadSeenPlace = LastSquadSeenPlace;
		if (lastSquadSeenPlace != null && lastSquadSeenPlace.ShallClear)
		{
			RemovePlace(LastSquadSeenPlace);
			LastSquadSeenPlace = null;
		}
		if (AllEnemyPlaces.Count > 0)
		{
			AllEnemyPlaces.RemoveAll((EnemyPlace x) => x == null);
			AllEnemyPlaces.Sort((EnemyPlace x, EnemyPlace y) => x.TimeSincePositionUpdated.CompareTo(y.TimeSincePositionUpdated));
		}
	}

	private void LastKnownPosUpdated(EnemyPlace place)
	{
		if (place != null)
		{
			SearchedAllKnownLocations = false;
			TimeLastKnownUpdated = Time.time;
			LastKnownPlace = place;
			base.Enemy.Events.LastKnownUpdated(place);
		}
	}
}
