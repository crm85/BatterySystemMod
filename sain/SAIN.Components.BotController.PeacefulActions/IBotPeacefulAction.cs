namespace SAIN.Components.BotController.PeacefulActions;

public interface IBotPeacefulAction
{
	bool Complete { get; }

	void Update();

	void Start();

	void Stop();
}
