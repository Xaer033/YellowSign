using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="YellowSign/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameObject gameplayCamera;

    public CreepHealthUIView creepHealthUIPrefab;
    public CreepUIItemView creepUIItemPrefab;
    public TowerHighlighter highlighterPrefab;
    public GameObject towerRangePrefab;
    public WaveSequence waveSequence;
}
