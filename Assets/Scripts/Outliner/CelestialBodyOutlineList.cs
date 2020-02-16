using UnityEngine;

public class CelestialBodyOutlineList : MonoBehaviour
{
	public PlanetPositionController[] Planets;
	public GameObject PlanetOutlinePrefab;
	public float OverallScale = 2;
	public CameraPan CameraPan;

	private void Start()
	{
		foreach (PlanetPositionController planet in Planets)
		{
			GameObject planetOutline = Instantiate(PlanetOutlinePrefab, transform);

			var planetRenderer = planetOutline.GetComponentInChildren<MeshRenderer>();
			planetRenderer.material = planet.Body.OutlineMaterial;
			planetRenderer.transform.localScale = Vector3.one * (planet.Body.OutlineRadius * OverallScale);

			var planetOutlineItem = planetOutline.GetComponent<CelestialBodyOutlineItem>();
			planetOutlineItem.OnClick.AddListener(() => CameraPan.Follow(planet.gameObject));
		}
	}
}