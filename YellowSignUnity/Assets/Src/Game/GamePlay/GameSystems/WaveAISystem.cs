using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;

public class 
    WaveAISystem : EventDispatcher
{
    public enum State
    {
        NONE,

        INIT,
        INIT_TO_BEGIN_WAVE,
        BEGIN_WAVE,
        BEGIN_WAVE_TO_END_WAVE,
        END_WAVE,
        END_WAVE_TO_INTERMISSION,
        INTERMISSION
    }

    private State _waveState;
    private WaveSequence _waveSequence;
    private GameTimer _prepareTimer;
    private GameTimerManager _timerManager;
    private WaveSpawnerSystem _waveSpawner;
    private GameState _gameState;

    private int _sequenceIndex;

    public WaveAISystem(
        GameState gameState, 
        GameTimerManager timerManager, 
        WaveSpawnerSystem waveSpawner)
    {
        _gameState = gameState;
        _timerManager = timerManager;
        _waveSpawner = waveSpawner;
        _waveState = State.NONE;

        _sequenceIndex = 0;



        //_timer = _timerManager.Create(1, -1);
        //_timer.onTimerUpdate += (x) => Debug.Log("OnUpdate: " + x);
        //_timer.onTimerComplete += () => Debug.Log("OnComplete");
        //_timer.Start();
    }

    public void Start(WaveSequence sequence)
    {
        _waveSequence = sequence;
        _waveState = State.INIT;
        
        _prepareTimer = _timerManager.Create(1, _waveSequence.prepareTime);
        _prepareTimer.onTimerComplete += onPreparationComplete;
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        handleState(fixedDeltaTime);
    }

    public void CleanUp()
    {
        _timerManager.Remove(_prepareTimer);
    }

    private bool handleState(FP dt)
    {
        switch(_waveState)
        {
            case State.INIT:                        return introState(dt);
            case State.INIT_TO_BEGIN_WAVE:          return introToBegintoWaveState(dt);
            case State.BEGIN_WAVE:                  return beginWaveState(dt);
            case State.BEGIN_WAVE_TO_END_WAVE:      return beginWaveToEndWaveState(dt);
            case State.END_WAVE:                    return endWaveState(dt);
            case State.END_WAVE_TO_INTERMISSION:    return endWaveToIntermissionState(dt);
            case State.INTERMISSION:                return intermissionState(dt);
        }
        return false;
    }

    private bool introState(FP dt)
    {
        //_waveSequence.
        return true;
    }

    private bool introToBegintoWaveState(FP dt)
    {
        _waveSpawner.AddListener(GameplayEventType.WAVE_COMPLETE, onWaveSpawnerComplete);
        WaveInfo waveInfo = _waveSequence.GetWaveInfo(_sequenceIndex);
        //_waveSpawner.AddWave()to
        return true;
    }

    private bool beginWaveState(FP dt)
    {
        return true;
    }

    private bool beginWaveToEndWaveState(FP dt)
    {
        return true;
    }

    private bool endWaveState(FP dt)
    {
        return true;
    }

    private bool endWaveToIntermissionState(FP dt)
    {
        return true;
    }

    private bool intermissionState(FP dt)
    {
        return true;
    }


    private void onPreparationComplete()
    {
        _waveState = State.BEGIN_WAVE;
    }

    private void onWaveSpawnerComplete(GeneralEvent e)
    {

    }
}
