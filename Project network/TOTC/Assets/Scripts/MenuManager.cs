using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public SurveyHandler surveyHandler;

    private void Start()
    {
        if (GameObject.Find("AnalyticsManager") == null)
        {
            AnalyticsManager.Instance();
        }
    }

    public void LocalMode()
    {
        AnalyticsManager.Instance().LogEvent("Player Chose Local Mode",0,0);
        SceneManager.LoadScene("GameSetup");
    }

    public void PlayLocalMode()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Multiplayer()
    {
        AnalyticsManager.Instance().LogEvent("Player Chose Online Mode",0,0);
        SceneManager.LoadScene("Lobby");
    }

    public void Home()
    {
        if(GameObject.Find("GameSetupManager") != null)
        {
            Destroy(GameObject.Find("GameSetupManager"));
        }
        if (GameObject.Find("NetworkManager") != null)
        {
            Destroy(GameObject.Find("NetworkManager"));
        }
        SceneManager.LoadScene("Main Menu");
        AnalyticsManager.Instance().LogSession("Return to Main Menu", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished, AnalyticsManager.Instance().timeElapsed);
    }

    public void HomeFromGameboard()
    {
        if (GameObject.Find("GameSetupManager") != null)
        {
            Destroy(GameObject.Find("GameSetupManager"));
        }
        if (GameObject.Find("NetworkManager") != null)
        {
            Destroy(GameObject.Find("NetworkManager"));
        }
        SceneManager.LoadScene("Main Menu");
        AnalyticsManager.Instance().LogSession("Return to Main Menu", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished, AnalyticsManager.Instance().timeElapsed);
        surveyHandler.OpenSurvey();
    }

    public void Exit()
    {
        AnalyticsManager.Instance().LogSession("Exit Game", AnalyticsManager.Instance().totalTurnsMade, AnalyticsManager.Instance().gamesFinished,AnalyticsManager.Instance().timeElapsed);
        Application.Quit();
    }
}
