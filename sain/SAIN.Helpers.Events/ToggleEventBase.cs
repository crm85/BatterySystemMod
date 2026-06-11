namespace SAIN.Helpers.Events;

public abstract class ToggleEventBase
{
	public bool Value { get; protected set; }

	public ToggleEventBase(bool defaultValue)
	{
		SetValue(defaultValue);
	}

	protected virtual void SetValue(bool value)
	{
		Value = value;
	}
}
