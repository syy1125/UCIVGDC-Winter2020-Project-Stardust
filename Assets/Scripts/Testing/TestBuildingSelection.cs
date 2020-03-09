using UnityEngine;

public class TestBuildingSelection : MonoBehaviour
{
	private bool _loaded;

	private void Update()
	{
		if (Input.GetButtonDown("Submit"))
		{
			var selectionUI = GetComponent<BuildingSelectionUI>();
			if (_loaded)
			{
				selectionUI.UnloadBuildings();
				_loaded = false;
			}
			else
			{
				_loaded = true;
			}
		}
	}
}