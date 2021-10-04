using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ModalWindow : MonoBehaviour
{
    [Header("Header")]
    [SerializeField]
    private Transform windowHeaderArea;
    [SerializeField]
    private TextMeshProUGUI windowTitleText;

    [Header("Content")]
    [SerializeField]
    private Transform windowParentArea;
    [SerializeField]
    private Transform verticalContentArea;
    [SerializeField]
    private Image contentImage;
    [SerializeField]
    private TextMeshProUGUI contentText;

    [Header("Content")]
    [SerializeField]
    private Transform windowFooterArea;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button alternateButton;
    [SerializeField]
    private Button cancelButton;

    private Action action_Confirm;
    private Action action_Cancel;
    private Action action_Alternate;

    public void ShowOnlyImage(string imageToShowName)
    {
        ShowOnlyImage(Resources.Load<Sprite>(string.Format("Sprites/Other/{0}", imageToShowName)));
        Debug.Log(string.Format("Modal window showing image at Sprites/Other/{0}", imageToShowName));
    }
    public void ShowOnlyImage(Sprite imageToShow)
    {
        windowHeaderArea.gameObject.SetActive(false);
        windowFooterArea.gameObject.SetActive(false);
        verticalContentArea.gameObject.SetActive(true);
        contentImage.gameObject.SetActive(true);
        contentText.gameObject.SetActive(false);
        contentImage.sprite = imageToShow;
        Show();
    }

    public void ShowTextAndHeader(string title, string message, int fontSize = 34)
    {
        windowHeaderArea.gameObject.SetActive(true);
        windowTitleText.gameObject.SetActive(true);
        windowTitleText.text = title;
        verticalContentArea.gameObject.SetActive(true);
        contentImage.gameObject.SetActive(false);
        contentText.gameObject.SetActive(true);
        contentText.text = message;
        contentText.fontSize = fontSize;
        windowFooterArea.gameObject.SetActive(false);
        Show();
    }

    public void ShowTextAndHeaderAndConfirmButton(string title, string message, int fontSize = 34, Action confirm = null)
    {
        windowHeaderArea.gameObject.SetActive(true);
        windowTitleText.gameObject.SetActive(true);
        windowTitleText.text = title;
        verticalContentArea.gameObject.SetActive(true);
        contentImage.gameObject.SetActive(false);
        contentText.gameObject.SetActive(true);
        contentText.text = message;
        contentText.fontSize = fontSize;
        windowFooterArea.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        alternateButton.gameObject.SetActive(false);
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";
        Show();
    }

    public void Show()
    {
        windowParentArea.gameObject.SetActive(true);
    }

    public void Close()
    {
        windowParentArea.gameObject.SetActive(false);
    }

    public void Confirm()
    {
        action_Confirm?.Invoke();
        Close();
    }
    public void Cancel()
    {
        action_Cancel?.Invoke();
        Close();
    }
    public void Alternate()
    {
        action_Alternate?.Invoke();
        Close();
    }
}
