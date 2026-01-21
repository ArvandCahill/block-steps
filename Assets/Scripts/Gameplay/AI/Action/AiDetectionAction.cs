using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Detection", story: "Update [Agent] [Detector] and Assign [Player]", category: "Action/Game", id: "7ed23eab34ac4f543c5ad3db01a90fd9")]
public partial class AiDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<AnimalUnit> Agent;
    [SerializeReference] public BlackboardVariable<AIDetector> Detector;
    [SerializeReference] public BlackboardVariable<AnimalUnit> Player;

    protected override Status OnStart()
    {
        Player.Value = Detector.Value.UpdateRangeDetector();
        if(Player.Value != null) Agent.Value.stopMovement = true;
        return Status.Success;
    }
}

