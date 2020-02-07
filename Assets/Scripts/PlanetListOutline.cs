using UnityEngine;

public class PlanetListOutline : MonoBehaviour
{
	public PlanetManager[] Planets;
	public GameObject PlanetOutlinePrefab;
	public float OverallScale = 2;

	private void Start()
	{
		foreach (PlanetManager planet in Planets)
		{
			GameObject planetOutline = Instantiate(PlanetOutlinePrefab, transform);

			var planetRenderer = planetOutline.GetComponentInChildren<MeshRenderer>();
			planetRenderer.material = planet.Body.OutlineMaterial;
			planetRenderer.transform.localScale = Vector3.one * (planet.Body.OutlineRadius * OverallScale);
		}
	}
}