using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetSettingsPanelActive(true);  
        }
    }

    public void SetSettingsPanelActive(bool state)
    {
        if (state == true)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SetCreditsPanelActive(bool state)
    {
        if (state == true)
        {
            creditsPanel.SetActive(true);
        }
        else
        {
            creditsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
