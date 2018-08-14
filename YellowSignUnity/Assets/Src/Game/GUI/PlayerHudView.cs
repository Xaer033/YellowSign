using GhostGen;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class PlayerHudView : UIView
{
    public TMP_Text _killCount;

    public Button _towerButton;
    public Image _towerPortrait;

    public Button _creepButton;
    public Image _creepPortrait;

    private string _killText;
    private int creepsKilled;
    private int livesLost;
    private CreepDef _currentCreepDef;

    [Inject]
    private CreepSystem _creepSystem;

    public void Start()
    {
        _creepSystem.AddListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepButton.onClick.AddListener(onCreepButton);
        _towerButton.onClick.AddListener(onTowerButton);

        creepsKilled = 0;
        livesLost = 0;

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


    public CreepDef currentCreepDef
    {
        set
        {
            if(_currentCreepDef != value)
            {
                _currentCreepDef = value;
                invalidateFlag |= InvalidationFlag.DYNAMIC_DATA;
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

            if(_currentCreepDef != null && _creepPortrait != null)
            {
                _creepPortrait.overrideSprite = _currentCreepDef.icon;
                _creepPortrait.color = UnityEngine.Color.white;
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

    private void onCreepButton()
    {
        DispatchEvent(PlayerUIEventType.TOGGLE_CREEP_VIEW);
    }

    private void onTowerButton()
    {
        DispatchEvent(PlayerUIEventType.TOGGLE_TOWER_VIEW);
    }
}
