using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class VisualizeOrbit : MonoBehaviour
{
	public CelestialBody Body;
	public int Segments;

	private void Start()
	{
		UpdateLineRenderer();
	}

	private void Update()
	{
		if (Application.isPlaying) return;
		UpdateLineRenderer();
	}

	private void UpdateLineRenderer()
	{
		if (Body == null || Body.Orbit == null) return;

		Vector3[] positions = Body.Orbit.GetDrawPositions(Segments);

		var lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.startColor = lineRenderer.endColor = Body.OverviewColor;
		lineRenderer.positionCount = positions.Length;
		lineRenderer.SetPositions(positions);
	}
}