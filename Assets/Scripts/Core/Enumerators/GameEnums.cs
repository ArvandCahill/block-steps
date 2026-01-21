using Unity.Behavior;

public enum GameState
{
    MainMenu,
    Playing,
    Building,
    Paused,
    GameOver,
    Victory
}

public enum InputState
{
    idle,
    pressed,
    dragging
}

public enum BlockType
{
    Start,
    Finish,
    Walkable,
}

public enum MovementSpeed
{
    Normal,
    Fast
}

[BlackboardEnum]
public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Attack
}

public enum ButtonType
{
    LoadScene,
    UIAction
}