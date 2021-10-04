using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistractionButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI distractionTextLabel;
    public Image currentImage;
    public Sprite defaultSprite;
    public Sprite preparedSprite;

    public void Init(string distractionText)
    {
        currentImage.sprite = defaultSprite;
        distractionTextLabel.text = distractionText;
        button.interactable = true;
    }

    public void Prepare()
    {
        currentImage.sprite = preparedSprite;
        distractionTextLabel.text = "Prepared";
        button.interactable = false;
    }

    public void Demolish(string newDistractionText)
    {
        Init(newDistractionText);
    }
}
