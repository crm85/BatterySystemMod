namespace SAIN.SAINComponent.Classes.Talk;

public struct PhraseInfo
{
	public float TimeLastSaid;

	public EPhraseTrigger Phrase { get; }

	public int Priority { get; }

	public float TimeDelay { get; }

	public PhraseInfo(EPhraseTrigger trigger, int priority, float timeDelay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Phrase = trigger;
		Priority = priority;
		TimeDelay = timeDelay;
		TimeLastSaid = 0f;
	}
}
