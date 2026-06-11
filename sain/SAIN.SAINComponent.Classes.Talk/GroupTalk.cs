using System;
using System.Collections.Generic;
using EFT;
using SAIN.BotController.Classes;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Talk;

public class GroupTalk : BotBase
{
	private float _nextRandomTalkTime;

	private float _nextsayNeedSniperTime;

	private float _needSniperFreq = 60f;

	private float _needSniperChance = 50f;

	private float _groupTalkFreq = 0.5f;

	private readonly List<EPhraseTrigger> LootPhrases = new List<EPhraseTrigger>
	{
		(EPhraseTrigger)94,
		(EPhraseTrigger)96,
		(EPhraseTrigger)65,
		(EPhraseTrigger)103
	};

	private readonly List<EPhraseTrigger> reloadPhrases = new List<EPhraseTrigger>
	{
		(EPhraseTrigger)23,
		(EPhraseTrigger)87,
		(EPhraseTrigger)20
	};

	private float _nextReportReloadTime;

	private float _nextCheckEnemyHPTime;

	private bool _friendIsClose;

	private float _nextCheckFriendsTime;

	private float CheckFriendliesTimer = 0f;

	private float EnemyPosTimer = 0f;

	private float _nextCheckTalkRetreatTime;

	private float _underFireNeedHelpTime;

	private float _hearNoiseTime;

	private float _leaderCommandTime = 0f;

	private float _leadTime = 0f;

	private float TalkTimer = 0f;

	private float HurtTalkTimer = 0f;

	private bool Subscribed = false;

	private float _reportReloadingChance = 33f;

	private float _reportReloadingFreq = 1f;

	private float _reportLostVisualChance = 40f;

	private float _reportRatChance = 33f;

	private float _reportRatTimeSinceSeen = 60f;

	private float _reportEnemyConversationChance = 10f;

	private float _reportEnemyMaxDist = 70f;

	private float _reportEnemyHealthChance = 40f;

	private float _reportEnemyHealthFreq = 8f;

	private float _reportEnemyKilledChance = 60f;

	private float _reportEnemyKilledSquadLeadChance = 60f;

	private bool _reportEnemyKilledToxicSquadLeader = false;

	private float _friendCloseDist = 40f;

	private float _reportFriendKilledChance = 60f;

	private float _talkRetreatChance = 60f;

	private float _talkRetreatFreq = 10f;

	private EPhraseTrigger _talkRetreatTrigger = (EPhraseTrigger)30;

	private ETagStatus _talkRetreatMask = (ETagStatus)4;

	private bool _talkRetreatGroupDelay = true;

	private float _underFireNeedHelpChance = 45f;

	private EPhraseTrigger _underFireNeedHelpTrigger = (EPhraseTrigger)88;

	private ETagStatus _underFireNeedHelpMask = (ETagStatus)4;

	private bool _underFireNeedHelpGroupDelay = true;

	private float _underFireNeedHelpFreq = 1f;

	private float _hearNoiseChance = 40f;

	private float _hearNoiseMaxDist = 70f;

	private float _hearNoiseFreq = 1f;

	private float _enemyLocationTalkChance = 60f;

	private float _enemyLocationTalkTimeSinceSeen = 3f;

	private float _enemyNeedHelpChance = 40f;

	private float _enemyLocationTalkFreq = 1f;

	private float _enemyLocationBehindAngle = 90f;

	private float _enemyLocationSideAngle = 45f;

	private float _enemyLocationFrontAngle = 90f;

	public bool FriendIsClose
	{
		get
		{
			if ((Object)(object)base.Player == (Object)null)
			{
				return false;
			}
			if (_nextCheckFriendsTime > Time.time)
			{
				return _friendIsClose;
			}
			_nextCheckFriendsTime = Time.time + 1f;
			updateFriendClose();
			return _friendIsClose;
		}
	}

	public SAINBotTalkClass LeaderComponent => base.Bot.Squad.LeaderComponent?.Talk;

	private float Randomized => Random.Range(0.75f, 1.25f);

	private SAINSquadClass BotSquad => base.Bot.Squad;

