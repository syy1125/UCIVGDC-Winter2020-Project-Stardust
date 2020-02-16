using System;
using UnityEngine;

public class CameraZoom : MonoBehaviour, IResetListener
{
	public string ZoomAxis;
	public float ZoomSpeed;
	public string ScrollAxis;
	public float ScrollZoomSpeed;

	public float MinZoom;
	public float MaxZoom;
	private float _zoom;
	private float _initialZoom;

	private void Start()
	{
		_initialZoom = _zoom = 1;
	}

	private void Update()
	{
		float input = Input.GetAxis(ZoomAxis) * ZoomSpeed + Input.GetAxis(ScrollAxis) * ScrollZoomSpeed;
		_zoom = Mathf.Clamp(_zoom * Mathf.Exp(-input * Time.deltaTime), MinZoom, MaxZoom);

		ApplyZoom();
	}

	private void ApplyZoom()
	{
		transform.localScale = Vector3.one * _zoom;
	}

	public Action<float> GetProgressResetAction()
	{
		float currentZoom = _zoom;
		return progress =>
		{
			_zoom = Mathf.Exp(Mathf.Lerp(Mathf.Log(currentZoom), Mathf.Log(_initialZoom), progress));
			ApplyZoom();
		};
	}
}