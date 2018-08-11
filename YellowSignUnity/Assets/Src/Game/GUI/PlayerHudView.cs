using GhostGen;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class PlayerHudView : UIView
{
    public TMP_Text _killCount;
    public Button _towerButton;
    public Button _creepButton;

    private string _killText;
    private int creepsKilled;
    private int livesLost;

    [Inject]
    private CreepSystem _creepSystem;

    public void Start()
    {
        creepsKilled = 0;
        livesLost = 0;
        _creepSystem.AddListener(GameplayEventType.CREEP_KILLED, onCreepKilled);

        killText = "Creeps Killed: 0, Lives lost: 0";
    }

    public string killText
    {
        set
        {
            if(_killText != value)
            {
                _killText = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }
    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if(_killCount != null)
            {
                _killCount.text = _killText;
            }
        }
    }

    private void onCreepKilled(GeneralEvent e)
    {
        Creep c = e.data as Creep;
        if(c.isDead)
        {
            creepsKilled++;
        }
        else if(c.reachedTarget)
        {
            livesLost++;
        }

        killText = string.Format("Creeps Killed: {0}, Lives lost: {1}", creepsKilled.ToString(), livesLost.ToString());
    }
}
