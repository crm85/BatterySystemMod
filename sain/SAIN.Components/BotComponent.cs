using System;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.Communications;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;
using SAIN.Preset.GlobalSettings.Categories;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.Debug;
using SAIN.SAINComponent.Classes.Decision;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Info;
using SAIN.SAINComponent.Classes.Memory;
using SAIN.SAINComponent.Classes.Mover;
using SAIN.SAINComponent.Classes.Search;
using SAIN.SAINComponent.Classes.Talk;
using SAIN.SAINComponent.Classes.WeaponFunction;
using UnityEngine;

namespace SAIN.Components;

public class BotComponent : BotComponentBase
{
	private bool _Activated = false;

	public float LastCheckVisibleTime;

	private readonly List<IBotClass> BotClasses = new List<IBotClass>();

	private readonly List<IBotClass> AlwaysTickClasses = new List<IBotClass>();

	private readonly List<IBotClass> TickWhenActiveClasses = new List<IBotClass>();

	private readonly List<IBotClass> TickWhenNoSleepClasses = new List<IBotClass>();

	private readonly List<IBotClass> TickWhenCombatClasses = new List<IBotClass>();

	private float defaultMoveSpeed;

	private float defaultSprintSpeed;

	public bool IsInCombat => BotActivation.BotInCombat;

	public bool IsCheater { get; private set; }

	public bool BotActive => BotActivation.BotActive;

	public bool BotInStandBy => BotActivation.BotInStandBy;

	public AILimitSetting CurrentAILimit => AILimit.CurrentAILimit;

	public bool HasEnemy => EnemyController.GoalEnemy?.EnemyPerson.Active ?? false;

	public bool HasLastEnemy => EnemyController.LastGoalEnemy?.EnemyPerson.Active ?? false;

	public Enemy Enemy => HasEnemy ? EnemyController.GoalEnemy : null;

	public Enemy LastEnemy => HasLastEnemy ? EnemyController.LastGoalEnemy : null;

	public Vector3? CurrentTargetPosition => CurrentTarget.CurrentTargetPosition;

	public Vector3? CurrentTargetDirection => CurrentTarget.CurrentTargetDirection;

	public float CurrentTargetDistance => CurrentTarget.CurrentTargetDistance;

	public BotGlobalEventsClass GlobalEvents { get; private set; }

	public BotBusyHandsDetector BusyHandsDetector { get; private set; }

	public ShootDeciderClass Shoot { get; private set; }

	public BotWeightManagement WeightManagement { get; private set; }

	public SAINBotMedicalClass Medical { get; private set; }

	public SAINActivationClass BotActivation { get; private set; }

	public DoorOpener DoorOpener { get; private set; }

	public ManualShootClass ManualShoot { get; private set; }

	public CurrentTargetClass CurrentTarget { get; private set; }

	public BotBackpackDropClass BackpackDropper { get; private set; }

	public BotLightController BotLight { get; private set; }

	public SAINBotSpaceAwareness SpaceAwareness { get; private set; }

	public AimDownSightsController AimDownSightsController { get; private set; }

	public SAINAILimit AILimit { get; private set; }

	public SAINBotSuppressClass Suppression { get; private set; }

	public SAINVaultClass Vault { get; private set; }

	public SAINSearchClass Search { get; private set; }

	public SAINMemoryClass Memory { get; private set; }

	public SAINEnemyController EnemyController { get; private set; }

	public SAINNoBushESP NoBushESP { get; private set; }

	public SAINFriendlyFireClass FriendlyFire { get; private set; }

	public SAINVisionClass Vision { get; private set; }

	public SAINMoverClass Mover { get; private set; }

	public SAINBotUnstuckClass BotStuck { get; private set; }

	public SAINHearingSensorClass Hearing { get; private set; }

	public SAINBotTalkClass Talk { get; private set; }

	public SAINDecisionClass Decision { get; private set; }

	public SAINCoverClass Cover { get; private set; }

	public SAINBotInfoClass Info { get; private set; }

	public SAINSquadClass Squad { get; private set; }

	public SAINSelfActionClass SelfActions { get; private set; }

	public BotGrenadeManager Grenade { get; private set; }

	public SAINSteeringClass Steering { get; private set; }

	public AimClass Aim { get; private set; }

	public bool IsDead => !base.Person.ActivationClass.IsAlive;

