using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Machine : MonoBehaviour
{
    //public ProgressBar raidCooldownBar;
    public int currentBuildLevel;
    public Image buildImage;
    public Sprite[] buildStages;

    public void Awake()
    {

    }

    public void UpdateImageAccordToBuildLevel()
    {
        if (currentBuildLevel > buildStages.Length - 1)
            currentBuildLevel = buildStages.Length - 1;
        else if (currentBuildLevel < 0)
            currentBuildLevel = 0;
        buildImage.sprite = buildStages[currentBuildLevel];
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            UpdateImageAccordToBuildLevel();
        }
    }
}
