using System;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyAnglesClass : EnemyBase, IBotEnemyClass, IBotClass, IDisposable
{
	private const float CALC_ANGLE_FREQ = 1f / 15f;

	private const float CALC_ANGLE_FREQ_AI = 0.25f;

	private const float CALC_ANGLE_FREQ_KNOWN = 1f / 30f;

	private const float CALC_ANGLE_FREQ_KNOWN_AI = 1f / 15f;

	private const float CALC_ANGLE_CURRENT_COEF = 0.5f;

	private float _calcAngleTime;

	public bool CanBeSeen { get; private set; }

	public float MaxVisionAngle { get; private set; }

	public float AngleToEnemy { get; private set; }

	public float AngleToEnemyHorizontal { get; private set; }

	public float AngleToEnemyHorizontalSigned { get; private set; }

	public float AngleToEnemyVertical { get; private set; }

	public float AngleToEnemyVerticalSigned { get; private set; }

	public EnemyAnglesClass(Enemy enemy)
		: base(enemy)
	{
	}

	public override void ManualUpdate()
	{
		CalcAngles();
		base.ManualUpdate();
	}

	public void OnEnemyKnownChanged(bool known, Enemy enemy)
	{
	}

	private void CalcAngles()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (_calcAngleTime < Time.time)
		{
			float num = ((!base.Enemy.IsAI) ? (base.Enemy.EnemyKnown ? (1f / 30f) : (1f / 15f)) : (base.Enemy.EnemyKnown ? (1f / 15f) : 0.25f));
			if (base.Enemy.IsCurrentEnemy)
			{
				num *= 0.5f;
			}
			_calcAngleTime = Time.time + num;
			MaxVisionAngle = base.Enemy.Bot.Info.FileSettings.Core.VisibleAngle / 2f;
			Vector3 lookDirection = base.Bot.LookDirection;
			Vector3 enemyDirectionNormal = base.Enemy.EnemyDirectionNormal;
			AngleToEnemy = Vector3.Angle(enemyDirectionNormal, lookDirection);
			CanBeSeen = AngleToEnemy <= MaxVisionAngle;
			float yDiff;
			float num2 = (AngleToEnemyVertical = CalcVerticalAngle(enemyDirectionNormal, lookDirection, out yDiff));
			AngleToEnemyVerticalSigned = ((yDiff >= 0f) ? num2 : (0f - num2));
			float num4 = (AngleToEnemyHorizontalSigned = CalcHorizontalAngle(enemyDirectionNormal, lookDirection));
			AngleToEnemyHorizontal = Mathf.Abs(num4);
		}
	}

	public static float CalcVerticalAngle(Vector3 enemyDirNormal, Vector3 lookDirection, out float yDiff)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(lookDirection.x, enemyDirNormal.y, lookDirection.z);
		yDiff = (val.y - lookDirection.y).Round100();
		if (yDiff == 0f)
		{
			return 0f;
		}
		return Vector3.Angle(lookDirection, val);
	}

	public static float CalcHorizontalAngle(Vector3 enemyDirNormal, Vector3 lookDirection)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		enemyDirNormal.y = 0f;
		lookDirection.y = 0f;
		return Vector3.SignedAngle(lookDirection, enemyDirNormal, Vector3.up);
	}
}
