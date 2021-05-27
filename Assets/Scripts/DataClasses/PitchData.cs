using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PitchData
{
    public float PitchSpeed;

    public PitchData(float pitchSpeed)
    {
        PitchSpeed = pitchSpeed;
    }
}
