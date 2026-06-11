using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class ExpirableBool
{
	public float TimeSet;

	private bool _value;

	private float _resetTime;

	private readonly float _expireTime;

	private readonly float _randomMin;

	private readonly float _randomMax;

	public bool Value
	{
		get
		{
			if (_value && _resetTime < Time.time)
			{
				_value = false;
			}
			return _value;
		}
		set
		{
			if (value)
			{
				TimeSet = Time.time;
				_resetTime = TimeSet + _expireTime * Random.Range(_randomMin, _randomMax);
			}
			_value = value;
		}
	}

	public ExpirableBool(float expireTime, float randomMin, float randomMax)
	{
		_expireTime = expireTime;
		_randomMin = randomMin;
		_randomMax = randomMax;
	}
}
