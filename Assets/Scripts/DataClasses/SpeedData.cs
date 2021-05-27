using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpeedData
{
    public float Speed;
    public float MinSpeed;
    public float MaxSpeed;
    public float AccelerationSpeed;

    public SpeedData(float speed, float minSpeed, float maxSpeed, float accelerationSpeed)
    {
        Speed = speed;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
        AccelerationSpeed = accelerationSpeed;
    }
}
