using System.Collections.Generic;
using UnityEngine;

public class ShipQueueUI : GameUI
{
	public GameObject QueuedShipPrefab;
	public ShipConstructionUI ConstructionUI;

	public override void UpdateDisplay()
	{
		List<SpaceshipTemplate> constructionQueue = GameController.Instance.GetSelectedLogic().ShipQueue;

		int childCount = transform.childCount;
		if (childCount > constructionQueue.Count)
		{
			for (int i = childCount - 1; i >= constructionQueue.Count; i--)
			{
				Destroy(transform.GetChild(i).gameObject);
			}
		}
		else if (childCount < constructionQueue.Count)
		{
			for (int i = childCount; i < constructionQueue.Count; i++)
			{
				GameObject row = Instantiate(QueuedShipPrefab, transform);
				var shipUI = row.GetComponent<QueuedShipUI>();
				shipUI.Controller = this;
				shipUI.ConstructionUI = ConstructionUI;
			}
		}

		foreach (Transform row in transform)
		{
			row.GetComponent<QueuedShipUI>().UpdateDisplay();
		}
	}
}