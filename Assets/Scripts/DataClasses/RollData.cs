using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RollData
{
    public float RollSpeed, RollReturnSpeed, MaxRollLeft, MaxRollRight;

    public RollData(float rollSpeed, float rollReturnSpeed, float maxRollLeft, float maxRollRight)
    {
        RollSpeed = rollSpeed;
        RollReturnSpeed = rollReturnSpeed;
        MaxRollLeft = maxRollLeft;
        MaxRollRight = maxRollRight;
    }
}
