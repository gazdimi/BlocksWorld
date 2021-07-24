using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager game;
    public GameObject A;
    public GameObject B;
    public GameObject C;
    public GameObject Table;
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
        RandomPositions();
    }

    public void RandomPositions() {
        //x: 0, y: from 0.5 to 2.5 (step 1.0), z: from -1 to 2 (step 1.5)
        float x = 0f;
        List<float> y = new List<float> { 0.5f, 1.5f, 2.5f };
        List<float> z = new List<float> { -1f, 0.5f, 2f };

        List<List<int>> combinations = new List<List<int>> { new List<int> { 3 }, new List<int> { 1, 2 }, new List<int> { 2, 1 }, new List<int> { 1, 1, 1 } };
        List<int> randomChoice = combinations[new System.Random().Next(combinations.Count)];
        List<GameObject> letters = new List<GameObject> { A, B, C };
        if (randomChoice.Count == 1)                                                                                        //single stack of blocks
        {
            while (letters.Count != 0)
            {
                GameObject randomG = letters[new System.Random().Next(letters.Count)];
                randomG.transform.position = new Vector3(x, y[0], z[0]);
                y.RemoveAt(0);
                letters.Remove(randomG);                                                                                    //set its position, then remove it
            }
        }
        else if (randomChoice.Count == 2)                                                                                   //two stacks of blocks
        {
            if (randomChoice[0] == 1)                                                                                       //first stack has 1 block, second stack has 2 blocks
            {
                GameObject randomG = letters[new System.Random().Next(letters.Count)];
                randomG.transform.position = new Vector3(x, y[0], z[0]);
                letters.Remove(randomG);

                while (letters.Count != 0)
                {
                    randomG = letters[new System.Random().Next(letters.Count)];
                    randomG.transform.position = new Vector3(x, y[0], z[1]);
                    y.RemoveAt(0);
                    letters.Remove(randomG);
                }
            }
            else
            {                                                                                                          //first stack has 2 blocks, second stack has 1 block
                GameObject randomG;
                while (letters.Count != 1)                                                                                  //keep one remaining block in order to place it in the 2nd stack
                {
                    randomG = letters[new System.Random().Next(letters.Count)];
                    randomG.transform.position = new Vector3(x, y[0], z[0]);
                    y.RemoveAt(0);
                    letters.Remove(randomG);
                }

                randomG = letters[new System.Random().Next(letters.Count)];
                randomG.transform.position = new Vector3(x, 0.5f, z[1]);
            }
        }
        else
        {                                                                                                             //three stacks of blocks
            while (letters.Count != 0)
            {
                GameObject randomG = letters[new System.Random().Next(letters.Count)];
                randomG.transform.position = new Vector3(x, y[0], z[0]);
                z.RemoveAt(0);
                letters.Remove(randomG);
            }
        }
    }

    //OnClick method
    public void BS_Start()
    {
        Debug.Log("Breadth first search started...");
        menu.SetActive(false);
        Breadth_First_Search.State solution = Breadth_First_Search.Search(A,B,C,Table);
        Breadth_First_Search.PrintSolution(solution);
    }

    //OnClick method
    public void AS_Start()
    {
        Debug.Log("A* search started...");
        menu.SetActive(false);
        Astar.State solution = Astar.Search(A, B, C, Table);
        //Astar.PrintSolution(solution);
    }
}
