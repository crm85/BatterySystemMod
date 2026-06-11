using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Comfort.Common;
using EFT;
using EFT.Communications;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.Preset;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.Personalities;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Talk;

public class EnemyTalk : BotBase
{
	private float _nextCheckTime;

	private float _randomizationFactor = 1f;

	private float FakeDeathChance = 2f;

	private bool CanTaunt = true;

	private bool CanFakeDeath = false;

	private bool CanBegForLife = false;

	private bool _canRespondToEnemy = true;

	private float TauntDist = 40f;

	private float TauntFreq = 30f;

	private float _nextGestureTime;

	private float _friendlyResponseFrequencyLimit = 1f;

	private float _friendlyResponseMinRandom = 0.33f;

	private float _friendlyResponseMaxRandom = 0.75f;

	private float _saySilenceTime;

	private float _beggingTimer;

	private bool _isBegging;

	private float _fakeDeathTimer = 0f;

	private float _begTimer = 0f;

	private static readonly EPhraseTrigger[] BegPhrases;

	private float _friendlyResponseDistance = 60f;

	private float _friendlyResponseDistanceAI = 35f;

	private float _friendlyResponseChance = 85f;

	private float _friendlyResponseChanceAI = 80f;

	private float _nextResponseTime;

	private float _tauntTimer = 0f;

	private PersonalityTalkSettings PersonalitySettings => base.Bot?.Info?.PersonalitySettings.Talk;

	private SAINSettingsClass FileSettings => base.Bot?.Info?.FileSettings;

	public bool IsBeggingForLife
	{
		get
		{
			if (_isBegging && _beggingTimer < Time.time)
			{
				_isBegging = false;
			}
			return _isBegging;
		}
		private set
		{
			if (value)
			{
				_beggingTimer = Time.time + 60f;
			}
			_isBegging = value;
		}
	}

	public EnemyTalk(BotComponent bot)
		: base(bot)
	{
		_randomizationFactor = Random.Range(0.75f, 1.25f);
	}

