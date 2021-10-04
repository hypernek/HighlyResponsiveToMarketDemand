using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class GoodEvilSlider : MonoBehaviour
{
    public RectTransform TicksPanel;
    public Tick tick;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void prepareTicks()
    {
        Slider slider = GetComponent<Slider>();

        for (int i = (int)slider.minValue; i < (int)slider.maxValue; i++)
        {
            Tick newTick = Instantiate<Tick>(tick);
            newTick.transform.SetParent(TicksPanel, false);
        }

        TicksPanel.GetComponent<VerticalLayoutGroup>().spacing =
            (float)TicksPanel.rect.height / ((float)(slider.maxValue - slider.minValue)) - tick.GetComponent<RectTransform>().rect.height;
    }
}
