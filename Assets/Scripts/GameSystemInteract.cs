using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameSystemInteract : MonoBehaviour
{
    // Takes care of forwarding user input to the game system and updating the UI to reflect the game state.
    public EventSystem eventSystem;
    public Canvas gameCanvas;
    public ModalWindow modalWindow;
    public GameSystem gameSystem;
    public Button endTurnButton;
    public TextMeshProUGUI turnText;
    public ProgressBar timelineBar;
    public ProgressBar goodEvilBar;
    public TextMeshProUGUI goldBalanceText;
    public TextMeshProUGUI goldIncomeText;

    public Canvas TooltipCanvas;

    private bool inputEnabled = false;

    void Start()
    {
        
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && inputEnabled)
        {
            RequestNextTurn();
        }
    }

    public void RequestNextTurn()
    {
        DisableUI();
        StartCoroutine(gameSystem.NextTurn());
        eventSystem.SetSelectedGameObject(null); // deselect everything       
    }

    public void UpdateUI()
    {
        timelineBar.current = gameSystem.currentTurn;
        turnText.text = string.Format("{0}/{1}", gameSystem.currentTurn, gameSystem.maxTurns);
        
        goldBalanceText.text = string.Format("{0}", gameSystem.goldBalance);
        goldIncomeText.text = WriteGoldIncome(gameSystem.goldIncome);

        goodEvilBar.current = gameSystem.currentLevelOfEvil;
    }
  
    public void InitUI()
    {
        // init and populate with values
        timelineBar.minimum = 0;
        timelineBar.maximum = gameSystem.maxTurns;
        timelineBar.current = 0;
        turnText.text = string.Format("{0}/{1}", gameSystem.currentTurn, gameSystem.maxTurns);
        
        goldBalanceText.text = string.Format("{0}", gameSystem.goldBalance);
        goldBalanceText.text = string.Format("{0}",gameSystem.initialMoney);
        goldIncomeText.text = WriteGoldIncome(gameSystem.baseIncome);

        goodEvilBar.minimum = gameSystem.minimumEvil;
        goodEvilBar.maximum = gameSystem.maximumEvil;
        goodEvilBar.current = gameSystem.initialEvil;

        endTurnButton.enabled = true;

        EnableUI();
    }

    public void EnableUI()
    {
        inputEnabled = true;
        gameCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        TooltipCanvas.gameObject.SetActive(true);
    }

    public void DisableUI()
    {
        inputEnabled = false;
        gameCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        TooltipCanvas.gameObject.SetActive(false);
    }


    public string WriteGoldIncome(int gain)
    {
        return string.Format("(+{0}/turn)", gain);
    }
}
