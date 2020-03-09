using System.Collections.Generic;
using UnityEngine;

public class OrbitingShipsUI : GameUI
{
	public GameObject ShipDisplayPrefab;

	public override void UpdateDisplay()
	{
		List<Spaceship> orbitingShips =
			GameController.Instance.State.Spaceships.FindAll(
				ship => ship.OrbitingBody == GameController.Instance.SelectedBody
			);

		int childCount = transform.childCount;
		if (childCount > orbitingShips.Count)
		{
			for (int i = childCount - 1; i >= orbitingShips.Count; i--)
			{
				Destroy(transform.GetChild(i).gameObject);
			}
		}
		else if (childCount < orbitingShips.Count)
		{
			for (int i = childCount; i < orbitingShips.Count; i++)
			{
				GameObject row = Instantiate(ShipDisplayPrefab, transform);
				var shipUI = row.GetComponent<ExistingShipRowUI>();
				shipUI.Controller = this;
			}
		}

		for (int i = 0; i < orbitingShips.Count; i++)
		{
			var row = transform.GetChild(i).GetComponent<ExistingShipRowUI>();
			row.SetShip(orbitingShips[i]);
			row.UpdateDisplay();
		}
	}
}