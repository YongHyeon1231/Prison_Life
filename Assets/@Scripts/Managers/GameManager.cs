using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 JoystickDir { get; set; } = Vector2.zero;
}
