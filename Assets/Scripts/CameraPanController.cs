using System;
using UnityEngine;

public class CameraPanController : MonoBehaviour
{
	public string HorizontalAxis;
	public string VerticalAxis;
	public string ResetCameraButton;

	public Bounds Bounds;
	public float PanSpeed;

	public bool UseBorderPan;
	public float PanBorderThickness;

	public GameObject FollowTarget { get; private set; }

	private Vector3 _initialPosition;

	private void Start()
	{
		_initialPosition = transform.position;
	}

	private void Update()
	{
		if (Input.GetButtonDown(ResetCameraButton))
		{
			transform.position = _initialPosition;
		}

		var input = new Vector2(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis));

		if (UseBorderPan)
		{
			Vector2 mouse = Input.mousePosition;
			if (mouse.x <= PanBorderThickness) input.x = -1;
			if (mouse.x >= Screen.width - PanBorderThickness) input.x = 1;
			if (mouse.y <= PanBorderThickness) input.y = -1;
			if (mouse.y >= Screen.height - PanBorderThickness) input.y = 1;
		}

		Transform t = transform;
		if (!Mathf.Approximately(input.magnitude, 0))
		{
			FollowTarget = null;
			if (input.magnitude > 1) input.Normalize();
			input *= PanSpeed;

			t.position = Bounds.ClosestPoint(t.position + t.TransformVector(input.x, 0, input.y));
		}
		else if (FollowTarget != null)
		{
			t.position = Bounds.ClosestPoint(FollowTarget.transform.position);
		}
	}

	public void Follow(GameObject target)
	{
		FollowTarget = target;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}