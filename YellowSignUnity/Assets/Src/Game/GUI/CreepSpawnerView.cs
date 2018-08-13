﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GhostGen;
using Zenject;
using DG.Tweening;


public class CreepSpawnerView : UIView
{
    public CanvasGroup _canvasGroup;
    public Button _dismissBtn;
    public Slider _amountSlider;
    public TMP_Text _amountTxt;
    public Button _spawnCreepBtn;

    public Transform _creepListParent;
    public ToggleGroup _toggleGroup;


    private string _currentCreepId;
    private List<CreepDef> _creepDataList;
    private GameplayResources _gameplayResources;
    private ViewFactory _viewFactory;

    [Inject]
    private void Construct(
        GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }

    private void Awake()
    {
        _amountSlider.onValueChanged.AddListener( onSpawnAmountChanged);
        _spawnCreepBtn.onClick.AddListener(onCreepSpawn);
        _dismissBtn.onClick.AddListener(onDismiss);

        AddListener(PlayerUIEventType.SELECT_CREEP_TYPE, onCreepSelected);

        _spawnCreepBtn.interactable = _toggleGroup.AnyTogglesOn();
        gameObject.SetActive(false);
        
    }


    public void SetCreepList(List<CreepDef> creepDataList)
    {
        _creepDataList = creepDataList;

        GameObjectUtilities.DestroyAllChildren(_creepListParent);

        for(int i = 0; i < _creepDataList.Count; ++i)
        {
            CreepUIItemView item = GameObject.Instantiate<CreepUIItemView>(_gameplayResources.creepUIItemPrefab, _creepListParent);
            item.Setup(_creepDataList[i], _toggleGroup);
        }
        invalidateFlag |= InvalidationFlag.DYNAMIC_DATA;
    }

    public void Show(Action onComplete)
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0.0f;

        Tween tween = _canvasGroup.DOFade(1.0f, 0.25f);
        tween.OnComplete(() =>
        {
            if(onComplete != null)
            {
                onComplete();
            }
        });
    }

    public void Hide(Action onComplete)
    {

        Tween tween = _canvasGroup.DOFade(0.0f, 0.25f);
        tween.OnComplete(() =>
        {
            gameObject.SetActive(false);
            if(onComplete != null)
            {
                onComplete();
            }
        });
    }

    private void onSpawnAmountChanged(float newValue)
    {
        _amountTxt.text = newValue.ToString();
    }

    private void onCreepSpawn()
    {
        int spawnCount = (int)_amountSlider.value;
        
        ICommand command = new SpawnCreepCommand(_currentCreepId, spawnCount);
        DispatchEvent(PlayerUIEventType.SPAWN_CREEPS, true, command);
    }

    private void onCreepSelected(GeneralEvent e)
    {
        CreepDef def = e.data as CreepDef;
        _currentCreepId = def.id;

        _spawnCreepBtn.interactable = _toggleGroup.AnyTogglesOn();
    }

    private void onDismiss()
    {
        DispatchEvent(PlayerUIEventType.DISMISS_VIEW, true);
    }
}
