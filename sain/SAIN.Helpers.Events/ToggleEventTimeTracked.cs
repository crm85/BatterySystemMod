using System;

namespace SAIN.Helpers.Events;

public class ToggleEventTimeTracked : ToggleEventTimeTrackBase
{
	public Action<bool> OnToggle;

	public void CheckToggle(bool value)
	{
		if (base.Value != value)
		{
			SetValue(value);
			OnToggle?.Invoke(value);
		}
	}

	public ToggleEventTimeTracked(bool defaultValue = false)
		: base(defaultValue)
	{
	}
}
