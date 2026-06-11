using System;
using System.Collections;
using SAIN.Components;
using SAIN.Helpers.Events;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SteeringController : BotBase
{
	private Coroutine _controller;

	public Vector3 LookDirection => base.Bot.Transform.LookDirection;

	public Vector3 TargetSteerDirection { get; private set; }

	public SteeringController(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public override void Init()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Combine(botActiveToggle.OnToggle, new Action<bool>(onBotActive));
		onBotActive(value: true);
		base.Init();
	}

	public override void Dispose()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Remove(botActiveToggle.OnToggle, new Action<bool>(onBotActive));
		base.Dispose();
	}

	private void onBotActive(bool value)
	{
		if (value)
		{
			if (_controller == null)
			{
				_controller = ((MonoBehaviour)base.Bot).StartCoroutine(controlSteeringLoop());
			}
		}
		else if (_controller != null)
		{
			((MonoBehaviour)base.Bot).StopCoroutine(_controller);
			_controller = null;
		}
	}

	private IEnumerator controlSteeringLoop()
	{
		while (true)
		{
			yield return null;
		}
	}
}
