namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public abstract class PersonSubClass : PersonBase
{
	protected PersonClass Person { get; }

	public PersonSubClass(PersonClass person, PlayerData playerData)
		: base(playerData)
	{
		Person = person;
	}
}
