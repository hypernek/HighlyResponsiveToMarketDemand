using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;


public class CutsceneSystem : MonoBehaviour
{
    public UnityEvent event_CutsceneEnded;
    public UnityEvent event_CGzoomEnded;

    public GlobalStateManager stateManager;
    public CutsceneLoader cutsceneLoader;

    // class responsible for starting and reading dialog.
    private Queue<CutsceneItem> cutsceneLines;
    private Coroutine textAnimationCoroutine;
    private Coroutine CGZoomCoroutine;
    public AudioSource SpeakSource;
    public AudioSource SFXSource;
    public AudioSource musicSource;
    public bool speak;
 
    public int dialogSpeed;
    public int soundLoopPeriod;
    public float slowTextSpeedFactor;
    public float CGfadeTime;
    public TextMeshProUGUI textSpeakerNameLeft;
    public TextMeshProUGUI textDialog;
    public RectTransform speakerNamePanel;
    public Image speakerPortrait;
    public Button skipButton;

    public bool CutsceneInProgress { get; private set; }
    private bool NextLineBlocked = false;
    private bool WindowOpen = false;
    private RectTransform originalPortraitPosition;

    // no paramaters so that it can be linked to button
    public void StartIntroCutscene()
    {
        speakerNamePanel.gameObject.SetActive(false);
        textDialog.text = "";
        StartCutscene(cutsceneLoader.getCutsceneByName("INTRO_CUTSCENE"));
    }
    public void StartEndingCutsceneWin()
    {
        speakerNamePanel.gameObject.SetActive(false);
        textDialog.text = "";
        Debug.Log("Starting ending cutscene WIN");
        skipButton.gameObject.SetActive(false);
        StartCutscene(cutsceneLoader.getCutsceneByName("WIN_CUTSCENE"));
    }

    public void StartEndingCutsceneLoss()
    {
        speakerNamePanel.gameObject.SetActive(false);
        textDialog.text = "";
        Debug.Log("Starting ending cutscene LOSS");
        skipButton.gameObject.SetActive(false);
        StartCutscene(cutsceneLoader.getCutsceneByName("LOSS_CUTSCENE"));
    }

    void Start()
    {
        event_CutsceneEnded = new UnityEvent();
        cutsceneLines = new Queue<CutsceneItem>();
        CutsceneInProgress = false;
        cutsceneLines.Enqueue(new CutsceneItem("MIKO", "smug", "Of course I am on a bank note!"));
        SpeakSource = GetComponent<AudioSource>();
    }

    public void StartCutscene(Cutscene cutscene)
    {
        ResetCGZoomNoTween();
        cutsceneLines.Clear();
        foreach(CutsceneItem line in cutscene.Lines)
        {
            cutsceneLines.Enqueue(line);
        }
        Debug.Log("Starting Dialog " + cutscene.cutsceneChapter);
        CutsceneInProgress = true;
        DisplayNextLine();
    }

    public void SkipCutscene()
    {
        Debug.Log("Skipping Cutscene");
        StopCoroutine(textAnimationCoroutine);
        if (WindowOpen)
        {
            FindObjectOfType<ModalWindow>().Close();
            WindowOpen = false;
        }
        cutsceneLines.Clear();
        SpeakSource.Stop();
        EndCutscene();
    }


