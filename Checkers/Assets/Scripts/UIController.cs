using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject MenuScreen;
    public GameObject GameOverScreenBlackWins;
    public GameObject GameOverScreenWhiteWins;
    public GameObject GameOverScreenDraw;
    public GameObject GameOverlay;
    public GameObject GameStatsWindow;
    public TextMeshProUGUI GameStatsWindowText;
    public SwitchManager PruningSwitch;
    public bool InMenus;

    public void ShowMenuScreen()
    {
        GameOverlay.SetActive(false);
        GameStatsWindow.SetActive(false);
        GameOverScreenBlackWins.SetActive(false);
        GameOverScreenWhiteWins.SetActive(false);
        GameOverScreenDraw.SetActive(false);
        MenuScreen.SetActive(true);
        InMenus = true;
    }

    public void ShowGameOverScreen(string winner)
    {
        switch(winner)
        {
            case "black":
                GameOverScreenBlackWins.SetActive(true);
                break;
            case "white":
                GameOverScreenWhiteWins.SetActive(true);
                break;
            case "draw":
                GameOverScreenDraw.SetActive(true);
                break;
        }

        GameOverlay.SetActive(false);
        MenuScreen.SetActive(false);
        InMenus = true;
    }

    public void ShowGameOverlay(bool showGameStats)
    {
        GameOverlay.SetActive(true);
        GameStatsWindow.SetActive(showGameStats);
        MenuScreen.SetActive(false);
        GameOverScreenBlackWins.SetActive(false);
        GameOverScreenWhiteWins.SetActive(false);
        GameOverScreenDraw.SetActive(false);
        PrintGameStats(-1, -1, -1);
        InMenus = false;
    }

    public void PrintGameStats(float lastMoveTime, int movesExplored, int boardPosition)
    {
        GameStatsWindowText.text = "Last Move Time: " + lastMoveTime + "\nMoves Explored: " + movesExplored + "\nBoard Position: " + boardPosition;
    }
}
