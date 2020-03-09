using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShipConstructionUI : MonoBehaviour, ICanSelectIndex
{
	public GameObject ButtonPrefab;
	public SpaceshipTemplate[] Ships;
	private GameObject[] _buttons;
	public ShipQueueUI QueueUI;

	private void Awake()
	{
		_buttons = new GameObject[Ships.Length];

		for (int i = 0; i < Ships.Length; i++)
		{
			_buttons[i] = Instantiate(ButtonPrefab, transform);

			var buttonController = _buttons[i].GetComponent<IndexSelectionButton>();
			buttonController.Controller = this;
			buttonController.Index = i;

			_buttons[i].GetComponentInChildren<Text>().text = Ships[i].DisplayName;
		}
	}

	private void OnEnable()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		GameController.Instance.OnEndAdvanceTurn.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	private static CelestialBodyResources GetSelectedResources()
	{
		return GameController.Instance.GetSelectedLogic().Resources;
	}

	public void UpdateDisplay()
	{
		CelestialBodyResources resources = GetSelectedResources();

		foreach (GameObject button in _buttons)
		{
			button.GetComponent<Button>().interactable = CanAfford(
				resources,
				Ships[button.GetComponent<IndexSelectionButton>().Index]
			);
		}
	}

	private static bool CanAfford(CelestialBodyResources resources, SpaceshipTemplate template)
	{
		return template.Costs.All(cost => resources[cost.Resource] >= cost.Amount);
	}

	public void SelectIndex(int index)
	{
		ConstructShip(Ships[index]);
	}

	private void ConstructShip(SpaceshipTemplate template)
	{
		CelestialBodyResources resources = GetSelectedResources();

		if (!CanAfford(resources, template))
		{
			Debug.LogError($"Tried to construct {template.DisplayName} but can't afford!");
			return;
		}

		foreach (ShipCost cost in template.Costs)
		{
			resources[cost.Resource] -= cost.Amount;
		}

		GameController.Instance.GetSelectedLogic().ShipQueue.Add(template);

		UpdateDisplay();
		QueueUI.UpdateDisplay();
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