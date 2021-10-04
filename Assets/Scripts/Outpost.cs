using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Outpost : MonoBehaviour
{
    public bool available;
    public bool purchased;
    public TextMeshProUGUI outpostText;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void updateOutpostState()
    {
        if (purchased)
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            if (available)
            {
                GetComponent<Image>().color = Color.white;
                GetComponent<Button>().enabled = true;
                GetComponent<Button>().interactable = true;
            }
            else
            {
                GetComponent<Image>().color = Color.black;
                GetComponent<Button>().enabled = false;
                GetComponent<Button>().interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
