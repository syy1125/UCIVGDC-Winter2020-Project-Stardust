using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ResourceDisplayEntry
{
	public Resource Resource;
	public Text Display;
}

public class ResourceUI : MonoBehaviour
{
	[Header("References")]
	public Resource Energy;
	public Text PowerDisplay;
	public ResourceDisplayEntry[] ResourceDisplays;

	[Header("Config")]
	public Color DeficitWarningColor;
	public Color DeficitColor;

	private void OnEnable()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		GameController.Instance.OnEndAdvanceTurn.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		if (GameController.Instance.SelectedBody == null || !gameObject.activeSelf) return;

		CelestialBodyResources resources = GameController.Instance
			.State
			.StarSystem
			.SkipWhile(planet => planet.Body != GameController.Instance.SelectedBody)
			.First()
			.Resources;

		if (Energy != null)
		{
			int powerProduction = resources.GetRawProduction(Energy);
			int powerConsumption = resources.GetRawConsumption(Energy);

			PowerDisplay.text = $"{powerConsumption} / {powerProduction}";
			PowerDisplay.color = powerConsumption > powerProduction ? DeficitColor : Color.white;
		}

		Dictionary<Resource, int> resourceDelta = resources.GetIdealResourceDelta();
		Dictionary<Resource, int> resourceCapacity = resources.GetIdealResourceCapacity();

		foreach (ResourceDisplayEntry entry in ResourceDisplays)
		{
			int storage = resources[entry.Resource];
			resourceDelta.TryGetValue(entry.Resource, out int delta);
			resourceCapacity.TryGetValue(entry.Resource, out int capacity);

			entry.Display.text = $"{storage}{delta:+0;-0} / {capacity}";
			if (delta < 0 && storage + delta < 0)
			{
				entry.Display.color = DeficitColor;
			}
			else if (delta < 0 && storage + delta * 10 < 0)
			{
				entry.Display.color = DeficitWarningColor;
			}
			else
			{
				entry.Display.color = Color.white;
			}
		}
	}

	private void OnDisable()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnBodySelectionChanged.RemoveListener(UpdateDisplay);
			GameController.Instance.OnEndAdvanceTurn.RemoveListener(UpdateDisplay);
		}
	}
}