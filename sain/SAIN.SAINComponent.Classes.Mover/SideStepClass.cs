using SAIN.Components;
using SAIN.Preset.GlobalSettings;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public class SideStepClass : BotBase
{
	private float ResetCanShoot;

	private float SideStepTimer = 0f;

	public SideStepSetting SideStepSetting { get; private set; }

	public bool SideStepActive => SideStepSetting != SideStepSetting.None && CurrentSideStep != 0f;

	public float CurrentSideStep => base.Player.MovementContext.GetSidestep();

	public SideStepClass(BotComponent sain)
		: base(sain)
	{
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		float currentSideStep = CurrentSideStep;
		if (SideStepSetting != SideStepSetting.None && currentSideStep == 0f)
		{
			SideStepSetting = SideStepSetting.None;
		}
		if (!base.Bot.SAINLayersActive)
		{
			ResetSideStep(currentSideStep);
			return;
		}
		Enemy enemy = base.Bot.Enemy;
		ECombatDecision currentCombatDecision = base.Bot.Decision.CurrentCombatDecision;
		if (enemy == null || currentCombatDecision != ECombatDecision.HoldInCover)
		{
			ResetSideStep(currentSideStep);
			return;
		}
		if (GlobalSettingsClass.Instance.General.AILimit.LimitAIvsAIGlobal && enemy.IsAI && base.Bot.CurrentAILimit != AILimitSetting.None)
		{
			ResetSideStep(currentSideStep);
			return;
		}
		if (enemy.CanShoot)
		{
			if (ResetCanShoot == -1f)
			{
				ResetCanShoot = Time.time + 2f;
			}
			if (ResetCanShoot < Time.time)
			{
				ResetSideStep(currentSideStep);
			}
			return;
		}
		ResetCanShoot = -1f;
		if (!(SideStepTimer > Time.time))
		{
			float num;
			switch (base.Bot.Mover.Lean.LeanDirection)
			{
			case LeanSetting.Left:
				num = -1f;
				SideStepSetting = SideStepSetting.Left;
				break;
			case LeanSetting.Right:
				num = 1f;
				SideStepSetting = SideStepSetting.Right;
				break;
			default:
				num = 0f;
				SideStepSetting = SideStepSetting.None;
				break;
			}
			if (num != 0f)
			{
				SideStepTimer = Time.time + 2f;
			}
			else
			{
				SideStepTimer = Time.time + 0.5f;
			}
			SetSideStep(num, currentSideStep);
		}
	}

	public void ResetSideStep(float current)
	{
		SideStepSetting = SideStepSetting.None;
		if (current != 0f)
		{
			base.Player.MovementContext.SetSidestep(0f);
		}
	}

	public void SetSideStep(float value, float current)
	{
		if (current != value)
		{
			base.Player.MovementContext.SetSidestep(value);
		}
	}
}
