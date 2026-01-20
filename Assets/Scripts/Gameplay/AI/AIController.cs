using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AIController : MonoBehaviour
{
    [SerializeField] private List<BehaviorGraphAgent> agents;

    IEnumerator Start()
    {
        agents = new List<BehaviorGraphAgent>(FindObjectsByType<BehaviorGraphAgent>(FindObjectsSortMode.None));
        yield return new WaitForSeconds(0.2f);
        AssignSharedBBVariables();

        foreach (BehaviorGraphAgent agent in agents)
        {
            AssignBBVariables(agent);
        }
    }

    void AssignSharedBBVariables()
    {
        agents[0].SetVariableValue("player", GameplayManager.instance.playerUnit);
        agents[0].SetVariableValue("Pathfinding", PathFinding.instance);
        agents[0].SetVariableValue("Gameplay Manager", GameplayManager.instance);
        agents[0].SetVariableValue("patrolArea", PathFinding.instance.blocks.Where(pair => pair.Value.type == BlockType.Walkable)
                                                                             .Select(pair => pair.Key).ToList());
    }

    void AssignBBVariables(BehaviorGraphAgent agent)
    {
        agent.SetVariableValue("agent", agent.gameObject.GetComponent<AnimalUnit>());
        agent.SetVariableValue("aiState", AIState.Idle);
    }
}
