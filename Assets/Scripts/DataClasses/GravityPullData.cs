using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GravityPullData
{
    public float MinGravityPull, MaxGravityPull, MinSpeedRequiredToRemoveGravityPull;

    public GravityPullData(float minGravityPull, float maxGravityPull, float minSpeedRequiredToRemoveGravityPull)
    {
        MinGravityPull = minGravityPull;
        MaxGravityPull = maxGravityPull;
        MinSpeedRequiredToRemoveGravityPull = minSpeedRequiredToRemoveGravityPull;
    }
}
