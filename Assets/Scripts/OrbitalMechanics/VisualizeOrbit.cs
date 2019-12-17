using System.Collections;
using System.Collections.Generic;
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

		Ellipse ellipse = Body.Orbit.GetLocalEllipse();

		var positions = new Vector3 [Segments + 1];
		for (int i = 0; i <= Segments; i++)
		{
			float angle = (float) i / Segments * 2 * Mathf.PI;
			positions[i] = ellipse.Center + ellipse.SemimajorAxis * Mathf.Cos(angle)
			                              + ellipse.SemiminorAxis * Mathf.Sin(angle);
		}
		
		var lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.startColor = lineRenderer.endColor = Body.OverviewColor;
		lineRenderer.positionCount = positions.Length;
		lineRenderer.SetPositions(positions);
	}
}