using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberDriverMod
{
    public class VariableTimer
    {
        public delegate void TimerExpired(object sender);

        public event TimerExpired OnTimerExpired;

        int TimerMax;
        decimal TimerCounter;
        bool IsRunning;
        public bool AutoReset;

        public VariableTimer(int interval)
        {
            TimerCounter = interval;
            TimerMax = interval;
        }

        public void Update(float timescale) 
        {
            if (!IsRunning) return;

            float timerElapse = Game.LastFrameTime * 1000f;

            TimerCounter -= (decimal)(timerElapse * timescale);

            if(TimerCounter <= 0)
            {
                OnTimerExpired.Invoke(this);

                if(AutoReset)
                {
                    TimerCounter += TimerMax;
                } else
                {
                    Stop();
                }
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Reset()
        {
            TimerCounter = TimerMax;
        }

        public int Counter
        {
            get { return (int)TimerCounter; }
        }
    }
}
