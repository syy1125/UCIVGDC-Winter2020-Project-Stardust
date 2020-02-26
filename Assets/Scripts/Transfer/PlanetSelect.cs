using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class PlanetSelect : MonoBehaviour
{
	private void Start()
	{
		var dropdown = GetComponent<Dropdown>();

		dropdown.options = GameController.Instance.State.StarSystem
			.Select(celestialBodyLogic => celestialBodyLogic.Body.DisplayName)
			.Select(bodyName => new Dropdown.OptionData(bodyName))
			.ToList();
	}
}