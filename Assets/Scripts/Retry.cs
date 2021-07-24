using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Retry : MonoBehaviour
{
    public static Retry retry;
    public GameObject menu;
    public Button yourButton;
    public GameObject retry_canvas;
    public static bool first_time = true;

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

    void Update()
    {
        if (first_time)
        {
            Button btn = yourButton.GetComponent<Button>();
            btn.onClick.AddListener(RetryAlgorithm);
            first_time = false;
        }
    }

    public void RetryAlgorithm()
    {
        GameManager.game.RandomPositions();
        menu.SetActive(true);
    }
}
