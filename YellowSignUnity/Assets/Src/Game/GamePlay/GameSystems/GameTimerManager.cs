using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;

public class GameTimerManager : EventDispatcher
{
    private SyncStepper _stepper;
    private List<GameTimer> _timerList = new List<GameTimer>();

    public GameTimerManager(SyncStepper.Factory stepperFactory)
    {
        _stepper = stepperFactory.Create();
        _stepper.onSyncedStep += onSyncStep;
    }

    public GameTimer Create(FP duration, FP interval)
    {
        GameTimer timer = new GameTimer(duration, interval);
        _timerList.Add(timer);
        return timer;
    }

    public bool Remove(GameTimer timer)
    {
        if(timer == null)
        {
            return false;
        }

        timer.CleanUp();
        return _timerList.Remove(timer);
    }

    private void onSyncStep(FP fixedDeltaTime)
    {
        int count = _timerList.Count;
        for(int i = count - 1; i >= 0; --i)
        {
            _timerList[i].FixedStep(fixedDeltaTime);
        }
    }
}
