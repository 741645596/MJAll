// FPSCounter.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/12
using System;

public class FPSCounter
{
    private float updateInterval;
    private int frames;
    private float accumulator;
    private float timeLeft;

    public FPSCounter(float updateInterval)
    {
        if (updateInterval <= 0f)
        {
            return;
        }

        this.updateInterval = updateInterval;
        Reset();
    }

    public float UpdateInterval
    {
        get
        {
            return updateInterval;
        }
        set
        {
            if (value <= 0f)
            {
                return;
            }

            updateInterval = value;
            Reset();
        }
    }

    public float CurrentFps { get; private set; }

    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        frames++;
        accumulator += realElapseSeconds;
        timeLeft -= realElapseSeconds;

        if (timeLeft <= 0f)
        {
            CurrentFps = accumulator > 0f ? frames / accumulator : 0f;
            CurrentFps = (float)Math.Floor(CurrentFps);
            frames = 0;
            accumulator = 0f;
            timeLeft += updateInterval;
        }
    }

    private void Reset()
    {
        CurrentFps = 0f;
        frames = 0;
        accumulator = 0f;
        timeLeft = 0f;
    }
}