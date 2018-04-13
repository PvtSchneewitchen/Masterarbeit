using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementData
{
    public Util.MovementMode MoveMode { get; set; }
    public bool ReducePoints { get; set; }
    public float TransSpeed { get; set; }
    public float TransAcceleration { get; set; }
    public float RotSpeed { get; set; }
    public float RotAcceleration { get; set; }
    public float TeleportDistance { get; set; }
    public float TeleportAngle { get; set; }
    public bool Twinkle { get; set; }

    public MovementData()
    {
        MoveMode = Util.MovementMode.FreeFly;
        ReducePoints = false;
        TransSpeed = 50;
        TransAcceleration = 40;
        RotSpeed = 50;
        RotAcceleration = 40;
        TeleportDistance = 400;
        TeleportAngle = 20;
        Twinkle = false;
    }
}
