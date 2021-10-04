using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class GlobalStateManager : MonoBehaviour
{
    public static UnityEvent event_canvasFadeComplete = new UnityEvent();
    public AudioSource musicSource;
    public float GUIfadeTime;

    public Canvas tooltipCanvas;
    public Canvas UICanvas;
    public Canvas mainMenuCanvas;
    public Canvas cutsceneCanvas;
    public Canvas gameCanvas;

    public Button startButton;
    public CutsceneSystem cutsceneSystem;
    public CutsceneLoader cutsceneLoader;
    public GameSystem gameSystem;

    public ModalWindow modalWindow;

    private LTDescr ltdesc_canvasFadeIn;
    private LTDescr ltdesc_canvasFadeOut;
    // Update is called once per frame
    private void Start()
    {
        InitGameMainMenu();
        
    }

    public void InitGameMainMenu()
    {
        cutsceneCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        gameCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        mainMenuCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        cutsceneCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        gameCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        mainMenuCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        cutsceneCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
        FadeInMainMenuCanvas();
        event_canvasFadeComplete.AddListener(() => startButton.interactable = true);
        startButton.onClick.AddListener(EnableCutsceneCanvasFromMainMenu);
    }

    public void FadeInMainMenuCanvas()
    {
        modalWindow.Close();
        Debug.Log("Enabling Main Menu Canvas");
        StartCoroutine(CanvasTransition(mainMenuCanvas));
    }

    public void EnableCutsceneCanvasFromMainMenu()
    {
        StopAllCoroutines();
        LeanTween.cancelAll();
        musicSource.Play();
        modalWindow.Close();
        Debug.Log("Enabling Cutscene Canvas from Menu");
        event_canvasFadeComplete.RemoveAllListeners();
        event_canvasFadeComplete.AddListener(cutsceneSystem.StartIntroCutscene);
        cutsceneSystem.event_CutsceneEnded.AddListener(EnableGameCanvasFromCutscene);
        StartCoroutine(CanvasTransition(cutsceneCanvas, mainMenuCanvas));
    }

    public void EnableCutsceneCanvasFromGameWin()
    {
        //StopAllCoroutines();
        //LeanTween.cancelAll();
        modalWindow.Close();
        Debug.Log("Enabling Cutscene Canvas from Game");
        event_canvasFadeComplete.RemoveAllListeners();
        event_canvasFadeComplete.AddListener(cutsceneSystem.StartEndingCutsceneWin);
        StartCoroutine(CanvasTransition(cutsceneCanvas, gameCanvas));
        SwapCanvasesNoFade(cutsceneCanvas, gameCanvas);
        cutsceneSystem.StartEndingCutsceneWin();
    }

    public void EnableCutsceneCanvasFromGameLoss()
    {
        //StopAllCoroutines();
        //LeanTween.cancelAll();
        modalWindow.Close();
        Debug.Log("Enabling Cutscene Canvas from Game");
        event_canvasFadeComplete.RemoveAllListeners();
        event_canvasFadeComplete.AddListener(cutsceneSystem.StartEndingCutsceneLoss);
        //StartCoroutine(CanvasTransition(cutsceneCanvas, gameCanvas));
        SwapCanvasesNoFade(cutsceneCanvas, gameCanvas);
        cutsceneSystem.StartEndingCutsceneLoss();
    }



    public void EnableGameCanvasFromCutscene()
    {
        StopAllCoroutines();
        modalWindow.Close();
        Debug.Log("Enabling Game Canvas from Cutscene");
        event_canvasFadeComplete.RemoveAllListeners();
        LeanTween.cancelAll();
        event_canvasFadeComplete.AddListener(gameSystem.InitGame);
        // ugly hack
        gameSystem.event_GameEndedVictory.AddListener(EnableCutsceneCanvasFromGameWin);
        gameSystem.event_GameEndedLoss.AddListener(EnableCutsceneCanvasFromGameLoss);
        StartCoroutine(CanvasTransition(gameCanvas, cutsceneCanvas));

    }


    //public IEnumerator CanvasTransition(Canvas to, Canvas from = null)
    //{
    //    if (from != null)
    //    {
    //        yield return FadeCanvas(from, true);
    //    }
    //    if (to != null)
    //    {
    //        yield return FadeCanvas(to, false);
    //    }
    //    yield break;
    //}

    public IEnumerator CanvasTransition(Canvas destinationCanvas, Canvas startingCanvas = null)
    {
        //LeanTween.cancel(ltdesc_canvasFadeIn.uniqueId);
        //LeanTween.cancel(ltdesc_canvasFadeOut.uniqueId);
        //LeanTween.cancelAll();
        if (startingCanvas == null)
        {
            destinationCanvas.gameObject.SetActive(true);
            ltdesc_canvasFadeIn = LeanTween.alphaCanvas(destinationCanvas.GetComponent<CanvasGroup>(), 1.0f, GUIfadeTime).
            setOnComplete(() =>
            {
                destinationCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                Debug.Log("Fade in finished");
                event_canvasFadeComplete.Invoke();
            }).setEase(LeanTweenType.easeInCirc);
            yield return new WaitForSeconds(GUIfadeTime);
        }
        else
        {
            CanvasGroup startCanvasGroup = startingCanvas.GetComponent<CanvasGroup>();
            CanvasGroup destCanvasGroup = destinationCanvas.GetComponent<CanvasGroup>();

            GraphicRaycaster startGR = startingCanvas.GetComponent<GraphicRaycaster>();
            GraphicRaycaster destGR = destinationCanvas.GetComponent<GraphicRaycaster>();

            startGR.enabled = false;
            Debug.Log("Starting Fade Out");
            //startingCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            ltdesc_canvasFadeOut = LeanTween.alphaCanvas(startCanvasGroup, 0.0f, GUIfadeTime).
                setOnComplete(() => {
                    Debug.Log("Fade out complete");
                    destinationCanvas.gameObject.SetActive(true);
                    ltdesc_canvasFadeIn = LeanTween.alphaCanvas(destCanvasGroup, 1.0f, GUIfadeTime).
                    setOnComplete(() =>
                    {
                        destGR.enabled = true;
                        startingCanvas.gameObject.SetActive(false);
                        Debug.Log("Fade out fade in finished");
                        event_canvasFadeComplete.Invoke();
                    }).setEase(LeanTweenType.easeInCirc);
                }).setEase(LeanTweenType.easeOutCirc);
            yield return new WaitForSeconds(GUIfadeTime * 2);
        }
        
    }


    public void SwapCanvasesNoFade(Canvas destinationCanvas, Canvas startingCanvas = null)
    {
        StopAllCoroutines();
        LeanTween.cancelAll();
        CanvasGroup startCanvasGroup = startingCanvas.GetComponent<CanvasGroup>();
        CanvasGroup destCanvasGroup = destinationCanvas.GetComponent<CanvasGroup>();

        GraphicRaycaster startGR = startingCanvas.GetComponent<GraphicRaycaster>();
        GraphicRaycaster destGR = destinationCanvas.GetComponent<GraphicRaycaster>();

        startGR.enabled = false;
        destGR.enabled = true;
        destinationCanvas.gameObject.SetActive(true);
        startingCanvas.gameObject.SetActive(false);

        startCanvasGroup.alpha = 0.0f;
        destCanvasGroup.alpha = 1.0f;

    }


    IEnumerator FadeCanvas(Canvas canvasToFade, bool fadeOut)
    {
        if (fadeOut)
        {
            canvasToFade.GetComponent<GraphicRaycaster>().enabled = false;
            LeanTween.alphaCanvas(canvasToFade.GetComponent<CanvasGroup>(), 0f, GUIfadeTime).
                setOnComplete(() => {
                    canvasToFade.gameObject.SetActive(false);
                    Debug.Log("Fade out finished");
                }).setEase(LeanTweenType.easeOutCirc);
        }
        else
        {
            canvasToFade.gameObject.SetActive(true);
            LeanTween.alphaCanvas(canvasToFade.GetComponent<CanvasGroup>(), 1.0f, GUIfadeTime).
                setOnComplete(() =>
                {
                    canvasToFade.GetComponent<GraphicRaycaster>().enabled = true;
                    Debug.Log("Fade in finished");
                }).setEase(LeanTweenType.easeInCirc);
        }
        event_canvasFadeComplete.Invoke();
        yield break;
    }

   
    void Update()
    {
        
    }
}
