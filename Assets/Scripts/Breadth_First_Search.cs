using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search : MonoBehaviour
{
    private static GameObject A;
    private static GameObject B;
    private static GameObject C;
    private static GameObject Table;

    private static bool move = false;
    public float speed = 1.0f;                                                                                  // Adjust the speed for the application
    private static GameObject start;
    private static GameObject target;



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

    void Update()
    {
        if (move)
        {
            float step = speed * Time.deltaTime; // calculate distance to move                                  // Move our position a step closer to the target.
            start.transform.position = Vector3.MoveTowards(start.transform.position, target.transform.position, step);
            Debug.Log("should move");
            // Check if the position of the start and target are approximately equal
            if (Vector3.Distance(transform.position, target.transform.position) < 0.001f)
            {
                move = false;
            }
        }
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
            List<State> children = SequentialStates(current_state);
            foreach (State child in children) 
            {
                if (!(closed_set.Contains(child)) || !(search_frontier.Contains(child)))
                {
                    search_frontier.Push(child);
                }
            }
        }
        return null;
    }

    public static List<State> SequentialStates(State current_state) { //x same for each block
        List<State> children = new List<State>();
        State new_state = new State();
        if (current_state.clear_on_top[C])                                                                  //C block clear on top
        {
            if (current_state.clear_on_top[B])                                                              //B block clear on top 
            {

                //movement B on top of C
                move = true;
                start = B;
                target = C;

                if (!move) {
                    new_state.On(B, C);
                    new_state.On(C, Table);
                    new_state.On(A, Table);
                    new_state.Clear(A);
                    new_state.Clear(B);
                    new_state.Clear(C);
                    new_state.parent = current_state;
                    children.Add(new_state);
                }
            }
        }
        else {
            if (current_state.clear_on_top[B])
            {



            }
            else {                                                                                          //A on top of C
            
            
            }


        }
        return children;
    }

    public class State {

        public Dictionary<GameObject, GameObject> on_top_of = new Dictionary<GameObject, GameObject>();     //(key) block on top of block or table (value) 
        public Dictionary<GameObject,bool> clear_on_top = new Dictionary<GameObject, bool>();              //(key) block (value) clear on top
        public State parent;
       
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
            else {
                clear_on_top.Add(x, false);
            }
        }

        public string GetOnTop(GameObject gameObject)
        {
            RaycastHit hit;
            var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));
            if (Physics.Raycast(ray, out hit, 25))
            {
                return hit.collider.name;
            }
            return null;
        }

        public bool ProblemSolved() {
            if (on_top_of[A] == B && on_top_of[B]==C && on_top_of[C]==Table) {
                return true;                                                                                //goal state, problem solved
            }
            return false;
        }
    }
}
