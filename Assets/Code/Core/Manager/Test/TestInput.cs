using UnityEngine;
using Aqua.InputEvent;
using UnityEngine.EventSystems;

public class TestInput : MonoBehaviour , IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{


    private InputStateBase _mInputStateBase;


    void Start()
    {
        _mInputStateBase = InputManager.Instance.InputStatesBase[1];
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        //_mInputStateBase.OnKeyCodePress();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _mInputStateBase.OnKeyCodePress(true);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _mInputStateBase.OnKeyCodePress(false);
    }


}
