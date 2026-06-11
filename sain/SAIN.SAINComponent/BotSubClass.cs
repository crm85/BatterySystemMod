namespace SAIN.SAINComponent;

public abstract class BotSubClass<T>(T sainClass) : BotBase(sainClass.Bot) where T : IBotClass
{
	protected T BaseClass { get; } = sainClass;
}
