using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour, IResetListener
{
	public string RotateButton;
	public string AzimuthAxis;
	public string ElevationAxis;

	public bool InvertAzimuth;
	public bool InvertElevation;

	public float RotateSpeed;

	private float _azimuth;
	private float _initialAzimuth;
	private float _elevation;
	private float _initialElevation;

	private void Start()
	{
		Vector3 eulerAngles = transform.eulerAngles;
		_initialAzimuth = _azimuth = eulerAngles.y;
		_initialElevation = _elevation = eulerAngles.x;
	}

	private void Update()
	{
		if (Input.GetButton(RotateButton))
		{
			var input = new Vector2(Input.GetAxis(AzimuthAxis), Input.GetAxis(ElevationAxis));
			if (InvertAzimuth) input.x *= -1;
			if (InvertElevation) input.y *= -1;
			input *= RotateSpeed;

			_azimuth += input.x * Time.deltaTime;
			_elevation += input.y * Time.deltaTime;
		}

		ApplyRotation();
	}

	private void ApplyRotation()
	{
		transform.eulerAngles = new Vector3(_elevation, _azimuth, 0);
	}

	public Action<float> GetProgressResetAction()
	{
		float currentAzimuth = _azimuth;
		float currentElevation = _elevation;
		return progress =>
		{
			_azimuth = Mathf.LerpAngle(currentAzimuth, _initialAzimuth, progress);
			_elevation = Mathf.Lerp(currentElevation, _initialElevation, progress);
			ApplyRotation();
		};
	}

	public void OnEndReset()
	{}
}