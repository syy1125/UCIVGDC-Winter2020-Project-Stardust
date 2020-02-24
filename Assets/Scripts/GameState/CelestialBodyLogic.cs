using UnityEditor;

public class CelestialBodyLogic : ISaveLoad<CelestialBodyLogic.Serialized>, IHasTurnLogic
{
	public struct Serialized
	{
		public string CelestialBodyPath;
		public CelestialBodyBuildings.Serialized Buildings;
		public CelestialBodyResources.Serialized Resources;
	}

	public CelestialBody Body { get; private set; }
	public CelestialBodyBuildings Buildings { get; }
	public CelestialBodyResources Resources { get; }

	public CelestialBodyLogic(CelestialBody body)
	{
		Body = body;
		Buildings = new CelestialBodyBuildings(this);
		Resources = new CelestialBodyResources(this);
	}

	public void DoTurnLogic()
	{
		Resources.DoTurnLogic();
	}

	public Serialized Save()
	{
		return new Serialized
		{
			CelestialBodyPath = AssetDatabase.GetAssetPath(Body),
			Buildings = Buildings.Save(),
			Resources = Resources.Save()
		};
	}

	public void Load(Serialized serialized)
	{
		Body = AssetDatabase.LoadAssetAtPath<CelestialBody>(serialized.CelestialBodyPath);
		Buildings.Load(serialized.Buildings);
		Resources.Load(serialized.Resources);
	}
}