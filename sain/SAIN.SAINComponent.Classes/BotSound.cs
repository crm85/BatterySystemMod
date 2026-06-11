using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.SAINComponent.Classes;

public class BotSound
{
	public SoundInfoData Info;

	public SoundResultsData Results;

	public SoundRangeData Range;

	public SoundDispersionData Dispersion;

	public BulletData BulletData;

	public Enemy Enemy { get; }

	public float Distance { get; }

	public BotSound(SoundInfoData info, Enemy enemy, float baseRange)
	{
		Enemy = enemy;
		Distance = enemy.RealDistance;
		Info = info;
		Results = new SoundResultsData();
		Range = new SoundRangeData(baseRange);
		Dispersion = new SoundDispersionData();
		BulletData = new BulletData();
	}
}
