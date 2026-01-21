using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AIIdle", story: "[Agent] Idle and Pick a [Coordinates] From [BlockList]", category: "Action/Game", id: "9ddfa0cb191bf66753304cf2a0bbda58")]
public partial class AiIdleAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;
    [SerializeReference] public BlackboardVariable<Vector3Int> Coordinates;
    [SerializeReference] public BlackboardVariable<List<Vector3Int>> BlockList;
    protected override Status OnStart()
    {
        if (Agent?.Value == null)
        {
            Debug.LogError("AIIdle: Agent is NULL");
            return Status.Failure;
        }

        if (BlockList?.Value == null || BlockList.Value.Count == 0)
        {
            Debug.LogError("AIIdle: BlockList is empty or NULL");
            return Status.Failure;
        }

        PathFinding pathFinding = PathFinding.instance;

        int index = UnityEngine.Random.Range(0, BlockList.Value.Count);
        Vector3Int target = BlockList.Value[index];

        if (pathFinding.blocks[target].isWalkable == false)
        {
            Debug.LogError($"AIIdle: Target Block {target} is not walkable");
            return Status.Failure;
        }

        var path = pathFinding.FindPath(
            pathFinding.GetPlayerPosition(Agent.Value.position),
            target
        );

        if (path == null || path.Count == 0)
        {
            return Status.Failure; 
        }

        Coordinates.Value = target;

        return Status.Success;
    }
}