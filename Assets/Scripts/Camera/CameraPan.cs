using System;
using UnityEngine;

public class CameraPan : MonoBehaviour, IResetListener, IFollowTargetListener
{
	public string HorizontalAxis;
	public string VerticalAxis;

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

			t.position = Bounds.ClosestPoint(t.position + t.TransformVector(input.x, 0, input.y) * Time.deltaTime);
		}
		else if (FollowTarget != null)
		{
			t.position = FollowTarget.transform.position;
		}
	}

	public Action<float> GetProgressResetAction()
	{
		Vector3 currentPosition = transform.position;
		return progress => transform.position = Vector3.Lerp(currentPosition, _initialPosition, progress);
	}

	public void OnEndReset()
	{
		FollowTarget = null;
	}

	public void Follow(GameObject target)
	{
		FollowTarget = target;
	}

	public Action<float> GetProgressFollowAction(GameObject target)
	{
		Vector3 currentPosition = transform.position;
		return progress => transform.position = Vector3.Lerp(currentPosition, target.transform.position, progress);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Bounds.center, Bounds.size);
	}
}