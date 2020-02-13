using System;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
	public string ZoomAxis;
	public float ZoomSpeed;
	public string ScrollAxis;
	public float ScrollZoomSpeed;
	public string ResetCameraButton;

	public float MinZoom;
	public float MaxZoom;

	private Vector3 _initialPosition;

	private void Start()
	{
		_initialPosition = transform.localPosition;
	}

	private void Update()
	{
		if (Input.GetButtonDown(ResetCameraButton))
		{
			transform.localPosition = _initialPosition;
		}
		
		float input = Input.GetAxis(ZoomAxis) * ZoomSpeed + Input.GetAxis(ScrollAxis) * ScrollZoomSpeed;

		Transform t = transform;
		Vector3 position = t.localPosition;
		position.z = Mathf.Clamp(position.z * Mathf.Exp(-input), -MaxZoom, -MinZoom);
		t.localPosition = position;
	}
}