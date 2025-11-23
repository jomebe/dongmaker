using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundReturn : MonoBehaviour, IPointerClickHandler
{
    public GameMenuController gameMenuController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameMenuController != null)
        {
            gameMenuController.OpenMainPanel();
        }
    }
}
