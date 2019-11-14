using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour {

    [System.Serializable]
    public class Action
    {
        public Color CircleColor;
        public Sprite sprite;
        public string title;
        public Color ImageColor;
        [HideInInspector] public bool OnCoolDown;
        [HideInInspector] public int CoolDownValue = 1;
    }

    [SerializeField] private Color _fadeColor;
    public string title;
    public Action[] options;

    public void FadeAction(string actionName, int coolDownTime = 0)
    {
        foreach (var action in options)
        {
            if(action.title == actionName)
            {
                action.ImageColor = _fadeColor;
                action.OnCoolDown = true;
                if(coolDownTime > 0)
                {
                    action.CoolDownValue = coolDownTime;
                }
                else
                {
                    action.OnCoolDown = false;
                }
            }
        }
    }
    public void RemoveFade(string actionName)
    {
        foreach (var action in options)
        {
            if (action.title == actionName)
            {
                action.ImageColor = Color.white;
                action.OnCoolDown = false;
            }
        }
    }

    private void Start()
    {
        if(title == "" || title == null)
        {
            title = gameObject.name;
        }
    }
    private void OnMouseDown()
    {
       // RadialMenuSpawner.instance.SpawnMenu(this);
    }
}
