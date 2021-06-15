using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search
{
    private static GameObject A;
    private static GameObject B;
    private static GameObject C;
    private static GameObject Table;

    public static bool Clear(GameObject gameObject)
    {
        //return Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));

        RaycastHit hit;
        var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));
        if (Physics.Raycast(ray, out hit, 25))
        {
            Debug.Log(hit.collider.name);
            return true;
        }
        return false;
    }

    public static State Search(GameObject a, GameObject b, GameObject c, GameObject table) 
    {
        A = a; B = b; C = c; Table = table;
        State initial_state = new State();
        initial_state.on_top_of.Add(A,Table); 
        initial_state.on_top_of.Add(B, Table); 
        initial_state.on_top_of.Add(C, Table);
        initial_state.Clear(A);
        initial_state.Clear(B);
        initial_state.Clear(C);

        if (initial_state.ProblemSolved()) {
            return initial_state;
        }
        Stack<State> search_frontier = new Stack<State>();
        List<State> closed_set = new List<State>();

        search_frontier.Push(initial_state);
        while (search_frontier.Count != 0) 
        {
            State current_state = search_frontier.Pop();                                                    //removes & returns the object at the top of the Stack
            if (current_state.ProblemSolved()) {
                return current_state;
            }
            closed_set.Add(current_state);
            
        }
        return null;
    }

    public List<State> SequentialStates(State current_state) {
        List<State> children = new List<State>();
        return children;
    }

    public class State {

        public Dictionary<GameObject, GameObject> on_top_of = new Dictionary<GameObject, GameObject>();     //(key) block on top of block or table (value) 
        private Dictionary<GameObject,bool> clear_on_top = new Dictionary<GameObject, bool>();              //(key) block (value) clear on top
       
        public State() {  }

        public void On(GameObject b, GameObject x) {                                                        //block b on top of x (block or table)
            RaycastHit hit;
            var ray = new Ray(x.transform.position, x.transform.TransformDirection(Vector3.up));
            if (Physics.Raycast(ray, out hit, 25))
            {
                if (hit.collider.name.Equals(b.name)) {
                    on_top_of.Add(b, x);
                }
            }
        }

        public void Clear(GameObject x) {                                                                   //a block can be placed on top of x
            RaycastHit hit;
            var ray = new Ray(x.transform.position, x.transform.TransformDirection(Vector3.up));
            if (!Physics.Raycast(ray, out hit, 25))
            {
                clear_on_top.Add(x, true);
            }
        }

        public bool ProblemSolved() {
            if (on_top_of[A] == B && on_top_of[B]==C) {
                return true;                                                                                //goal state, problem solved
            }
            return false;
        }
    }
}
