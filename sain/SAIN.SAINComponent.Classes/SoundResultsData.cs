using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SoundResultsData
{
	public bool Heard;

	public bool VisibleSource;

	public bool LimitedByAI;

	public bool SoundFarFromPlayer;

	public float ChanceToHear;

	public Vector3 EstimatedPosition;

	public SoundResultsData(bool defaults = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Heard = defaults;
		VisibleSource = false;
		LimitedByAI = false;
		SoundFarFromPlayer = false;
		ChanceToHear = 100f;
		EstimatedPosition = Vector3.zero;
	}
}
