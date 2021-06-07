using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class CSP<V,D>
{
    private readonly Dictionary<V, List<D>> _domain;
    private readonly V[] _variables;
    private readonly Dictionary<V, List<Constraint<V,D>>> _constraint; 
 
    public CSP(V[] variables, Dictionary<V,List<D>> domain)
    {
        _domain = domain;
        _constraint = new Dictionary<V, List<Constraint<V,D>>>();
        _variables = variables;
        foreach (V variable in _variables)
            _constraint.Add(variable,new List<Constraint<V, D>>());
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

    public Dictionary<V, D> BacktrackingSearchRecursive(Dictionary<V, D> assignment)
    {
        if (assignment.Count == _variables.Length)
            return assignment; //Asignacion de todas las variables

        var unassigned = CollectUnassigned(assignment);
        V first = unassigned[0];

        foreach (D value in _domain[first])
        {
            var localAssignment = new Dictionary<V, D>(assignment);
            localAssignment[first] = value;

            if (Consistent(first, localAssignment))
            {
                var result = BacktrackingSearchRecursive(localAssignment);
                if (result != null)
                    return result;
            }
        }
        return null;
    }

    public void AllBacktrackingSearchRecursive(Dictionary<V, D> assignment, ref List<Dictionary<V, D>> solutions)
    {
        if (assignment.Count == _variables.Length)//Asignacion de todas las variables
        {
            solutions.Add(assignment);
            return; 
        }

        var unassigned = CollectUnassigned(assignment);
        V first = unassigned[0];

        foreach (D value in _domain[first])
        {
            var localAssignment = new Dictionary<V, D>(assignment);
            localAssignment[first] = value;

            if (Consistent(first, localAssignment))
                AllBacktrackingSearchRecursive(localAssignment, ref solutions);
        }
        return;
    }
   

    public List<Dictionary<V, D>> BacktrackingSearchAllIterative(int maxSolutionCount = int.MaxValue)
    {
        Dictionary<V, D> assignment = new Dictionary<V, D>();
        List<Dictionary<V, D>> solutions = new List<Dictionary<V, D>>();
        var list = new List<V>(_variables);
        list.Reverse();
        Stack<V> next = new Stack<V>(list);
        Stack<V> previous = new Stack<V>();

        while (next.Count > 0) //Mientras hay valores sin asignar seguimos
        {
            V currentVariable = next.Pop(); //Sacamos el siguiente valor sin asignacion correcta
            previous.Push(currentVariable);
            int valueIndex = 0;

            if (assignment.ContainsKey(currentVariable)) //Si el valor fue asignado significa que su valor fue asignado pero incorrecto (no llego a una solucion)
                valueIndex = _domain[currentVariable].IndexOf(assignment[currentVariable])+1;

            if (valueIndex < _domain.Count) //Si el index es mayor al numero de posibles valores en el dominio, significa que este camino no es "consistente" y se debe hacer un backtracking
            {

                assignment[currentVariable] = _domain[currentVariable][valueIndex];
                if (!Consistent(currentVariable, assignment))
                    next.Push(previous.Pop());
                else if (assignment.Count == _variables.Length)
                {
                    solutions.Add(new Dictionary<V, D>(assignment));

                    if(solutions.Count != maxSolutionCount) //Si todavia no completamos el numero de soluciones necesarias, seguimos como si no hubieramos encontrado nada
                        next.Push(previous.Pop());
                }
            }
            else
            {
                next.Push(previous.Pop());
                if (previous.Count == 0)
                    break; //Ya no hay mas posibles combinaciones

                //Agregamos la variable previa a la pila y eliminamos el valor de la variable actual, para iterar nuevamente desde 0
                assignment.Remove(currentVariable);
                next.Push(previous.Pop());
            }
        }

        return solutions;
    }

    public List<Dictionary<V, D>> RandomSolutions(int maxSolutionCount = int.MaxValue)
    {
        Dictionary<V, D> assignment = new Dictionary<V, D>();
        List<Dictionary<V, D>> solutions = new List<Dictionary<V, D>>();
        var list = new List<V>(_variables);
        list.Reverse();
        Stack<V> next = new Stack<V>(list);
        Stack<V> previous = new Stack<V>();

        Dictionary<V, Queue<D>> activeTempDomain = new Dictionary<V, Queue<D>>();

        Dictionary<V, List<D>> activeRandomOrder = new Dictionary<V, List<D>>();

        foreach (var cVariable in _variables)
        {
            Queue<D> possibleValues = new Queue<D>();
            var cVariableDomain = new List<D>(_domain[cVariable]);
            int count = cVariableDomain.Count;

            for (int i = 0; i < count; i++)
            {
                int index = UnityEngine.Random.Range(0, cVariableDomain.Count);
                var value = cVariableDomain[index];
                possibleValues.Enqueue(value);
                cVariableDomain.RemoveAt(index);
            }
            activeRandomOrder.Add(cVariable, new List<D>(possibleValues));
        }


        while (next.Count > 0) //Mientras hay valores sin asignar seguimos
        {
            V currentVariable = next.Pop(); //Sacamos el siguiente valor sin asignacion correcta
            previous.Push(currentVariable);

            if (!activeTempDomain.ContainsKey(currentVariable))
                activeTempDomain.Add(currentVariable, new Queue<D>(activeRandomOrder[currentVariable]));

            var valuesQueue = activeTempDomain[currentVariable];

            if (valuesQueue.Count > 0)
            {
                assignment[currentVariable] = valuesQueue.Dequeue();

                if (!Consistent(currentVariable, assignment))
                {
                    next.Push(previous.Pop());
                }
                else if (assignment.Count == _variables.Length)
                {
                    solutions.Add(new Dictionary<V, D>(assignment));
                    if (solutions.Count != maxSolutionCount) //Si todavia no completamos el numero de soluciones necesarias, seguimos como si no hubieramos encontrado nada
                        next.Push(previous.Pop());
                }
            }
            else
            {
                assignment.Remove(currentVariable);
                activeTempDomain.Remove(currentVariable);
                next.Push(previous.Pop());
                if (previous.Count == 0)
                    break; //Ya no hay mas posibles combinaciones

                var previousValue = previous.Pop();
                next.Push(previousValue);
            }
        }

        return solutions;
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
            //Debug.Log($"Assignment: [{pair.Key}]=={pair.Value}");
            int currentColumn = pair.Key;
            int currentRow = pair.Value;

            for (int i = currentColumn+1; i < Variables.Length+1; i++)
            {
             
                if (!assignment.ContainsKey(i))
                    continue;

                int targetColumn = i;
                int targetRow = assignment[i];
                //Debug.Log($"[{currentColumn},{currentRow}] [{targetColumn},{targetRow}]");

                if (currentRow == targetRow)
                {
                    //Debug.Log("Assignment:False SAME ROW}");
                    return false;//Same row
                }

//                Debug.Log($"{Mathf.Abs(currentColumn - targetColumn)}=={Mathf.Abs(currentRow - targetRow)}");
                if (Mathf.Abs(currentColumn - targetColumn) == Mathf.Abs(currentRow - targetRow))
                {
                    //Debug.Log("Assignment:False     SAME DIAGONAL}");
                    return false;//Misma diagonal
                }
            }
        }
        //Debug.Log("Assignment:True}");
        return true;
    }
}
