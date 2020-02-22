﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IResetListener : IEventSystemHandler
{
	bool enabled { set; }
	Action<float> GetProgressResetAction();
	void OnEndReset();
}
