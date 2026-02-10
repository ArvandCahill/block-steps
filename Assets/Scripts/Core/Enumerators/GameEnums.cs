using Unity.Behavior;

public enum GameState
{
    MainMenu,
    Playing,
    Building,
    Paused,
    GameOver,
    Win,
    Lose
}

public enum InputState
{
    idle,
    pressed,
    dragging
}

public enum MoveState
{
    AtStart,
    AtTarget
}

public enum MoveDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public enum BlockType
{
    Start,
    Finish,
    Walkable,
    Decoration
}

public enum MovementSpeed
{
    Slow,
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
    UIAction,
    Restart
}

public enum PolaroidViewState
{
    Grid,       
    Zoomed,     
    Animating   
}