using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Event Bus", fileName = "Event")]
public class EventBus : ScriptableObject
{
	private Action _event;

	public void Invoke()
	{
		_event?.Invoke();
	}

	public void AddListener(Action callback)
	{
		_event += callback;
	}

	public void RemoveListener(Action callback)
	{
		_event -= callback;
	}

	private void OnDestroy()
	{
		_event = null;
	}
}