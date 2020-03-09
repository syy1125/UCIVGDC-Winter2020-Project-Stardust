using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueuedShipUI : MonoBehaviour
{
	private int ShipIndex => transform.GetSiblingIndex();
	public Text ShipName;
	[NonSerialized]
	public ShipQueueUI Controller;
	[NonSerialized]
	public ShipConstructionUI ConstructionUI;

	public void UpdateDisplay()
	{
		List<SpaceshipTemplate> queue = GameController.Instance.GetSelectedLogic().ShipQueue;

		if (ShipIndex < queue.Count)
		{
			SpaceshipTemplate template = GameController.Instance.GetSelectedLogic().ShipQueue[ShipIndex];
			ShipName.text = template.DisplayName;
		}
		else
		{
			ShipName.text = "";
		}
	}

	public void CancelConstruction()
	{
		CelestialBodyLogic logic = GameController.Instance.GetSelectedLogic();
		SpaceshipTemplate template = logic.ShipQueue[ShipIndex];

		logic.ShipQueue.RemoveAt(ShipIndex);
		// Refund
		foreach (ShipCost cost in template.Costs)
		{
			logic.Resources[cost.Resource] += cost.Amount;
		}

		Controller.UpdateDisplay();
		ConstructionUI.UpdateDisplay();
	}
}