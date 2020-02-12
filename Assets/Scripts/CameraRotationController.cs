using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
	public string RotateButton;
	public string AzimuthAxis;
	public string ElevationAxis;

	public bool InvertAzimuth;
	public bool InvertElevation;

	public float RotateSpeed;

	private float _azimuth;
	private float _elevation;

	private void Start()
	{
		Vector3 eulerAngles = transform.eulerAngles;
		_azimuth = eulerAngles.y;
		_elevation = eulerAngles.x;
	}

	private void Update()
	{
		if (!Input.GetButton(RotateButton)) return;

		var input = new Vector2(Input.GetAxis(AzimuthAxis), Input.GetAxis(ElevationAxis));
		if (InvertAzimuth) input.x *= -1;
		if (InvertElevation) input.y *= -1;
		input *= RotateSpeed;

		_azimuth += input.x;
		_elevation += input.y;
		transform.eulerAngles = new Vector3(_elevation, _azimuth, 0);
	}
}