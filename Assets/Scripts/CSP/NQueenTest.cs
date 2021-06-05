using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NQueenTest : MonoBehaviour
{
    [Button]
    public void Execute()
    {

        var columns = new int[] {1, 2, 3, 4, 5, 6, 7, 8};
        var rows = new Dictionary<int, List<int>>();

        foreach (int column in columns)
            rows[column] = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8};

        var csp = new CSP<int, int>(columns, rows);

        csp.AddConstraint(new QueenConstraint(columns));

        Dictionary<int, int> solution = csp.BacktrackingSearch(new Dictionary<int, int>());
        
        if(solution == null)
            Debug.Log("No Solution found");
        else
            Debug.Log(solution.ToString());
    }
}


if __name__ == "__main__":
    columns: List[int] = [1, 2, 3, 4, 5, 6, 7, 8]
    rows: Dict[int, List[int]] = {}
    for column in columns:
        rows[column] = [1, 2, 3, 4, 5, 6, 7, 8]
    csp: CSP[int, int] = CSP(columns, rows)
    csp.add_constraint(QueensConstraint(columns))
    solution: Optional[Dict[int, int]] = csp.backtracking_search()
    if solution is None:
        print("No solution found!")
    else:
        print(solution)