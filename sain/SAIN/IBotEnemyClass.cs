using System;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN;

public interface IBotEnemyClass : IBotClass, IDisposable
{
	void OnEnemyKnownChanged(bool known, Enemy enemy);
}
