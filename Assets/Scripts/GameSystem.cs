using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    public UnityEvent event_GameEndedVictory;
    public UnityEvent event_GameEndedLoss;

    public GameSystemInteract gameSystemInteract;
    public GlobalStateManager stateManager;
    // The game itself
    [Header("GAME RULES")]
    public int maxTurns;
    public int initialMoney;
    public int baseIncome;
    public int attackPeriod;

    [Header("GAME BALANCE")]
    public int difficultyModifier = 1;
    public int[] outpostIncomes = {1,2,5,10 };
    public int[] outpostPrices = {5,10,20,50 };
    public int distractionPrice = 25;
    public int minimumEvil = -2;
    public int maximumEvil = 8;
    public int initialEvil = 0;
    public int raidEvilCost = 2;
    public int raidGoldGain = -100;
    public int raidCooldownPeriod = 4;

    [Header("GAME OBJECTS")]
    public Site[] gameSites;
    public Machine machine;


    public int currentTurn { get; private set; }
    public int goldBalance { get; private set; }
    public int goldIncome { get; private set; }
    public int currentLevelOfEvil { get; private set; }

    void Start()
    {
        event_GameEndedVictory = new UnityEvent();
        event_GameEndedLoss = new UnityEvent();
    }

    void Update()
    {
        
    }

    public void InitGame()
    {
        goldBalance = initialMoney;
        goldIncome = baseIncome;
        foreach(Site s in gameSites)
        {
            s.InitSite();
        }
        gameSystemInteract.InitUI();
    }

    public void RecalculateIncome()
    {
        goldIncome = baseIncome;
        int i;
        foreach (Site s in gameSites)
        {
            i = 0;
            for(; i < s.outpostLevel; i++ )
            {
                goldIncome += outpostIncomes[i];
            }
            
        }
        Debug.Log(string.Format("Current income: {0}", goldIncome));
        gameSystemInteract.UpdateUI();
    }

    public bool CheckVictory()
    {
        if (goldBalance >= 1000)
            return true;
        else
        {
            machine.currentBuildLevel = goldBalance / 250;
            machine.UpdateImageAccordToBuildLevel();
            return false;
        }
            
    }

    public IEnumerator NextTurn()
    {
        if (CheckVictory())
        {
            EndGame(true);
            yield break;
        }
        else
        {
            if (currentTurn < maxTurns)
            {
                currentTurn++;               
                goldBalance += goldIncome; // gimme hard-earned cash
                foreach(Site s in gameSites)
                {
                    s.UpdateSiteRaid();
                }
                if (currentTurn % attackPeriod == 0 && currentTurn != 0)
                {
                    Debug.Log(string.Format("Turn {0} is divisible by {1}, performing attack", currentTurn, attackPeriod));
                    yield return PerformAttack(); // get rekt 
                }
                RecalculateIncome();
                gameSystemInteract.UpdateUI();
                gameSystemInteract.EnableUI();
            }
            else
            {
                Debug.Log("Time has run out, game ended");
                EndGame(false);
                yield break;
            }
        }
        yield break;
    }

    public IEnumerator PerformAttack()
    {
        gameSystemInteract.DisableUI();
        yield return new WaitForSeconds(1.0f);
        int attackStrengthTotal = currentLevelOfEvil / difficultyModifier;
        int[] siteAttackStrengths = new int[4];
        for (int i = 0; i < attackStrengthTotal; i++)
        {
            int nextSiteIndex = Random.Range(0, 4);
            siteAttackStrengths[nextSiteIndex]++;
        }

        Debug.Log(string.Format("Total attack strength: {0} - distributed as {1},{2},{3},{4}",
            attackStrengthTotal, siteAttackStrengths[0], siteAttackStrengths[1], siteAttackStrengths[2], siteAttackStrengths[3]));

        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(gameSites[i].AttackSite(siteAttackStrengths[i]));
        }
        gameSystemInteract.EnableUI();
        yield break;
    }


    public bool Purchase(int goldPrice, int evilPrice = 0)
    {
        if (evilPrice == 0)
        {
            if (goldPrice <= goldBalance)
            {
                goldBalance -= goldPrice;
                Debug.Log(string.Format("Subtracted {0} gold from balance, {1} gold remaining", goldPrice, goldBalance));
                gameSystemInteract.UpdateUI();
                return true;
            }
            else
            {
                Debug.Log(string.Format("Not enough gold to pay {0}", goldPrice));
                return false;
            }
        }
        else
        {
            int resultantEvil = currentLevelOfEvil + evilPrice;
            if (minimumEvil <= resultantEvil && resultantEvil <= maximumEvil)
            {
                currentLevelOfEvil = resultantEvil;
                goldBalance -= goldPrice;
                Debug.Log(string.Format("Moved {0:+#;-#;+0} on the evil scale, {1} gold remaining", evilPrice, goldBalance));
                gameSystemInteract.UpdateUI();
                return true;
            }
            else
            {
                if (resultantEvil < minimumEvil)
                    Debug.Log(string.Format("Cannot move on the evil scale, would pass absolute Good"));
                else
                    Debug.Log(string.Format("Cannot move on the evil scale, would pass absolute Evil"));
                return false;
            }
        }

    }
    public void EndGame(bool victory)
    {
        if (victory)
        {
            Debug.Log("You win! ^^");
            event_GameEndedVictory.Invoke();
        }
        else
        {
            Debug.Log("You lose! :(");
            event_GameEndedLoss.Invoke();
        }
        
    }
}
