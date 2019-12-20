using System;
using UnityEngine;

public class VisualizeOrbitGizmo : MonoBehaviour
{
	public Orbit Orbit;
	public int Segments = 20;
	public Color OrbitColor = Color.green;
	public Color BodyColor = Color.blue;

	private void OnDrawGizmos()
	{
		if (Orbit == null) return;

		Vector3[] positions = Orbit.GetDrawPositions(Segments);
		Gizmos.color = OrbitColor;

		for (int i = 0; i < Segments; i++)
		{
			Gizmos.DrawLine(positions[i], positions[i + 1]);
		}

		Gizmos.color = BodyColor;
		(Vector3 position, Vector3 velocity) = Orbit.GetGlobalPositionAndVelocityAt(0);
		Gizmos.DrawWireSphere(position, Orbit.SemilatusRectum * 0.05f);
		Gizmos.DrawLine(position, position + velocity);
	}
}