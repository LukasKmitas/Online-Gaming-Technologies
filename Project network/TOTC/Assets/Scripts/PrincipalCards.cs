using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static Questions;
using UnityEngine.SceneManagement;

public class PrincipalCards : MonoBehaviour
{
    public List<string> loadedPrincipalCards;

    public List<PrincipalCard> principalCards = new List<PrincipalCard>();

    public int nextMove = 0;
    public string nextDirection = "";

    public struct PrincipalCard
    {
        public string cardText;
        public string rule;
    }

    // Start is called before the first frame update
    void Start()
    {
        var loadedPrincipal = Resources.Load("PrincipalOffice");
        loadedPrincipalCards = loadedPrincipal.ToString().Split(new[] { '\n' }).ToList<string>();
        SplitCards();
    }

    public void SplitCards()
    {
        Debug.Log(loadedPrincipalCards);
        PrincipalCard card = new PrincipalCard();
        card.cardText = "";
        card.rule = "";

        for (int i = 0; i < loadedPrincipalCards.Count -1; i++)
        {
            var data = loadedPrincipalCards[i].Split(",");
            for (int j = 1; j < 3; j++)
            {
                //Debug.Log(j.ToString() + data[j].ToString());
                if (j == 1)
                {
                    card.cardText = data[j].ToString();
                }
                else if (j == 2)
                {
                    card.rule = data[j].ToString();
                }
            }
            principalCards.Add(card);
        }
    }

    public void DrawRandomPrincipalCard()
    {
        int randomCard = Random.Range(1, principalCards.Count);
        if(SceneManager.GetActiveScene().name == "Online")
        {
            GetComponent<OnlineGameManager>().principalText.text = principalCards[randomCard].cardText + "\n\n" + principalCards[randomCard].rule;
        }
        else
        {
            GetComponent<GameManager>().principalText.text = principalCards[randomCard].cardText + "\n\n" + principalCards[randomCard].rule;
        }
        
        if(principalCards[randomCard].rule.Contains("forward"))
        {
            nextDirection = "forward";
        }
        else if(principalCards[randomCard].rule.Contains("back"))
        {
            nextDirection = "back";
        }

        //Change to Switch
        if(principalCards[randomCard].rule.Contains("1"))
        {
            nextMove = 1;
        }
        else if (principalCards[randomCard].rule.Contains("2"))
        {
            nextMove = 2;
        }
        else if (principalCards[randomCard].rule.Contains("3"))
        {
            nextMove = 3;
        }
        else if (principalCards[randomCard].rule.Contains("4"))
        {
            nextMove = 4;
        }
        else if (principalCards[randomCard].rule.Contains("5"))
        {
            nextMove = 5;
        }
        else if (principalCards[randomCard].rule.Contains("6"))
        {
            nextMove = 6;
        }
    }
}
