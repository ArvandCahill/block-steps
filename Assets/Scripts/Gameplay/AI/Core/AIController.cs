using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AIController : MonoBehaviour
{
    public List<BehaviorGraphAgent> agents;
    public List<Vector3Int> blocks = new();

    IEnumerator Start()
    {
        agents = new List<BehaviorGraphAgent>(FindObjectsByType<BehaviorGraphAgent>(FindObjectsSortMode.None));

        foreach (BehaviorGraphAgent agent in agents)
        {
            AssignBBVariables(agent);
        }

        yield return new WaitForSeconds(0.5f);
        blocks = PathFinding.instance.blocks.Where(pair => pair.Value.type == BlockType.Walkable)
                                                                             .Select(pair => pair.Key).ToList();
        AssignSharedBBVariables();
    }

    void AssignSharedBBVariables()
    {
        agents[0].SetVariableValue("patrolArea", blocks);
    }

    void AssignBBVariables(BehaviorGraphAgent agent)
    {
        agent.SetVariableValue("agent", agent.gameObject.GetComponent<AnimalUnit>());
        agent.SetVariableValue("aiDetector", agent.gameObject.GetComponent<AIDetector>());
        agent.SetVariableValue("aiState", AIState.Idle);
    }

    public void DisableAI(AnimalUnit unit)
    {
        unit.GetComponent<BehaviorGraphAgent>().enabled = false;
    }

    public void DisableAllAI()
    {
        foreach (BehaviorGraphAgent agent in agents)
        {
            agent.enabled = false;
        }
    }
}
