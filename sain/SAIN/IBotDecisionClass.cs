using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN;

public interface IBotDecisionClass
{
	bool GetDecision(Enemy enemy, out string reason);
}
