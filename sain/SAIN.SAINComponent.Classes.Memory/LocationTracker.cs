using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Memory;

public class LocationTracker : BotBase
{
	private float _checkIndoorsTime;

	public Collider BotZoneCollider => (Collider)(object)BotZone?.Collider;

	public AIPlaceInfo BotZone => base.BotOwner.AIData.PlaceInfo;

	public bool IsIndoors { get; private set; }

	public LocationTracker(BotComponent sain)
		: base(sain)
	{
	}

	public override void ManualUpdate()
	{
		if (_checkIndoorsTime < Time.time)
		{
			_checkIndoorsTime = Time.time + 0.2f;
			IsIndoors = base.Player.AIData.EnvironmentId != 0;
		}
		base.ManualUpdate();
	}
}
