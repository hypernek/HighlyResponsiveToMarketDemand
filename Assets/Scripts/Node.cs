using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Transform selectionCircle;
    public SiteLabelPanel labelPanel;
    public string locationName;
    public int Yield;
    public int Danger;
    public int OutpostLevel;
    public Node[] neighbours;
    public bool visitedByLineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        visitedByLineRenderer = false;
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited node " + locationName);
        selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered node " + locationName);
        selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse over node " + locationName);
        selectionCircle.gameObject.SetActive(true);
        labelPanel.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse no longer over node " + locationName);
        selectionCircle.gameObject.SetActive(false);
        labelPanel.gameObject.SetActive(false);
    }

}
