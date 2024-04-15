using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Efects : MonoBehaviour
{
    public GameManager gameManager;
    public Aument aument;
    public DelCard delCard;

    void Start()
    {
        aument = FindAnyObjectByType<Aument>();
        delCard = FindAnyObjectByType<DelCard>();
    }

    public void PlayEfect(CardData card)
    {
        StartCoroutine(Wait());
        IEnumerator Wait(){ yield return new WaitForSecondsRealtime(0.2f);

        int efectFactor = card.efectFactor;
        int cardID = card.id;
        gameManager.fileToPlayEfct = null;
        gameManager.UIcardToPlayEfct = null;
        gameManager.cardToPlayEfct = null;
        aument.card = card;
        delCard.card = card;

        if (cardID == 3 || cardID == 8 || cardID == 18 || cardID == 23 || cardID == 6 || cardID == 22) //Aumentar una fila
        {
            gameManager.playerTurn = 0;
            aument.playEfectF = true; 
        }

        if (cardID == 1 || cardID == 17) //Duplicar el poder de una carta
        {
            gameManager.playerTurn = 0;
            aument.playEfectC = true;
        }

        if (cardID == 2 || cardID == 16) //Eliminar la carta con mas poder del campo
        {
            List<CardUI> list = new List<CardUI>(){new CardUI()};
            
            for (int i = 0; i < gameManager.tablero.Length; i++)
            {
                for (int j = 0; j < gameManager.tablero[i].cards.Count; j++)
                {
                    if (list[0] == null || gameManager.tablero[i].cards[j].actPower > list[0].actPower)
                    {
                        list = new List<CardUI>(){gameManager.tablero[i].transform.GetChild(j).gameObject.GetComponent<CardUI>()};
                    }
                    else if (gameManager.tablero[i].cards[j].actPower == list[0].actPower)
                    {
                        list.Add(gameManager.tablero[i].transform.GetChild(j).gameObject.GetComponent<CardUI>());
                    }
                }
            }
            if (list.Count > 1)
            {
                gameManager.playerTurn = 0;
                delCard.cardList = list;
                delCard.delCard = true;
            }
            else if (list.Count == 1)
            {
                list[0].transform.GetComponentInParent<File>().cards.Remove(list[0].card);
                Destroy(list[0].gameObject);  
            } 
        }

        if (cardID == 4 || cardID == 19) //Igualar el poder de las cartas al promedio
        {
            int totalPower = 0;
            int cards = 0;
            int average = 0;
            for (int i = 0; i < gameManager.tablero.Length; i++)
            {
                for (int j = 0; j < gameManager.tablero[i].cards.Count; j++)
                {
                    totalPower += gameManager.tablero[i].cards[j].actPower;
                    cards++;
                    average = totalPower/cards;
                }
            }

            for (int i = 0; i < gameManager.tablero.Length; i++)
            {
                for (int j = 0; j < gameManager.tablero[i].cards.Count; j++)
                {
                    if(!gameManager.tablero[i].cards[j].isGold)
                    gameManager.tablero[i].cards[j].powerVar = average - gameManager.tablero[i].cards[j].attackPower;
                }
            }
        }

        if (cardID == 5 || cardID == 20 || cardID == 25) //Robar dos cartas
        {
            if (card.player == 1)
            {
                StartCoroutine(DrawCards(gameManager.playerDeck1));
                IEnumerator DrawCards(PlayerDeck playerDeck)
                {
                    gameManager.cardSpawner.cardPrefab = gameManager.cardPrefab1;
                    gameManager.cardSpawner.fileFile = gameManager.playerHand1;
                    gameManager.cardSpawner.fieldTransform = gameManager.playerHand1.transform;

                    yield return new WaitForSecondsRealtime(1f);
                    gameManager.cardSpawner.SpawnCard(playerDeck.DrawCard());
                    yield return new WaitForSecondsRealtime(1f);
                    gameManager.cardSpawner.SpawnCard(playerDeck.DrawCard());
                }
            }

            if (card.player == 2)
            {
                StartCoroutine(DrawCards(gameManager.playerDeck2));
                IEnumerator DrawCards(PlayerDeck playerDeck)
                {
                    gameManager.cardSpawner.cardPrefab = gameManager.cardPrefab2;
                    gameManager.cardSpawner.fileFile = gameManager.playerHand2;
                    gameManager.cardSpawner.fieldTransform = gameManager.playerHand2.transform;

                    yield return new WaitForSecondsRealtime(1f);
                    gameManager.cardSpawner.SpawnCard(playerDeck.DrawCard());
                    if (card.efectFactor == 2){
                    yield return new WaitForSecondsRealtime(1f);
                    gameManager.cardSpawner.SpawnCard(playerDeck.DrawCard());}
                }
            }
        }

        if (cardID == 7 || cardID == 21) //Eliminar la carta con menos poder del campo rival
        {
            List<CardUI> list = new List<CardUI>(){new CardUI()};
            
            for (int i = 0; i < gameManager.tablero.Length; i++)
            {
                for (int j = 0; j < gameManager.tablero[i].cards.Count; j++)
                {
                    if (gameManager.tablero[i].player == card.player%2 + 1 && (list[0] == null || gameManager.tablero[i].cards[j].actPower < list[0].actPower))
                    {
                        list = new List<CardUI>(){gameManager.tablero[i].transform.GetChild(j).gameObject.GetComponent<CardUI>()};
                    }
                    else if (gameManager.tablero[i].player == card.player%2 + 1 && gameManager.tablero[i].cards[j].actPower == list[0].actPower)
                    {
                        list.Add(gameManager.tablero[i].transform.GetChild(j).gameObject.GetComponent<CardUI>());
                    }
                }
            }
            if (list.Count > 1)
            {
                gameManager.playerTurn = 0;
                delCard.cardList = list;
                delCard.delCard = true;
            }
            else if (list.Count == 1)
            {
                list[0].transform.GetComponentInParent<File>().cards.Remove(list[0].card);
                Destroy(list[0].gameObject);  
            }
        }

        if (cardID == 9 || cardID == 24) //Limpiar la fila con menos unidades
        {
            List<File> list = new List<File>(){new File()};
            for (int i = 0; i < gameManager.tablero.Length; i++)
            {  
                if (gameManager.tablero[i].cards.Count > 0 && (list[0] == null || gameManager.tablero[i].cards.Count < list[0].cards.Count))
                {
                    list = new List<File>(){gameManager.tablero[i]};
                }
                else if (gameManager.tablero[i].cards.Count == list[0].cards.Count)
                {
                    list.Add(gameManager.tablero[i]);
                }
            }
            if (list.Count > 1)
            {
                gameManager.playerTurn = 0;
                delCard.fileList = list;
                delCard.delFile = true;
            }
            else if (list.Count == 1)
            {
                for (int i = 0; i < list[0].cards.Count; i++)
                {
                    Destroy(list[0].transform.GetChild(0).gameObject);
                }
                list[0].cards.Clear();
            }
        }

    }} 
}
