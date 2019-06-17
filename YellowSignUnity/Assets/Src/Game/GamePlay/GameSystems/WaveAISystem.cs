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
        MIDDLE_OF_WAVE,
        END_WAVE,
        END_WAVE_TO_INTERMISSION,
        INTERMISSION
    }

    private State _waveState;
    private WaveSequence _waveSequence;
    private GameTimer _prepareTimer;
    private GameTimer _betweenWaveTimer;
    private GameTimerManager _timerManager;
    private WaveSpawnerSystem _waveSpawner;
    private GameState _gameState;

    private Commander _commander;
    private bool _waveFlaggedAsComplete;
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

        // Steal the player's commander lol.
        _commander = GameObject.FindObjectOfType<Commander>();
        

        //_timer = _timerManager.Create(1, -1);
        //_timer.onTimerUpdate += (x) => Debug.Log("OnUpdate: " + x);
        //_timer.onTimerComplete += () => Debug.Log("OnComplete");
        //_timer.Start();
    }

    public void Start(WaveSequence sequence)
    {
        _waveSequence = sequence;
        _waveState = State.INIT;
        
        _betweenWaveTimer = _timerManager.Create(1, _waveSequence.intervalTime);
        
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        handleState(fixedDeltaTime);
    }

    public void CleanUp()
    {
        _timerManager.Remove(_prepareTimer);
    }

    private State state
    {
        set
        {
            if (value != _waveState)
            {
                Debug.Log("WaveAISystem: State: " + _waveState + " -> " + value);
                _waveState = value;
            }
        }
    }
    private bool handleState(FP dt)
    {
        switch(_waveState)
        {
            case State.INIT:                        return introState(dt);
            case State.INIT_TO_BEGIN_WAVE:          return introToBeginToWaveState(dt);
            case State.BEGIN_WAVE:                  return beginWaveState(dt);
            case State.MIDDLE_OF_WAVE:              return middleOfWave(dt);
            case State.END_WAVE:                    return endWaveState(dt);
            case State.END_WAVE_TO_INTERMISSION:    return endWaveToIntermissionState(dt);
            case State.INTERMISSION:                return intermissionState(dt);
        }
        return false;
    }

    private bool introState(FP dt)
    {
        _prepareTimer = _timerManager.Create(1, _waveSequence.prepareTime);
        _prepareTimer.onTimerComplete += onPreparationComplete;
        _prepareTimer.Start();
        
        state = State.INIT_TO_BEGIN_WAVE;
        return true;
    }

    private bool introToBeginToWaveState(FP dt)
    {
        
        return true;
    }

    private bool beginWaveState(FP dt)
    {
        if (_sequenceIndex >= _waveSequence.waveCount)
        {
            state = State.NONE;
            Debug.LogError("Sequence index > wave count");
        }
        else
        {
            _waveSpawner.AddListener(GameplayEventType.WAVE_COMPLETE, onWaveSpawnerComplete);
            WaveInfo waveInfo = _waveSequence.GetWaveInfo(_sequenceIndex);
            _commander.AddCommand(new SpawnCreepCommand(waveInfo.creepId, waveInfo.count));

            _waveFlaggedAsComplete = false;
            state = State.MIDDLE_OF_WAVE;
        }
        return true;
    }

    private bool middleOfWave(FP dt)
    {
        if (_waveFlaggedAsComplete && _gameState.creepList.Count == 0)
        {
            _waveFlaggedAsComplete = false;
            state = State.END_WAVE;
        }
        
        return true;
    }


    private bool endWaveState(FP dt)
    {
        state = State.END_WAVE_TO_INTERMISSION;
        return true;
    }

    private bool endWaveToIntermissionState(FP dt)
    {
        _sequenceIndex++;
        
        if (_sequenceIndex >= _waveSequence.waveCount)
        {
            state = State.NONE;
            Debug.Log("Completed level");
        }
        else
        {
            _betweenWaveTimer.onTimerComplete += onIntervalTimeComplete;
            _betweenWaveTimer.Start();
            
            state = State.INTERMISSION;
        }
        
        return true;
    }

    private bool intermissionState(FP dt)
    {
       
        return true;
    }


    private void onPreparationComplete()
    {
        _prepareTimer.onTimerComplete -= onPreparationComplete;
        _timerManager.Remove(_prepareTimer);
        _prepareTimer = null;
        
        state = State.BEGIN_WAVE;
    }

    private void onWaveSpawnerComplete(GeneralEvent e)
    {
        _waveSpawner.RemoveListener(GameplayEventType.WAVE_COMPLETE, onWaveSpawnerComplete);
        _waveFlaggedAsComplete = true;
        
       
    }

    private void onIntervalTimeComplete()
    {
        _betweenWaveTimer.onTimerComplete -= onIntervalTimeComplete;
        _betweenWaveTimer.Reset();
        _betweenWaveTimer.isPaused = true;

        state = State.BEGIN_WAVE;
    }
}
