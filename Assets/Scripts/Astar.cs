using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    public GameObject retry_canvas;

    private static GameObject A;
    private static GameObject B;
    private static GameObject C;
    private static GameObject Table;


    public float speed = 1.0f;                                                                                  // Adjust the speed for the application
    private static List<KeyValuePair<GameObject, GameObject>> movement = new List<KeyValuePair<GameObject, GameObject>>(); //key = start, value = target   
    private static bool flag = false;
    private int retry = 0;

    public static State Search(GameObject a, GameObject b, GameObject c, GameObject table) {
        A = a; B = b; C = c; Table = table;
        State initial_state = new State();

        initial_state.on_top_of.Add(A, initial_state.GetDownName(A));
        initial_state.on_top_of.Add(B, initial_state.GetDownName(B));
        initial_state.on_top_of.Add(C, initial_state.GetDownName(C));
        initial_state.clear_on_top.Add(A, initial_state.Clear(A));
        initial_state.clear_on_top.Add(B, initial_state.Clear(B));
        initial_state.clear_on_top.Add(C, initial_state.Clear(C));
        initial_state.CalculateH();
        initial_state.g = 0;
        return null;
    }

    public class State
    {
        public Dictionary<GameObject, GameObject> on_top_of = new Dictionary<GameObject, GameObject>();     //(key) block on top of block or table (value) 
        public Dictionary<GameObject, bool> clear_on_top = new Dictionary<GameObject, bool>();              //(key) block (value) clear on top
        public int h;                                                                                       //estimation of distance from the final state (solution)
        public int g;                                                                                       //distance from initial state
        public State parent;

        public State() { }

        public bool Clear(GameObject x)
        {                                                                                                   //a block can be placed on top of x
            RaycastHit hit;
            var ray = new Ray(x.transform.position, x.transform.TransformDirection(Vector3.up));
            if (!Physics.Raycast(ray, out hit, 25))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameObject GetDownName(GameObject gameObject)
        {
            RaycastHit hit;
            var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.down));
            if (Physics.Raycast(ray, out hit, 25))
            {
                return hit.transform.gameObject;
            }
            return null;
        }

        public void CalculateH()
        {
            int _h = 0;
            //C cost
            if (clear_on_top[C] && on_top_of[C] != Table)
            {
                _h++;
            }
            else if (!(clear_on_top[C]) && on_top_of[C] != Table) 
            {
                _h += 2;
            }

            //B cost
            if (!(clear_on_top[B]) && on_top_of[B] == Table)
            {
                if (on_top_of[A] != Table && on_top_of[C] != Table)
                {
                    _h += 3;
                }
                else if (on_top_of[A] == B)
                {
                    _h += 2;
                }
                else {
                    _h++;
                }
            }
            else if (!(clear_on_top[B]) && on_top_of[B] != Table)
            {
                _h += 2;
            }
            else if (clear_on_top[B] && on_top_of[B] != C) {
                _h++;
            }

            //A cost
            if (!(clear_on_top[A]) && on_top_of[A] == Table)
            {
                _h++;
            }
            else if (!(clear_on_top[A]) && on_top_of[A] != Table)
            {
                _h += 2;
            }
            else if (clear_on_top[A] && on_top_of[A] != B)
            {
                if (on_top_of[A] == Table)
                {
                    _h++;
                }
                else {
                    _h += 2;
                }
            }
            this.h = _h;
        }

        public bool ProblemSolved()
        {
            if (on_top_of[A] == B && on_top_of[B] == C && on_top_of[C] == Table)
            {
                return true;                                                                                //goal state, problem solved
            }
            return false;
        }

    }
}
