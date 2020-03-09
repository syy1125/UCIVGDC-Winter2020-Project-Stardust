using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraRotation : MonoBehaviour, IResetListener
{
	[FormerlySerializedAs("RotateButton")]
	public string MouseRotateButton;
	[FormerlySerializedAs("AzimuthAxis")]
	public string MouseAzimuthAxis;
	[FormerlySerializedAs("ElevationAxis")]
	public string MouseElevationAxis;
	public string KeyboardAzimuthAxis;

	[FormerlySerializedAs("RotateSpeed")]
	public float MouseRotateSpeed;
	public float KeyboardRotateSpeed;
	public float AzimuthFactor;
	public float ElevationFactor;

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
		Vector2 input = new Vector2(Input.GetAxis(KeyboardAzimuthAxis), 0) * KeyboardRotateSpeed;

		if (Input.GetButton(MouseRotateButton))
		{
			input += new Vector2(Input.GetAxis(MouseAzimuthAxis), Input.GetAxis(MouseElevationAxis)) * MouseRotateSpeed;
		}

		_azimuth += input.x * AzimuthFactor * Time.deltaTime;
		_elevation += input.y * ElevationFactor * Time.deltaTime;

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