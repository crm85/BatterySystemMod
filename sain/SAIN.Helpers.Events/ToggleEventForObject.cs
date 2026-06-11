using System;

namespace SAIN.Helpers.Events;

public class ToggleEventForObject<T> : ToggleEventBase
{
	public Action<bool, T> OnToggle;

	private readonly T Object;

	public bool CheckToggle(bool value)
	{
		if (base.Value != value)
		{
			base.SetValue(value);
			OnToggle?.Invoke(value, Object);
			return true;
		}
		return false;
	}

	public ToggleEventForObject(T _object, bool defaultValue = false)
		: base(defaultValue)
	{
		Object = _object;
	}
}