	public GroupTalk(BotComponent bot)
		: base(bot)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		_nextRandomTalkTime = Time.time + 15f;
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		if (!base.Bot.Talk.CanTalk)
		{
			return;
		}
		if (!BotSquad.BotInGroup || !base.Bot.Info.FileSettings.Mind.SquadTalk || SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching)
		{
			if (Subscribed)
			{
				unsub();
			}
			return;
		}
		if (!Subscribed)
		{
			sub();
		}
		if (!base.Bot.Talk.IsSpeaking)
		{
			CheckGroupTalk();
		}
	}

	private void CheckGroupTalk()
	{
		if (TalkTimer < Time.time)
		{
			TalkTimer = Time.time + _groupTalkFreq;
			if (FriendIsClose && (!base.Bot.Squad.IAmLeader || !UpdateLeaderCommand()) && !CheckEnemyContact() && !TalkEnemyLocation() && !ShallReportLostVisual() && !ShallReportNeedHelp())
			{
				randomTalk();
			}
		}
	}

	private void randomTalk()
	{
		if (_nextRandomTalkTime < Time.time && base.BotOwner.Memory.IsPeace && base.Bot.Talk.EnemyTalk.ShallBeChatty())
		{
			float num = Random.Range(60f, 240f);
			_nextRandomTalkTime = Time.time + num;
			base.Bot.Talk.Say((EPhraseTrigger)104);
		}
	}

	private void OnDecisionMade(ECombatDecision solo, ESquadDecision squad, ESelfDecision self, BotComponent me)
	{
		if (base.Bot.Talk.CanTalk && !base.Bot.Talk.IsSpeaking && (!base.Bot.Squad.IAmLeader || !(_leaderCommandTime < Time.time) || !LeaderMadeDecision(solo, squad)) && !TalkSelfDecision(self) && !TalkSoloDecision(solo))
		{
		}
	}

	private void OnMemberMadeDecision(ECombatDecision solo, ESquadDecision squad, ESelfDecision self, BotComponent member)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Squad.IAmLeader || !base.Bot.Talk.CanTalk || base.Bot.Talk.IsSpeaking || _leaderCommandTime > Time.time)
		{
			return;
		}
		_leaderCommandTime = Time.time + base.Bot.Info.FileSettings.Mind.SquadLeadTalkFreq;
		EPhraseTrigger val = (EPhraseTrigger)8;
		EPhraseTrigger phrase = (EPhraseTrigger)8;
		EInteraction val2 = (EInteraction)0;
		switch (solo)
		{
		case ECombatDecision.Retreat:
		case ECombatDecision.RunToCover:
		case ECombatDecision.RunAway:
			val2 = (EInteraction)7;
			val = (EPhraseTrigger)(EFTMath.RandomBool() ? 45 : 32);
			phrase = (EPhraseTrigger)67;
			break;
		case ECombatDecision.RushEnemy:
			val2 = (EInteraction)1;
			val = (EPhraseTrigger)34;
			phrase = (EPhraseTrigger)27;
			break;
		}
		if ((int)val == 8)
		{
			switch (squad)
			{
			case ESquadDecision.Suppress:
				val2 = (EInteraction)1;
				val = (EPhraseTrigger)43;
				phrase = (EPhraseTrigger)60;
				break;
			case ESquadDecision.PushSuppressedEnemy:
				val2 = (EInteraction)1;
				val = (EPhraseTrigger)34;
				phrase = (EPhraseTrigger)59;
				break;
			case ESquadDecision.Regroup:
				val2 = (EInteraction)7;
				val = (EPhraseTrigger)47;
				phrase = (EPhraseTrigger)67;
				break;
			}
		}
		if ((int)val == 8 || !base.Bot.Talk.GroupSay(val, (ETagStatus)4, withGroupDelay: false, 66f))
		{
			return;
		}
		if ((int)val2 != 0 && base.Bot.Squad.VisibleMembers.Count > 0)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null && !enemy.IsVisible)
			{
				base.Player.HandsController.ShowGesture(val2);
			}
		}
		member.Talk.Say(phrase, (ETagStatus)4);
	}

	private bool TalkSelfDecision(ESelfDecision self)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		switch (self)
		{
		case ESelfDecision.Reload:
			if (_nextReportReloadTime < Time.time && base.Bot.Talk.GroupSay(GClass1835.PickRandom<EPhraseTrigger>((IReadOnlyList<EPhraseTrigger>)reloadPhrases), null, withGroupDelay: false, _reportReloadingChance))
			{
				_nextReportReloadTime = Time.time + _reportReloadingFreq;
				return true;
			}
			break;
		case ESelfDecision.FirstAid:
		case ESelfDecision.Stims:
		case ESelfDecision.Surgery:
			if (base.Bot.Talk.GroupSay((EPhraseTrigger)30))
			{
				return true;
			}
			break;
		}
		return false;
	}

	private bool TalkSoloDecision(ECombatDecision solo)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (((uint)(solo - 1) <= 1u || solo == ECombatDecision.RunAway) && _nextCheckTalkRetreatTime < Time.time && base.Bot.HasEnemy && (base.Bot.Enemy.IsVisible || base.Bot.Enemy.InLineOfSight) && base.Bot.Talk.GroupSay(_talkRetreatTrigger, _talkRetreatMask, _talkRetreatGroupDelay, _talkRetreatChance))
		{
			_nextCheckTalkRetreatTime = Time.time + _talkRetreatFreq;
			return true;
		}
		return false;
	}

	private bool LeaderMadeDecision(ECombatDecision solo, ESquadDecision squad)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		EPhraseTrigger val = (EPhraseTrigger)8;
		EPhraseTrigger memberTrigger = (EPhraseTrigger)8;
		EInteraction gesture = (EInteraction)0;
		switch (squad)
		{
		case ESquadDecision.Search:
		case ESquadDecision.GroupSearch:
			gesture = (EInteraction)1;
			val = (EPhraseTrigger)31;
			memberTrigger = (EPhraseTrigger)59;
			break;
		case ESquadDecision.Help:
			gesture = (EInteraction)1;
			val = (EPhraseTrigger)34;
			memberTrigger = (EPhraseTrigger)59;
			break;
		case ESquadDecision.Suppress:
		case ESquadDecision.PushSuppressedEnemy:
			gesture = (EInteraction)1;
			val = (EPhraseTrigger)43;
			memberTrigger = (EPhraseTrigger)60;
			break;
		}
		switch (solo)
		{
		case ECombatDecision.HoldInCover:
			gesture = (EInteraction)2;
			val = (EPhraseTrigger)36;
			memberTrigger = (EPhraseTrigger)67;
			break;
		case ECombatDecision.Retreat:
			val = (EPhraseTrigger)40;
			memberTrigger = (EPhraseTrigger)(EFTMath.RandomBool() ? 68 : 38);
			break;
		case ECombatDecision.RushEnemy:
			gesture = (EInteraction)1;
			val = (EPhraseTrigger)34;
			memberTrigger = (EPhraseTrigger)27;
			break;
		}
		if ((int)val != 8 && checkLeaderTalk(gesture, val, memberTrigger))
		{
			return true;
		}
		return false;
	}

	private bool ShallReportLostVisual()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null && enemy.Vision.ShallReportLostVisual)
		{
			enemy.Vision.ShallReportLostVisual = false;
			if (EFTMath.RandomBool(_reportLostVisualChance))
			{
				ETagStatus val = (ETagStatus)(PersonIsClose(enemy.EnemyPlayer) ? 4 : 2);
				if (enemy.TimeSinceSeen > _reportRatTimeSinceSeen && EFTMath.RandomBool(_reportRatChance))
				{
					return base.Bot.Talk.GroupSay((EPhraseTrigger)82, null, withGroupDelay: false, 100f);
				}
				return base.Bot.Talk.GroupSay((EPhraseTrigger)77, null, withGroupDelay: false, 100f);
			}
		}
		return false;
	}

	private void EnemyConversation(EPhraseTrigger trigger, ETagStatus status, Player player)
	{
		if (!((Object)(object)player == (Object)null) && !base.Bot.Talk.IsSpeaking && !base.Bot.HasEnemy && FriendIsClose)
		{
			Enemy enemy = base.Bot.EnemyController.GetEnemy(player.ProfileId, mustBeActive: true);
			if (enemy != null && !(enemy.RealDistance > _reportEnemyMaxDist))
			{
				base.Bot.Talk.GroupSay((EPhraseTrigger)17, null, withGroupDelay: false, _reportEnemyConversationChance);
			}
		}
	}

	public void TalkEnemySniper()
	{
		if (FriendIsClose)
		{
			base.Bot.Talk.TalkAfterDelay((EPhraseTrigger)71, (ETagStatus)4, Random.Range(0.5f, 1f));
		}
	}

	public override void Dispose()
	{
		unsub();
		base.Dispose();
	}

	private void unsub()
	{
		if (Subscribed)
		{
			Subscribed = false;
			Squad squad = base.Bot?.Squad?.SquadInfo;
			if (squad != null)
			{
				squad.OnMemberKilled -= friendlyDown;
				squad.OnMemberHeardEnemy -= enemyHeard;
				squad.OnMemberDecisionMade -= OnMemberMadeDecision;
			}
			BotManagerComponent instance = BotManagerComponent.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				instance.BotHearing.PlayerTalk -= EnemyConversation;
			}
			if (base.Bot.EnemyController != null)
			{
				base.Bot.EnemyController.Events.OnEnemyKilled -= OnEnemyDown;
				base.Bot.EnemyController.Events.OnEnemyHealthChanged -= onEnemyHealthChanged;
			}
			base.BotOwner.DeadBodyWork.OnStartLookToBody -= OnLootBody;
			base.Bot.Decision.DecisionManager.OnDecisionMade -= OnDecisionMade;
			base.Bot.Memory.Health.HealthStatusChanged -= myHealthChanged;
		}
	}

	private void sub()
	{
		Squad squad = base.Bot?.Squad?.SquadInfo;
		if (!Subscribed && squad != null)
		{
			Subscribed = true;
			squad.OnMemberKilled += friendlyDown;
			squad.OnMemberHeardEnemy += enemyHeard;
			squad.OnMemberDecisionMade += OnMemberMadeDecision;
			BotManagerComponent.Instance.BotHearing.PlayerTalk += EnemyConversation;
			base.BotOwner.DeadBodyWork.OnStartLookToBody += OnLootBody;
			base.Bot.EnemyController.Events.OnEnemyKilled += OnEnemyDown;
			base.Bot.EnemyController.Events.OnEnemyHealthChanged += onEnemyHealthChanged;
			base.Bot.Decision.DecisionManager.OnDecisionMade += OnDecisionMade;
			base.Bot.Memory.Health.HealthStatusChanged += myHealthChanged;
		}
	}

	private void onEnemyHealthChanged(ETagStatus health, Enemy enemy)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		if (base.Bot.Talk.CanTalk && !base.Bot.Talk.IsSpeaking && enemy != null && enemy.IsCurrentEnemy && ((int)health == 8192 || (int)health == 4096) && EFTMath.RandomBool(_reportEnemyHealthChance) && _nextCheckEnemyHPTime < Time.time)
		{
			_nextCheckEnemyHPTime = Time.time + _reportEnemyHealthFreq;
			base.Bot.Talk.GroupSay((EPhraseTrigger)19, null, withGroupDelay: false, 100f);
		}
	}

	private bool CheckEnemyContact()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (FriendIsClose && enemy != null)
		{
			if (enemy.FirstContactOccured && !enemy.FirstContactReported)
			{
				enemy.FirstContactReported = true;
				if (EFTMath.RandomBool(40f))
				{
					ETagStatus value = (ETagStatus)(PersonIsClose(enemy.EnemyPlayer) ? 4 : 2);
					return base.Bot.Talk.GroupSay((EPhraseTrigger)12, value, withGroupDelay: true, 100f);
				}
			}
			if (enemy.Vision.ShallReportRepeatContact)
			{
				enemy.Vision.ShallReportRepeatContact = false;
				if (EFTMath.RandomBool(40f))
				{
					ETagStatus value2 = (ETagStatus)(PersonIsClose(enemy.EnemyPlayer) ? 4 : 2);
					return base.Bot.Talk.GroupSay((EPhraseTrigger)21, value2, withGroupDelay: false, 100f);
				}
			}
		}
		return false;
	}

	private void OnEnemyDown(Player player)
	{
		if (base.Bot.Talk.IsSpeaking)
		{
			return;
		}
		if (!_reportEnemyKilledToxicSquadLeader)
		{
			object obj;
			if (player == null)
			{
				obj = null;
			}
			else
			{
				Profile profile = player.Profile;
				if (profile == null)
				{
					obj = null;
				}
				else
				{
					InfoClass info = profile.Info;
					obj = ((info != null) ? info.Settings : null);
				}
			}
			ProfileInfoSettingsClass val = (ProfileInfoSettingsClass)obj;
			if (val == null || !base.BotOwner.BotsGroup.IsPlayerEnemy((IPlayer)(object)player))
			{
				return;
			}
		}
		if (FriendIsClose && PersonIsClose(player) && EFTMath.RandomBool(_reportEnemyKilledChance))
		{
			float num = Random.Range(0.2f, 0.6f);
			base.Bot.Talk.TalkAfterDelay((EPhraseTrigger)75, null, num);
			BotComponent botComponent = base.Bot.Squad.SquadInfo?.LeaderComponent;
			if (botComponent?.Person?.IPlayer != null && !base.Bot.Squad.IAmLeader && EFTMath.RandomBool(_reportEnemyKilledSquadLeadChance) && PersonIsClose(botComponent.Person.IPlayer))
			{
				botComponent.Talk.TalkAfterDelay((EPhraseTrigger)66, null, num + 0.75f);
			}
		}
	}

	private bool PersonIsClose(IPlayer player)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		int result;
		if (player != null && (Object)(object)base.BotOwner != (Object)null)
		{
			Vector3 val = player.Position - base.BotOwner.Position;
			result = ((((Vector3)(ref val)).magnitude < 30f) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private bool PersonIsClose(Player player)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int result;
		if ((Object)(object)player != (Object)null && (Object)(object)base.BotOwner != (Object)null)
		{
			Vector3 val = player.Position - base.BotOwner.Position;
			result = ((((Vector3)(ref val)).magnitude < 30f) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	private void updateFriendClose()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		float num = _friendCloseDist.Sqr();
		_friendIsClose = false;
		foreach (BotComponent value in base.Bot.Squad.Members.Values)
		{
			if ((Object)(object)value != (Object)null && !value.IsDead && value.Player.ProfileId != base.Player.ProfileId)
			{
				Vector3 val = value.Position - base.Bot.Position;
				if (((Vector3)(ref val)).sqrMagnitude < num)
				{
					_friendIsClose = true;
					break;
				}
			}
		}
		if (!_friendIsClose && base.Bot.Squad.HumanFriendClose)
		{
			_friendIsClose = true;
		}
	}

	private void friendlyDown(IPlayer player, DamageInfoStruct damage, float time)
	{
		if (base.Bot.Talk.CanTalk && !base.Bot.Talk.IsSpeaking && !base.BotOwner.IsDead && base.Bot.BotActive && EFTMath.RandomBool(_reportFriendKilledChance))
		{
			updateFriendClose();
			if (_friendIsClose && PersonIsClose(player))
			{
				base.Bot.Talk.TalkAfterDelay((EPhraseTrigger)14, (ETagStatus)4, Random.Range(0.33f, 0.66f));
			}
		}
	}

	private void OnLootBody(float num)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (base.Bot.BotActive && !base.Bot.Talk.IsSpeaking && FriendIsClose)
		{
			EPhraseTrigger phrase = GClass1835.PickRandom<EPhraseTrigger>((IReadOnlyList<EPhraseTrigger>)LootPhrases);
			base.Bot.Talk.Say(phrase, null, withGroupDelay: true);
		}
	}

	private void allMembersSay(EPhraseTrigger trigger, ETagStatus mask, EPhraseTrigger commandTrigger, float delay = 1.5f, float chance = 100f)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected I4, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.Bot.Squad.LeaderComponent == (Object)null)
		{
			return;
		}
		bool flag = false;
		foreach (BotComponent value in BotSquad.Members.Values)
		{
			if (!((Object)(object)value != (Object)null) || value.IsDead || value.Talk.IsSpeaking || !EFTMath.RandomBool(chance) || value.Squad.IAmLeader || !(value.Squad.DistanceToSquadLeader <= 40f))
			{
				continue;
			}
			flag = true;
			EPhraseTrigger phrase = trigger;
			switch (commandTrigger - 31)
			{
			case 1:
			case 5:
				if (value.Decision.CurrentSquadDecision == ESquadDecision.GroupSearch)
				{
					phrase = (EPhraseTrigger)62;
					break;
				}
				switch (value.Decision.CurrentCombatDecision)
				{
				case ECombatDecision.Search:
					phrase = (EPhraseTrigger)62;
					break;
				case ECombatDecision.HoldInCover:
					phrase = (EPhraseTrigger)(EFTMath.RandomBool() ? 67 : 64);
					break;
				case ECombatDecision.RushEnemy:
					phrase = (EPhraseTrigger)62;
					break;
				}
				break;
			case 0:
			case 3:
				if (value.Decision.CurrentSquadDecision == ESquadDecision.GroupSearch)
				{
					phrase = (EPhraseTrigger)(EFTMath.RandomBool() ? 63 : 59);
					break;
				}
				switch (value.Decision.CurrentCombatDecision)
				{
				case ECombatDecision.Search:
					phrase = (EPhraseTrigger)(EFTMath.RandomBool() ? 63 : 59);
					break;
				case ECombatDecision.HoldInCover:
					phrase = (EPhraseTrigger)(EFTMath.RandomBool() ? 62 : 60);
					break;
				case ECombatDecision.RushEnemy:
					phrase = (EPhraseTrigger)27;
					break;
				}
				break;
			}
			value.Talk.TalkAfterDelay(phrase, mask, delay * Random.Range(0.75f, 1.25f));
		}
		if (flag && !EFTMath.RandomBool(5f))
		{
		}
	}

	private bool UpdateLeaderCommand()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (LeaderComponent == null)
		{
			return false;
		}
		if (!BotSquad.IAmLeader)
		{
			return false;
		}
		if (_leadTime >= Time.time)
		{
			return false;
		}
		_leadTime = Time.time + Randomized * base.Bot.Info.FileSettings.Mind.SquadLeadTalkFreq;
		if (CheckIfLeaderShouldCommand())
		{
			return true;
		}
		if (CheckFriendliesTimer < Time.time && CheckFriendlyLocation(out var trigger) && base.Bot.Talk.Say(trigger))
		{
			CheckFriendliesTimer = Time.time + base.Bot.Info.FileSettings.Mind.SquadLeadTalkFreq * 5f;
			ETagStatus mask = (ETagStatus)((!EFTMath.RandomBool()) ? 1 : 2);
			allMembersSay((EPhraseTrigger)67, mask, trigger, Random.Range(0.65f, 1.25f), 50f);
			return true;
		}
		return false;
	}

	private void myHealthChanged(ETagStatus status)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Invalid comparison between Unknown and I4
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Invalid comparison between Unknown and I4
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Invalid comparison between Unknown and I4
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Invalid comparison between Unknown and I4
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Bot.Talk.CanTalk || base.Bot.Talk.IsSpeaking || (base.Bot.HasEnemy && base.Bot.Enemy.RealDistance < 30f) || !FriendIsClose || !(HurtTalkTimer < Time.time))
		{
			return;
		}
		if ((int)status == 8192 || (int)status == 4096)
		{
			BotFirstAidClass firstAid = base.BotOwner.Medecine.FirstAid;
			if (firstAid != null && !((GClass469)firstAid).HaveSmth2Use && base.Bot.Talk.Say((EPhraseTrigger)90, null, withGroupDelay: true))
			{
				HurtTalkTimer = Time.time + base.Bot.Info.FileSettings.Mind.SquadMemberTalkFreq * 5f * Random.Range(0.5f, 1.5f);
				return;
			}
		}
		EPhraseTrigger val = (EPhraseTrigger)8;
		if ((int)status != 2048)
		{
			if ((int)status != 4096)
			{
				if ((int)status != 8192)
				{
					return;
				}
				if (EFTMath.RandomBool(75f))
				{
					val = (EPhraseTrigger)56;
				}
			}
			else if (EFTMath.RandomBool(75f))
			{
				val = (EPhraseTrigger)(EFTMath.RandomBool() ? 53 : 55);
			}
		}
		else if (EFTMath.RandomBool(60f))
		{
			val = (EPhraseTrigger)(EFTMath.RandomBool() ? 84 : 53);
		}
		if ((int)val != 8 && base.Bot.Talk.Say(val))
		{
			HurtTalkTimer = Time.time + base.Bot.Info.FileSettings.Mind.SquadMemberTalkFreq * 5f * Random.Range(0.5f, 1.5f);
		}
	}

	private bool ShallReportNeedHelp()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (!FriendIsClose)
		{
			return false;
		}
		if (_underFireNeedHelpTime < Time.time && EFTMath.RandomBool(_underFireNeedHelpChance) && base.Bot.Enemy != null && base.BotOwner.Memory.IsUnderFire && base.Bot.Memory.LastUnderFireSource == base.Bot.Enemy.EnemyIPlayer)
		{
			_underFireNeedHelpTime = Time.time + _underFireNeedHelpFreq;
			return base.Bot.Talk.Say(_underFireNeedHelpTrigger, _underFireNeedHelpMask, _underFireNeedHelpGroupDelay);
		}
		return false;
	}

	private void enemyHeard(EnemyPlace place, Enemy enemy, SAINSoundType soundType)
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (base.Bot.Talk.CanTalk && !base.Bot.Talk.IsSpeaking)
		{
			float time = Time.time;
			if (base.Bot.BotActive && !(_hearNoiseTime > time) && (!base.Bot.HasEnemy || !(base.Bot.Enemy.TimeSinceSeen < 120f)) && base.Bot.Talk.GroupTalk.FriendIsClose && place != null && !soundType.IsGunShot() && !(enemy.RealDistance > _hearNoiseMaxDist) && EFTMath.RandomBool(_hearNoiseChance))
			{
				_hearNoiseTime = time + _hearNoiseFreq;
				EPhraseTrigger phrase = (EPhraseTrigger)((soundType == SAINSoundType.Conversation) ? 17 : 80);
				base.Bot.Talk.TalkAfterDelay(phrase, (ETagStatus)2, 0.33f);
			}
		}
	}

	public bool CheckIfLeaderShouldCommand()
	{
		if (_leaderCommandTime < Time.time)
		{
			if (base.BotOwner.DoorOpener.Interacting && EFTMath.RandomBool(33f) && checkLeaderTalk((EInteraction)0, (EPhraseTrigger)102, (EPhraseTrigger)67))
			{
				_leaderCommandTime = Time.time + base.Bot.Info.FileSettings.Mind.SquadLeadTalkFreq;
				return true;
			}
			if (_nextsayNeedSniperTime < Time.time)
			{
				Enemy enemy = base.Bot.Enemy;
				if (enemy != null && enemy.IsSniper && base.Bot.Talk.CanSay((EPhraseTrigger)86, withGroupDelay: true, skipCheck: false))
				{
					_nextsayNeedSniperTime = Time.time + _needSniperFreq;
					if (EFTMath.RandomBool(_needSniperChance) && base.Bot.Talk.Say((EPhraseTrigger)86, (ETagStatus)4, withGroupDelay: false, skipCheck: true))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool checkLeaderTalk(EInteraction gesture, EPhraseTrigger commandTrigger, EPhraseTrigger memberTrigger)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		int count = base.Bot.Squad.VisibleMembers.Count;
		int num;
		if ((int)gesture != 0 && count > 0)
		{
			Enemy enemy = base.Bot.Enemy;
			num = ((enemy != null && !enemy.IsVisible) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag = (byte)num != 0;
		if ((float)count / (float)base.Bot.Squad.Members.Count < 0.5f && base.Bot.Talk.GroupSay(commandTrigger, null, withGroupDelay: false, 100f))
		{
			_leaderCommandTime = Time.time + base.Bot.Info.FileSettings.Mind.SquadLeadTalkFreq;
			if (flag)
			{
				base.Player.HandsController.ShowGesture(gesture);
			}
			allMembersSay(memberTrigger, (ETagStatus)2, commandTrigger, Random.Range(0.75f, 1.5f), 35f);
			return true;
		}
		if (flag)
		{
			base.Player.HandsController.ShowGesture(gesture);
		}
		return false;
	}

	public bool TalkEnemyLocation()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (EnemyPosTimer < Time.time && base.Bot.Enemy != null)
		{
			EnemyPosTimer = Time.time + _enemyLocationTalkFreq;
			EPhraseTrigger trigger = (EPhraseTrigger)8;
			ETagStatus mask = (ETagStatus)2;
			Enemy enemy = base.Bot.Enemy;
			if (base.Bot.Enemy.IsVisible && enemy.EnemyLookingAtMe && EFTMath.RandomBool(_enemyNeedHelpChance))
			{
				mask = (ETagStatus)4;
				trigger = (EPhraseTrigger)((!base.Bot.Memory.Health.Healthy && !base.Bot.Memory.Health.Injured) ? 88 : 21);
			}
			else if ((enemy.IsVisible || (enemy.Seen && enemy.TimeSinceSeen < _enemyLocationTalkTimeSinceSeen)) && EFTMath.RandomBool(_enemyLocationTalkChance))
			{
				EnemyDirectionCheck(enemy.EnemyPosition, out trigger, out mask);
			}
			if ((int)trigger != 8)
			{
				return base.Bot.Talk.Say(trigger, mask, withGroupDelay: true);
			}
		}
		return false;
	}

	private bool EnemyDirectionCheck(Vector3 enemyPosition, out EPhraseTrigger trigger, out ETagStatus mask)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnemyInDirection(enemyPosition, 180f, AngleToDot(_enemyLocationBehindAngle)))
		{
			mask = (ETagStatus)2;
			trigger = (EPhraseTrigger)74;
			return true;
		}
		if (IsEnemyInDirection(enemyPosition, -90f, AngleToDot(_enemyLocationSideAngle)))
		{
			mask = (ETagStatus)2;
			trigger = (EPhraseTrigger)69;
			return true;
		}
		if (IsEnemyInDirection(enemyPosition, 90f, AngleToDot(_enemyLocationSideAngle)))
		{
			mask = (ETagStatus)2;
			trigger = (EPhraseTrigger)72;
			return true;
		}
		if (IsEnemyInDirection(enemyPosition, 0f, AngleToDot(_enemyLocationFrontAngle)))
		{
			mask = (ETagStatus)4;
			trigger = (EPhraseTrigger)73;
			return true;
		}
		trigger = (EPhraseTrigger)8;
		mask = (ETagStatus)1;
		return false;
	}

	private float AngleToRadians(float angle)
	{
		return angle * (float)Math.PI / 180f;
	}

	private float AngleToDot(float angle)
	{
		return Mathf.Cos(AngleToRadians(angle));
	}

	private bool CheckFriendlyLocation(out EPhraseTrigger trigger)
	{
		trigger = (EPhraseTrigger)8;
		Squad squadInfo = base.Bot.Squad.SquadInfo;
		if (squadInfo != null && squadInfo.MemberIsRegrouping)
		{
			trigger = (EPhraseTrigger)47;
			return true;
		}
		return false;
	}

	private bool IsEnemyInDirection(Vector3 enemyPosition, float angle, float threshold)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = enemyPosition - base.BotOwner.Transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 playerRealForward = base.Player.MovementContext.PlayerRealForward;
		Vector3 normalized2 = ((Vector3)(ref playerRealForward)).normalized;
		Vector3 val2 = Quaternion.Euler(0f, angle, 0f) * normalized2;
		return Vector3.Dot(normalized, val2) > threshold;
	}

	public void updateConfigSettings(SAINPresetClass preset)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		SquadTalkSettings squadTalkSettings = SAINPlugin.LoadedPreset?.GlobalSettings?.SquadTalk;
		if (squadTalkSettings != null)
		{
			_reportReloadingChance = squadTalkSettings._reportReloadingChance;
			_reportReloadingFreq = squadTalkSettings._reportReloadingFreq;
			_reportLostVisualChance = squadTalkSettings._reportLostVisualChance;
			_reportRatChance = squadTalkSettings._reportRatChance;
			_reportRatTimeSinceSeen = squadTalkSettings._reportRatTimeSinceSeen;
			_reportEnemyConversationChance = squadTalkSettings._reportEnemyConversationChance;
			_reportEnemyMaxDist = squadTalkSettings._reportEnemyMaxDist;
			_reportEnemyHealthChance = squadTalkSettings._reportEnemyHealthChance;
			_reportEnemyHealthFreq = squadTalkSettings._reportEnemyHealthFreq;
			_reportEnemyKilledChance = squadTalkSettings._reportEnemyKilledChance;
			_reportEnemyKilledSquadLeadChance = squadTalkSettings._reportEnemyKilledSquadLeadChance;
			_reportEnemyKilledToxicSquadLeader = squadTalkSettings._reportEnemyKilledToxicSquadLeader;
			_friendCloseDist = squadTalkSettings._friendCloseDist;
			_reportFriendKilledChance = squadTalkSettings._reportFriendKilledChance;
			_talkRetreatChance = squadTalkSettings._talkRetreatChance;
			_talkRetreatFreq = squadTalkSettings._talkRetreatFreq;
			_talkRetreatTrigger = squadTalkSettings._talkRetreatTrigger;
			_talkRetreatMask = squadTalkSettings._talkRetreatMask;
			_talkRetreatGroupDelay = squadTalkSettings._talkRetreatGroupDelay;
			_underFireNeedHelpChance = squadTalkSettings._underFireNeedHelpChance;
			_underFireNeedHelpTrigger = squadTalkSettings._underFireNeedHelpTrigger;
			_underFireNeedHelpMask = squadTalkSettings._underFireNeedHelpMask;
			_underFireNeedHelpGroupDelay = squadTalkSettings._underFireNeedHelpGroupDelay;
			_underFireNeedHelpFreq = squadTalkSettings._underFireNeedHelpFreq;
			_hearNoiseChance = squadTalkSettings._hearNoiseChance;
			_hearNoiseMaxDist = squadTalkSettings._hearNoiseMaxDist;
			_hearNoiseFreq = squadTalkSettings._hearNoiseFreq;
			_enemyLocationTalkChance = squadTalkSettings._enemyLocationTalkChance;
			_enemyLocationTalkTimeSinceSeen = squadTalkSettings._enemyLocationTalkTimeSinceSeen;
			_enemyNeedHelpChance = squadTalkSettings._enemyNeedHelpChance;
			_enemyLocationTalkFreq = squadTalkSettings._enemyLocationTalkFreq;
			_enemyLocationBehindAngle = squadTalkSettings._enemyLocationBehindAngle;
			_enemyLocationSideAngle = squadTalkSettings._enemyLocationSideAngle;
			_enemyLocationFrontAngle = squadTalkSettings._enemyLocationFrontAngle;
		}
	}
}