	public bool GameEnding => BotActivation.GameEnding;

	public bool SAINLayersActive => BotActivation.SAINLayersActive;

	public float DistanceToAimTarget
	{
		get
		{
			if (base.BotOwner.AimingManager.CurrentAiming != null)
			{
				return base.BotOwner.AimingManager.CurrentAiming.LastDist2Target;
			}
			return CurrentTarget.CurrentTargetDistance;
		}
	}

	public ESAINLayer ActiveLayer
	{
		get
		{
			return BotActivation.ActiveLayer;
		}
		set
		{
			BotActivation.SetActiveLayer(value);
		}
	}

	public event Action<BotComponent> OnBotActivated;

	public void ActivateIfBotActive(BotOwner botOwner, PersonClass Person)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)botOwner.BotState == 2)
		{
			Activate(botOwner);
		}
	}

	public void Activate(BotOwner botOwner)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		if (_Activated)
		{
			return;
		}
		PersonClass personClass = ((Component)botOwner).GetComponent<PlayerComponent>()?.Person;
		if (personClass == null)
		{
			Logger.LogError("Person Null");
		}
		else if ((int)botOwner.BotState == 2)
		{
			if (InitializeBot(personClass))
			{
				_Activated = true;
				this.OnBotActivated?.Invoke(this);
			}
			else
			{
				Dispose();
			}
		}
	}

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		BotOwner botOwner = base.BotOwner;
		if (!((Object)(object)botOwner != (Object)null))
		{
			return;
		}
		Player getPlayer = botOwner.GetPlayer;
		if ((Object)(object)getPlayer != (Object)null)
		{
			TickClassGroup(AlwaysTickClasses, currentTime);
			bool flag = (int)botOwner.BotState == 2 && getPlayer.HealthController.IsAlive;
			if (flag)
			{
				Vision.BotLook.UpdateLook();
				TickClassGroup(TickWhenActiveClasses, currentTime);
			}
			bool flag2 = !flag || BotInStandBy;
			if (!flag2)
			{
				TickClassGroup(TickWhenNoSleepClasses, currentTime);
				handleDumbShit();
			}
			bool flag3 = flag && !flag2 && SAINLayersActive && CurrentTarget.CurrentTargetEnemy != null;
			BotActivation.SetInCombat(flag3);
			if (flag3)
			{
				TickClassGroup(TickWhenCombatClasses, currentTime);
			}
		}
	}

	private void DrawDebugGizmos()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Enemy currentTargetEnemy = CurrentTarget.CurrentTargetEnemy;
		DebugGizmos.Line(base.Transform.WeaponRoot, base.Transform.WeaponRoot + base.PlayerComponent.SmoothController.CurrentControlLookDirection, Color.yellow, 0.04f, 0.02f);
		DebugGizmos.Line(base.Transform.WeaponRoot, base.Transform.WeaponRoot + base.LookDirection * 0.66f, Color.green, 0.02f, 0.02f);
	}

	private static void TickClassGroup(List<IBotClass> List, float CurrentTime)
	{
		for (int i = 0; i < List.Count; i++)
		{
			List[i]?.ManualUpdate();
		}
	}

	public bool InitializeBot(PersonClass person)
	{
		if (!base.Init(person))
		{
			return false;
		}
		if (!CreateClasses())
		{
			return false;
		}
		if (!AddToSquad())
		{
			return false;
		}
		if (!InitClasses())
		{
			return false;
		}
		if (!FinishInit())
		{
			return false;
		}
		return true;
	}

	private bool CreateClasses()
	{
		try
		{
			Info = new SAINBotInfoClass(this);
			NoBushESP = ((Component)this).gameObject.AddComponent<SAINNoBushESP>();
			Squad = new SAINSquadClass(this);
			BusyHandsDetector = new BotBusyHandsDetector(this);
			GlobalEvents = new BotGlobalEventsClass(this);
			Shoot = new ShootDeciderClass(this);
			WeightManagement = new BotWeightManagement(this);
			Memory = new SAINMemoryClass(this);
			BotStuck = new SAINBotUnstuckClass(this);
			Hearing = new SAINHearingSensorClass(this);
			Talk = new SAINBotTalkClass(this);
			Decision = new SAINDecisionClass(this);
			Cover = new SAINCoverClass(this);
			SelfActions = new SAINSelfActionClass(this);
			Steering = new SAINSteeringClass(this);
			Grenade = new BotGrenadeManager(this);
			Mover = new SAINMoverClass(this);
			EnemyController = new SAINEnemyController(this);
			FriendlyFire = new SAINFriendlyFireClass(this);
			Vision = new SAINVisionClass(this);
			Search = new SAINSearchClass(this);
			Vault = new SAINVaultClass(this);
			Suppression = new SAINBotSuppressClass(this);
			AILimit = new SAINAILimit(this);
			AimDownSightsController = new AimDownSightsController(this);
			SpaceAwareness = new SAINBotSpaceAwareness(this);
			DoorOpener = new DoorOpener(this);
			Medical = new SAINBotMedicalClass(this);
			BotLight = new BotLightController(this);
			BackpackDropper = new BotBackpackDropClass(this);
			CurrentTarget = new CurrentTargetClass(this);
			ManualShoot = new ManualShootClass(this);
			BotActivation = new SAINActivationClass(this);
			Aim = new AimClass(this);
		}
		catch (Exception arg)
		{
			Logger.LogError($"Error When Creating Classes, Disposing... : {arg}");
			return false;
		}
		return true;
	}

	public void AddBotClass(IBotClass Class)
	{
		if (Class == null)
		{
			Logger.LogError("Bot Class of is null, cannot add it to list!");
		}
		else
		{
			BotClasses.Add(Class);
		}
	}

	public void AddBotTickClass(IBotClass Class)
	{
		if (Class.CanEverTick)
		{
			switch (Class.TickRequirement)
			{
			case ESAINTickState.AlwaysUpdate:
				AlwaysTickClasses.Add(Class);
				break;
			case ESAINTickState.OnlyBotActive:
				TickWhenActiveClasses.Add(Class);
				break;
			case ESAINTickState.OnlyNoSleep:
				TickWhenNoSleepClasses.Add(Class);
				break;
			case ESAINTickState.OnlyBotInCombat:
				TickWhenCombatClasses.Add(Class);
				break;
			}
		}
	}

	private bool AddToSquad()
	{
		try
		{
			Squad.SquadInfo.AddMember(this);
		}
		catch (Exception arg)
		{
			Logger.LogError($"Error adding member to squad!: {arg}");
			return false;
		}
		return true;
	}

	private bool InitClasses()
	{
		try
		{
			NoBushESP.Init(base.Person.AIInfo.BotOwner, this);
		}
		catch (Exception arg)
		{
			Logger.LogError($"Error When Initializing Components, Disposing... : {arg}");
			return false;
		}
		foreach (IBotClass botClass in BotClasses)
		{
			try
			{
				botClass.Init();
			}
			catch (Exception arg2)
			{
				Logger.LogError($"Error When Initializing Class [{botClass}], Disposing... : {arg2}");
				return false;
			}
		}
		return true;
	}

	private bool FinishInit()
	{
		try
		{
			if (!VerifyBrain(base.Person))
			{
				Logger.LogError("Init SAIN ERROR, Disposing...");
				return false;
			}
			try
			{
				base.BotOwner.LookSensor.MaxShootDist = float.MaxValue;
				IAIData aIData = base.BotOwner.AIData;
				GClass567 val = (GClass567)(object)((aIData is GClass567) ? aIData : null);
				if (val != null)
				{
					val.IsNoOffsetShooting = false;
				}
			}
			catch (Exception arg)
			{
				Logger.LogError($"Error setting MaxShootDist during init, but continuing with initialization...: {arg}");
			}
			try
			{
				JokeSettings jokes = GlobalSettingsClass.Instance.General.Jokes;
				if (jokes.RandomCheaters && (EFTMath.RandomBool(jokes.RandomCheaterChance) || base.Player.Profile.Nickname.ToLower().Contains("solarint")))
				{
					IsCheater = true;
				}
			}
			catch (Exception arg2)
			{
				Logger.LogWarning($"Error when initializing dumb shit for this bot, continuing anyways since its some dumb shit. Error: {arg2}");
			}
		}
		catch (Exception arg3)
		{
			Logger.LogError($"Error When Finishing Bot Initialization, Disposing... : {arg3}");
			return false;
		}
		return true;
	}

	private bool VerifyBrain(PersonClass person)
	{
		object obj;
		if (person == null)
		{
			obj = null;
		}
		else
		{
			PersonAIInfo aIInfo = person.AIInfo;
			if (aIInfo == null)
			{
				obj = null;
			}
			else
			{
				BotOwner botOwner = aIInfo.BotOwner;
				if (botOwner == null)
				{
					obj = null;
				}
				else
				{
					StandartBotBrain brain = botOwner.Brain;
					if (brain == null)
					{
						obj = null;
					}
					else
					{
						BaseBrain baseBrain = brain.BaseBrain;
						obj = ((baseBrain != null) ? baseBrain.ShortName() : null);
					}
				}
			}
		}
		string assignedBrainName = (string)obj;
		if (Info.Profile.IsPMC)
		{
			IEnumerable<string> allowedBrainNames = from brain2 in AIBrains.GetAllowedPMCBrains()
				select brain2.ToString();
			return IsAssignedBrainAllowed(assignedBrainName, allowedBrainNames, "PMC") ? true : false;
		}
		if (Info.Profile.IsPlayerScav)
		{
			IEnumerable<string> allowedBrainNames2 = from brain2 in AIBrains.GetAllowedPlayerScavBrains()
				select brain2.ToString();
			return IsAssignedBrainAllowed(assignedBrainName, allowedBrainNames2, "PlayerScav") ? true : false;
		}
		if (Info.Profile.IsScav)
		{
			IEnumerable<string> allowedBrainNames3 = from brain2 in AIBrains.GetAllowedScavBrains()
				select brain2.ToString();
			return IsAssignedBrainAllowed(assignedBrainName, allowedBrainNames3, "Scav") ? true : false;
		}
		return true;
	}

	private bool IsAssignedBrainAllowed(string assignedBrainName, IEnumerable<string> allowedBrainNames, string botCategory)
	{
		if (allowedBrainNames.Contains(assignedBrainName))
		{
			return true;
		}
		Logger.LogAndNotifyError(((Object)base.BotOwner).name + " is a $" + botCategory + " but does not have any of these BaseBrains: $" + string.Join(", ", allowedBrainNames) + "! Current Brain Assignment: [" + assignedBrainName + "] : SAIN Server mod is either missing or another mod is overwriting it. Destroying SAIN for this bot...", (ENotificationDurationType)1);
		return false;
	}

	private void OnDisable()
	{
		BotActivation.SetActive(botActive: false);
		((MonoBehaviour)this).StopAllCoroutines();
	}

	public void LateUpdate()
	{
	}

	private void handleDumbShit()
	{
		if (IsCheater)
		{
			if (defaultMoveSpeed == 0f)
			{
				defaultMoveSpeed = base.Player.MovementContext.MaxSpeed;
				defaultSprintSpeed = base.Player.MovementContext.SprintSpeed;
			}
			((Behaviour)base.Player.Grounder).enabled = Enemy == null;
			if (Enemy != null)
			{
				base.Player.MovementContext.SetCharacterMovementSpeed(350f, true);
				base.Player.MovementContext.SprintSpeed = 50f;
				base.Player.ChangeSpeed(100f);
				base.Player.UpdateSpeedLimit(100f, (ESpeedLimit)4);
				base.Player.MovementContext.ChangeSpeedLimit(100f, (ESpeedLimit)4);
				base.BotOwner.SetTargetMoveSpeed(100f);
			}
			else
			{
				base.Player.MovementContext.SetCharacterMovementSpeed(defaultMoveSpeed, false);
				base.Player.MovementContext.SprintSpeed = defaultSprintSpeed;
			}
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		BotActivation?.SetActive(botActive: false);
		((MonoBehaviour)this).StopAllCoroutines();
		foreach (IBotClass botClass in BotClasses)
		{
			try
			{
				botClass.Dispose();
			}
			catch (Exception arg)
			{
				Logger.LogError($"Dispose Class [{botClass}] Error: {arg}");
			}
		}
		if ((Object)(object)NoBushESP != (Object)null)
		{
			Object.Destroy((Object)(object)NoBushESP);
		}
		if ((Object)(object)base.BotOwner != (Object)null)
		{
			base.BotOwner.OnBotStateChange -= resetBot;
		}
		Object.Destroy((Object)(object)this);
	}

	private void resetBot(EBotState state)
	{
		Decision.ResetDecisions(active: false);
	}
}
