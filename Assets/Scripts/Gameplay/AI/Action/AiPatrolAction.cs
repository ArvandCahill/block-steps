using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AiPatrol", story: "[Agent] Patrolling to [TargetBlock]", category: "Action/Game", id: "81969c8b4e0742e9a61486c8063ae62f")]
public partial class AiPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<AnimalUnit> Agent;
    [SerializeReference] public BlackboardVariable<Vector3Int> TargetBlock;

    protected override Status OnStart()
    {
        PathFinding.instance.Move(Agent.Value, TargetBlock.Value);
        Debug.Log($"AiPatrol: Moving to {TargetBlock.Value}");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if  (Agent.Value.isMoving)
        {
            return Status.Running;
        }

        return Status.Success;
    }
}

