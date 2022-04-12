using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersGame
{
    public CheckersBoard Board { get; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentPlayer { get; set; }

    public CheckersGame(PlayerType player1Type, PlayerType player2Type)
    {
        Board = new CheckersBoard(this);
        Player1 = new Player(player1Type, Color.black);
        Player2 = new Player(player2Type, Color.white);
        CurrentPlayer = Player1;
    }
}
