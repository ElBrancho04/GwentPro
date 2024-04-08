using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTurn : MonoBehaviour
{
    public int player = -1;
    public GameManager gameManager;
    
    public void OnClick()
    {
        gameManager.playerPass[player] = true;
        if (player == 0 && gameManager.playerPass[1] == false)
                  {
                        gameManager.playerTurn = 2;
                  }
                  else if(gameManager.playerPass[0] == false)
                  {
                        gameManager.playerTurn = 1;
                  }
    }
}
