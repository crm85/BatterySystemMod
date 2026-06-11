using System;

namespace SAIN.Helpers.Events;

public class ToggleEvent : ToggleEventBase
{
	public Action<bool> OnToggle;

	public void CheckToggle(bool value)
	{
		if (base.Value != value)
		{
			base.SetValue(value);
			OnToggle?.Invoke(value);
		}
	}

	public ToggleEvent(bool defaultValue = false)
		: base(defaultValue)
	{
	}
}
public class ToggleEvent<A> : ToggleEventBase
{
	public Action<bool, A> OnToggle;

	public A TypeValue { get; private set; }

	public void CheckToggle(bool value, A a)
	{
		if (base.Value != value)
		{
			base.SetValue(value);
			TypeValue = a;
			OnToggle?.Invoke(value, a);
		}
	}

	public ToggleEvent(bool defaultValue = false)
		: base(defaultValue)
	{
	}
}
public class ToggleEvent<A, B> : ToggleEventBase
{
	public Action<bool, A, B> OnToggle;

	public void CheckToggle(bool value, A a, B b)
	{
		if (base.Value != value)
		{
			base.SetValue(value);
			OnToggle?.Invoke(value, a, b);
		}
	}

	public ToggleEvent(bool defaultValue = false)
		: base(defaultValue)
	{
	}
}
public class ToggleEvent<A, B, C> : ToggleEventBase
{
	public Action<bool, A, B, C> OnToggle;

	public void CheckToggle(bool value, A a, B b, C c)
	{
		if (base.Value != value)
		{
			base.SetValue(value);
			OnToggle?.Invoke(value, a, b, c);
		}
	}

	public ToggleEvent(bool defaultValue = false)
		: base(defaultValue)
	{
	}
}
