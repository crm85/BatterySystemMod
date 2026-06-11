namespace SAIN.SAINComponent.Classes.Talk;

public struct BotTalkPackage
{
	public PhraseInfo phraseInfo;

	public ETagStatus Mask;

	public BotTalkPackage(PhraseInfo phrase, ETagStatus mask)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		phraseInfo = phrase;
		Mask = mask;
	}
}
