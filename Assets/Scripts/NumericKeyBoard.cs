using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class NumericKeyboard : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField _inputField;
    private TouchScreenKeyboard _keyboard;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.shouldHideMobileInput = false;
        _inputField.shouldHideSoftKeyboard = false;
        _inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!TouchScreenKeyboard.isSupported)
            return;

        _keyboard = TouchScreenKeyboard.Open(
            _inputField.text,
            TouchScreenKeyboardType.NumberPad,
            false,
            false,
            false,
            false
        );
    }

    private void Update()
    {
        if (_keyboard == null)
            return;

        if (_keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            _inputField.text = _keyboard.text;
            _keyboard = null;
        }
        else if (_keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            _keyboard = null;
        }
        else if (_keyboard.active)
        {
            _inputField.text = _keyboard.text;
        }
    }
}