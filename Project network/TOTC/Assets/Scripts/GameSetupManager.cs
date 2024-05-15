using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameSetupManager : MonoBehaviour
{
    [Header("Information")]
    public GameSetupManager instance;
    public int numberOfPlayers = 4;

    [Header("Pink Player")]
    public bool pinkOver12 = true;
    [SerializeField] TextMeshProUGUI pinkOverText;
    [SerializeField] TextMeshProUGUI pinkUnderText;
    [SerializeField] GameObject pinkOverImage;
    [SerializeField] GameObject pinkUnderImage;

    [Header("Blue Player")]
    public bool blueOver12 = true;
    [SerializeField] TextMeshProUGUI blueOverText;
    [SerializeField] TextMeshProUGUI blueUnderText;
    [SerializeField] GameObject blueOverImage;
    [SerializeField] GameObject blueUnderImage;

    [Header("Red Player")]
    public bool redOver12 = true;
    [SerializeField] TextMeshProUGUI redOverText;
    [SerializeField] TextMeshProUGUI redUnderText;
    [SerializeField] GameObject redOverImage;
    [SerializeField] GameObject redUnderImage;
    [SerializeField] GameObject redAgeButton;

    [Header("Green Player")]
    public bool greenOver12 = true;
    [SerializeField] TextMeshProUGUI greenOverText;
    [SerializeField] TextMeshProUGUI greenUnderText;
    [SerializeField] GameObject greenOverImage;
    [SerializeField] GameObject greenUnderImage;
    [SerializeField] GameObject greenAgeButton;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "GameSetup")
        {
            pinkUnderText.gameObject.SetActive(false);
            blueUnderText.gameObject.SetActive(false);
            redUnderText.gameObject.SetActive(false);
            greenUnderText.gameObject.SetActive(false);
        }
    }

    public void togglePink()
    {
        pinkOver12 ^= true;
        if(pinkOver12)
        {
            pinkOverText.gameObject.SetActive(true);
            pinkOverImage.GetComponent<Image>().color = Color.magenta;

            pinkUnderText.gameObject.SetActive(false);
            pinkUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            pinkOverText.gameObject.SetActive(false);
            pinkOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            pinkUnderText.gameObject.SetActive(true);
            pinkUnderImage.GetComponent<Image>().color = Color.magenta;
        }
    }

    public void toggleBlue()
    {
        blueOver12 ^= true;
        if (blueOver12)
        {
            blueOverText.gameObject.SetActive(true);
            blueOverImage.GetComponent<Image>().color = Color.cyan;

            blueUnderText.gameObject.SetActive(false);
            blueUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            blueOverText.gameObject.SetActive(false);
            blueOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            blueUnderText.gameObject.SetActive(true);
            blueUnderImage.GetComponent<Image>().color = Color.cyan;
        }
    }

    public void toggleRed()
    {
        redOver12 ^= true;
        if (redOver12)
        {
            redOverText.gameObject.SetActive(true);
            redOverImage.GetComponent<Image>().color = Color.red;

            redUnderText.gameObject.SetActive(false);
            redUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);
        }
        else
        {
            redOverText.gameObject.SetActive(false);
            redOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            redUnderText.gameObject.SetActive(true);
            redUnderImage.GetComponent<Image>().color = Color.red;
        }

    }

    public void toggleGreen()
    {
        greenOver12 ^= true;
        if (greenOver12)
        {
            greenOverText.gameObject.SetActive(true);
            greenOverImage.GetComponent<Image>().color = Color.green;

            greenUnderText.gameObject.SetActive(false);
            greenUnderImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

        }
        else
        {
            greenOverText.gameObject.SetActive(false);
            greenOverImage.GetComponent<Image>().color = new Color(0.132f, 0.132f, 0.132f, 1);

            greenUnderText.gameObject.SetActive(true);
            greenUnderImage.GetComponent<Image>().color = Color.green;
        }
    }

    public void FourPlayerGame()
    {
        numberOfPlayers = 4;

        redAgeButton.SetActive(true);
        redOverImage.SetActive(true);
        redUnderImage.SetActive(true);

        greenAgeButton.SetActive(true);
        greenOverImage.SetActive(true);
        greenUnderImage.SetActive(true);
    }

    public void ThreePlayerGame()
    {
        numberOfPlayers = 3;

        redAgeButton.SetActive(true);
        redOverImage.SetActive(true);
        redUnderImage.SetActive(true);

        greenAgeButton.SetActive(false);
        greenOverImage.SetActive(false);
        greenUnderImage.SetActive(false);
    }

    public void TwoPlayerGame()
    {
        numberOfPlayers = 2;

        redAgeButton.SetActive(false);
        redOverImage.SetActive(false);
        redUnderImage.SetActive(false);

        greenAgeButton.SetActive(false);
        greenOverImage.SetActive(false);
        greenUnderImage.SetActive(false);
    }
}
