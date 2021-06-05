using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CSP<V,D>
{
    private Dictionary<V, List<D>> _domain;
    private readonly V[] _variables;
    private readonly Dictionary<V, List<Constraint<V,D>>> _constraint; 
 
    public CSP(V[] variables, Dictionary<V,List<D>> domain)
    {
        _domain = domain;
        _constraint = new Dictionary<V, List<Constraint<V,D>>>();
        _variables = variables;
    }

    public void AddConstraint(Constraint<V, D> nConstraint)
    {
        foreach (var variable in nConstraint.Variables)
        {
            if (Exists(variable))
                _constraint[variable].Add(nConstraint);
            else
                throw new Exception("Variable doesnt exist");
        }
    }

    private bool Exists(V variable)
    {
        for (int i = 0; i < _variables.Length; i++)
            if (_variables[i].Equals(variable)) return true;
        return false;
    }

    public bool Consistent(V variable, Dictionary<V, D> assignment)
    {
        foreach (Constraint<V,D> constraint in _constraint[variable])
        {
            if (!constraint.Satisfied(assignment))
                return false;
        }
        return true;
    }

    private List<V> CollectUnassigned(Dictionary<V, D> assignment)
    {
        var unassigned = new List<V>();
        foreach (V variable in _variables)
        {
            if (!assignment.ContainsKey(variable))
                unassigned.Add(variable);
        }

        return unassigned;
    }

    
    
    public Dictionary<V, D> BacktrackingSearch(Dictionary<V, D> assignment)
    {
        if (assignment.Count == _variables.Length)
            return assignment; //Asignacion de todas las variables

        var unassigned = CollectUnassigned(assignment);
        V first = unassigned[0];


        foreach (D value in _domain[first])
        {
            var localAssignment = new Dictionary<V, D>(assignment);
            localAssignment[first] = value;

            if (!Consistent(first, localAssignment)) continue;
            
            var result = BacktrackingSearch(localAssignment);
            if (result != null)
                return result;
        }
        return null;
    }
  /*

    def backtracking_search(self, assignment: Dict[V, D] = {}) -> Optional[Dict[V, D]]:
# assignment is complete if every variable is assigned (our base case)
    if len(assignment) == len(self.variables):
        return assignment

# get all variables in the CSP but not in the assignment
        unassigned: List[V] = [v for v in self.variables if v not in assignment]

# get the every possible domain value of the first unassigned variable
    first: V = unassigned[0]
    for value in self.domains[first]:
        local_assignment = assignment.copy()
        local_assignment[first] = value
        # if we're still consistent, we recurse (continue)
        if self.consistent(first, local_assignment):
            result: Optional[Dict[V, D]] = self.backtracking_search(local_assignment)
            # if we didn't find the result, we will end up backtracking
            if result is not None:
                return result
    return None
    */
}

public abstract class Constraint<T, B>
{
    public T[] Variables { get; }

    protected Constraint(T[] variables)
    {
        Variables = variables;
    }

    public abstract bool Satisfied(Dictionary<T, B>  assignment);
}


public abstract class Variable<T>
{

}

public abstract class Domain<T>
{
}


public class QueenConstraint : Constraint<int, int>
{
    public QueenConstraint(int[] variables) : base(variables)
    {
    }

    public override bool Satisfied(Dictionary<int, int> assignment)
    {
        foreach (KeyValuePair<int,int> pair in assignment)
        {
            int currentColumn = pair.Key;
            int currentRow = pair.Value;

            for (int i = currentColumn; i < Variables.Length; i++)
            {
                if(!assignment.ContainsKey(i)) continue;
                int targetColumn = i;
                int targetRow = assignment[i];

                if (currentColumn == targetColumn) return false;//misma columna
                if(Mathf.Abs(currentRow - targetRow) == Mathf.Abs(currentColumn-targetColumn))  return false;//Misma diagonal
            }
        }
        return true;
    }
}
