using Boo.Lang;

public class GameState
{
	public int CurrentTurn;
	public StarSystem StarSystem = new StarSystem();
	public List<Planet> Planets = new List<Planet>();
}