    public void EndCutscene()
    {
        CutsceneInProgress = false;
        Debug.Log("Cutscene ended");
        event_CutsceneEnded.Invoke();
        event_CutsceneEnded.RemoveAllListeners();
    }
    public void DisplayNextLine()
    {
        if (WindowOpen)
        {
            FindObjectOfType<ModalWindow>().Close();
            WindowOpen = false;
        }
        if (cutsceneLines.Count != 0)
        {
            CutsceneItem nextDialogItem = cutsceneLines.Dequeue();         
            if(nextDialogItem.SpeakerName == "Window")
            { 
                if (nextDialogItem.Mood == "imageonly")
                {
                    FindObjectOfType<ModalWindow>().ShowOnlyImage(nextDialogItem.Line);
                    WindowOpen = true;
                }
            }
            else if(nextDialogItem.SpeakerName == "CG")
            {
                StopAllCoroutines();
                LeanTween.cancelAll();
                Sprite nextCG = Resources.Load<Sprite>(String.Format("CGs/{0}", nextDialogItem.Mood));       
                if (nextCG != null)
                {
                    if (nextDialogItem.Line == "zoom1")
                    {
                        //NextLineBlocked = true;
                        LeanTween.scale(speakerPortrait.GetComponent<RectTransform>(), new Vector3(2.5f, 2.5f, 2.5f), 0f);
                        LeanTween.move(speakerPortrait.GetComponent<RectTransform>(), new Vector3(425f, -1437f, 10.0f), 0f);
                        CGZoomCoroutine = StartCoroutine(FadeInNextCG(nextCG));
                        event_CGzoomEnded.AddListener(() => unblockDialog());
                    }
                    else if (nextDialogItem.Line == "zoomOut1")
                    {
                        //NextLineBlocked = true;
                        LeanTween.scale(speakerPortrait.GetComponent<RectTransform>(), new Vector3(1.0f, 1.0f, 1.0f), CGfadeTime).
                            setEase(LeanTweenType.easeInCirc);
                        LeanTween.move(speakerPortrait.GetComponent<RectTransform>(), new Vector3(0f, -360f, 10.0f), CGfadeTime).
                            setOnComplete(() => event_CGzoomEnded.Invoke()).
                            setEase(LeanTweenType.easeInCirc);
                        event_CGzoomEnded.AddListener(() => unblockDialog());
                    }
                    else if (nextDialogItem.Line == "standard")
                    {
                        //NextLineBlocked = true;
                        ResetCGZoomNoTween();
                        CanvasGroup speakerPortraitCanvasGroup = speakerPortrait.GetComponent<CanvasGroup>();
                        LeanTween.alphaCanvas(speakerPortraitCanvasGroup, 0f, CGfadeTime / 2f).
                            setOnComplete(() => {
                                speakerPortrait.sprite = nextCG;
                                LeanTween.alphaCanvas(speakerPortraitCanvasGroup, 1.0f, CGfadeTime / 2f).
                                    setOnComplete(() => {
                                        event_CGzoomEnded.Invoke();
                                    }).
                                    setEase(LeanTweenType.easeInCirc);
                                }).
                            setEase(LeanTweenType.easeInCirc);
                    }
                    else
                    {
                        // no animation
                        speakerPortrait.sprite = nextCG;
                    }
                }
                else
                {
                    Debug.Log(string.Format("Failed to load resource at CGs/{0}", nextDialogItem.Mood));
                }
            }
            else if (nextDialogItem.SpeakerName == "Sound")
            {
                AudioClip nextClip = Resources.Load<AudioClip>(String.Format("SFX/Sounds/{0}", nextDialogItem.Mood));
                if (nextClip != null)
                {
                    if (nextDialogItem.Line == "noblock")
                    {
                        SFXSource.clip = nextClip;
                        SFXSource.Play();
                    }
                    else if (nextDialogItem.Line == "stopOther")
                    {
                        musicSource.Stop();
                        SFXSource.clip = nextClip;
                        SFXSource.Play();
                    }
                    else
                    {
                        SFXSource.clip = nextClip;
                        SFXSource.Play();
                    }
                }
                else
                {
                    Debug.Log(string.Format("Failed to load resource at SFX/Sounds/{0}", nextDialogItem.Mood));
                }
            }
            else
            {
                if (textAnimationCoroutine != null)
                {
                    StopCoroutine(textAnimationCoroutine);
                }
                if (nextDialogItem.SpeakerName != "Narrator")
                {
                    speakerNamePanel.gameObject.SetActive(true);
                    textSpeakerNameLeft.text = nextDialogItem.SpeakerName;
                }
                else
                {
                    // hide name
                    speakerNamePanel.gameObject.SetActive(false);
                }
                if(speak)
                {
                    SpeakSource.clip = Resources.Load<AudioClip>(String.Format("SFX/Voice/{0}", nextDialogItem.SpeakerName));
                }
                textAnimationCoroutine = StartCoroutine(AnimateLine(nextDialogItem));
                Sprite portrait;
                if (nextDialogItem.Mood != "noimage")
                {
                    portrait = Resources.Load<Sprite>(String.Format(
                   "Portraits/port_{0}_{1}", nextDialogItem.SpeakerName, nextDialogItem.Mood));
                    if (portrait == null)
                    {
                        Debug.Log(String.Format(
                            "Portrait of speaker \"{0}\" with mood \"{1}\" not found!", nextDialogItem.SpeakerName, nextDialogItem.Mood));
                        // hide portrait
                        speakerPortrait.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    }
                    else
                    {
                        speakerPortrait.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        speakerPortrait.sprite = portrait;
                    }
                } 
            }            
        }
        else
        {
            EndCutscene();
        }

    }
    IEnumerator AnimateLine(CutsceneItem ditem)
    {
        int counter = 0;
        bool playSoundNextLetter = true;
        bool slowText = false; // slow down for emphasis
        textDialog.text = "";
        char[] lineAsArray = ditem.Line.ToCharArray();
        for (int i = 0; i < lineAsArray.Length; i++)
        {
            char letter = lineAsArray[i];
            if (letter == '<')
            {
                while (letter != '>' && i < lineAsArray.Length)
                {
                    textDialog.text += letter;
                    i++;
                    letter = lineAsArray[i];
                }
                textDialog.text += letter;
                yield return null;
                continue;
            }
            else if (letter == '~')
            {
                if(slowText)
                {
                    slowText = false;
                }
                else
                {
                    slowText = true;
                }
                continue;
            }
            if (speak)
            {
                if (playSoundNextLetter)
                {
                    SpeakSource.Play();
                    playSoundNextLetter = false;
                }
                if (letter == ' ' || counter % soundLoopPeriod == soundLoopPeriod - 1)
                {
                    counter = 0;
                    playSoundNextLetter = true;
                }
                else
                {
                    counter++;
                }
            }              
                
            textDialog.text += letter;

            if (slowText)
                yield return new WaitForSeconds(1.0f / (dialogSpeed * slowTextSpeedFactor));
            else
                yield return new WaitForSeconds(1.0f / dialogSpeed);
        }
        textDialog.text += " ■";
        SpeakSource.Stop();
        yield break;
    }

