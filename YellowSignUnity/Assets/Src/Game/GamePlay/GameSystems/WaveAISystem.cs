using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;

public class WaveAISystem : EventDispatcher
{
    public enum State
    {
        NONE,

        INIT,
        BEGIN_WAVE,
        END_WAVE,
        WAVE_INTERMISSION
    }

    private State _waveState;
    private WaveSequence _waveSequence;
    private GameTimer _timer;
    private GameTimerManager _timerManager;

    public WaveAISystem(GameTimerManager timerManager)
    {
        _waveState = State.NONE;
        _timerManager = timerManager;

        //_timer = _timerManager.Create(5, 1.0);
        //_timer.onTimerUpdate += (x) => Debug.Log("OnUpdate: " + x);
        //_timer.onTimerComplete += () => Debug.Log("OnComplete");
        //_timer.Start();
    }

    public void Start(WaveSequence sequence)
    {
        _waveSequence = sequence;
        _waveState = State.INIT;
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        handleState(fixedDeltaTime);
    }

    public void CleanUp()
    {
        _timerManager.Remove(_timer);
        _timer = null;
    }

    private bool handleState(FP dt)
    {
        switch(_waveState)
        {
            case State.INIT:                return introState(dt);
            case State.BEGIN_WAVE:          return beginWaveState(dt);
            case State.END_WAVE:            return endWaveState(dt);
            case State.WAVE_INTERMISSION:   return waveIntermissionState(dt);
        }
        return false;
    }

    private bool introState(FP dt)
    {
        return true;
    }

    private bool beginWaveState(FP dt)
    {
        return true;
    }

    private bool endWaveState(FP dt)
    {
        return true;
    }

    private bool waveIntermissionState(FP dt)
    {
        return true;
    }
}
