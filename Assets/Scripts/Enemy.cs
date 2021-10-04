using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static UnityEvent event_enemyFlyCompleted = new UnityEvent();
    public AudioSource audioSource;

    public SpriteRenderer enemySpriteRenderer;
    public Animator enemyAnimator;
    public TooltipTrigger tooltipTrigger;

    public Transform enemyTransform;
    public float flyTime;
    public RectTransform SitePanel;
    public Transform ChosenSite;
    public RectTransform ChosenDistraction;
    public RectTransform ChosenOutpost1;

    private Vector3 startPosition;
    private static LTDescr move;

    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }

    private void Start()
    {
        startPosition = enemyTransform.position;
    }

    public void Update()
    {
        // this is how I found out how RectTransforms actually work
        /*
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(string.Format("Flying to SitePanel at anchored position ({0}; {1})", SitePanel.anchoredPosition.x, SitePanel.anchoredPosition.y));
            move = LeanTween.move(rectTransform, SitePanel.anchoredPosition, flyTime);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Vector2 target = SitePanel.anchoredPosition + ChosenSite.anchoredPosition;
            target.y -= -ChosenSite.rect.height / 2;
            Debug.Log(string.Format("Flying to Site at anchored position ({0}; {1})", target.x, target.y));
            move = LeanTween.move(rectTransform, target, flyTime);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector2 target = SitePanel.anchoredPosition + ChosenSite.anchoredPosition + ChosenSpecialPanel.anchoredPosition;
            target.y += ChosenSite.rect.height / 2;
            Debug.Log(string.Format("Flying to SpecialPanel at anchored position ({0}; {1})", target.x, target.y));
            move = LeanTween.move(rectTransform, target, flyTime);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector2 target = SitePanel.anchoredPosition + ChosenSite.anchoredPosition + ChosenOutpostPanel.anchoredPosition;
            target.y -= -ChosenSite.rect.height / 2;
            Debug.Log(string.Format("Flying to OutpostPanel at anchored position ({0}; {1})", target.x, target.y));
            move = LeanTween.move(rectTransform, target, flyTime);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector2 target = SitePanel.anchoredPosition + ChosenSite.anchoredPosition + ChosenSpecialPanel.anchoredPosition + ChosenDistraction.anchoredPosition;
            target.y += ChosenSite.rect.height / 2;
            target.x -= ChosenSpecialPanel.rect.width / 2; 
            Debug.Log(string.Format("Flying to Distraction at anchored position ({0}; {1})", target.x, target.y));
            move = LeanTween.move(rectTransform, target, flyTime);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Vector2 target = SitePanel.anchoredPosition + ChosenSite.anchoredPosition + ChosenOutpostPanel.anchoredPosition + ChosenOutpost1.anchoredPosition;
            target.y += ChosenSite.rect.height / 2;
            target.x -= ChosenOutpostPanel.rect.width / 2;
            Debug.Log(string.Format("Flying to Outpost 1 at anchored position ({0}; {1})", target.x, target.y));
            move = LeanTween.move(rectTransform, target, flyTime);
        }
        */
        if (Input.GetKeyDown(KeyCode.D))
        {
            //Vector2 target = new Vector2(
            //    ChosenSite.localPosition.x + ChosenDistraction.anchoredPosition.y,
            //    ChosenSite.localPosition.y + ChosenDistraction.anchoredPosition.y
            //    );
            Vector2 target = ChosenSite.localPosition + ChosenDistraction.localPosition;
            Debug.Log(string.Format("Flying to Distraction at anchored position ({0}; {1})", target.x, target.y));
            StartCoroutine(FlyTo(target));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            //Vector2 target = new Vector2(
            //    ChosenSite.localPosition.x + ChosenOutpost1.anchoredPosition.y,
            //    ChosenSite.localPosition.y + ChosenOutpost1.anchoredPosition.y
            //    );
            Vector2 target = ChosenSite.localPosition + ChosenOutpost1.localPosition;
            Debug.Log(string.Format("Flying to Outpost at anchored position ({0}; {1})", target.x, target.y));
            StartCoroutine(FlyTo(target));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(GoHome());
        }
        
    }

    public IEnumerator Attack(Vector2 targetPosition)
    {
        enemyAnimator.Play("move");
        tooltipTrigger.enabled = false;
        Vector2 newAnchor = targetPosition;
        Debug.Log(string.Format("Flying to position ({0}; {1})", newAnchor.x, newAnchor.y));
        LeanTween.move(enemyTransform.gameObject, newAnchor, flyTime).setOnComplete(() =>
        {
            enemyAnimator.Play("attack");
            audioSource.Play();
            LeanTween.scale(enemyTransform.gameObject, new Vector3(1.0f, 1.0f, 1.0f), flyTime / 3).setOnComplete(
                () => event_enemyFlyCompleted.Invoke()
                );
        }).setEase(LeanTweenType.easeInCubic);
        yield return new WaitForSeconds(flyTime * 2);
    }

    public IEnumerator FlyTo(Vector2 targetPosition, bool attack = false)//(RectTransform target)
    {
        enemyAnimator.SetBool("Moving", true);
        tooltipTrigger.enabled = false;
        Vector2 newAnchor = targetPosition;
        Debug.Log(string.Format("Flying to position ({0}; {1})", newAnchor.x, newAnchor.y));
        LeanTween.move(enemyTransform.gameObject, newAnchor, flyTime).setOnComplete(() => 
        {
            if(attack)
            {
                enemyAnimator.Play("attack");
                audioSource.Play();
            }
            enemyAnimator.SetBool("Moving", true);
            event_enemyFlyCompleted.Invoke();
        }).setEase(LeanTweenType.easeInCubic);
        yield return new WaitForSeconds(flyTime);
    }

    public IEnumerator GoHome()
    {
        enemyAnimator.Play("move");
        event_enemyFlyCompleted.RemoveAllListeners();
        Debug.Log(string.Format("Returning to Shrine"));
        event_enemyFlyCompleted.AddListener(
            () =>
            {
                enemyAnimator.SetBool("Moving", false);
            }
            );
        yield return FlyTo(startPosition);
        tooltipTrigger.enabled = true;
    }

    
   
        
}
