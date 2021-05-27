using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BoostData
{
    public float BoostScale;
    public float MinBoostScale;
    public float MaxBoostScale;
    public float BoostAccelerationSpeed;

    public BoostData(float boostScale, float minBoostScale, float maxBoostScale, float boostAccelerationSpeed)
    {
        BoostScale = boostScale;
        MinBoostScale = minBoostScale;
        MaxBoostScale = maxBoostScale;
        BoostAccelerationSpeed = boostAccelerationSpeed;
    }
}
