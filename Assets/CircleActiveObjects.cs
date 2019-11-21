using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleActiveObjects : MonoBehaviour
{
    [SerializeField] private ActionSelectorButton[] _buttons;
    private List<ActionSelectorButton> _activeButtons;

    void Awake()
    {
        _activeButtons = new List<ActionSelectorButton>();
       // CirculateButtons();
    }

    public void OnEnable()
    {
        _activeButtons.Clear();

        foreach (var button in _buttons)
        {
            if (button.Active)
            {
                _activeButtons.Add(button);
            }
        }
        CirculatePosition();
    }

    public void CirculatePosition()
    {
        for (int i = 0; i < _activeButtons.Count; i++)
        {
            float theta = (2 * Mathf.PI / _activeButtons.Count) * i;
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);
            _activeButtons[i].transform.localPosition = new Vector3(xPos, yPos, 0f) * 50f;
        }
    }
}
