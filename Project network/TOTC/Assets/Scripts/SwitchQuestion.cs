using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchQuestion : MonoBehaviour
{
    public void SwitchQuestionSubject()
    {
        var clicked = EventSystem.current.currentSelectedGameObject;

        string chosenSubject = clicked.tag;
        switch (chosenSubject)
        {
            case "General":
                FindObjectOfType<Questions>().nextSubject = "General";
                break;
            case "Geography":
                FindObjectOfType<Questions>().nextSubject = "Geography";
                break;
            case "Science":
                FindObjectOfType<Questions>().nextSubject = "Science";
                break;
            case "History":
                FindObjectOfType<Questions>().nextSubject = "History";
                break;
            case "Maths":
                FindObjectOfType<Questions>().nextSubject = "Maths";
                break;
            case "English":
                FindObjectOfType<Questions>().nextSubject = "English";
                break;
            default:
                break;
        }

        FindObjectOfType<GameManager>().ResetQuestionTimer();
        FindObjectOfType<GameManager>().StartQuestionTimer();

        for (int j = 0; j < FindObjectOfType<GameManager>().answerButtons.Length; j++)
        {
            FindObjectOfType<GameManager>().answerButtons[j].SetActive(true);
        }

        FindObjectOfType<GameManager>().subjectText.gameObject.SetActive(true);
        FindObjectOfType<GameManager>().questionText.gameObject.SetActive(true);
        FindObjectOfType<GameManager>().questionCard.SetActive(true);

        for (int i = 0; i < FindObjectOfType<GameManager>().players.Count; i++)
        {
            if(FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().playerOver12)
                {
                    GetComponent<Questions>().SelectQuestionOver12();
                }
                else
                {
                    GetComponent<Questions>().SelectQuestionUnder12();
                }
            }

            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToGKButton.SetActive(false);
            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToGeographyButton.SetActive(false);
            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToScienceButton.SetActive(false);
            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToHistoryButton.SetActive(false);
            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToMathsButton.SetActive(false);
            FindObjectOfType<GameManager>().players[i].GetComponent<PlayerController>().switchToEnglishButton.SetActive(false);
        }
    }
}
