using System;
using System.Collections.Generic;
using EFT;
using EFT.HealthSystem;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.BotController.Classes;

public class Squad
{
	public enum ESearchPointType
	{
		Hearing,
		Flashlight
	}

	private const float SOUND_DIST_ALWAYS_DANGER = 25f;

	private const float SOUND_DIST_GUNSHOT_ALWAYS_DANGER = 100f;

	private BotsGroup _botsGroup;

	private float _recheckSquadTime;

	private float _checkSquadTime;

	private float _maxReportActionRangeSqr;

	public const float LEADER_KILL_COOLDOWN = 60f;

	public Dictionary<string, BotComponent> Members { get; } = new Dictionary<string, BotComponent>();

	public Dictionary<string, MemberInfo> MemberInfos { get; } = new Dictionary<string, MemberInfo>();

	public string Id { get; private set; } = string.Empty;

	public string GUID { get; } = Guid.NewGuid().ToString("N");

	public bool SquadReady { get; private set; }

	public ESquadPersonality SquadPersonality { get; private set; }

	public SquadPersonalitySettings SquadPersonalitySettings { get; private set; }

	public BotComponent LeaderComponent { get; private set; }

	public string LeaderId { get; private set; }

	public float LeaderPowerLevel { get; private set; }

	public bool LeaderIsDeadorNull
	{
		get
		{
			int result;
			if (!((Object)(object)LeaderComponent?.Player == (Object)null))
			{
				BotComponent leaderComponent = LeaderComponent;
				if (leaderComponent == null)
				{
					result = 0;
				}
				else
				{
					Player player = leaderComponent.Player;
					result = ((((player != null) ? new bool?(player.HealthController.IsAlive) : ((bool?)null)) == false) ? 1 : 0);
				}
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
	}

	public float TimeThatLeaderDied { get; private set; }

	public List<PlaceForCheck> GroupPlacesForCheck
	{
		get
		{
			BotsGroup botsGroup = BotsGroup;
			return (botsGroup != null) ? botsGroup.PlacesForCheck : null;
		}
	}

	public Dictionary<ESquadRole, BotComponent> Roles { get; } = new Dictionary<ESquadRole, BotComponent>();

	public Dictionary<string, PlaceForCheck> PlayerPlaceChecks { get; } = new Dictionary<string, PlaceForCheck>();

	public bool MemberIsFallingBack => MemberHasDecision(ECombatDecision.Retreat, ECombatDecision.RunAway, ECombatDecision.RunToCover);

	public bool MemberIsRegrouping => MemberHasDecision(ESquadDecision.Regroup);

	public float SquadPowerLevel
	{
		get
		{
			float num = 0f;
			foreach (MemberInfo value in MemberInfos.Values)
			{
				if ((Object)(object)value.Bot != (Object)null && !value.Bot.IsDead)
				{
					num += value.PowerLevel;
				}
			}
			return num;
		}
	}

	public BotsGroup BotsGroup
	{
		get
		{
			return _botsGroup;
		}
		private set
		{
			if (_botsGroup != value)
			{
				if (_botsGroup != null)
				{
					_botsGroup.OnMemberRemove -= removeMember;
				}
				if (value != null)
				{
					value.OnMemberRemove += removeMember;
				}
				_botsGroup = value;
			}
		}
	}

	public event Action<ECombatDecision, ESquadDecision, ESelfDecision, BotComponent> OnMemberDecisionMade;

	public event Action<EnemyPlace, Enemy, SAINSoundType> OnMemberHeardEnemy;

	public event Action<Squad> OnSquadEmpty;

	public event Action<IPlayer, DamageInfoStruct, float> LeaderKilled;

	public event Action<IPlayer, DamageInfoStruct, float> OnMemberKilled;

	public event Action<BotComponent, float> NewLeaderFound;

	public Squad()
	{
		_checkSquadTime = Time.time + 10f;
		PresetHandler.OnPresetUpdated += updateSettings;
		updateSettings(SAINPresetClass.Instance);
	}

	private void updateSettings(SAINPresetClass preset)
	{
		_maxReportActionRangeSqr = preset.GlobalSettings.Hearing.MaxRangeToReportEnemyActionNoHeadset.Sqr();
	}

	public void ReportEnemyPosition(Enemy reportedEnemy, EnemyPlace place, bool seen)
	{
		if (Members == null || Members.Count <= 1)
		{
			return;
		}
		float num = 3f;
		if (SquadPersonalitySettings != null)
		{
			num = SquadPersonalitySettings.CoordinationLevel;
			num = Mathf.Clamp(num, 1f, 5f);
		}
		float num2 = 25f;
		float chanceInPercent = num2 + num * 15f;
		foreach (BotComponent value in Members.Values)
		{
			if (EFTMath.RandomBool(chanceInPercent) && (Object)(object)value?.Player != (Object)null && (Object)(object)reportedEnemy.Player != (Object)null && (Object)(object)reportedEnemy.EnemyPlayer != (Object)null && reportedEnemy.Player.ProfileId != value.ProfileId)
			{
				value.EnemyController.GetEnemy(reportedEnemy.EnemyPlayer.ProfileId, mustBeActive: true)?.EnemyPositionReported(place, seen);
			}
		}
	}

	public string GetId()
	{
		if (GClass1437.IsNullOrEmpty(Id))
		{
			return GUID;
		}
		return Id;
	}

	public bool SquadIsSuppressEnemy(string profileId, out BotComponent suppressingMember)
	{
		foreach (KeyValuePair<string, BotComponent> member in Members)
		{
			Enemy enemy = member.Value?.Enemy;
			if ((Object)(object)enemy?.EnemyPlayer != (Object)null && enemy.EnemyPlayer.ProfileId == profileId && enemy.Status.EnemyIsSuppressed)
			{
				suppressingMember = member.Value;
				return true;
			}
		}
		suppressingMember = null;
		return false;
	}

	public void AddPointToSearch(Enemy Enemy, Vector3 EstimatedPosition, AISoundData Sound, BotComponent sain)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		AddPlaceForCheck(EstimatedPosition, Sound.SoundType, sain, Enemy, heard: true, CheckSoundIsDanger(Sound));
	}

	private static bool CheckSoundIsDanger(AISoundData sound)
	{
		if (sound.Enemy.InLineOfSight)
		{
			return true;
		}
		if (sound.PlayerDistance < 25f)
		{
			return true;
		}
		if (sound.IsGunShot && sound.PlayerDistance < 100f)
		{
			return true;
		}
		return false;
	}

	private void AddPlaceForCheck(Vector3 position, SAINSoundType soundType, BotComponent bot, Enemy enemy, bool heard, bool isDanger)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (BotsGroup == null)
		{
			BotsGroup = bot.BotOwner.BotsGroup;
		}
		position.y = enemy.EnemyPosition.y;
		SAINHearingReport heard2 = new SAINHearingReport
		{
			position = position,
			soundType = soundType,
			placeType = EEnemyPlaceType.Hearing,
			isDanger = isDanger,
			shallReportToSquad = true
		};
		EnemyPlace enemyPlace = enemy.Hearing.SetHeard(heard2);
		if (heard && enemyPlace != null)
		{
			this.OnMemberHeardEnemy?.Invoke(enemyPlace, enemy, soundType);
		}
	}

