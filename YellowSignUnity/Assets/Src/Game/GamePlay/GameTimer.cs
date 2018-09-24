using System;
using TrueSync;

public class GameTimer
{
    private const int kBigNumber = 9999999;

    private FP _duration;
    private FP _updateInterval;

    private FP _timer;
    private FP _lastInterval;

    private event Action _onTimerComplete;
    private event Action<FP> _onTimerUpdate;


    public event Action onTimerComplete
    {
        add { _onTimerComplete += value; }
        remove { _onTimerComplete -= value; }
    }

    public event Action<FP> onTimerUpdate
    {
        add { _onTimerUpdate += value; }
        remove { _onTimerUpdate -= value; }
    }

    public bool isPaused { get; set; }

    public GameTimer(FP updateInterval, FP duration)
    {
        _duration = duration <= 0 ? kBigNumber : duration;
        _updateInterval = updateInterval;

        Reset();

        isPaused = true;
    }

    public void Start()
    {
        isPaused = false;
    }

    public void Reset()
    {
        _timer = _duration;
        _lastInterval = _duration;
    }

    public void CleanUp()
    {
        _onTimerComplete = null;
        _onTimerUpdate = null;
    }

    public void FixedStep(FP dt)
    {
        if(!isPaused && _timer > 0)
        {
            _timer -= dt;

            if(_lastInterval - _timer > _updateInterval)
            {
                callTimerUpdate();
                _lastInterval = _timer;
            }

            if(_timer <= 0)
            {
                if(_duration <= 0)
                {
                    _timer = int.MaxValue;
                }
                else
                {
                    callTimerComplete();
                }
            }
        }
    }

    private void callTimerUpdate()
    {
        if(_onTimerUpdate != null)
        {
            _onTimerUpdate(_timer);
        }
    }

    private void callTimerComplete()
    {
        if(_onTimerComplete != null)
        {
            _onTimerComplete();
        }
    }
}
