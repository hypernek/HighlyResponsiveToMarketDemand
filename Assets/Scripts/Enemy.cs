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
        if(Input.GetKeyDown(KeyCode.A))
        {
            enemyAnimator.Play("attack");
            audioSource.Play();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            enemyAnimator.SetBool("Moving", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            enemyAnimator.SetBool("Moving", true);
        }
    }

    public IEnumerator Attack(Vector2 targetPosition)
    {
        enemyAnimator.SetBool("Moving", true);
        tooltipTrigger.enabled = false;
        Vector2 newAnchor = targetPosition;
        Debug.Log(string.Format("Attacking position ({0}; {1})", newAnchor.x, newAnchor.y));
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
