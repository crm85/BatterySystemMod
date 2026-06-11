using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class BotSurgery : BotBase
{
	private bool _surgeryStarted;

	private bool _areaClear;

	private float _nextCheckClearTime;

	private float _checkClearFreq = 0.25f;

	private bool _allClear;

	private float _nextCheckEnemiesTime;

	public bool SurgeryStarted
	{
		get
		{
			return _surgeryStarted;
		}
		set
		{
			if (_surgeryStarted != value && value)
			{
				SurgeryStartTime = Time.time;
			}
			_surgeryStarted = value;
		}
	}

	public float SurgeryStartTime { get; private set; }

	public bool AreaClearForSurgery
	{
		get
		{
			if (_nextCheckClearTime < Time.time)
			{
				_nextCheckClearTime = Time.time + _checkClearFreq;
				_areaClear = shallTrySurgery();
			}
			return _areaClear;
		}
	}

	public bool _canStartSurgery
	{
		get
		{
			BotOwner botOwner = base.BotOwner;
			int result;
			if (botOwner != null)
			{
				BotMedecine medecine = botOwner.Medecine;
				bool? obj;
				if (medecine == null)
				{
					obj = null;
				}
				else
				{
					GClass473 surgicalKit = medecine.SurgicalKit;
					obj = ((surgicalKit != null) ? new bool?(surgicalKit.ShallStartUse()) : ((bool?)null));
				}
				bool? flag = obj;
				if (flag == true)
				{
					BotOwner botOwner2 = base.BotOwner;
					if (botOwner2 == null)
					{
						result = 0;
					}
					else
					{
						BotMedecine medecine2 = botOwner2.Medecine;
						bool? obj2;
						if (medecine2 == null)
						{
							obj2 = null;
						}
						else
						{
							BotFirstAidClass firstAid = medecine2.FirstAid;
							obj2 = ((firstAid != null) ? new bool?(firstAid.IsBleeding) : ((bool?)null));
						}
						result = ((obj2 == false) ? 1 : 0);
					}
					goto IL_00a0;
				}
			}
			result = 0;
			goto IL_00a0;
			IL_00a0:
			return (byte)result != 0;
		}
	}

	public BotSurgery(BotComponent bot)
		: base(bot)
	{
		base.CanEverTick = false;
	}

	private bool shallTrySurgery()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (_canStartSurgery)
		{
			Enemy enemy = base.Bot.Enemy;
			if (base.Bot.EnemyController.AtPeace)
			{
				if (!base.Bot.CurrentTargetPosition.HasValue)
				{
					result = true;
				}
				else
				{
					Vector3 val = base.Bot.CurrentTargetPosition.Value - base.Bot.Position;
					if (((Vector3)(ref val)).sqrMagnitude > 10000f)
					{
						result = true;
					}
				}
			}
			else
			{
				result = checkAllClear(SurgeryStarted);
			}
		}
		return result;
	}

	private bool checkAllClear(bool surgeryStarted)
	{
		if (_nextCheckEnemiesTime < Time.time)
		{
			float num = (surgeryStarted ? 0.5f : 0.1f);
			_nextCheckEnemiesTime = Time.time + num;
			float minPathDist = (surgeryStarted ? 50f : 100f);
			float minTimeSinceLastKnown = (surgeryStarted ? 30f : 60f);
			_allClear = checkEnemies(minPathDist, minTimeSinceLastKnown);
		}
		return _allClear;
	}

	private bool checkEnemies(float minPathDist, float minTimeSinceLastKnown)
	{
		bool result = true;
		Dictionary<string, Enemy> enemies = base.Bot.EnemyController.Enemies;
		foreach (Enemy value in enemies.Values)
		{
			if (!checkThisEnemy(value, minPathDist, minTimeSinceLastKnown))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private bool checkThisEnemy(Enemy enemy, float minPathDist, float minTimeSinceLastKnown)
	{
		if (enemy != null)
		{
			Player enemyPlayer = enemy.EnemyPlayer;
			if (((enemyPlayer != null) ? new bool?(enemyPlayer.HealthController.IsAlive) : ((bool?)null)) == true && (enemy.Seen || enemy.Heard) && enemy.TimeSinceLastKnownUpdated < 360f)
			{
				if (enemy.IsVisible)
				{
					return false;
				}
				if (enemy.TimeSinceLastKnownUpdated < minTimeSinceLastKnown)
				{
					return false;
				}
				if (enemy.Path.PathLength < minPathDist)
				{
					return false;
				}
			}
		}
		return true;
	}
}
