namespace SAIN.SAINComponent.Classes.Search;

public enum EPathCalcFailReason
{
	None,
	NullDestination,
	NoTarget,
	NullPlace,
	TooClose,
	SampleStart,
	SampleEnd,
	CalcPath,
	LastCorner
}
