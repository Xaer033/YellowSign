using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct WaveInfo
{
    
    public string creepId;
    public int count;
}

[CreateAssetMenu(menuName = "YellowSign/Wave Sequence")]
public class WaveSequence : ScriptableObject
{
    public int prepareTime;
    public int intervalTime;
    public List<WaveInfo> sequenceList;
    
    public WaveInfo GetWaveInfo(int index)
    {
        Assert.IsTrue(index >= 0 && index < sequenceList.Count);
        return sequenceList[index];
    }

    public int waveCount
    {
        get { return sequenceList.Count; }
    }
}
