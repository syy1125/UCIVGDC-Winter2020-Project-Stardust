using UnityEngine.EventSystems;

public interface ITurnLogicListener : IEventSystemHandler
{
	void DoTurnLogic();
}
