using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public struct HardTargetData
{
	public string ProfileId;

	public Enemy Enemy;

	public HardTargetData(Enemy enemy)
	{
		ProfileId = enemy.EnemyProfileId;
		Enemy = enemy;
	}
}
