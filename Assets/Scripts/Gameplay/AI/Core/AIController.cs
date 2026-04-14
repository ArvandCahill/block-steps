using UnityEngine;
using Unity.Behavior;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AIController : MonoBehaviour
{
    public List<BehaviorGraphAgent> agents = new();

    private float detectionTime => GameplayManager.instance.levelData.isNightMode ? 0f : 1f;


    private void Start()
    {
        agents = FindObjectsByType<BehaviorGraphAgent>(FindObjectsSortMode.None).ToList();

        if (agents.Count == 0 ) return;

        foreach (BehaviorGraphAgent agent in agents)
        {
            AssignBBVariables(agent);
        }

        AssignSharedBBVariables();
    }

    void AssignSharedBBVariables()
    {
        agents[0].SetVariableValue("patrolArea", GetPatrolableBlock());
        agents[0].SetVariableValue("detectionTime", detectionTime);
    }

    void AssignBBVariables(BehaviorGraphAgent agent)
    {
        agent.SetVariableValue("agent", agent.gameObject.GetComponent<AnimalUnit>());
        agent.SetVariableValue("aiDetector", agent.gameObject.GetComponent<AIDetector>());
        agent.SetVariableValue("aiState", AIState.Idle);

        if (GameplayManager.instance != null && GameplayManager.instance.levelData.isNightMode)
        {
            foreach(AnimalUnit unit in agent.gameObject.GetComponents<AnimalUnit>())
            {
                unit.movementSpeed = 1f;
                unit.animator.SetFloat("speed", 1f);
            }
        }
    }
    
    private List<Vector3Int> GetPatrolableBlock()
    {
        return PathFinding.instance.blocks.
            Where(pair => pair.Value.type == BlockType.Walkable).
            Select(pair => pair.Key).ToList();
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
