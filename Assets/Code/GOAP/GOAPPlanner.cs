using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPActionPlan
    {
        public GOAPGoal AgentGoal { get; }
        public Stack<GOAPAction> Actions { get; }
        public float TotalCost { get; set; }

        public GOAPActionPlan(GOAPGoal goal, Stack<GOAPAction> actions, float totalCost)
        {
            AgentGoal = goal;
            Actions = actions;
            TotalCost = totalCost;
        }
    }

    public interface IGOAPPlanner
    {
        GOAPActionPlan Plan(GOAPAgent agent, HashSet<GOAPGoal> goals, GOAPGoal previousGoal = null);
    }

    public class GOAPPlanner : IGOAPPlanner
    {
        public class GOAPPlannerNode
        {
            public GOAPPlannerNode Parent { get; }
            public GOAPAction Action { get; }
            public HashSet<GOAPBelief> RequiredEffects { get; }
            public List<GOAPPlannerNode> Leaves { get; }
            public float Cost { get; }

            public bool IsLeafDead => Leaves.Count == 0 && Action == null;

            public GOAPPlannerNode(GOAPPlannerNode parent, GOAPAction action, HashSet<GOAPBelief> effects, float cost)
            {
                Parent = parent;
                Action = action;
                RequiredEffects = new HashSet<GOAPBelief>(effects);
                Leaves = new List<GOAPPlannerNode>();
                Cost = cost;
            }
        }

        public GOAPActionPlan Plan(GOAPAgent agent, HashSet<GOAPGoal> goals, GOAPGoal previousGoal = null)
        {
            // order goals by priority, descending
            List<GOAPGoal> orderedGoals = goals
                .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
                .OrderByDescending(g => g == previousGoal ? float.MaxValue : g.PriorityValue)
                .ToList();

            // Try to solve each goal in order
            foreach (GOAPGoal goal in orderedGoals)
            {
                GOAPPlannerNode goalNode = new GOAPPlannerNode(null, null, goal.DesiredEffects, 0);

                // If we can find a path to the goal, return the plan
                if (FindPath(goalNode, agent.Actions))
                {
                    // If the goalNode has no leaves and no action to perform try a different goal
                    if (goalNode.IsLeafDead) continue;

                    Stack<GOAPAction> actionStack = new Stack<GOAPAction>();
                    while (goalNode.Leaves.Count > 0)
                    {
                        var cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
                        goalNode = cheapestLeaf;
                        actionStack.Push(cheapestLeaf.Action);
                    }

                    return new GOAPActionPlan(goal, actionStack, goalNode.Cost);
                }
            }

            Debug.LogWarning("No plan found");
            return null;
        }

        bool FindPath(GOAPPlannerNode parent, HashSet<GOAPAction> actions)
        {
            // order by cost, ascending
            List<GOAPAction> orderedActions = actions
                .OrderBy(a => a.CostValue)
                .ToList();

            foreach (GOAPAction action in orderedActions)
            {
                HashSet<GOAPBelief> requiredEffects = parent.RequiredEffects;

                // remove effects that are already satisfied
                requiredEffects.RemoveWhere(b => b.Evaluate());

                if (requiredEffects.Count == 0)
                {
                    // all effects are satisfied
                    return true;
                }
                if (action.Effects.Any(requiredEffects.Contains))
                {
                    var newRequiredEffects = new HashSet<GOAPBelief>(requiredEffects);
                    newRequiredEffects.ExceptWith(action.Effects);
                    newRequiredEffects.UnionWith(action.Preconditions);

                    var newAvailableActions = new HashSet<GOAPAction>(actions);
                    newAvailableActions.Remove(action);

                    var newNode = new GOAPPlannerNode(parent, action, newRequiredEffects, parent.Cost + action.CostValue);

                    // Explore the new node recursively
                    if (FindPath(newNode, newAvailableActions))
                    {
                        parent.Leaves.Add(newNode);
                        newRequiredEffects.ExceptWith(newNode.Action.Preconditions);
                    }

                    // If all effects at this depth have been satisfied, return true
                    if (newRequiredEffects.Count == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}