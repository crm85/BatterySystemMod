using System;

namespace SAIN.Helpers.Events;

public class ToggleEventForObjectTimeTracked<T> : ToggleEventTimeTrackBase
{
	public Action<bool, T> OnToggle;

	private readonly T Object;

	public void CheckToggle(bool value)
	{
		if (base.Value != value)
		{
			SetValue(value);
			OnToggle?.Invoke(value, Object);
		}
	}

	public ToggleEventForObjectTimeTracked(T _object, bool defaultValue = false)
		: base(defaultValue)
	{
		Object = _object;
	}
}
