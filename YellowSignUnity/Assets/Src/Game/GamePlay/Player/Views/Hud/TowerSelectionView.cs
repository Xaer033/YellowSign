using System.Collections;
using System.Collections.Generic;
using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TowerSelectionView : UIView
{
    public Image _mainIcon;
    public TMP_Text _healthText;
    public TMP_Text _titleText;
    public TMP_Text _description;

    private SelectionViewData _selectionData;
    
    private GameplayResources _gameplayResources;
    
    public void Awake()
    {
        _gameplayResources = Singleton.instance.gameConfig.gameplayResources;
    }

    public void SetSelectionData(SelectionViewData data)
    {
        if (_selectionData != data)
        {
            _selectionData = data;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    protected override void OnViewUpdate()
    {
        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if (_mainIcon)
            {
                _mainIcon.sprite = _selectionData != null ? _gameplayResources.GetIcon(_selectionData.mainIconName) : null;
            }

            if (_healthText)
            {
                _healthText.text = _selectionData != null ? _selectionData.healthText : "";
            }
            
            if (_titleText)
            {
                _titleText.text = _selectionData != null ? _selectionData.titleText : "";
            }
            
            if (_description)
            {
                _description.text = _selectionData != null ? _selectionData.descriptionText : "";
            }
        }
    }
}
