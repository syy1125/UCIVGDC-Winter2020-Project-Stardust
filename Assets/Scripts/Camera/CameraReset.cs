using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraReset : MonoBehaviour
{
	public string ResetButton;
	public float ResetDuration;
	public AnimationCurve ResetCurve;
	private bool _active;

	private void Start()
	{
		_active = true;
	}

	private void Update()
	{
		if (_active && Input.GetButtonDown(ResetButton))
		{
			StartCoroutine(DoReset());
		}
	}

	private IEnumerator DoReset()
	{
		_active = false;

		// Why do it this way? It's explained later in this function.
		var listeners = new List<IResetListener>();
		ExecuteEvents.Execute<IResetListener>(
			gameObject,
			null,
			(listener, _) => listeners.Add(listener)
		);

		var actions = new List<Action<float>>();
		foreach (IResetListener listener in listeners)
		{
			listener.enabled = false;
			actions.Add(listener.GetProgressResetAction());
		}

		yield return new ForEachFrame(
			time =>
			{
				foreach (Action<float> action in actions)
				{
					action(ResetCurve.Evaluate(time));
				}
			},
			ResetDuration
		);

		// Can't use ExecuteEvents here because it doesn't trigger on disabled components. Has to loop over the initial listeners.
		// So I made the initial disable code match this as well.
		foreach (IResetListener listener in listeners)
		{
			listener.enabled = true;
		}

		_active = true;
	}
}