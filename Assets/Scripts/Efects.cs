using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Efects : MonoBehaviour
{
    public GameManager gameManager;

    public void PlayEfect(CardData card)
    {
        StartCoroutine(Wait());
        IEnumerator Wait(){ yield return new WaitForSecondsRealtime(0.2f);

        int efectFactor = card.efectFactor;
        int cardID = card.id;
        gameManager.fileToPlayEfct = null;

        if (cardID == 3 || cardID == 8 || cardID == 18 || cardID == 23)
        {
            Aument aument = FindAnyObjectByType<Aument>();
            aument.card = card;
            gameManager.playerTurn = 0;
            aument.playEfect = true; 
        }
    }} 
}
