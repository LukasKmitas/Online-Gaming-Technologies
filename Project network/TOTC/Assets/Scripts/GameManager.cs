using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public DebugLogger logger;
    public MenuManager menuManager;

    [Header("Data Collection")]
    public int turnsTaken;
    public float gameCompleteTime;
    public int numberOfPlayers;
    private string finalStandings;

    [Header("Game Lists")]
    public List<GameObject> gameSquares;
    public List<GameObject> officeSquares;
    public List<GameObject> players;
    public List<GameObject> playersForText;
    public List<GameObject> playersCompletedGame;
    public List<Vector3> buttonPositions;

    [Header("Game Arrays")]
    public int[] officeNumbers = {1,11,16,20,27,31,36,41,46 };
    public int[] missTurnNumbers = {10,26,40};
    public int[] move3SpacesNumbers = {6,15,30,45};

    [Header("UI")]
    public GameObject[] answerButtons;
    public GameObject[] timerComponent;

    public GameObject continueButton;

    public GameObject questionCard;
    public TextMeshProUGUI questionText;

    public GameObject principalCard;
    public TextMeshProUGUI principalText;

    public TextMeshProUGUI rolledText;
    public TextMeshProUGUI missedTurnText;
    public TextMeshProUGUI subjectText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI leaderboardText;
    public TextMeshProUGUI currentPlayerText;

    [Header("Active Player Position")]
    public float activePlayerPosX;
    public float activePlayerPosY;

    [Header("Timer")]
    public float timeForQuestion = 30.0f;
    public bool timerRunning = false;

    [Header("Dice Sprites")]
    public Sprite gkSprite;
    public Sprite geoSprite;
    public Sprite scienceSprite;
    public Sprite historySprite;
    public Sprite mathSprite;
    public Sprite englishSprite;

    public Sprite oneSprite;
    public Sprite twoSprite;
    public Sprite threeSprite;
    public Sprite fourSprite;
    public Sprite fiveSprite;
    public Sprite sixSprite;

    [Header("Exit Panel")]
    [SerializeField] 
    private GameObject exitPanel;
    [SerializeField]
    private TextMeshProUGUI exitText;
    [SerializeField]
    private GameObject cancelButton;
    [SerializeField]
    private GameObject confirmButton;
    [SerializeField]
    private GameObject homeButton;
    private bool exitPanelToggle = false;

    [Header("Button Colour Change")]
    private Color currentColour = Color.white;
    private bool toGreen = true;
    private bool toBig = true;
    [SerializeField] bool continuePulse = true;

    // Start is called before the first frame update
    void Start()
    {
        PrepareLists();
        PrepareTimer();
        AssignSprites();
        PreparePlayers();
        PrepareAnswersUI();
        SetGameObjectsToFalse();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                //Update Player Positions
                activePlayerPosX = players[i].transform.position.x;
                activePlayerPosY = players[i].transform.position.y;

                //Make buttons stand out
                FlashButtonGreen(i);
                PulseActiveButton(i);

                if (continuePulse)
                {
                    //Make Active Player Stand Out
                    StartCoroutine("PulseActivePlayer");
                }

                //Check if player is on go to office square
                for (int j = 0; j < officeNumbers.Length; j++)
                {
                    if(officeNumbers[j] == players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        GoToOffice();
                    }
                }

                //Check if player is on move forward 3 square
                for (int l = 0; l < move3SpacesNumbers.Length; l++)
                {
                    if (move3SpacesNumbers[l] == players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        StartCoroutine(Move3(i, move3SpacesNumbers[l]));
                    }
                }

                //Check if player is on POTY square
                if (players[i].GetComponent<PlayerController>().currentPosition == 50)
                {
                    StartCoroutine("POTY", i);
                }

                //Check if player completed game
                if (players[i].GetComponent<PlayerController>().gameComplete)
                {
                    AnalyticsManager.Instance().LogEvent("Player: " + players[i].GetComponent<PlayerController>().playerId + " Completed Game", turnsTaken, gameCompleteTime);
                    playersCompletedGame.Add(players[i]);
                    if (playersCompletedGame.Count == numberOfPlayers)
                    {
                        finalStandings += "Player: " + playersCompletedGame[i].GetComponent<PlayerController>().playerId + ", ";
                        AnalyticsManager.Instance().LogEvent("Game Finished, First to Last Place: " + finalStandings , turnsTaken, gameCompleteTime);
                        AnalyticsManager.Instance().gamesFinished++;
                        menuManager.HomeFromGameboard();
                    }
                    NextPlayerTurn();
                    players.RemoveAt(i);
                }   
            }
        }

        SetGameWinner();
        UpdateTimer(); //Question Timer
        DisplayCurrentPlayerText();
    }

    private void PrepareLists()
    {
        gameSquares = new List<GameObject> { };
        officeSquares = new List<GameObject> { };
        playersCompletedGame = new List<GameObject> { };
        buttonPositions = new List<Vector3> { };

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Square");
        var orderedList = objects.OrderBy(objects => objects.GetComponent<SquarePositionHolder>().squareBoardPosition);
        gameSquares = orderedList.ToList();

        GameObject[] offices = GameObject.FindGameObjectsWithTag("OfficeSquare");
        var officesOrderedList = offices.OrderBy(offices => offices.GetComponent<OfficePositionOnBoard>().officeBoardPosition);
        officeSquares = officesOrderedList.ToList();

        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("PlayerPiece");
        players = playerArray.ToList();
        playersForText = playerArray.ToList();

        numberOfPlayers = FindObjectOfType<GameSetupManager>().numberOfPlayers;
    }

    private void PrepareTimer()
    {
        timerComponent = GameObject.FindGameObjectsWithTag("Timer");
        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }
        gameCompleteTime = 0;
    }

    private void AssignSprites()
    {
        gkSprite = Resources.Load<Sprite>("DiceGk");
        geoSprite = Resources.Load<Sprite>("DiceGeo");
        scienceSprite = Resources.Load<Sprite>("DiceScience");
        historySprite = Resources.Load<Sprite>("DiceHistory");
        mathSprite = Resources.Load<Sprite>("DiceMaths");
        englishSprite = Resources.Load<Sprite>("DiceEnglish");

        oneSprite = Resources.Load<Sprite>("DiceOne");
        twoSprite = Resources.Load<Sprite>("Dice2");
        threeSprite = Resources.Load<Sprite>("Dice3");
        fourSprite = Resources.Load<Sprite>("Dice4");
        fiveSprite = Resources.Load<Sprite>("Dice5");
        sixSprite = Resources.Load<Sprite>("Dice6");
    }

    private void PreparePlayers()
    {
        if (FindObjectOfType<GameSetupManager>().numberOfPlayers == 2)
        {
            players[3].SetActive(false);
            players[2].SetActive(false);
            players.RemoveAt(3);
            players.RemoveAt(2);

        }
        else if (FindObjectOfType<GameSetupManager>().numberOfPlayers == 3)
        {
            players[3].SetActive(false);
            players.RemoveAt(3);
        }
        else if (FindObjectOfType<GameSetupManager>().numberOfPlayers == 4)
        {

        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().playerId == 0)
            {
                players[i].GetComponent<PlayerController>().isActivePlayer = true;
                players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().pinkOver12;
            }
            else
            {
                players[i].GetComponent<PlayerController>().isActivePlayer = false;
            }

            if (players[i].GetComponent<PlayerController>().playerId == 1)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().blueOver12;
            }
            else if (players[i].GetComponent<PlayerController>().playerId == 2)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().redOver12;
            }
            else if (players[i].GetComponent<PlayerController>().playerId == 3)
            {
                players[i].GetComponent<PlayerController>().playerOver12 = FindObjectOfType<GameSetupManager>().greenOver12;
            }
        }
    }

    private void PrepareAnswersUI()
    {
        answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
        var orderedAnswers = answerButtons.OrderBy(answerButtons => answerButtons.GetComponent<AnswerIdentifier>().placeInList);
        answerButtons = orderedAnswers.ToArray();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].SetActive(false);
            buttonPositions.Add(answerButtons[i].transform.position);
        }
        questionCard = GameObject.FindGameObjectWithTag("QuestionCard");
    }

    private void SetGameObjectsToFalse()
    {
        subjectText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        questionCard.SetActive(false);
        continueButton.SetActive(false);
        rolledText.gameObject.SetActive(false);
        missedTurnText.gameObject.SetActive(false);
        leaderboardText.gameObject.SetActive(false);
        principalCard.SetActive(false);
        principalText.gameObject.SetActive(false);
        exitPanel.SetActive(false);
        exitText.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (timerRunning)
        {
            if (timeForQuestion > 0)
            {
                timeForQuestion -= Time.deltaTime;
            }
            else
            {
                StartCoroutine("TurnButtonOneRed");
            }
        }
        timerText.text = ((int)timeForQuestion).ToString();

        gameCompleteTime += Time.deltaTime;
    }

    /// <summary>
    /// Make the dice roll button and subject roll button flash green for active player
    /// </summary>
    private void FlashButtonGreen(int activePlayer)
    {
        if (players[activePlayer].GetComponent<PlayerController>().isActivePlayer)
        {
            if (toGreen)
            {
                currentColour += new Color(-1.5f, 0, -1.5f, 0) * Time.deltaTime;

                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }
            }
            else
            {
                currentColour += new Color(1.5f, 0, 1.5f, 0) * Time.deltaTime;
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }

                if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = currentColour;
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().color = new Color(1, 0.49f, 0, 1);
                }
            }

            if (currentColour.r < 0)
            {
                toGreen = false;
            }
            else if (currentColour.r > 1)
            {
                toGreen = true;
            }
        }
    }

    /// <summary>
    /// Make Active Player Pulse
    /// </summary>
    private IEnumerator PulseActivePlayer()
    {
        continuePulse = false;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                for (float j = 0; j <= 1; j += 0.01f)
                {
                    players[i].transform.localScale = new Vector3(
                        Mathf.Lerp(transform.localScale.x, transform.localScale.x + 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.y, transform.localScale.y + 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.z, transform.localScale.z + 0.15f, Mathf.SmoothStep(0f, 1f, j)));
                }
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                for (float j = 0; j <= 1; j += 0.01f)
                {
                    players[i].transform.localScale = new Vector3(
                        Mathf.Lerp(transform.localScale.x, transform.localScale.x - 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.y, transform.localScale.y - 0.15f, Mathf.SmoothStep(0f, 1f, j)),
                        Mathf.Lerp(transform.localScale.z, transform.localScale.z - 0.15f, Mathf.SmoothStep(0f, 1f, j)));
                }
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                players[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

        }
        continuePulse = true;
    }

    /// <summary>
    /// <Make Dice Buttons Pulse
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <returns></returns>
    private void PulseActiveButton(int activePlayer)
    {
        Transform trasformToChange = players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform;

        if (players[activePlayer].GetComponent<PlayerController>().isActivePlayer)
        {
            if (toBig)
            {
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform;
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    trasformToChange = players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform;
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(
                    (trasformToChange.localScale.x + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.y + 0.225f * Time.deltaTime),
                    (trasformToChange.localScale.z + 0.225f * Time.deltaTime));
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                if (players[activePlayer].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (players[activePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled)
                {
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    players[activePlayer].GetComponent<PlayerController>().diceRollButton.transform.localScale = new Vector3(1, 1, 1);
                    players[activePlayer].GetComponent<PlayerController>().subjectRollButton.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (trasformToChange.localScale.x > 1.3f)
            {
                toBig = false;
            }
            else if (trasformToChange.localScale.x <= 1.0f)
            {
                toBig = true;
            }
        }
    }

    private IEnumerator POTY(int player)
    {
        rolledText.gameObject.SetActive(true);
        rolledText.text = "Pupil Of The Year \nGo To Summer Tests";
        yield return new WaitForSeconds(2.0f);

        rolledText.gameObject.SetActive(false);
        players[player].GetComponent<PlayerController>().currentPosition = 53;
        players[player].transform.position = gameSquares[53].transform.position;
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        yield break;
    }

    private IEnumerator Move3(int player, int spaceOccupied)
    {
        rolledText.gameObject.SetActive(true);
        rolledText.text = "Move 3 Spaces Forward";
        yield return new WaitForSeconds(2.0f);

        rolledText.gameObject.SetActive(false);
        if(spaceOccupied == 6)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 9;
            players[player].transform.position = gameSquares[9].transform.position;
        }
        else if (spaceOccupied == 15)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 18;
            players[player].transform.position = gameSquares[18].transform.position;
        }
        else if (spaceOccupied == 30)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 33;
            players[player].transform.position = gameSquares[33].transform.position;
        }
        else if (spaceOccupied == 45)
        {
            players[player].GetComponent<PlayerController>().currentPosition = 48;
            players[player].transform.position = gameSquares[48].transform.position;
        }

        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;

        yield break;
    }

    private IEnumerator ShowRolledNum(string text, int player)
    {
        yield return new WaitForSeconds(1.0f);
        players[player].GetComponent<PlayerController>().MovePlayer();
        players[player].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
        CheckIfMissTurn(player);
        yield break;
    }

    private IEnumerator ShowRolledSubject(string text, int player)
    {
        yield return new WaitForSeconds(1.0f);
        if (players[player].GetComponent<PlayerController>().playerOver12)
        {
            Over12Question();
        }
        else
        {
            Under12Question();
        }
        players[player].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;

        if (players[player].GetComponent<PlayerController>().finalExamsStage == false)
        {
            players[player].GetComponent<PlayerController>().subjectSwitchOne.SetActive(true);
            players[player].GetComponent<PlayerController>().subjectSwitchTwo.SetActive(true);
            players[player].GetComponent<PlayerController>().subjectSwitchThree.SetActive(true);
        }
        yield break;
    }

    private IEnumerator MissTurn()
    {
        missedTurnText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        missedTurnText.gameObject.SetActive(false);
        yield break;
    }

    private void SetGameWinner()
    {
        if(playersCompletedGame.Count > 0)
        {
            leaderboardText.gameObject.SetActive(true);

            switch (playersCompletedGame[0].GetComponent<PlayerController>().playerId)
            {
                case 0:
                    leaderboardText.color = Color.magenta;
                    leaderboardText.text = "Pink Player Wins";
                    break;
                case 1:
                    leaderboardText.color = Color.blue;
                    leaderboardText.text = "Blue Player Wins";
                    break;
                case 2:
                    leaderboardText.color = Color.red;
                    leaderboardText.text = "Red Player Wins";
                    break;
                case 3:
                    leaderboardText.color = Color.green;
                    leaderboardText.text = "Green Player Wins";
                    break;
                default:
                    break;
            }
        } 
    }

    public void DiceRoll()
    {
        int roll = 0;
        roll += Random.Range(1, 6);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (players[i].GetComponent<PlayerController>().finalExamsStage == false)
                {
                    switch (roll)
                    {
                        case 1:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                            break;
                        case 2:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = twoSprite;
                            break;
                        case 3:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = threeSprite;
                            break;
                        case 4:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = fourSprite;
                            break;
                        case 5:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = fiveSprite;
                            break;
                        case 6:
                            players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = sixSprite;
                            break;
                        default:
                            break;
                    }

                    logger.LogInformation("Player: " + i + " Rolled dice ");
                    players[i].GetComponent<PlayerController>().rolledNumber = roll;
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                }
                else
                {
                    roll = 1;
                    logger.LogInformation("Player: " + i + " In exam stage rolled dice ");
                    players[i].GetComponent<PlayerController>().rolledNumber = 1;
                    StartCoroutine(ShowRolledNum(roll.ToString(), i));
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Image>().sprite = oneSprite;
                    players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = false;
                }
            }
        }
    }

    public void SubjectDiceRoll()
    {
        int roll = 0;
        roll += Random.Range(0, 6);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (players[i].GetComponent<PlayerController>().finalExamsStage == false)
                {
                    switch (roll)
                    {
                        case 0:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 1:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 2:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 3:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 4:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 5:
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }

                    FindObjectOfType<Questions>().nextSubject = FindObjectOfType<Questions>().subjects[roll];
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<Questions>().nextSubject, i));

                    players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                    players[i].GetComponent<PlayerController>().CheckSwitches();
                }
                else
                {
                    switch(players[i].GetComponent<PlayerController>().currentPosition)
                    {
                        case 53:
                            FindObjectOfType<Questions>().nextSubject = "General";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = gkSprite;
                            break;
                        case 54:
                            FindObjectOfType<Questions>().nextSubject = "Geography";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = geoSprite;
                            break;
                        case 55:
                            FindObjectOfType<Questions>().nextSubject = "Science";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = scienceSprite;
                            break;
                        case 56:
                            FindObjectOfType<Questions>().nextSubject = "History";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = historySprite;
                            break;
                        case 57:
                            FindObjectOfType<Questions>().nextSubject = "Maths";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = mathSprite;
                            break;
                        case 58:
                            FindObjectOfType<Questions>().nextSubject = "English";
                            players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Image>().sprite = englishSprite;
                            break;
                        default:
                            break;
                    }
                    StartCoroutine(ShowRolledSubject(FindObjectOfType<Questions>().nextSubject, i));

                    players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                }
            }
        }
    }

    private void DisplayCurrentPlayerText()
    {
        if (playersForText[0].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = "Pink Player Turn";
            currentPlayerText.color = Color.magenta;
            questionCard.GetComponent<Image>().color = Color.magenta;
        }
        else if(playersForText[1].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = "Blue Player Turn";
            currentPlayerText.color = Color.cyan;
            questionCard.GetComponent<Image>().color = Color.cyan;
        }
        else if (playersForText[2].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = "Red Player Turn";
            currentPlayerText.color = Color.red;
            questionCard.GetComponent<Image>().color = Color.red;
        }
        else if (playersForText[3].GetComponent<PlayerController>().isActivePlayer)
        {
            currentPlayerText.text = "Green Player Turn";
            currentPlayerText.color = Color.green;
            questionCard.GetComponent<Image>().color = Color.green;
        }
    }

    public void SwitchSubject()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {

                if (players[i].GetComponent<PlayerController>().subjectSwitchTwo.GetComponent<Button>().enabled == false)
                {
                    players[i].GetComponent<PlayerController>().switchThreeUsed = true;
                }
                else if (players[i].GetComponent<PlayerController>().subjectSwitchOne.GetComponent<Button>().enabled == false)
                {
                    players[i].GetComponent<PlayerController>().switchTwoUsed = true;
                }
                else
                {
                    players[i].GetComponent<PlayerController>().switchOneUsed = true;
                }

                players[i].GetComponent<PlayerController>().switchToGKButton.SetActive(true);
                players[i].GetComponent<PlayerController>().switchToGeographyButton.SetActive(true);
                players[i].GetComponent<PlayerController>().switchToScienceButton.SetActive(true);
                players[i].GetComponent<PlayerController>().switchToHistoryButton.SetActive(true);
                players[i].GetComponent<PlayerController>().switchToMathsButton.SetActive(true);
                players[i].GetComponent<PlayerController>().switchToEnglishButton.SetActive(true);

                for (int j = 0; j < answerButtons.Length; j++)
                {
                    answerButtons[j].SetActive(false);
                }

                subjectText.gameObject.SetActive(false);
                questionText.gameObject.SetActive(false);
                questionCard.SetActive(false);

                players[i].GetComponent<PlayerController>().CheckSwitches();
            }
        }
    }

    public void Under12Question()
    {
        StartQuestionTimer();
        for (int j = 0; j < answerButtons.Length; j++)
        {
             answerButtons[j].SetActive(true);
        }
        SetRandomButtonPos();
        subjectText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        questionCard.SetActive(true);
 
        GetComponent<Questions>().SelectQuestionUnder12();
    }
    public void Over12Question()
    {
        StartQuestionTimer();
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].SetActive(true);
        }
        SetRandomButtonPos();
        subjectText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        questionCard.SetActive(true);

        GetComponent<Questions>().SelectQuestionOver12();
    }

    //If Answered Wrong
    public void AnswerButtonOne()
    {
        StartCoroutine("TurnButtonOneRed");
    }
    public void AnswerButtonTwo()
    {
        StartCoroutine("TurnButtonTwoRed");
    }

    //If Answered Correctly
    public void CorrectAnswer()
    {
        StartCoroutine("TurnButtonGreen");
    }

    private IEnumerator TurnButtonOneRed()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[1].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }
        subjectText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        questionCard.SetActive(false);

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }

        ResetQuestionTimer();
        NextPlayerTurn();
        yield break;
    }

    private IEnumerator TurnButtonTwoRed()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        answerButtons[2].GetComponent<Image>().color = Color.red;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }
        subjectText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        questionCard.SetActive(false);

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }

        ResetQuestionTimer();
        NextPlayerTurn();
        yield break;
    }

    private IEnumerator TurnButtonGreen()
    {
        answerButtons[0].GetComponent<Image>().color = Color.green;
        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(1.0f);

        for (int j = 0; j < answerButtons.Length; j++)
        {
            answerButtons[j].GetComponent<Button>().enabled = true;
            answerButtons[j].SetActive(false);
            answerButtons[j].GetComponent<Image>().color = Color.white;
        }
        subjectText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        questionCard.SetActive(false);

        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].GetComponent<PlayerController>().diceRollButton.GetComponent<Button>().enabled = true;
                players[i].GetComponent<PlayerController>().subjectSwitchOne.SetActive(false);
                players[i].GetComponent<PlayerController>().subjectSwitchTwo.SetActive(false);
                players[i].GetComponent<PlayerController>().subjectSwitchThree.SetActive(false);
            }
        }
        ResetQuestionTimer();
        yield break;
    }

    public void GoToOffice()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = false;
                continueButton.SetActive(true);
            }
        }
    }

    public void StartQuestionTimer()
    {
        for (int i = 0; i < timerComponent.Length; i++)
        {
            timerComponent[i].SetActive(true);
        }
        timerRunning = true;
    }

    public void ResetQuestionTimer()
    {
        timerRunning = false;
        timeForQuestion = 30;
    }

    public void NextPlayerTurn()
    {
        int nextActivePlayer = 0;
        int currentActivePlayer = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                currentActivePlayer = i;
                nextActivePlayer = i+1;

                if (nextActivePlayer >= players.Count)
                {
                    nextActivePlayer = 0;
                }
                if (players[nextActivePlayer].GetComponent<PlayerController>().gameComplete == true)
                {
                    nextActivePlayer += 1;
                }
            }
            players[i].GetComponent<PlayerController>().subjectSwitchOne.SetActive(false);
            players[i].GetComponent<PlayerController>().subjectSwitchTwo.SetActive(false);
            players[i].GetComponent<PlayerController>().subjectSwitchThree.SetActive(false);
        }
        players[currentActivePlayer].GetComponent<PlayerController>().isActivePlayer = false;
        AnalyticsManager.Instance().LogEvent("Player: " + currentActivePlayer + " Turn Over", turnsTaken,0);
        turnsTaken++;
        AnalyticsManager.Instance().totalTurnsMade++;
        AnalyticsManager.Instance().LogEvent("Player: " + nextActivePlayer + " Turn Start", turnsTaken,0);
        players[nextActivePlayer].GetComponent<PlayerController>().isActivePlayer = true;
        players[nextActivePlayer].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;

    }

    public void ContinueToPrincipal()
    {  
        for (int i = 0; i < players.Count; i++)
        {
            //Change to switch and/or simplify
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                switch (players[i].GetComponent<PlayerController>().currentPosition)
                {
                    case 1: //good
                        players[i].transform.position = officeSquares[0].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 11: //good
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 16: //bad
                        players[i].transform.position = officeSquares[0].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[0].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 20: //good
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 27: //good
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 31:  //bad
                        players[i].transform.position = officeSquares[1].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[1].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 36:  //bad
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 41: //good
                        players[i].transform.position = officeSquares[3].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[3].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    case 46: //bad
                        players[i].transform.position = officeSquares[2].transform.position;
                        players[i].GetComponent<PlayerController>().currentPosition = officeSquares[2].GetComponent<OfficePositionOnBoard>().officeBoardPosition;
                        break;
                    default:
                        break;
                }
                continueButton.SetActive(false);

                GetComponent<PrincipalCards>().DrawRandomPrincipalCard();
                principalCard.SetActive(true);
                principalText.gameObject.SetActive(true);
                players[i].GetComponent<PlayerController>().acceptRuleButton.SetActive(true);
            }
        }
    }


    public void AcceptRule()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerController>().isActivePlayer)
            {
                if (GetComponent<PrincipalCards>().nextDirection == "forward")
                {
                    players[i].GetComponent<PlayerController>().currentPosition += GetComponent<PrincipalCards>().nextMove;
                    players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[players[i].GetComponent<PlayerController>().currentPosition].transform.position;

                }
                else if (GetComponent<PrincipalCards>().nextDirection == "back")
                {
                    int newPosMinus = players[i].GetComponent<PlayerController>().currentPosition - GetComponent<PrincipalCards>().nextMove;
                    if (newPosMinus >= 0)
                    {
                        players[i].GetComponent<PlayerController>().currentPosition -= GetComponent<PrincipalCards>().nextMove;
                    }
                    else
                    {
                        players[i].GetComponent<PlayerController>().currentPosition = 0;
                    }
                    players[i].transform.position = FindObjectOfType<GameManager>().gameSquares[players[i].GetComponent<PlayerController>().currentPosition].transform.position;
                    logger.LogInformation("Rule Accepted, Next position: " + newPosMinus);
                }
                
                players[i].GetComponent<PlayerController>().acceptRuleButton.SetActive(false);
                principalCard.SetActive(false);
                principalText.gameObject.SetActive(false);
                players[i].GetComponent<PlayerController>().subjectRollButton.GetComponent<Button>().enabled = true;
                CheckIfMissTurn(i);
            }
        }
    }

    public void SetRandomButtonPos()
    {
        int roll = Random.Range(0, 3);
        answerButtons[0].transform.position = buttonPositions[roll];

        switch (roll)
        {
            case 0:
                roll = Random.Range(1, 3);
                answerButtons[1].transform.position = buttonPositions[roll];
                if(roll == 1)
                {
                    answerButtons[2].transform.position = buttonPositions[2];
                }
                else
                {
                    answerButtons[2].transform.position = buttonPositions[1];
                }
                break;
            case 1:
                answerButtons[1].transform.position = buttonPositions[0];
                answerButtons[2].transform.position = buttonPositions[2];
                break;
            case 2:
                roll = Random.Range(0, 2);
                answerButtons[1].transform.position = buttonPositions[roll];
                if (roll == 0)
                {
                    answerButtons[2].transform.position = buttonPositions[1];
                }
                else
                {
                    answerButtons[2].transform.position = buttonPositions[0];
                }
                break;
            default:
                break;
        }
    }

    private void CheckIfMissTurn(int playerid)
    {
        for (int k = 0; k < missTurnNumbers.Length; k++)
        {
            Debug.Log("checking square: " + missTurnNumbers[k] + "for player: " + playerid + "who is at square: " +
                players[playerid].GetComponent<PlayerController>().currentPosition);
            if (missTurnNumbers[k] == players[playerid].GetComponent<PlayerController>().currentPosition)
            {
                StartCoroutine("MissTurn");
                ResetQuestionTimer();
                NextPlayerTurn();
            }
        }
    }

    public void ToggleExitScreen()
    {
        exitPanelToggle = !exitPanelToggle;
        exitPanel.SetActive(exitPanelToggle);
        exitText.gameObject.SetActive(exitPanelToggle);
        cancelButton.gameObject.SetActive(exitPanelToggle);
        confirmButton.gameObject.SetActive(exitPanelToggle);
    }
}
