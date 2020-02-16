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
	private float _zoom;

	private Vector3 _initialScale;

	private void Start()
	{
		_initialScale = transform.localScale;
		_zoom = 1;
	}

	private void Update()
	{
		if (Input.GetButtonDown(ResetCameraButton))
		{
			transform.localScale = _initialScale;
		}

		float input = Input.GetAxis(ZoomAxis) * ZoomSpeed + Input.GetAxis(ScrollAxis) * ScrollZoomSpeed;
		_zoom = Mathf.Clamp(_zoom * Mathf.Exp(-input * Time.deltaTime), MinZoom, MaxZoom);

		transform.localScale = Vector3.one * _zoom;
	}
}