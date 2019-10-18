using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


[CreateAssetMenu(menuName ="YellowSign/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameObject gameplayCamera;

    public CreepHealthUIView creepHealthUIPrefab;
    public CreepUIItemView creepUIItemPrefab;
    public TowerHighlighter highlighterPrefab;
    public GameObject towerRangePrefab;
    public WaveSequence waveSequence;

    public SpriteAtlas iconAtlas;


    public Sprite GetIcon(string name)
    {
        Sprite result = null;
        
        if (iconAtlas != null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                result = iconAtlas.GetSprite(name);
                if (result == null)
                {
                    Debug.LogErrorFormat("Could not find sprite with name: {0}", name);
                }
            }
        }
        else
        {
            Debug.LogError("iconAtlas is not set on gameplay resources object!");
        }

        return result;
    }
}
