using UnityEditor;

public class Planet : ISaveLoad<Planet.Serialized>
{
	public struct Serialized
	{
		public string CelestialBodyPath;
		public PlanetBuildings.Serialized Buildings;
		public PlanetResources.Serialized Resources;
	}

	public CelestialBody Body { get; private set; }
	public PlanetBuildings Buildings { get; }
	public PlanetResources Resources { get; }

	public Planet(CelestialBody body)
	{
		Body = body;
		Buildings = new PlanetBuildings(Body);
		Resources = new PlanetResources(Buildings);
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