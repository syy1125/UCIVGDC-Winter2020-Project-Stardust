using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CelestialBodyOutlineList : MonoBehaviour
{
	public GameObject PlanetOutlinePrefab;
	public float OverallScale = 2;
	public GameObject CameraControl;
	public float FollowDuration;
	public AnimationCurve FollowCurve;

	private GameObject _currentTarget;
	private Coroutine _followCoroutine;

	public void SpawnOutlinerItem(PlanetVisualController planet)
	{
		GameObject planetOutline = Instantiate(PlanetOutlinePrefab, transform);

		var planetRenderer = planetOutline.GetComponentInChildren<MeshRenderer>();
		planetRenderer.material = planet.Body.OutlineMaterial;
		planetRenderer.transform.localScale = Vector3.one * (planet.Body.OutlineRadius * OverallScale);

		var planetOutlineItem = planetOutline.GetComponent<CelestialBodyOutlineItem>();
		planetOutlineItem.OnClick.AddListener(
			() =>
			{
				if (_currentTarget == planet.gameObject) return;
				if (_followCoroutine != null) StopCoroutine(_followCoroutine);
				_followCoroutine = StartCoroutine(FollowPlanet(planet.gameObject));
			}
		);
	}

	private IEnumerator FollowPlanet(GameObject planet)
	{
		_currentTarget = planet;

		var listeners = new List<IFollowTargetListener>();
		ExecuteEvents.Execute<IFollowTargetListener>(CameraControl, null, (listener, _) => listeners.Add(listener));

		var actions = new List<Action<float>>();
		foreach (IFollowTargetListener listener in listeners)
		{
			listener.enabled = false;
			actions.Add(listener.GetProgressFollowAction(planet));
		}

		yield return new ForEachFrame(
			time =>
			{
				foreach (Action<float> action in actions)
				{
					action(FollowCurve.Evaluate(time));
				}
			},
			FollowDuration
		);

		foreach (IFollowTargetListener listener in listeners)
		{
			listener.Follow(planet);
			listener.enabled = true;
		}

		_currentTarget = null;
		_followCoroutine = null;
	}
}