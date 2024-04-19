using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressEnterToCont : MonoBehaviour
{
    public GameManager gameManager;
    public int player;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            gameObject.SetActive(false);
            if (player == 1)
            gameManager.waitForActivate1 = false;
            else
            gameManager.waitForActivate2 = false;
        }
    }
}
