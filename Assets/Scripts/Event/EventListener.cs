using System;
using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
	public EventBus EventBus;
	public bool ActiveWhenDisabled = true;
	public UnityEvent OnEvent = new UnityEvent(); 

	private void Awake()
	{
		EventBus.AddListener(EventHandler);
	}

	private void EventHandler()
	{
		if (enabled || ActiveWhenDisabled)
		{
			OnEvent.Invoke();
		}
	}

	private void OnDestroy()
	{
		EventBus.RemoveListener(EventHandler);
	}
}
