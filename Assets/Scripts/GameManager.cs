using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager game;
    public GameObject A;
    public GameObject B;
    public GameObject C;
    public GameObject menu;

    //gets called before application starts
    private void Awake()
    {
        if (game == null)
        {
            game = this;
        }
        else if (game != this) {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //OnClick method
    public void BS_Start()
    {
        Debug.Log("Breadth first search started...");
        menu.SetActive(false);
        Debug.Log(Breadth_First_Search.Clear(A));
        Debug.Log(Breadth_First_Search.Clear(B));
    }

    //OnClick method
    public void AS_Start()
    {
        Debug.Log("A* search started...");
        menu.SetActive(false);
    }
}
