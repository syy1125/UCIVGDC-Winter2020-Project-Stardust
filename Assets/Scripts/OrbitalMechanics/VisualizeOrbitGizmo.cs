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

		Ellipse ellipse = Orbit.GetLocalEllipse();
		Vector3 last = ellipse.Center + ellipse.SemimajorAxis;

		Gizmos.color = OrbitColor;
		
		for (int i = 1; i <= Segments; i++)
		{
			float angle = (float) i / Segments * 2 * Mathf.PI;
			Vector3 position = ellipse.Center + ellipse.SemimajorAxis * Mathf.Cos(angle)
			                          + ellipse.SemiminorAxis * Mathf.Sin(angle);
			
			Gizmos.DrawLine(last, position);
			last = position;
		}

		Gizmos.color = BodyColor;
		Gizmos.DrawWireSphere(Orbit.GetLocalPositionAt(0), 0.1f);
	}
}