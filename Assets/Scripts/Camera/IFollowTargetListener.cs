using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IFollowTargetListener : IEventSystemHandler
{
	bool enabled { set; }
	Action<float> GetProgressFollowAction(GameObject target);
	void Follow(GameObject target);
}