using UnityEngine;

public class CameraPanController : MonoBehaviour
{
	public string HorizontalAxis;
	public string VerticalAxis;

	public Bounds Bounds;
	public float PanSpeed;

	public bool UseBorderPan;
	public float PanBorderThickness;

	private void Update()
	{
		var input = new Vector2(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis));

		if (UseBorderPan)
		{
			Vector2 mouse = Input.mousePosition;
			if (mouse.x <= PanBorderThickness) input.x = -1;
			if (mouse.x >= Screen.width - PanBorderThickness) input.x = 1;
			if (mouse.y <= PanBorderThickness) input.y = -1;
			if (mouse.y >= Screen.height - PanBorderThickness) input.y = 1;
		}

		if (input.magnitude > 1) input.Normalize();
		input *= PanSpeed;

		Transform t = transform;
		t.position = Bounds.ClosestPoint(t.position + new Vector3(input.x, 0, input.y));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}