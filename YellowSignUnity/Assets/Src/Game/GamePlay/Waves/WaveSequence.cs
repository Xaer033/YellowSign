﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveInfo
{
    public string creepId;
    public int count;
}

[CreateAssetMenu(menuName = "YellowSign/Wave Sequence")]
public class WaveSequence : ScriptableObject
{
    public List<WaveInfo> sequenceList;
}