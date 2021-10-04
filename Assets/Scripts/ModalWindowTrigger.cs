using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalWindowTrigger : MonoBehaviour
{
    public ModalWindow window;
    public string windowTitle;
    [Multiline(5)]
    public string windowContent;
    public int contentFontSize;
    public bool confirmButton;


    public void triggerWindow()
    {
        if(confirmButton)
        {
            window.ShowTextAndHeaderAndConfirmButton(windowTitle, windowContent, contentFontSize);
        }
        else
        {
            window.ShowTextAndHeader(windowTitle, windowContent, contentFontSize);
        }
    }

}
