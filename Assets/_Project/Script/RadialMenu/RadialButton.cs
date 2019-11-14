using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image circle;
    public Image icon;
    public string title;
    public RadialMenu myMenu;
    public float speed = 8f;
    public TextMeshProUGUI _coolDownText;

    Color defaultColor;

    public void Animate()
    {
        StartCoroutine(AnimateIn());
    }
    IEnumerator AnimateIn()
    {
        transform.localScale = Vector3.zero;
        float timer = 0;
            
        while(timer < 1/speed)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.one * timer;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        defaultColor = circle.color;
        circle.color = Color.white;
        myMenu.selected = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        circle.color = defaultColor;
        myMenu.selected = null;
    }
}
