using System;
using System.Linq;
using UnityEditor;

public class StarSystem : ISaveLoad<StarSystem.Serialized>
{
	[Serializable]
	public struct Serialized
	{
		public string[] CelestialBodyPaths;
	}

	public CelestialBody[] CelestialBodies;

	public Serialized Save()
	{
		return new Serialized
		{
			CelestialBodyPaths = CelestialBodies.Select(AssetDatabase.GetAssetPath).ToArray()
		};
	}

	public void Load(Serialized serialized)
	{
		CelestialBodies = serialized.CelestialBodyPaths
			.Select(AssetDatabase.LoadAssetAtPath<CelestialBody>)
			.ToArray();
	}
}