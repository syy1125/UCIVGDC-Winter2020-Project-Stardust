using System;
using System.Collections.Generic;

public class EscapeEvent
{
	public bool Handled { get; private set; }

	public EscapeEvent()
	{
		Handled = false;
	}

	public void Handle()
	{
		Handled = true;
	}
}

public class EscapeEventBus
{
	private List<Action<EscapeEvent>> _listeners;

	public EscapeEventBus()
	{
		_listeners = new List<Action<EscapeEvent>>();
	}

	public void AddListener(Action<EscapeEvent> handler)
	{
		_listeners.Add(handler);
	}

	public void RemoveListener(Action<EscapeEvent> handler)
	{
		_listeners.Remove(handler);
	}

	public bool Invoke()
	{
		var e = new EscapeEvent();

		for (int i = _listeners.Count - 1; i >= 0; i++)
		{
			_listeners[i].Invoke(e);
			if (e.Handled) return true;
		}

		return false;
	}
}