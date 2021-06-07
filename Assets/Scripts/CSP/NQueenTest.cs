using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NQueenTest : MonoBehaviour
{
    [Button]
    public void FirstSolutionComparison(int n = 8)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteRecursive(n, false);
        stopWatch.Stop();
        var recursive = stopWatch.Elapsed;

        stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteAllIterative(n, 1, false);
        stopWatch.Stop();

        var iterative = stopWatch.Elapsed;

        stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteAllIterativeRandomSolution(n, 1, false);
        stopWatch.Stop();

        var iterativeRandom = stopWatch.Elapsed;

        Debug.Log($"Recursive:{recursive.TotalMilliseconds}   |   IterativeLineal:{iterative.TotalMilliseconds} | IterativeRandom:{iterativeRandom.TotalMilliseconds}");
    }

    [Button]
    public void RandomSolutionMean(int n = 8, int repetitions = 100)
    {
        double totalTime = 0;

        for (int i = 0; i < repetitions; i++)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            ExecuteAllIterativeRandomSolution(n, 1, false);
            stopWatch.Stop();
            totalTime += stopWatch.Elapsed.TotalMilliseconds;

        }
        Debug.Log($"totalTime:{totalTime}");
        Debug.Log($"Mean:{totalTime / repetitions} Milliseconds");
    }


    [Button]
    public void AllSolutionsComparison(int n = 8)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteAllRecursive(n, false);
        stopWatch.Stop();

        var recursive = stopWatch.Elapsed;

        stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteAllIterative(n, int.MaxValue, false);
        stopWatch.Stop();

        var iterative = stopWatch.Elapsed;

        stopWatch = new Stopwatch();
        stopWatch.Start();
        ExecuteAllIterativeRandomSolution(n, int.MaxValue, false);
        stopWatch.Stop();

        var iterativeRandom = stopWatch.Elapsed;

        Debug.Log($"Recursive:{recursive}   |   IterativeLineal:{iterative} | IterativeRandom:{iterativeRandom}");

    }

    [Button]
    public void ExecuteRecursive(int n = 8, bool printSolution = true)
    {
        var columns = new int[n];
        for (int i = 0; i < n; i++)
            columns[i] = i + 1;

        var rows = new Dictionary<int, List<int>>();

        foreach (int column in columns)
            rows[column] = new List<int>(columns);

        var csp = new CSP<int, int>(columns, rows);
        csp.AddConstraint(new QueenConstraint(columns));

        Dictionary<int, int> solution = csp.BacktrackingSearchRecursive(new Dictionary<int, int>());

        if (!printSolution)
            return;
        if (solution == null)
            UnityEngine.Debug.Log("No Solution found");
        else
            SolutionToString(solution);
    }

    [Button]
    public void ExecuteAllRecursive(int n=8, bool printSolution = true)
    {
        var columns = new int[n];
        for (int i = 0; i < n; i++)
            columns[i] = i + 1;

        var rows = new Dictionary<int, List<int>>();

        foreach (int column in columns)
            rows[column] = new List<int>(columns);

        var csp = new CSP<int, int>(columns, rows);
        csp.AddConstraint(new QueenConstraint(columns));
        List<Dictionary<int, int>> solutions = new List<Dictionary<int, int>>();
        csp.AllBacktrackingSearchRecursive(new Dictionary<int, int>(), ref solutions);

        if (!printSolution) return;

        if(solutions.Count == 0)
            UnityEngine.Debug.Log("No Solution found");
        else
        {
            foreach (var solution in solutions)
                SolutionToString(solution);
        }
    }

    [Button]
    public void ExecuteAllIterative(int n = 8, int maxSolutionCount = int.MaxValue, bool printSolution = true)
    {
        var columns = new int[n];
        for (int i = 0; i < n; i++)
            columns[i] = i + 1;

        var rows = new Dictionary<int, List<int>>();

        foreach (int column in columns)
            rows[column] = new List<int>(columns);

        var csp = new CSP<int, int>(columns, rows);
        csp.AddConstraint(new QueenConstraint(columns));
        List<Dictionary<int, int>> solutions = csp.BacktrackingSearchAllIterative(maxSolutionCount);

        if (!printSolution) return;

        if (solutions.Count == 0)
            UnityEngine.Debug.Log("No Solution found");
        else
        {
            foreach (var solution in solutions)
                SolutionToString(solution);
        }
    }

    [Button]
    public void ExecuteAllIterativeRandomSolution(int n = 8, int maxSolutionCount = int.MaxValue, bool printSolution = true)
    {
        var columns = new int[n];
        for (int i = 0; i < n; i++)
            columns[i] = i + 1;

        var rows = new Dictionary<int, List<int>>();

        foreach (int column in columns)
            rows[column] = new List<int>(columns);

        var csp = new CSP<int, int>(columns, rows);
        csp.AddConstraint(new QueenConstraint(columns));
        List<Dictionary<int, int>> solutions = csp.RandomSolutions(maxSolutionCount);

        if (!printSolution) return;

        if (solutions.Count == 0)
            UnityEngine.Debug.Log("No Solution found");
        else
        {
            foreach (var solution in solutions)
                SolutionToString(solution);
        }
    }

    private void SolutionToString(Dictionary<int, int> solution)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var item in solution)
        {
            for (int i = 1; i < solution.Keys.Count+1; i++)
            {
                if(item.Value==i)
                    stringBuilder.Append("Q");
                else
                    stringBuilder.Append("#");
            }
            stringBuilder.Append("\n");
        }
        UnityEngine.Debug.Log(stringBuilder);
    }
}
