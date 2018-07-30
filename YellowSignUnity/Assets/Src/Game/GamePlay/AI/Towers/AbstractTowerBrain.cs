using TrueSync;
using UnityEngine;

[System.Serializable]
public abstract class AbstractTowerBrain : ScriptableObject, ITowerBrain
{
    //public abstract void Awake();
    public virtual void OnEnable()
    {
        
    }
    public abstract void FixedStep(Tower tower, FP fixedDeltaTime);
}
