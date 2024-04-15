using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Aument : MonoBehaviour
{
    public GameManager gameManager;
    public CardData card;
    public bool playEfectF;
    public bool playEfectC;
    public File file;
    public CardData cardToPlayEfct;
    public int player;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        file = gameManager.fileToPlayEfct;
        if (card != null)
        player = card.player;
        if (card != null && card.efectFactor < 0)
        player = card.player%2 + 1;

        if (playEfectF && file != null && file.player == player)
        {
            for (int i = 0; i < file.cards.Count; i++)
            {
                if (!file.cards[i].isGold)
                file.cards[i].powerVar += card.efectFactor;
            }
            playEfectF = false;
            gameManager.fileToPlayEfct = null;
            gameManager.playerTurn = card.player%2 + 1;
        }

        cardToPlayEfct = gameManager.cardToPlayEfct;
        if (playEfectC && cardToPlayEfct != null && !cardToPlayEfct.isGold && cardToPlayEfct.player == player) 
        {
           cardToPlayEfct.powerVar += gameManager.UIcardToPlayEfct.actPower;
           playEfectC = false;
           gameManager.cardToPlayEfct = null;
           gameManager.UIcardToPlayEfct = null;
           gameManager.playerTurn = card.player%2 + 1;
        }
    }
}