    IEnumerator FadeOutInNextCG(Sprite nextCG)
    {
        Debug.Log("Fading Out and In CG");
        LeanTween.alphaCanvas(speakerPortrait.GetComponent<CanvasGroup>(), 0.0f, CGfadeTime).setOnComplete(
        () => {
            speakerPortrait.sprite = nextCG;
            LeanTween.alphaCanvas(speakerPortrait.GetComponent<CanvasGroup>(), 1.0f, CGfadeTime).setOnComplete(
                () => {
                    event_CGzoomEnded.Invoke();
                }).setEase(LeanTweenType.easeOutCirc);
        }).setEase(LeanTweenType.easeOutCirc);
        yield break;
    }

    IEnumerator FadeInNextCG(Sprite nextCG)
    {
        Debug.Log("Fading In CG");
        LeanTween.alphaCanvas(speakerPortrait.GetComponent<CanvasGroup>(), 0.0f, 0.0f).setOnComplete(
        () => {
            speakerPortrait.sprite = nextCG;
            LeanTween.alphaCanvas(speakerPortrait.GetComponent<CanvasGroup>(), 1.0f, CGfadeTime).setOnComplete(
                () => {
                    event_CGzoomEnded.Invoke();
                }).setEase(LeanTweenType.easeOutCirc);
        }).setEase(LeanTweenType.easeOutCirc);
        yield break;
    }

    IEnumerator zoomCGMima()
    {
        Debug.Log("Zooming Image");
        
        LeanTween.scale(speakerPortrait.GetComponent<RectTransform>(), new Vector3(2.0f, 2.0f, 2.0f), 0f);
        LeanTween.move(speakerPortrait.GetComponent<RectTransform>(), new Vector3(2.0f, 2.0f, 2.0f), 0f);
        yield break;
    }

    void ResetCGZoomNoTween()
    {
        RectTransform speakerRect = speakerPortrait.GetComponent<RectTransform>();
        LeanTween.scale(speakerRect, new Vector3(1.0f, 1.0f, 1.0f), 0f);
        LeanTween.move(speakerRect, new Vector3(0.0f, -360f, 0.0f), 0f);
    }

    void unblockDialog()
    {
        NextLineBlocked = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown("space") && !NextLineBlocked && CutsceneInProgress)
        {
            DisplayNextLine();
        }
    }
}
