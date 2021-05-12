using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundUIScript : MonoBehaviour
{
    public Text winnerDisplay;
    public Text p1Score;
    public Text p2Score;
    public Text p3Score;
    public Text p4Score;

    // Start is called before the first frame update
    void Start()
    {
        int playerNumber = GameObject.FindGameObjectsWithTag("Player").Length;

        switch (playerNumber)
        {
            case 1:
                p2Score.color = Color.clear;
                p3Score.color = Color.clear;
                p4Score.color = Color.clear;
                break;
            case 2:
                p2Score.color = Color.clear;
                p3Score.color = Color.clear;
                p4Score.color = Color.clear;
                break;
            case 3:
                p3Score.color = Color.clear;
                p4Score.color = Color.clear;
                break;
            case 4:
                p4Score.color = Color.clear;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoundUI(List<int> score, string winner)
    {
        winnerDisplay.text = winner + " wins!";

        switch (score.Count)
        {
            case 1:
                p1Score.text = "P1 - " + score[0].ToString();
                break;
            case 2:
                p1Score.text = "P1 - " + score[0].ToString();
                p2Score.text = "P2 - " + score[1].ToString();
                break;
            case 3:
                p1Score.text = "P1 - " + score[0].ToString();
                p2Score.text = "P2 - " + score[1].ToString();
                p3Score.text = "P3 - " + score[2].ToString();
                break;
            case 4:
                p1Score.text = "P1 - " + score[0].ToString();
                p2Score.text = "P2 - " + score[1].ToString();
                p3Score.text = "P3 - " + score[2].ToString();
                p4Score.text = "P4 - " + score[3].ToString();
                break;
        }
    }
}
