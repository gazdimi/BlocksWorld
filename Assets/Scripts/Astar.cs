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

    void Update()
    {
        if (movement.Count != 0)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move                                  
            GameObject start = movement[0].Key;
            GameObject target = movement[0].Value;

            if (target != Table)
            {
                //move vertically
                if (start.transform.position.y != target.transform.position.y + 1f)
                {
                    start.transform.position = Vector3.MoveTowards(new Vector3(0f, start.transform.position.y, start.transform.position.z), new Vector3(0f, target.transform.position.y + 1f, start.transform.position.z), step);
                }
                else
                { //move horizontally
                    start.transform.position = Vector3.MoveTowards(new Vector3(0f, start.transform.position.y, start.transform.position.z), new Vector3(0f, start.transform.position.y, target.transform.position.z), step);
                }

                // Check if the position of the start and target are approximately equal
                if (Vector3.Distance(new Vector3(0f, 0f, start.transform.position.z), new Vector3(0f, 0f, target.transform.position.z)) < 0.001f)
                {
                    start.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, start.transform.position.z);
                    movement.RemoveAt(0);
                    retry++;
                }
            }
            else
            { //move block on top of the table
                //move horizontally
                if (start.transform.position.z != target.transform.position.z + 2f)
                {
                    start.transform.position = Vector3.MoveTowards(new Vector3(0f, start.transform.position.y, start.transform.position.z), new Vector3(0f, start.transform.position.y, target.transform.position.z + 2f), step);
                }
                else
                {//move vertically
                    start.transform.position = Vector3.MoveTowards(new Vector3(0f, start.transform.position.y, start.transform.position.z), new Vector3(0f, target.transform.position.y + 0.5f, start.transform.position.z), step);
                }

                // Check if the position of the start and target are approximately equal
                if (Vector3.Distance(new Vector3(0f, start.transform.position.y, start.transform.position.z), new Vector3(0f, target.transform.position.y + 0.5f, start.transform.position.z)) < 0.001f)
                {
                    start.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, start.transform.position.z);
                    movement.RemoveAt(0);
                    retry++;
                }
            }
        }
        else if (movement.Count == 0 && retry != 0)
        { //solution done
            retry_canvas.SetActive(true);
        }
    }

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

        if (initial_state.ProblemSolved())
        {
            return initial_state;
        }
        List<State> search_frontier = new List<State>();
        List<State> closed_set = new List<State>();

        search_frontier.Add(initial_state);
        while (search_frontier.Count != 0)
        {
            List<int> costs = new List<int>();
            foreach (State node in search_frontier)
            {
                costs.Add(node.h + node.g);
            }
            int index = costs.IndexOf(GetMin(costs));

            State current_state = search_frontier[index];
            search_frontier.RemoveAt(index);
            if (current_state.ProblemSolved())
            {
                return current_state;
            }
            closed_set.Add(current_state);
            List<State> children = SequentialStates(current_state);
            foreach (State child in children)
            {
                if (closed_set.Contains(child) && (current_state.g < child.g))
                {
                    child.g = current_state.g;
                    child.parent = current_state;
                }
                else if (search_frontier.Contains(child) && (current_state.g < child.g))
                {
                    child.g = current_state.g;
                    child.parent = current_state;
                }
                else {
                    search_frontier.Add(child);
                    child.g = current_state.g;
                }
            }
        }
        return null;
    }

    public static void PrintSolution(State solution)
    {
        List<State> path = new List<State> { solution };
        State parent = solution.parent;
        while (parent != null)
        {
            path.Add(parent);
            parent = parent.parent;
        }

        State previous = null;
        Debug.Log("-------------Solution above-------");
        for (int i = 0; i < path.Count; i++)
        {
            State state = path[path.Count - i - 1];
            if (previous != null)
            {                                                                     //block movement
                foreach (GameObject key in previous.on_top_of.Keys)
                {
                    if (previous.on_top_of[key] != state.on_top_of[key])
                    {
                        movement.Add(new KeyValuePair<GameObject, GameObject>(key, state.on_top_of[key]));
                    }
                }
            }
            Debug.Log("Move " + i);
            Debug.Log("A on top of " + state.on_top_of[A].name);
            Debug.Log("A clear on top " + state.clear_on_top[A] + "\n");

            Debug.Log("B on top of " + state.on_top_of[B].name);
            Debug.Log("B clear on top " + state.clear_on_top[B] + "\n");

            Debug.Log("C on top of " + state.on_top_of[C].name);
            Debug.Log("C clear on top " + state.clear_on_top[C] + "\n");
            previous = state;
        }
        return;
    }

    public static List<State> SequentialStates(State current_state)
    {
        List<State> children = new List<State>();
        State new_state;

        if (current_state.clear_on_top[C])                                                                  //C block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[C];
            //--------------------------------------move on the table-------------------------------------
            if (aboveBlock != Table)
            {
                new_state = new State();

                new_state.on_top_of.Add(C, Table);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);

                new_state.h = current_state.h - 1;
                new_state.g = current_state.g + 1;

                new_state.clear_on_top.Add(C, true);
                if (aboveBlock == A)
                {
                    new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                    new_state.clear_on_top.Add(A, true);
                }
                else if (aboveBlock == B)
                {
                    new_state.clear_on_top.Add(B, true);
                    new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                }
                else
                {
                    new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                    new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                }

                if (new_state.ValidMove())
                {                                                                                               //check for best move
                    new_state.parent = current_state;                                                           //keep previous state
                    children.Add(new_state);
                }
            }
        }

        if (current_state.clear_on_top[B])                                                                  //B block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[B];
            if (current_state.clear_on_top[C])                                                              //B can be placed on top of C
            {

                new_state = new State();

                new_state.on_top_of.Add(B, C);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);
                new_state.on_top_of.Add(A, current_state.on_top_of[A]);

                new_state.h = current_state.h - 1;
                new_state.g = current_state.g + 1;

                new_state.clear_on_top.Add(B, true);
                new_state.clear_on_top.Add(C, false);
                if (aboveBlock == A)
                {
                    new_state.clear_on_top.Add(A, true);
                }
                else
                {
                    new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                }

                if (new_state.ValidMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }
            else
            {
                //--------------------------------------move on the table-------------------------------------
                if (aboveBlock != Table)
                {
                    new_state = new State();

                    new_state.on_top_of.Add(B, Table);
                    new_state.on_top_of.Add(A, current_state.on_top_of[A]);
                    new_state.on_top_of.Add(C, current_state.on_top_of[C]);

                    new_state.h = current_state.h - 1;
                    new_state.g = current_state.g + 1;

                    new_state.clear_on_top.Add(B, true);
                    if (aboveBlock == A)
                    {
                        new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                        new_state.clear_on_top.Add(A, true);
                    }
                    else if (aboveBlock == C)
                    {
                        new_state.clear_on_top.Add(C, true);
                        new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                    }
                    else
                    {
                        new_state.clear_on_top.Add(A, current_state.clear_on_top[A]);
                        new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                    }

                    if (new_state.ValidMove())
                    {                                                                                               //check for best move
                        new_state.parent = current_state;                                                           //keep previous state
                        children.Add(new_state);
                    }
                }
            }
        }

        if (current_state.clear_on_top[A])                                                                  //A block clear on top [valid move]
        {
            GameObject aboveBlock = current_state.on_top_of[A];

            //---------------------------------move on top of another block--------------------------------
            if (current_state.clear_on_top[B])                                                              //A can be placed on top of B
            {
                new_state = new State();                                                                    //initialize & construct new state 
                new_state.on_top_of.Add(A, B);
                new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                new_state.on_top_of.Add(C, current_state.on_top_of[C]);

                new_state.h = current_state.h - 1;
                new_state.g = current_state.g + 1;

                new_state.clear_on_top.Add(B, false);
                new_state.clear_on_top.Add(A, true);
                if (aboveBlock == C)
                {
                    new_state.clear_on_top.Add(C, true);
                }
                else
                {
                    new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                }

                if (new_state.ValidMove())
                {                                                                                           //check for best move
                    new_state.parent = current_state;                                                       //keep previous state
                    children.Add(new_state);
                }
            }
            else
            {
                //--------------------------------------move on the table-------------------------------------
                if (aboveBlock != Table)
                {
                    new_state = new State();

                    new_state.on_top_of.Add(A, Table);
                    new_state.on_top_of.Add(B, current_state.on_top_of[B]);
                    new_state.on_top_of.Add(C, current_state.on_top_of[C]);

                    new_state.h = current_state.h - 1;
                    new_state.g = current_state.g + 1;

                    new_state.clear_on_top.Add(A, true);
                    if (aboveBlock == B)
                    {
                        new_state.clear_on_top.Add(B, true);
                        new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                    }
                    else if (aboveBlock == C)
                    {
                        new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                        new_state.clear_on_top.Add(C, true);
                    }
                    else
                    {
                        new_state.clear_on_top.Add(B, current_state.clear_on_top[B]);
                        new_state.clear_on_top.Add(C, current_state.clear_on_top[C]);
                    }

                    if (new_state.ValidMove())
                    {                                                                                               //check for best move
                        new_state.parent = current_state;                                                           //keep previous state
                        children.Add(new_state);
                    }
                }
            }
        }
        if (children.Count >= 2)
        {
            foreach (State child in children)
            {
                if (child.on_top_of[A] == Table && child.on_top_of[B] == Table && child.on_top_of[C] == Table)
                {
                    children.Remove(child);                                                                     //if another best move exists (than placing all blocks on the table)
                    break;
                }
            }
        }
        return children;
    }

    public static int GetMin(List<int> costs) {
        int min = costs[0];
        foreach (int cost in costs) {
            if (cost < min) {
                min = cost;
            }
        }
        return min;
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
                if (on_top_of[A] == B)
                {
                    _h += 2;
                }
                else {
                    _h++;
                }
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

        public bool ValidMove()
        {
            if (!(flag) && on_top_of[C] == Table)
            {
                flag = true;
                return true;
            }

            if (flag)                                                                                       //C is on the table, select the next best move
            {
                if ((on_top_of[C] == Table && on_top_of[B] == C && on_top_of[A] == B) ||
                    (on_top_of[C] == Table && on_top_of[B] == C) ||
                    (on_top_of[A] == Table && on_top_of[B] == Table && on_top_of[C] == Table))
                {

                    return true;
                }
            }
            return false;
        }
    }
}
