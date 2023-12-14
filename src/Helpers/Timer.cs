using System.Threading;

namespace BatteryTracker.Helpers;

class Timer
{
    public Action? Action { get; set; }

    System.Threading.Timer? _timer;

    int _interval; // in milliseconds
    bool _continue = true;

    public async void StartTimer(int interval)
    {
        _continue = true;
        _interval = interval;
        if (_timer != null)
        {
            await _timer.DisposeAsync();
        }
        _timer = new System.Threading.Timer(Tick, null, interval, Timeout.Infinite);
    }

    public void StopTimer() => _continue = false;

    void Tick(object state)
    {
        try
        {
            Action?.Invoke();
        }
        finally
        {
            if (_continue)
            {
                _timer?.Change(_interval, Timeout.Infinite);
            }
        }
    }

    ~Timer()
    {
        _timer?.Dispose();
    }
}

