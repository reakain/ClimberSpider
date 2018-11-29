using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitHandler : MonoBehaviour {

    public CanvasGroup uiCanvasGroup;
    public CanvasGroup confirmQuitCanvasGroup;
    public CanvasGroup splashCanvasGroup;

    bool splashMenuVisible = true;
    bool uiMenuVisible = false;
    bool confirmMenuVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiMenuVisible = !uiMenuVisible;
        }
        else if(Input.GetKeyDown(KeyCode.H))
        {
            splashMenuVisible = !splashMenuVisible;
        }

        splashCanvasGroup.alpha = 0;
        splashCanvasGroup.interactable = false;
        splashCanvasGroup.blocksRaycasts = false;

        uiCanvasGroup.alpha = 0;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;

        confirmQuitCanvasGroup.alpha = 0;
        confirmQuitCanvasGroup.interactable = false;
        confirmQuitCanvasGroup.blocksRaycasts = false;

        if (uiMenuVisible)
        {
            uiCanvasGroup.alpha = 1;
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;

            if (confirmMenuVisible)
            {
                uiCanvasGroup.alpha = 0.5f;
                confirmQuitCanvasGroup.alpha = 1;
                confirmQuitCanvasGroup.interactable = true;
                confirmQuitCanvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            confirmMenuVisible = false;
            if (splashMenuVisible)
            {
                splashCanvasGroup.alpha = 1;
                splashCanvasGroup.interactable = true;
                splashCanvasGroup.blocksRaycasts = true;
            }
        }
    }

    /*// Use this for initialization
    private void Awake()
    {
        //disable the quit confirmation panel
        DoConfirmQuitNo();
    }
    */
    /// <summary>
    /// Called if clicked on No (confirmation)
    /// </summary>
    public void DoConfirmQuitNo()
    {
        Debug.Log("Back to the game");

        confirmMenuVisible = false;

    }

    /// <summary>
    /// Called if clicked on Yes (confirmation)
    /// </summary>
    public void DoConfirmQuitYes()
    {
        Debug.Log("Ok bye bye");
        Application.Quit();
    }

    /// <summary>
    /// Called if clicked on Quit
    /// </summary>
    public void DoQuit()
    {
        Debug.Log("Check form quit confirmation");

        confirmMenuVisible = true;

    }

    /// <summary>
    /// Called if clicked on new game (example)
    /// </summary>
    public void DoNewGame()
    {
        Debug.Log("Launch a new game");
    }

    public void DoInstructionsClose()
    {
        splashMenuVisible = false;
    }
}
