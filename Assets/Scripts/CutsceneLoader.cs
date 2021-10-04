using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CutsceneLoader : MonoBehaviour
{
    public TextAsset chapterTextFile;
    private Dictionary<string,Cutscene> loadedCutscenes;
    // Start is called before the first frame update
    void Start()
    {
        loadedCutscenes = new Dictionary<string, Cutscene>();
        loadAndParseCutscenes();
    }

    // Parse dialog
    public void loadAndParseCutscenes()
    {
        StringReader reader = new StringReader(chapterTextFile.text);
        string line;
        bool middleOfCutscene = false;
        Cutscene newCutscene;
        string cutsceneName = "";
        while ((line = reader.ReadLine()) != null)
        {
            // ignore comments 
            if(line.StartsWith("#"))
            {
                continue;
            }
            // find beginning of dialog
            else if (line.StartsWith("@") && !middleOfCutscene )
            {
                cutsceneName = line.Substring(1);
                Debug.Log("Found start of cutscene with name: " + cutsceneName);

                newCutscene = new Cutscene(cutsceneName, new List<CutsceneItem>());
                middleOfCutscene = true;

                // process this whole dialog
                while ((line = reader.ReadLine())!= null)
                {
                    line = line.TrimStart(' ', '\t');
                    // ignore comments 
                    if (line.StartsWith("#"))
                    {
                        Debug.Log("Continuing on comment line");
                        continue;
                    }
                    else if (line.StartsWith("@@"))
                    {
                        middleOfCutscene = false;
                        loadedCutscenes.Add(cutsceneName, newCutscene);
                        Debug.Log(string.Format("Cutscene {0} parsed and loaded into dictionary", cutsceneName));
                        break;
                    }
                    else 
                    {
                        if (line.Length < 2)
                            continue;
                        string[] splits = { };
                        if (line.StartsWith("{"))
                        {
                            splits = line.Substring(1).Split('{', '|', '}');
                            Debug.Log(string.Format("Read Action Line {0}, {1}, {2}", splits[0], splits[1], splits[2]));
                        }   
                        else if (line.StartsWith("["))
                        {
                            splits = line.Substring(1).Split('[', '|', ']');
                            // sanit(ar)y check
                            //Debug.Log(string.Format("Read Dialog line {0}, {1}, {2}", splits[0], splits[1], splits[2]));
                        }

                        if (splits.Length < 3 || splits[0] == "" || splits[1] == "" || splits[2] == "")
                        {
                            Debug.Log("Continuing on empty line");
                            continue;
                        }
                        newCutscene.addLine(new CutsceneItem(splits[0], splits[1], splits[2].TrimStart()));
                    }
                }
            }
        }
        if (middleOfCutscene)
            throw new System.Exception(string.Format(
                "Error when parsing Dialog! Reached EOF before dialog with name {0} was properly terminated!", cutsceneName));
        reader.Close();
    }

    public Cutscene getCutsceneByName(string dialogName)
    {
        return loadedCutscenes[dialogName];
    }

}
