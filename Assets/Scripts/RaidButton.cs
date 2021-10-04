using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class RaidButton : MonoBehaviour
{
    //public ProgressBar raidCooldownBar;
    public TextMeshProUGUI raidText;
    public int turnsToRefresh = 0; // 0 - ready, 5 - just raided
    public int localRaidPeriod = 0;
    public Image raidImage;
    public Sprite[] raidIndicators;

    public Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
    }

    public void ResetRaidIndicator(int raidCooldownPeriod)
    {
        //raidCooldownBar.minimum = 0;
        //raidCooldownBar.maximum = raidCooldownPeriod;
        //raidCooldownBar.current = raidCooldownPeriod;
        localRaidPeriod = raidCooldownPeriod;
        button.interactable = true;
        SetRaidLevel(0);
    }

    public void SetText(string rt)
    {
        raidText.text = rt;
    }

    public void SetRaidLevel(int raidLevel, string label = "Raided")
    {
        turnsToRefresh = raidLevel;
        if(raidLevel > 0)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;   
        }
        SetText(label);
        raidImage.sprite = raidIndicators[raidLevel];
    }

    public void Update()
    {

    }
}
