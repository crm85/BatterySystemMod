using System.Collections.Generic;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINBotLookClass : BotBase
{
	private const float VISION_FREQ_INACTIVE_BOT_COEF = 5f;

	private const float VISION_FREQ_ACTIVE_BOT_COEF = 2f;

	private const float VISION_FREQ_CURRENT_ENEMY = 0.04f;

	private const float VISION_FREQ_UNKNOWN_ENEMY = 0.1f;

	private const float VISION_FREQ_KNOWN_ENEMY = 0.05f;

	private Dictionary<string, Enemy> _enemies;

	public readonly GClass589 LookData;

	private readonly List<Enemy> _cachedList = new List<Enemy>();

	public SAINBotLookClass(BotComponent component)
		: base(component)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		LookData = new GClass589();
	}

	public override void Init()
	{
		_enemies = base.Bot.EnemyController.Enemies;
		base.Init();
	}

	public int UpdateLook()
	{
		if (base.BotOwner.LeaveData == null || base.BotOwner.LeaveData.LeaveComplete)
		{
			return 0;
		}
		int result = UpdateLookForEnemies(LookData);
		UpdateLookData(LookData);
		return result;
	}

	public void UpdateLookData(GClass589 lookData)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < lookData.ReportsData.Count; i++)
		{
			GClass564 val = lookData.ReportsData[i];
			base.BotOwner.BotsGroup.ReportAboutEnemy(val.Enemy, val.VisibleOnlyBySence);
		}
		if (lookData.ReportsData.Count > 0)
		{
			base.BotOwner.Memory.SetLastTimeSeeEnemy();
		}
		if (lookData.ShallRecalcGoal)
		{
			base.BotOwner.CalcGoal();
		}
		lookData.Reset();
	}

	private int UpdateLookForEnemies(GClass589 lookAll)
	{
		int num = 0;
		_cachedList.Clear();
		_cachedList.AddRange(_enemies.Values);
		foreach (Enemy cached in _cachedList)
		{
			if (ShallCheckEnemy(cached) && CheckEnemy(cached, lookAll))
			{
				num++;
			}
		}
		_cachedList.Clear();
		return num;
	}

	private bool ShallCheckEnemy(Enemy enemy)
	{
		if (enemy == null || !enemy.CheckValid())
		{
			return false;
		}
		if (!enemy.InLineOfSight || !enemy.Vision.Angles.CanBeSeen)
		{
			SetNotVis(enemy);
			return false;
		}
		return true;
	}

	private void SetNotVis(Enemy enemy)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		foreach (EnemyPartData value in enemy.EnemyInfo.AllActiveParts.Values)
		{
			if (value.IsVisible || (int)value.VisibleType == 2)
			{
				value.UpdateVisibility(base.BotOwner, false, false, false, Time.time - enemy.Vision.VisionChecker.LastCheckLookTime, 1f);
			}
		}
		if (enemy.EnemyInfo.IsVisible)
		{
			enemy.EnemyInfo.SetVisible(false);
		}
	}

	private bool CheckEnemy(Enemy enemy, GClass589 lookAll)
	{
		float delay = GetDelay(enemy);
		EnemyVisionChecker visionChecker = enemy.Vision.VisionChecker;
		float num = Time.time - visionChecker.LastCheckLookTime;
		if (num >= delay)
		{
			visionChecker.LastCheckLookTime = Time.time;
			enemy.EnemyInfo.CheckLookEnemy(lookAll, num);
			return true;
		}
		return false;
	}

	private float GetDelay(Enemy enemy)
	{
		float num = enemy.UpdateFrequencyCoefNormal + 1f;
		float num2 = CalcBaseDelay(enemy) * num;
		if (!enemy.IsAI)
		{
			return num2;
		}
		SAINActivationClass botActivation = base.Bot.BotActivation;
		if (!botActivation.BotActive || botActivation.BotInStandBy)
		{
			return num2 * 5f;
		}
		return num2 * 2f;
	}

	private float CalcBaseDelay(Enemy enemy)
	{
		if (enemy.IsCurrentEnemy)
		{
			return 0.04f;
		}
		if (enemy.EnemyKnown)
		{
			return 0.05f;
		}
		return 0.1f;
	}
}
