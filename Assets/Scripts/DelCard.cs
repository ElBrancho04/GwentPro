using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelCard : MonoBehaviour
{
    public GameManager gameManager;
    public Efects efects;
    public CardData card;
    public List<CardUI> cardList;
    public List<File> fileList;
    public bool delCard;
    public bool delFile;
    public CardData cardToPlayEfct;
    public File fileToPlayEfct;


    void Update()
    {
        cardToPlayEfct = gameManager.cardToPlayEfct;

        if (delCard && cardToPlayEfct != null)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i].card == cardToPlayEfct)
                {
                    cardList[i].transform.GetComponentInParent<File>().cards.Remove(cardList[i].card);
                    Destroy(cardList[i].gameObject);
                    delCard = false;
                    gameManager.cardToPlayEfct = null;
                    gameManager.UIcardToPlayEfct = null;
                    gameManager.playerTurn = card.player%2 + 1;
                    break;
                }
            }
        }

        fileToPlayEfct = gameManager.fileToPlayEfct;

        if (delFile && fileToPlayEfct != null)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                if (fileList[i] == fileToPlayEfct)
                {
                    for (int j = 0; j < fileList[i].cards.Count; j++)
                    {
                        Destroy(fileList[i].transform.GetChild(0).gameObject);
                    }
                    fileList[i].cards.Clear();
                    delFile = false;
                    gameManager.fileToPlayEfct = null;
                    gameManager.playerTurn = card.player%2 + 1;
                    break;
                }
            }
        }
    }
   
}
