using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
	public string RotateButton;
	public string AzimuthAxis;
	public string ElevationAxis;
	public string ResetCameraButton;

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
		if (Input.GetButtonDown(ResetCameraButton))
		{
			_azimuth = _initialAzimuth;
			_elevation = _initialElevation;
		}
		
		if (Input.GetButton(RotateButton))
		{
			var input = new Vector2(Input.GetAxis(AzimuthAxis), Input.GetAxis(ElevationAxis));
			if (InvertAzimuth) input.x *= -1;
			if (InvertElevation) input.y *= -1;
			input *= RotateSpeed;

			_azimuth += input.x;
			_elevation += input.y;
		}
		
		transform.eulerAngles = new Vector3(_elevation, _azimuth, 0);
	}
}