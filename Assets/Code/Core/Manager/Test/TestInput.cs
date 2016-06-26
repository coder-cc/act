using Core.Manager;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TestInput : MonoBehaviour , IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{


    private InputKeyState _mInputKeyState;


    void Start()
    {
        _mInputKeyState = InputManager.Instance.InputKeyStates[1];
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        //_mInputKeyState.OnKeyCodePress();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _mInputKeyState.OnKeyCodePress(true);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        _mInputKeyState.OnKeyCodePress(false);
    }


}