	public void AddPointToSearch(Vector3 position, float soundPower, BotComponent sain, AISoundType soundType, IPlayer player, ESearchPointType searchType = ESearchPointType.Hearing)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = sain.EnemyController.CheckAddEnemy(player);
		if (enemy != null)
		{
			Vector3 val = position - sain.Position;
			bool isDanger = ((Vector3)(ref val)).sqrMagnitude < 625f;
			AddPlaceForCheck(position, soundType.Convert(), sain, enemy, heard: false, isDanger);
		}
	}

	private PlaceForCheck addNewPlaceForCheck(BotOwner botOwner, Vector3 position, PlaceForCheckType checkType, IPlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		if (findNavMesh(position, out var hitPosition, 10f))
		{
			if (PlayerPlaceChecks.TryGetValue(player.ProfileId, out var value) && value != null)
			{
				Vector3 val = value.BasePoint - position;
				if (((Vector3)(ref val)).sqrMagnitude <= 2500f)
				{
					Vector3 position2 = (position2 = Vector3.Lerp(value.BasePoint, hitPosition, 0.5f));
					if (findNavMesh(position2, out hitPosition, 10f) && (int)canPathToPoint(hitPosition, botOwner) != 2)
					{
						GroupPlacesForCheck.Remove(value);
						PlaceForCheck val2 = new PlaceForCheck(hitPosition, checkType);
						GroupPlacesForCheck.Add(val2);
						PlayerPlaceChecks[player.ProfileId] = val2;
						calcGoalForBot(botOwner);
						return val2;
					}
				}
			}
			if ((int)canPathToPoint(hitPosition, botOwner) != 2)
			{
				PlaceForCheck val3 = new PlaceForCheck(position, checkType);
				GroupPlacesForCheck.Add(val3);
				AddOrUpdatePlaceForPlayer(val3, player);
				calcGoalForBot(botOwner);
				return val3;
			}
		}
		return null;
	}

	private bool findNavMesh(Vector3 position, out Vector3 hitPosition, float navSampleDist = 2f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(position, ref val, navSampleDist, -1))
		{
			hitPosition = ((NavMeshHit)(ref val)).position;
			return true;
		}
		hitPosition = Vector3.zero;
		return false;
	}

	private NavMeshPathStatus canPathToPoint(Vector3 point, BotOwner botOwner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		NavMeshPath val = new NavMeshPath();
		NavMesh.CalculatePath(botOwner.Position, point, -1, val);
		return val.status;
	}

	private void calcGoalForBot(BotOwner botOwner)
	{
		try
		{
			if (!botOwner.Memory.GoalTarget.HavePlaceTarget() && botOwner.Memory.GoalEnemy == null)
			{
				botOwner.BotsGroup.CalcGoalForBot(botOwner);
			}
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
	}

	private void AddOrUpdatePlaceForPlayer(PlaceForCheck place, IPlayer player)
	{
		string profileId = player.ProfileId;
		if (PlayerPlaceChecks.ContainsKey(profileId))
		{
			PlayerPlaceChecks[profileId] = place;
			return;
		}
		player.OnIPlayerDeadOrUnspawn += clearPlayerPlace;
		PlayerPlaceChecks.Add(profileId, place);
	}

	private void clearPlayerPlace(IPlayer player)
	{
		if (player == null)
		{
			return;
		}
		player.OnIPlayerDeadOrUnspawn -= clearPlayerPlace;
		string profileId = player.ProfileId;
		if (!PlayerPlaceChecks.ContainsKey(profileId))
		{
			return;
		}
		GroupPlacesForCheck.Remove(PlayerPlaceChecks[profileId]);
		PlayerPlaceChecks.Remove(profileId);
		foreach (BotComponent value in Members.Values)
		{
			if (!((Object)(object)value != (Object)null) || !((Object)(object)value.BotOwner != (Object)null))
			{
				continue;
			}
			try
			{
				BotsGroup botsGroup = BotsGroup;
				if (botsGroup != null)
				{
					botsGroup.CalcGoalForBot(value.BotOwner);
				}
			}
			catch
			{
			}
		}
	}

	public bool MemberHasDecision(params ECombatDecision[] decisionsToCheck)
	{
		foreach (MemberInfo value in MemberInfos.Values)
		{
			if (value == null || !((Object)(object)value.Bot != (Object)null))
			{
				continue;
			}
			ECombatDecision soloDecision = value.SoloDecision;
			foreach (ECombatDecision eCombatDecision in decisionsToCheck)
			{
				if (eCombatDecision == soloDecision)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool MemberHasDecision(params ESquadDecision[] decisionsToCheck)
	{
		foreach (MemberInfo value in MemberInfos.Values)
		{
			if (value == null || !((Object)(object)value.Bot != (Object)null))
			{
				continue;
			}
			ESquadDecision squadDecision = value.SquadDecision;
			foreach (ESquadDecision eSquadDecision in decisionsToCheck)
			{
				if (eSquadDecision == squadDecision)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool MemberHasDecision(params ESelfDecision[] decisionsToCheck)
	{
		foreach (MemberInfo value in MemberInfos.Values)
		{
			if (value == null || !((Object)(object)value.Bot != (Object)null))
			{
				continue;
			}
			ESelfDecision selfDecision = value.SelfDecision;
			foreach (ESelfDecision eSelfDecision in decisionsToCheck)
			{
				if (eSelfDecision == selfDecision)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void getSquadPersonality()
	{
		SquadPersonality = SquadPersonalityManager.GetSquadPersonality(Members, out var settings);
		SquadPersonalitySettings = settings;
	}

	public void Update(float currentTime, float DeltaTime)
	{
		if (!SquadReady && _checkSquadTime < currentTime && Members.Count > 0)
		{
			SquadReady = true;
			findSquadLeader();
			_recheckSquadTime = currentTime + 10f;
			if (Members.Count > 1)
			{
				getSquadPersonality();
			}
		}
		if (!SquadReady || !(_recheckSquadTime < currentTime) || !LeaderIsDeadorNull)
		{
			return;
		}
		_recheckSquadTime = currentTime + 3f;
		if (TimeThatLeaderDied < currentTime + 60f)
		{
			findSquadLeader();
			return;
		}
		bool flag = true;
		foreach (MemberInfo value in MemberInfos.Values)
		{
			if (value.HasEnemy)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			findSquadLeader();
		}
	}

	public void Dispose()
	{
		if (MemberInfos.Count > 0)
		{
			foreach (string key in MemberInfos.Keys)
			{
				RemoveMember(key);
			}
		}
		if (BotsGroup != null)
		{
			BotsGroup.OnMemberRemove -= removeMember;
		}
		PresetHandler.OnPresetUpdated -= updateSettings;
		MemberInfos.Clear();
		Members.Clear();
	}

	private bool isInCommunicationRange(BotComponent a, BotComponent b)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)a != (Object)null && (Object)(object)b != (Object)null)
		{
			if (a.PlayerComponent.Equipment.GearInfo.HasEarPiece && b.PlayerComponent.Equipment.GearInfo.HasEarPiece)
			{
				return true;
			}
			Vector3 val = a.Position - b.Position;
			if (((Vector3)(ref val)).sqrMagnitude <= _maxReportActionRangeSqr)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateSharedEnemyStatus(IPlayer player, EEnemyAction action, BotComponent sain, SAINSoundType soundType, Vector3 position)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)sain == (Object)null)
		{
			return;
		}
		foreach (BotComponent value in Members.Values)
		{
			if ((Object)(object)value == (Object)null || value.ProfileId == sain.ProfileId || !isInCommunicationRange(sain, value))
			{
				continue;
			}
			Enemy enemy = value.EnemyController.CheckAddEnemy(player);
			if (enemy != null)
			{
				SAINHearingReport heard = new SAINHearingReport
				{
					position = position,
					soundType = soundType,
					placeType = EEnemyPlaceType.Hearing,
					isDanger = enemy.InLineOfSight,
					shallReportToSquad = false
				};
				enemy.Hearing.SetHeard(heard);
				if (action != EEnemyAction.None)
				{
					enemy.Status.SetVulnerableAction(action);
				}
			}
		}
	}

	private void memberWasKilled(Player player, IPlayer lastAggressor, DamageInfoStruct lastDamageInfoStruct, EBodyPart lastBodyPart)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode)
		{
			Logger.LogInfo("Member [" + ((player != null) ? player.Profile.Nickname : null) + "] was killed for Squad: [" + Id + "] by [" + ((lastAggressor != null) ? lastAggressor.Profile.Nickname : null) + "] " + $"at Time: [{Time.time}] " + $"by damage type: [{lastDamageInfoStruct.DamageType}] " + $"to Body part: [{lastBodyPart}]");
		}
		this.OnMemberKilled?.Invoke(lastAggressor, lastDamageInfoStruct, Time.time);
		if (MemberInfos.TryGetValue((player != null) ? player.ProfileId : null, out var value) && value != null && value.ProfileId == LeaderId)
		{
			if (SAINPlugin.DebugMode)
			{
				Logger.LogInfo("Leader [" + ((player != null) ? player.Profile.Nickname : null) + "] was killed for Squad: [" + Id + "]");
			}
			this.LeaderKilled?.Invoke(lastAggressor, lastDamageInfoStruct, Time.time);
			TimeThatLeaderDied = Time.time;
			LeaderComponent = null;
		}
		RemoveMember((player != null) ? player.ProfileId : null);
	}

	public void MemberExtracted(BotComponent sain)
	{
		if (SAINPlugin.DebugMode)
		{
			string[] obj = new string[5] { "Leader [", null, null, null, null };
			object obj2;
			if (sain == null)
			{
				obj2 = null;
			}
			else
			{
				Player player = sain.Player;
				obj2 = ((player != null) ? player.Profile.Nickname : null);
			}
			obj[1] = (string)obj2;
			obj[2] = "] Extracted for Squad: [";
			obj[3] = Id;
			obj[4] = "]";
			Logger.LogInfo(string.Concat(obj));
		}
		RemoveMember(sain?.ProfileId);
	}

	private void findSquadLeader()
	{
		float num = 0f;
		BotComponent botComponent = null;
		foreach (MemberInfo value in MemberInfos.Values)
		{
			if ((Object)(object)value.Bot == (Object)null || value.Bot.IsDead)
			{
				continue;
			}
			bool isBoss = value.Bot.Info.Profile.IsBoss;
			if (isBoss || value.PowerLevel > num)
			{
				num = value.PowerLevel;
				botComponent = value.Bot;
				if (isBoss)
				{
					break;
				}
			}
		}
		if ((Object)(object)botComponent != (Object)null)
		{
			assignSquadLeader(botComponent);
		}
	}

	private void assignSquadLeader(BotComponent sain)
	{
		if ((Object)(object)sain?.Player == (Object)null)
		{
			Logger.LogError("Tried to Assign Null SAIN Component or Player for Squad [" + Id + "], skipping");
			return;
		}
		LeaderComponent = sain;
		LeaderPowerLevel = sain.Info.Profile.PowerLevel;
		Player player = sain.Player;
		LeaderId = ((player != null) ? player.ProfileId : null);
		this.NewLeaderFound?.Invoke(sain, Time.time);
		if (SAINPlugin.DebugMode)
		{
			string[] obj = new string[7] { " Found New Leader. Name [", null, null, null, null, null, null };
			BotOwner botOwner = sain.BotOwner;
			object obj2;
			if (botOwner == null)
			{
				obj2 = null;
			}
			else
			{
				Profile profile = botOwner.Profile;
				obj2 = ((profile != null) ? profile.Nickname : null);
			}
			obj[1] = (string)obj2;
			obj[2] = "] for Squad: [";
			obj[3] = Id;
			obj[4] = "]";
			obj[5] = $" at Time: [{Time.time}]";
			obj[6] = $" Group Size: [{Members.Count}]";
			Logger.LogInfo(string.Concat(obj));
		}
	}

	public void AddMember(BotComponent bot)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		if ((Object)(object)bot?.Player != (Object)null && (Object)(object)bot.BotOwner != (Object)null && !Members.ContainsKey(bot.Person.ProfileId))
		{
			if (Members.Count == 0)
			{
				BotsGroup = bot.BotOwner.BotsGroup;
				Id = ((object)bot.Info.Profile.Side/*cast due to .constrained prefix*/).ToString() + "_" + GUID;
			}
			bot.Decision.DecisionManager.OnDecisionMade += memberMadeDecision;
			MemberInfo value = new MemberInfo(bot, this);
			MemberInfos.Add(bot.ProfileId, value);
			Members.Add(bot.ProfileId, bot);
			if (bot.Info.Profile.IsBoss)
			{
				assignSquadLeader(bot);
			}
			else if ((Object)(object)LeaderComponent != (Object)null && bot.Info.Profile.PowerLevel > LeaderPowerLevel && !LeaderComponent.Info.Profile.IsBoss)
			{
				assignSquadLeader(bot);
			}
			bot.Player.OnPlayerDead += new GDelegate69(memberWasKilled);
			if (Members.Count > 1)
			{
				getSquadPersonality();
			}
		}
	}

	private void memberMadeDecision(ECombatDecision solo, ESquadDecision squad, ESelfDecision self, BotComponent member)
	{
		this.OnMemberDecisionMade?.Invoke(solo, squad, self, member);
	}

	public void RemoveMember(BotComponent sain)
	{
		RemoveMember(sain?.ProfileId);
	}

	public void RemoveMember(string id)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		if (Members.ContainsKey(id))
		{
			Members.Remove(id);
		}
		if (MemberInfos.TryGetValue(id, out var value))
		{
			Player val = value.Bot?.Player;
			if ((Object)(object)val != (Object)null)
			{
				val.OnPlayerDead -= new GDelegate69(memberWasKilled);
			}
			value.Bot.Decision.DecisionManager.OnDecisionMade -= memberMadeDecision;
			value.Dispose();
			MemberInfos.Remove(id);
		}
		if (Members.Count == 0)
		{
			this.OnSquadEmpty?.Invoke(this);
		}
	}

	private void removeMember(BotOwner botOwner)
	{
		if (!((Object)(object)botOwner == (Object)null))
		{
			IHealthController healthController = botOwner.HealthController;
			if (healthController != null && healthController.IsAlive && Members.TryGetValue(botOwner.ProfileId, out var value) && (Object)(object)value != (Object)null)
			{
				value.Squad.RemoveFromSquad();
				RemoveMember(value);
			}
		}
	}
}
