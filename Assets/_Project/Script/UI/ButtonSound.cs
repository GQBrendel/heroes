using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play("MouseOver", true);
    }
}
