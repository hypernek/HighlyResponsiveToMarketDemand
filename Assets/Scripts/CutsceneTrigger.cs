using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public CutsceneLoader cutscLoader;
    public Cutscene dummyCutscene;
    public string cutsceneName;
    public void TriggerCutscene(string cutsceneName)
    {
        FindObjectOfType<CutsceneSystem>().StartCutscene(cutscLoader.getCutsceneByName(cutsceneName));
    }

}
