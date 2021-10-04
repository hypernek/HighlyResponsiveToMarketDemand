using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutsceneItem
{
    public string SpeakerName { get; private set; }
    public string Mood { get; private set; }
    public string Line { get; private set; }
    public CutsceneItem(string speakerName, string mood, string line)
    {
        SpeakerName = speakerName;
        Mood = mood;
        Line = line;
    }

}

[Serializable]
public class Cutscene
{
    public string cutsceneChapter;
    public List<CutsceneItem> Lines;

    public Cutscene(string cutsceneName, List<CutsceneItem> lines)
    {
        cutsceneChapter = cutsceneName;
        Lines = lines;
    }

    public void addLine(CutsceneItem ctscItem)
    {
        Lines.Add(ctscItem);
    }
}
