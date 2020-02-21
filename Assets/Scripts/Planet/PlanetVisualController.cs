using System;
using UnityEngine;

public class PlanetVisualController : MonoBehaviour
{
	public CelestialBody Body;

	private void OnEnable()
	{
		UpdatePosition();
		TurnAnimationController.Instance.OnTimeChanged.AddListener(UpdatePosition);
	}

	private void Start()
	{
		GetComponent<MeshRenderer>().material = Body.PlanetMaterial;
		transform.localScale = Vector3.one * Body.OutlineRadius;
	}

	private void UpdatePosition()
	{
		if (Body == null) return;

		transform.position = Body.GetGlobalPositionAndVelocityAt(TurnAnimationController.Instance.TurnTime).Item1;
	}

	private void OnDisable()
	{
		TurnAnimationController.Instance.OnTimeChanged.RemoveListener(UpdatePosition);
	}
}