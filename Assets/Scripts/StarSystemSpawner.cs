using UnityEngine;

public class StarSystemSpawner : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject CelestialBodyPrefab;
	public GameObject OrbitPrefab;
	public GameObject LightPrefab;

	[Header("References")]
	public CelestialBodyOutlineList Outliner;

	private void Start()
	{
		Instantiate(LightPrefab, transform);

		foreach (CelestialBody body in GameController.Instance.State.StarSystem.CelestialBodies)
		{
			GameObject bodyObject = Instantiate(CelestialBodyPrefab, transform);
			bodyObject.GetComponent<PlanetVisualController>().Body = body;
			bodyObject.SetActive(true);

			Outliner.SpawnOutlinerItem(bodyObject.GetComponent<PlanetVisualController>());

			if (body.Fixed) continue;
			GameObject orbitObject = Instantiate(OrbitPrefab, transform);
			orbitObject.GetComponent<VisualizeOrbit>().Body = body;
		}
	}
}