using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMap : MonoBehaviour
{
    public Canvas worldCanvas;
    public Camera mainCamera;
    public SiteLabelPanel siteLabel;
    public Node[] mapNodes;
    

        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        foreach (Node node in mapNodes)
        {
            CreateLabel(node);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateLabel(Node node)
    {
        SiteLabelPanel newSiteLabelPanel = Instantiate<SiteLabelPanel>(siteLabel);
        RectTransform rectTransform = newSiteLabelPanel.GetComponent<RectTransform>();
        rectTransform.SetParent(worldCanvas.transform, false);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(node.transform.position);
        //rectTransform.anchoredPosition = new Vector2(node.transform.position.x, node.transform.position.y);
        rectTransform.anchoredPosition = new Vector2(screenPos.x, screenPos.y);

        Debug.Log("Put label at position " + rectTransform.anchoredPosition);

        newSiteLabelPanel.labelText.text = node.locationName;
        node.labelPanel = newSiteLabelPanel;
        newSiteLabelPanel.gameObject.SetActive(false);
    }
}

