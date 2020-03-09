using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipQueueUI : MonoBehaviour
{
	public GameObject QueuedShipPrefab;
	public ShipConstructionUI ConstructionUI;

	private void OnEnable()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		GameController.Instance.OnEndAdvanceTurn.AddListener(UpdateDisplay);
		UpdateDisplay();
	}
	
	public void UpdateDisplay()
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

	private void OnDisable()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnBodySelectionChanged.RemoveListener(UpdateDisplay);
			GameController.Instance.OnEndAdvanceTurn.RemoveListener(UpdateDisplay);
		}
	}
}