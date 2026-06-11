using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace.Classes;

public class PlayerAISoundPlayer : AIDataBase
{
	private float _startPlaySoundsTime;

	public bool SoundMakerStarted { get; private set; }

	public PlayerAISoundPlayer(SAINAIData aidata)
		: base(aidata)
	{
		SoundMakerStarted = false;
		_startPlaySoundsTime = Time.time + 1f;
	}

	public bool ShallPlayAISound()
	{
		if (!SoundMakerStarted)
		{
			if (_startPlaySoundsTime > Time.time)
			{
				return false;
			}
			SoundMakerStarted = true;
		}
		return true;
	}
}
