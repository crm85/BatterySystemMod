using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public struct SoundStruct
{
	private const float TIME_TO_LOOK = 3f;

	private const float TIME_TO_CLEAR = 10f;

	public readonly Enemy Enemy;

	public readonly EnemyPlace Place;

	public float TimeSinceHeard => Place.TimeSincePositionUpdated;

	public Vector3 Position => Place.Position;

	public bool ShallLook => clearForLook && TimeSinceHeard < 3f;

	private bool clearForLook => Place != null && Enemy != null && Enemy.WasValid;

	public bool ShallClear => !clearForLook || TimeSinceHeard >= 10f;

	public SoundStruct(Enemy enemy, EnemyPlace place)
	{
		Enemy = enemy;
		Place = place;
	}
}
