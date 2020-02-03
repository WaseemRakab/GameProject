using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/*Handling Right Clicks On LoadGameSlots*/
public class OnDeletingLoadGames : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent _OnDeleteLoadGame;
    public void OnPointerClick(PointerEventData ButtonClicked)
    {
        if (ButtonClicked.button == PointerEventData.InputButton.Right)
        {
            if (transform.GetChild(1).gameObject.activeSelf)//SaveGameSlot , Not EmptySlot
                _OnDeleteLoadGame?.Invoke();
        }
    }
}