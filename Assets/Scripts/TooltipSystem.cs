using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public Tooltip theTooltip;
    public float fadeDelay;
    private static LTDescr fade;
    // Start is called before the first frame update
    void Awake()
    {
        current = this;
        fade = LeanTween.alphaCanvas(current.theTooltip.canvasGroup, 0.0f, current.fadeDelay).setEase(LeanTweenType.easeOutCirc);
    }

    public static void Show(string content, string header = "")
    {
        current.theTooltip.SetText(content, header);
        current.theTooltip.gameObject.SetActive(true);
        current.theTooltip.Update();
        LeanTween.cancel(fade.uniqueId);
        fade = LeanTween.alphaCanvas(current.theTooltip.canvasGroup, 1.0f, current.fadeDelay).setEase(LeanTweenType.easeInCirc);
    }

    // Update is called once per frame
    public static void Hide()
    {
        LeanTween.cancel(fade.uniqueId);
        fade = LeanTween.alphaCanvas(current.theTooltip.canvasGroup, 0.0f, current.fadeDelay).setEase(LeanTweenType.easeInCirc);
        current.theTooltip.gameObject.SetActive(false);
    }
}
