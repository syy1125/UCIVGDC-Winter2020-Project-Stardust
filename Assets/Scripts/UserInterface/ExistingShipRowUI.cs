using System;
using UnityEngine;
using UnityEngine.UI;

public class ExistingShipRowUI : MonoBehaviour
{
	public Text ShipName;
	[NonSerialized]
	public OrbitingShipsUI Controller;
	private Spaceship _ship;

	public void SetShip(Spaceship ship)
	{
		_ship = ship;
	}

	public void UpdateDisplay()
	{
		ShipName.text = _ship.Template.DisplayName;
	}

	public void OpenTransferUI()
	{}
}