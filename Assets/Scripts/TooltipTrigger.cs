using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;

    public string tooltipHeader;
    [Multiline()]
    public string tooltipContent;

    // UI interactions
    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () =>
        {
            TooltipSystem.Show(tooltipContent, tooltipHeader);
        });
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }


    // World space interactions (collider required)
    public void OnMouseEnter()
    {
        delay = LeanTween.delayedCall(0.5f, () =>
        {
            TooltipSystem.Show(tooltipContent, tooltipHeader);
        });
    }
    public void OnMouseExit()
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }
}
