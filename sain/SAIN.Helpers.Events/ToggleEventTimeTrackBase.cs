using UnityEngine;

namespace SAIN.Helpers.Events;

public abstract class ToggleEventTimeTrackBase : ToggleEventBase
{
	public float TimeLastTrue { get; private set; }

	public float TimeLastFalse { get; private set; }

	public float TimeSinceTrue => Time.time - TimeLastTrue;

	public float TimeSinceFalse => Time.time - TimeLastFalse;

	public ToggleEventTimeTrackBase(bool defaultValue)
		: base(defaultValue)
	{
	}

	protected override void SetValue(bool value)
	{
		if (value)
		{
			TimeLastTrue = Time.time;
		}
		else
		{
			TimeLastFalse = Time.time;
		}
		base.SetValue(value);
	}
}
