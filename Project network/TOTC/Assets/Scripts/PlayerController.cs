using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    public int playerId = 0;
    public int rolledNumber = 0;
    public int currentPosition = 0;
    public bool isActivePlayer;
    public bool gameComplete = false;
    public bool playerOver12 = true;
    public bool switchOneUsed = false;
    public bool switchTwoUsed = false;
    public bool switchThreeUsed = false;
    public bool finalExamsStage = false;

    [Header("3 Subject Switch Buttons")]
    public GameObject subjectSwitchOne;
    public GameObject subjectSwitchTwo;
    public GameObject subjectSwitchThree;

    [Header("Subject Choice Buttons")]
    public GameObject switchToGKButton;
    public GameObject switchToGeographyButton;
    public GameObject switchToScienceButton;
    public GameObject switchToHistoryButton;
    public GameObject switchToMathsButton;
    public GameObject switchToEnglishButton;

    [Header("Other UI Buttons")]
    public GameObject diceRollButton;
    public GameObject subjectRollButton;
    public GameObject acceptRuleButton;

    void Start()
    {
        SetGameObjectsToFalse();
    }

    void Update()
    {
        if (currentPosition >= 53)
        {
            finalExamsStage = true;
        }
    }

    private void SetGameObjectsToFalse()
    {
        subjectSwitchOne.SetActive(false);
        subjectSwitchTwo.SetActive(false);
        subjectSwitchThree.SetActive(false);
        switchToGKButton.SetActive(false);
        switchToGeographyButton.SetActive(false);
        switchToScienceButton.SetActive(false);
        switchToHistoryButton.SetActive(false);
        switchToMathsButton.SetActive(false);
        switchToEnglishButton.SetActive(false);
        acceptRuleButton.SetActive(false);
    }

    public void CheckSwitches()
    {
        if (switchOneUsed)
        {
            subjectSwitchOne.GetComponent<Button>().enabled = false;
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0,0,20);
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchOneUsed == false)
        {
            subjectSwitchOne.GetComponent<Button>().enabled = true;
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchOne.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }

        if (switchTwoUsed)
        {
            subjectSwitchTwo.GetComponent<Button>().enabled = false;
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 20);
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchTwoUsed == false)
        {
            subjectSwitchTwo.GetComponent<Button>().enabled = true;
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchTwo.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }

        if (switchThreeUsed)
        {
            subjectSwitchThree.GetComponent<Button>().enabled = false;
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().text = "USED";
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 20);
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else if (switchThreeUsed == false)
        {
            subjectSwitchThree.GetComponent<Button>().enabled = true;
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().text = "Switch \nSubject";
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().transform.rotation = Quaternion.Euler(0, 0, 0);
            subjectSwitchThree.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }
    }

    public void MovePlayer()
    {
        float posXStart = 16.1f;
        float posYTop = 11.0f;
        Vector3 currentPos;

        switch (playerId)
        {
            case 0:
                posXStart = 16.1f;
                posYTop = 11.0f;
                break;
            case 1:
                posXStart = 17;
                posYTop = 12;
                break;
            case 2:
                posXStart = 18;
                posYTop = 13;
                break;
            case 3:
                posXStart = 18.9f;
                posYTop = 14;
                break;
        }

        if(currentPosition >= 53)
        {
            finalExamsStage = true;
            currentPosition++;
            currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
            if (currentPosition > 58)
            {
                //WIN
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);
                gameComplete = true;  
            }
            else
            {
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);//Right Of Board
            }
        }
        else 
        {
            if (currentPosition + rolledNumber > 53)
            {
                currentPosition = 53;
                currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
                transform.position = new Vector3(posXStart, currentPos.y, currentPos.z); //Right Of Board
            }
            else
            {
                currentPosition = currentPosition + rolledNumber;
                currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
                if(currentPosition < 4)
                {
                    transform.position = new Vector3(posXStart, currentPos.y, currentPos.z);//Right Of Board
                }
                if(currentPosition == 4)
                {
                    transform.position = new Vector3(posXStart - 0.5f, currentPos.y - playerId/2, currentPos.z);//Principal Top Right
                }
                if (currentPosition >= 5 && currentPosition < 22)//Top Of Board
                {
                    transform.position = new Vector3(currentPos.x, posYTop, currentPos.z);
                }
                if (currentPosition == 22)
                {
                    transform.position = new Vector3(-posXStart, currentPos.y + (playerId/2), currentPos.z);//Principal Top Left
                }
                if (currentPosition >= 23 && currentPosition < 34) //Left Of Board
                {
                    transform.position = new Vector3(-posXStart, currentPos.y, currentPos.z);
                }
                if (currentPosition == 34)
                {
                    transform.position = new Vector3(-posXStart, currentPos.y - playerId, currentPos.z);//Principal Bottom Left
                }
                if (currentPosition >= 35 && currentPosition < 53) //Bottom Of Board
                {
                    transform.position = new Vector3(currentPos.x, -posYTop, currentPos.z);
                }
                if (currentPosition == 53)
                {
                    transform.position = new Vector3(posXStart, currentPos.y - playerId, currentPos.z);//Principal Bottom Right
                }
            }
        }
        if(currentPosition < 0)
        {
            currentPosition = 0;
            currentPos = FindObjectOfType<GameManager>().gameSquares[currentPosition].transform.position;
        }  
    }
}
