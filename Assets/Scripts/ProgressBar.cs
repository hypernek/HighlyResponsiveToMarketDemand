using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public int minimum;
    public int maximum;
    public int current;
    public int tickstep;
    public Image Mask;
    public RectTransform ProgressBarTicksPanel;
    public RectTransform rectTransform;
    public bool withSlider;
    public bool isVertical;
    public Image slider;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
        UpdateSliderPosition();
    }

    void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = (float)currentOffset / (float)maximumOffset;
        Mask.fillAmount = fillAmount;
    }
    void UpdateSliderPosition()
    {
        if(!withSlider)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            if(isVertical)
            {
                slider.rectTransform.anchoredPosition = new Vector2(0, + Mask.fillAmount * rectTransform.rect.height - rectTransform.rect.height);
            }
            else
            {
                slider.rectTransform.anchoredPosition = new Vector2(Mask.fillAmount * rectTransform.rect.width, 0);
            }
        }
    }

    
}
