using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Retry : MonoBehaviour
{
    public GameObject menu;
    public GameObject retry;
    public Button yourButton;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(RetryAlgorithm);
    }

    public void RetryAlgorithm()
    {
        GameManager.game.RandomPositions();
        menu.SetActive(true);
        retry.SetActive(false);
    }
}
