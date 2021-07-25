using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Retry : MonoBehaviour
{
    public static Retry retry;
    public GameObject menu;
    public GameObject retry_canvas;

    //gets called before application starts
    private void Awake()
    {
        if (retry == null)
        {
            retry = this;
        }
        else if (retry != this)
        {
            Destroy(this);
        }
    }

    public void RetryAlgorithm()
    {
        GameManager.game.RandomPositions();
        menu.SetActive(true);
    }
}
