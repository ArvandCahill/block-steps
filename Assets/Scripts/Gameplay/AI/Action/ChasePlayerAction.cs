using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "[Agents] Chase [Player]", category: "Action", id: "2b06d5cf239cb9a3179f7ea413245d6d")]
public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<AnimalUnit> Agents;
    [SerializeReference] public BlackboardVariable<AnimalUnit> Player;
    protected override Status OnStart()
    {
        if (Agents == null) return Status.Failure; 
        if (Vector3.Distance(Agents.Value.transform.position, Player.Value.transform.position) < 0.5f) return Status.Success; 

        Vector3Int playerPosition = Vector3Int.RoundToInt(Player.Value.transform.position +Vector3Int.down);
        PathFinding.instance.Move(Agents.Value, playerPosition);

        return Status.Success;
    }
}