	public override void Init()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (Singleton<BotEventHandler>.Instance != null)
		{
			Singleton<BotEventHandler>.Instance.OnGrenadeExplosive += new GDelegate17(tryFakeDeathGrenade);
		}
		BotManagerComponent.Instance.BotHearing.PlayerTalk += playerTalked;
		base.Bot.EnemyController.Events.OnEnemyKilled += enemyKilled;
		base.Init();
	}

	private void enemyKilled(Player player)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (base.Bot.Talk.CanTalk && CanTaunt && EFTMath.RandomBool(70f))
		{
			EPhraseTrigger phrase = ((EFTMath.RandomBool(15f) || ((int)base.Bot.Memory.Health.HealthStatus == 1024 && EFTMath.RandomBool())) ? ((EPhraseTrigger)66) : ((!EFTMath.RandomBool(10f)) ? ((EPhraseTrigger)27) : ((EPhraseTrigger)61)));
			base.Bot.Talk.Say(phrase, (ETagStatus)4);
		}
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		float time = Time.time;
		if (!(_nextCheckTime < time))
		{
			return;
		}
		if (ShallBegForLife())
		{
			_nextCheckTime = time + 1f;
			return;
		}
		if (base.Bot?.Enemy != null)
		{
			if (ShallFakeDeath())
			{
				_nextCheckTime = time + 15f;
				return;
			}
			if (CanTaunt && _tauntTimer < time)
			{
				float num = TauntFreq * Random.Range(0.5f, 1.5f);
				_tauntTimer = time;
				if (EFTMath.RandomBool(PersonalitySettings.TauntChance) && TauntEnemy())
				{
					_nextCheckTime = time + 1f;
					_tauntTimer += num;
				}
				_nextCheckTime = time + 0.1f;
				_tauntTimer += num / 3f;
				return;
			}
		}
		_nextCheckTime = time + 0.1f;
	}

	public override void Dispose()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (Singleton<BotEventHandler>.Instance != null)
		{
			Singleton<BotEventHandler>.Instance.OnGrenadeExplosive -= new GDelegate17(tryFakeDeathGrenade);
		}
		BotManagerComponent.Instance.BotHearing.PlayerTalk -= playerTalked;
		if (base.Bot?.EnemyController != null)
		{
			base.Bot.EnemyController.Events.OnEnemyKilled -= enemyKilled;
		}
		base.Dispose();
	}

	protected override void UpdatePresetSettings(SAINPresetClass preset)
	{
		if (PersonalitySettings != null && FileSettings != null)
		{
			CanFakeDeath = PersonalitySettings.CanFakeDeathRare;
			FakeDeathChance = PersonalitySettings.FakeDeathChance;
			CanBegForLife = PersonalitySettings.CanBegForLife;
			CanTaunt = PersonalitySettings.CanTaunt && FileSettings.Mind.BotTaunts;
			TauntDist = PersonalitySettings.TauntMaxDistance * _randomizationFactor;
			TauntFreq = PersonalitySettings.TauntFrequency * _randomizationFactor;
			_canRespondToEnemy = PersonalitySettings.CanRespondToEnemyVoice;
		}
		else
		{
			Logger.LogAndNotifyError("Personality settings or filesettings are null! Cannot Apply Settings!", (ENotificationDurationType)1);
		}
		TalkSettings talk = preset.GlobalSettings.Talk;
		_friendlyResponseChance = talk.FriendlyReponseChance;
		_friendlyResponseChanceAI = talk.FriendlyReponseChanceAI;
		_friendlyResponseDistance = talk.FriendlyReponseDistance;
		_friendlyResponseDistanceAI = talk.FriendlyReponseDistanceAI;
		_friendlyResponseFrequencyLimit = talk.FriendlyResponseFrequencyLimit;
		_friendlyResponseMinRandom = talk.FriendlyResponseMinRandomDelay;
		_friendlyResponseMaxRandom = talk.FriendlyResponseMaxRandomDelay;
	}

	private bool ShallFakeDeath()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		if (CanFakeDeath && EFTMath.RandomBool(FakeDeathChance) && base.Bot.Enemy != null && !base.Bot.Squad.BotInGroup && _fakeDeathTimer < Time.time && ((int)base.Bot.Memory.Health.HealthStatus == 8192 || (int)base.Bot.Memory.Health.HealthStatus == 4096))
		{
			Vector3 val = base.Bot.Enemy.EnemyPosition - base.BotOwner.Position;
			if (((Vector3)(ref val)).sqrMagnitude < 4900f)
			{
				_fakeDeathTimer = Time.time + 30f;
				base.Bot.Talk.Say((EPhraseTrigger)26);
				return true;
			}
		}
		return false;
	}

	private void tryFakeDeathGrenade(Vector3 grenadeExplosionPosition, string playerProfileID, bool isSmoke, float smokeRadius, float smokeLifeTime)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (CanFakeDeath && EFTMath.RandomBool(FakeDeathChance) && !isSmoke && base.Bot.Enemy != null && _fakeDeathTimer < Time.time && playerProfileID != base.Bot.ProfileId)
		{
			Vector3 val = grenadeExplosionPosition - base.Bot.Position;
			if (((Vector3)(ref val)).sqrMagnitude < 625f)
			{
				_fakeDeathTimer = Time.time + 30f;
				base.Bot.Talk.Say((EPhraseTrigger)26);
			}
		}
	}

	private bool ShallBegForLife()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (_begTimer > Time.time)
		{
			return false;
		}
		if (!EFTMath.RandomBool(25f))
		{
			_begTimer = Time.time + 10f;
			return false;
		}
		Vector3? currentTargetPosition = base.Bot.CurrentTargetPosition;
		if (!currentTargetPosition.HasValue)
		{
			_begTimer = Time.time + 10f;
			return false;
		}
		_begTimer = Time.time + 3f;
		if ((base.Bot.Info.Profile.IsPMC ? (!base.Bot.Squad.BotInGroup) : base.Bot.Info.Profile.IsScav) && CanBegForLife && (int)base.Bot.Memory.Health.HealthStatus != 1024)
		{
			Vector3 val = currentTargetPosition.Value - base.Bot.Position;
			if (((Vector3)(ref val)).sqrMagnitude < 2500f)
			{
				IsBeggingForLife = true;
				base.Bot.Talk.Say(GClass1835.PickRandom<EPhraseTrigger>((IReadOnlyList<EPhraseTrigger>)BegPhrases));
				return true;
			}
		}
		return false;
	}

	private bool TauntEnemy()
	{
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Enemy enemy = base.Bot.Enemy;
		if (!canTauntEnemy(enemy))
		{
			return false;
		}
		if (base.Bot.Info.PersonalitySettings.Talk.ConstantTaunt)
		{
			flag = EFTMath.RandomBool();
		}
		if (!flag && (enemy.IsVisible || enemy.TimeSinceSeen < 15f))
		{
			flag = enemy.EnemyLookingAtMe || base.Bot.Info.PersonalitySettings.Talk.FrequentTaunt;
		}
		if (!flag && enemy.TimeSinceLastKnownUpdated < 5f && EFTMath.RandomBool(5f))
		{
			flag = true;
		}
		if (flag)
		{
			if (!enemy.IsVisible && enemy.Seen)
			{
				if (enemy.TimeSinceSeen > 60f && EFTMath.RandomBool(10f) && base.Bot.Talk.Say((EPhraseTrigger)82, (ETagStatus)4, withGroupDelay: true))
				{
					return true;
				}
				if (enemy.TimeSinceSeen > 30f && EFTMath.RandomBool(20f) && base.Bot.Talk.Say((EPhraseTrigger)13, (ETagStatus)4, withGroupDelay: true))
				{
					return true;
				}
			}
			EPhraseTrigger? val = tauntTrigger();
			if (!val.HasValue)
			{
				return false;
			}
			if (base.Bot.Talk.Say(val.Value, (ETagStatus)4))
			{
				return true;
			}
		}
		return false;
	}

	private bool canTauntEnemy(Enemy enemy)
	{
		if (enemy == null)
		{
			return false;
		}
		if (!enemy.Seen && !enemy.Heard)
		{
			return false;
		}
		if (enemy.KnownPlaces.EnemyDistanceFromLastKnown > TauntDist)
		{
			return false;
		}
		if (base.Bot.Decision.IsSearching)
		{
			return true;
		}
		if (!enemy.Status.ShotByEnemyRecently && !enemy.Status.ShotAtMeRecently)
		{
			if (!enemy.Seen)
			{
				return false;
			}
			if (enemy.TimeSinceSeen > 90f)
			{
				return false;
			}
			if (enemy.TimeSinceHeard > 20f)
			{
				return false;
			}
		}
		return true;
	}

	private IEnumerator RespondToFriendly(EPhraseTrigger trigger, ETagStatus mask, float delay, Player sourcePlayer, float chance = 100f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!EFTMath.RandomBool(chance))
		{
			yield break;
		}
		yield return (object)new WaitForSeconds(delay);
		if ((Object)(object)sourcePlayer == (Object)null || (Object)(object)base.BotOwner == (Object)null || (Object)(object)base.Player == (Object)null || (Object)(object)base.Bot == (Object)null || !sourcePlayer.HealthController.IsAlive || !base.Player.HealthController.IsAlive || base.Bot.Talk.IsSpeaking)
		{
			yield break;
		}
		if (!base.BotOwner.Memory.IsPeace)
		{
			base.Bot.Talk.Say(trigger);
			yield break;
		}
		if (_nextGestureTime < Time.time)
		{
			_nextGestureTime = Time.time + 6f;
			base.Player.HandsController.ShowGesture((EInteraction)3);
			base.Bot.Steering.LookToPoint(sourcePlayer.Position + Vector3.up * 1.4f);
		}
		base.Bot.Talk.Say(trigger, mask);
	}

	private IEnumerator RespondToEnemy(EPhraseTrigger trigger, ETagStatus mask, float delay, Enemy enemy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(delay);
		if (enemy != null && enemy.CheckValid() && !((Object)(object)base.BotOwner == (Object)null) && !((Object)(object)base.Player == (Object)null) && !((Object)(object)base.Bot == (Object)null) && !base.Bot.Talk.IsSpeaking && base.Player.HealthController.IsAlive)
		{
			base.Bot.Talk.Say(trigger, mask, withGroupDelay: false, skipCheck: true);
		}
	}

	private EPhraseTrigger? tauntTrigger()
	{
		if (EFTMath.RandomBool(10f))
		{
			if (EFTMath.RandomBool(80f) && base.Bot.Talk.CanSay((EPhraseTrigger)61, withGroupDelay: false, skipCheck: false))
			{
				return (EPhraseTrigger)61;
			}
			if (EFTMath.RandomBool(20f) && base.Bot.Talk.CanSay((EPhraseTrigger)66, withGroupDelay: false, skipCheck: false))
			{
				return (EPhraseTrigger)66;
			}
		}
		if (base.Bot.Talk.CanSay((EPhraseTrigger)27, withGroupDelay: false, skipCheck: false))
		{
			return (EPhraseTrigger)27;
		}
		return null;
	}

	public void SetEnemyTalk(Enemy enemy)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (_canRespondToEnemy && _nextResponseTime < Time.time && EFTMath.RandomBool(60f))
		{
			EPhraseTrigger? val = tauntTrigger();
			if (val.HasValue)
			{
				_nextResponseTime = Time.time + 2f;
				((MonoBehaviour)base.Bot).StartCoroutine(RespondToEnemy(val.Value, (ETagStatus)4, Random.Range(0.4f, 0.75f), enemy));
			}
		}
	}

	private void playerTalked(EPhraseTrigger phrase, ETagStatus mask, Player player)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and I4
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Invalid comparison between Unknown and I4
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Invalid comparison between Unknown and I4
		if ((Object)(object)base.Bot == (Object)null || !base.Bot.BotActive || (Object)(object)player == (Object)null || base.Bot.ProfileId == player.ProfileId)
		{
			return;
		}
		bool flag = (int)phrase == 9 || (int)phrase == 15;
		float num = 50f;
		float num2 = (player.HeavyBreath ? 35f : 15f);
		Enemy enemy = base.Bot.EnemyController.GetEnemy(player.ProfileId, mustBeActive: true);
		if (enemy == null)
		{
			if (!flag && (int)phrase != 29 && (int)phrase != 27)
			{
				SetFriendlyTalked(player);
			}
			return;
		}
		if (flag)
		{
			if (enemy.RealDistance <= num)
			{
				Vector3 position = randomizePos(player.Position, enemy.RealDistance);
				SAINHearingReport heard = new SAINHearingReport
				{
					position = position,
					soundType = SAINSoundType.Pain,
					placeType = EEnemyPlaceType.Hearing,
					isDanger = (enemy.RealDistance < 25f || enemy.InLineOfSight),
					shallReportToSquad = true
				};
				enemy.Hearing.SetHeard(heard);
			}
			return;
		}
		if ((int)phrase == 29)
		{
			if (enemy.RealDistance <= num2)
			{
				Vector3 position2 = randomizePos(player.Position, enemy.RealDistance);
				SAINHearingReport heard2 = new SAINHearingReport
				{
					position = position2,
					soundType = SAINSoundType.Breathing,
					placeType = EEnemyPlaceType.Hearing,
					isDanger = (enemy.RealDistance < 25f || enemy.InLineOfSight),
					shallReportToSquad = true
				};
				enemy.Hearing.SetHeard(heard2);
			}
			return;
		}
		if (enemy.RealDistance <= 65f)
		{
			Vector3 position3 = randomizePos(player.Position, enemy.RealDistance);
			SAINHearingReport heard3 = new SAINHearingReport
			{
				position = position3,
				soundType = SAINSoundType.Conversation,
				placeType = EEnemyPlaceType.Hearing,
				isDanger = (enemy.RealDistance < 25f || enemy.InLineOfSight),
				shallReportToSquad = true
			};
			enemy.Hearing.SetHeard(heard3);
		}
		if (!base.Bot.Talk.IsSpeaking && !(enemy.RealDistance > TauntDist) && ((int)phrase == 27 || (int)phrase == 104))
		{
			SetEnemyTalk(enemy);
		}
	}

	private Vector3 randomizePos(Vector3 position, float distance, float dispersionFactor = 20f)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float num = distance / dispersionFactor;
		Vector3 val = Random.insideUnitSphere * num;
		val.y = 0f;
		return position + val;
	}

	public bool ShallBeChatty()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Invalid comparison between Unknown and I4
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Invalid comparison between Unknown and I4
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Invalid comparison between Unknown and I4
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Invalid comparison between Unknown and I4
		if (base.Bot.Info.Profile.IsScav)
		{
			return SAINPlugin.LoadedPreset.GlobalSettings.Talk.TalkativeScavs;
		}
		if (base.Bot.Info.Profile.IsPMC)
		{
			return SAINPlugin.LoadedPreset.GlobalSettings.Talk.TalkativePMCs;
		}
		WildSpawnType wildSpawnType = base.Bot.Info.Profile.WildSpawnType;
		if (base.Bot.Info.Profile.IsBoss || base.Bot.Info.Profile.IsFollower)
		{
			if ((int)wildSpawnType == 26 || (int)wildSpawnType == 28 || (int)wildSpawnType == 27)
			{
				return SAINPlugin.LoadedPreset.GlobalSettings.Talk.TalkativeGoons;
			}
			return SAINPlugin.LoadedPreset.GlobalSettings.Talk.TalkativeBosses;
		}
		if ((int)wildSpawnType == 9 || (int)wildSpawnType == 24)
		{
			return SAINPlugin.LoadedPreset.GlobalSettings.Talk.TalkativeRaidersRogues;
		}
		return false;
	}

	public void SetFriendlyTalked(Player player)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float num = (player.IsAI ? _friendlyResponseDistanceAI : _friendlyResponseDistance);
		Vector3 val = player.Position - base.Bot.Position;
		if (((Vector3)(ref val)).sqrMagnitude > num * num)
		{
			return;
		}
		if ((base.BotOwner.Memory.IsPeace || (base.Bot.Squad.HumanFriendClose && !player.IsAI)) && _nextResponseTime < Time.time)
		{
			_nextResponseTime = Time.time + _friendlyResponseFrequencyLimit;
			if (!player.IsAI || ShallBeChatty())
			{
				float chance = (player.IsAI ? _friendlyResponseChanceAI : _friendlyResponseChance);
				((MonoBehaviour)base.Bot).StartCoroutine(RespondToFriendly((EPhraseTrigger)104, (ETagStatus)(base.Bot.EnemyController.AtPeace ? 1 : 4), Random.Range(_friendlyResponseMinRandom, _friendlyResponseMaxRandom), player, chance));
			}
		}
		else if (base.Bot?.Squad.SquadInfo != null && base.Bot.Talk.GroupTalk.FriendIsClose && (base.Bot.Squad.SquadInfo.SquadPersonality != ESquadPersonality.GigaChads || base.Bot.Squad.SquadInfo.SquadPersonality != ESquadPersonality.Elite) && (base.Bot.Info.Personality == EPersonality.GigaChad || base.Bot.Info.Personality == EPersonality.Chad) && _saySilenceTime < Time.time)
		{
			_saySilenceTime = Time.time + 20f;
			((MonoBehaviour)base.Bot).StartCoroutine(RespondToFriendly((EPhraseTrigger)39, (ETagStatus)(base.Bot.EnemyController.AtPeace ? 1 : 2), Random.Range(0.2f, 0.5f), player, 33f));
		}
	}

	static EnemyTalk()
	{
		EPhraseTrigger[] array = new EPhraseTrigger[3];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		BegPhrases = (EPhraseTrigger[])(object)array;
	}
}
