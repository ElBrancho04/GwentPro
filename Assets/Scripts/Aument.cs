using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Aument : MonoBehaviour
{
    public GameManager gameManager;
    public CardData card;
    public bool playEfect;
    public File file;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        file = gameManager.fileToPlayEfct;
        if (playEfect && file != null && file.player == card.player)
        {
            for (int i = 0; i < file.cards.Count; i++)
            {
                file.cards[i].powerVar += card.efectFactor;
            }
            playEfect = false;
            gameManager.playerTurn = card.player%2 + 1;
        }
    }
}
