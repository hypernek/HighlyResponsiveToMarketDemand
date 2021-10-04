using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Site : MonoBehaviour
{
    public GameSystem gameSystem;
    public Transform siteTransform;
    public string siteName;
    public int outpostLevel;
    public int turnsUntilRaidRefresh; // the current level of raid cooldown
    public bool raided;
    public bool hasDistraction;
    public Outpost[] outposts;
    public RaidButton raidButton;
    public DistractionButton distraction;

    [Header("Enemy")]
    public Enemy enemy;

    private Coroutine attackRoutine;
    private RectTransform siteRectTransform;

    // Start is called before the first frame update
    void Awake()
    {
        //InitSite();
        siteRectTransform = GetComponent<RectTransform>();
        //sitePanelRectTransform = GetComponentInParent<RectTransform>();
    }

    public void InitSite()
    {
        for (int i = 0; i < outposts.Length; i++)
        {
            outposts[i].outpostText.text = string.Format("+{0}\n{1} G",
                gameSystem.outpostIncomes[i], gameSystem.outpostPrices[i]);
        }

        outposts[0].available = true;
        for (int i = 1; i < outposts.Length ; i++)
        {
            outposts[i].available = false;
        }
        outpostLevel = 0;
        turnsUntilRaidRefresh = 0;
        raided = false;
        UpdateOutposts();
        raidButton.SetRaidLevel(0, string.Format("Raid\n+{0} G\n+{1} Evil",
                -gameSystem.raidGoldGain, gameSystem.raidEvilCost));
    }

    public void BuyOutpost()
    {
        bool success = gameSystem.Purchase(gameSystem.outpostPrices[outpostLevel]);
        if (success)
        {
            if (outpostLevel == outposts.Length - 1)
            {
                outposts[outpostLevel].purchased = true;
            }
            else
            {
                outposts[outpostLevel].purchased = true;
                outposts[outpostLevel + 1].available = true;
            }
            outpostLevel++;
            UpdateOutposts();
            gameSystem.RecalculateIncome();
        }
    }
    
    void UpdateOutposts()
    {
        foreach(Outpost outpost in outposts)
        {
            outpost.updateOutpostState();
        }
    }

    public void UpdateSiteRaid()
    {
        Debug.Log("Update Site Raid");
        if (turnsUntilRaidRefresh > 0)
        {
            turnsUntilRaidRefresh--;
            if(turnsUntilRaidRefresh > 0)
            {
                raidButton.SetRaidLevel(turnsUntilRaidRefresh);
            }
            else
            {
                raidButton.SetRaidLevel(turnsUntilRaidRefresh, string.Format("Raid +{0} G +{1} Evil", 
                    -gameSystem.raidGoldGain, gameSystem.raidEvilCost));
            }
        }
        else
        {
            Debug.Log("Raid button refreshed");
            raidButton.SetRaidLevel(turnsUntilRaidRefresh, string.Format("Raid +{0} G +{1} Evil",
                -gameSystem.raidGoldGain, gameSystem.raidEvilCost));
        }
    }
    public void RaidSite()
    {
        bool success = gameSystem.Purchase(gameSystem.raidGoldGain, gameSystem.raidEvilCost);
        if(success)
        {
            turnsUntilRaidRefresh = gameSystem.raidCooldownPeriod;
            raidButton.SetRaidLevel(turnsUntilRaidRefresh);
            Debug.Log(string.Format("Site {0} raided", siteName));
        }
        else
        {
            Debug.Log(string.Format("Site {0} cannot be raided", siteName));
        }
    }

    public void PrepareDistraction()
    {
        bool success = gameSystem.Purchase(gameSystem.distractionPrice, 0);
        if (success)
        {
            hasDistraction = true;
            distraction.Prepare();
            Debug.Log(string.Format("Distraction successfully prepared at Site {0}", siteName));
        }
        else
        {
            Debug.Log(string.Format("Distraction cannot be prepared at Site {0}", siteName));
        }
    }

    public IEnumerator AttackSite(int attackStrength)
    {
        if(attackStrength == 0)
        {
            yield break;
        }
        else
        {
            if (hasDistraction)
            {
                RectTransform distractionRect = distraction.GetComponent<RectTransform>();
                //Debug.Log(string.Format("SitePanel anchoredPosition: ({0}; {1})", sitePanelRectTransform.anchoredPosition.x, sitePanelRectTransform.anchoredPosition.y));
                //Debug.Log(string.Format("Site anchoredPosition: ({0}; {1})", siteRectTransform.anchoredPosition.x, siteRectTransform.anchoredPosition.y));
                //Debug.Log(string.Format("specialPanel anchoredPosition: ({0}; {1})", specialPanel.anchoredPosition.x, specialPanel.anchoredPosition.y));
                //Debug.Log(string.Format("distraction anchoredPosition: ({0}; {1})", distractionRect.anchoredPosition.x, distractionRect.anchoredPosition.y));

                Vector2 target = siteTransform.localPosition + distractionRect.localPosition;
                yield return StartCoroutine(enemy.Attack(target));
                //enemy.event_enemyFlyCompleted.AddListener(() => StartCoroutine(enemy.GoHome()));
                hasDistraction = false;
                distraction.Demolish(string.Format("Prepare\nDistraction\n{0} G", gameSystem.distractionPrice));
                Debug.Log(string.Format("Site {0} has Distraction, attack successfully prevented", siteName));
                yield return StartCoroutine(enemy.GoHome());
            }
            else
            {
                int destroyedOutposts = 0;
                Debug.Log(string.Format("Site {0} has no Distraction, attacking {1} times", siteName, attackStrength));
                for (int i = 0; i < attackStrength; i++)
                {
                    if (outpostLevel > 0)
                    {
                        RectTransform outpostRect = outposts[outpostLevel - 1].GetComponent<RectTransform>();
                        // remove destroyed outpost
                        //Debug.Log(string.Format("SitePanel anchoredPosition: ({0}; {1})", sitePanelRectTransform.anchoredPosition.x, sitePanelRectTransform.anchoredPosition.y));
                        //Debug.Log(string.Format("Site anchoredPosition: ({0}; {1})", siteRectTransform.anchoredPosition.x, siteRectTransform.anchoredPosition.y));
                        //Debug.Log(string.Format("outpostPanel anchoredPosition: ({0}; {1})", outpostPanel.anchoredPosition.x, outpostPanel.anchoredPosition.y));
                        //Debug.Log(string.Format("outpost anchoredPosition: ({0}; {1})", outpostRect.anchoredPosition.x, outpostRect.anchoredPosition.y));

                        //Vector2 target = sitePanelRectTransform.anchoredPosition +
                        //                  siteRectTransform.anchoredPosition +
                        //                  outpostPanel.anchoredPosition +
                        //                  outpostRect.anchoredPosition;
                        //target.y += siteRectTransform.rect.height / 2;
                        //target.x -= outpostPanel.rect.width / 2;
                        Vector2 target = siteTransform.localPosition + outpostRect.localPosition;
                        yield return StartCoroutine(enemy.Attack(target));
                        //enemy.event_enemyFlyCompleted.AddListener(() => StartCoroutine(enemy.GoHome()));
                        outposts[outpostLevel - 1].purchased = false;
                        outposts[outpostLevel - 1].available = true;
                        if (outpostLevel != outposts.Length)
                        {
                            outposts[outpostLevel].available = false;
                        }
                        outpostLevel--;
                        destroyedOutposts++;
                        UpdateOutposts();
                    }
                    else
                    {
                        //yield return enemy.FlyTo(outposts[0].GetComponent<RectTransform>().position);

                    }


                }
                Debug.Log(string.Format("{0} outposts destroyed", destroyedOutposts));
                if(destroyedOutposts > 0)
                {
                    gameSystem.Purchase(0, - destroyedOutposts);
                }
                yield return StartCoroutine(enemy.GoHome());
            }
        }
        yield break;
    }
}
