using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AIInit", story: "AI Initialization", category: "Action/Game", id: "a3e3db369479e415fe63f754872da93d")]
public partial class AiInitAction : Action
{
    protected override Status OnStart()
    {


        return Status.Success;
    }
}

