using GTA;
using LemonUI.TimerBars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberDriverMod
{
    public class VariableStopwatch
    {
        decimal stopwatchCounter;
        public  bool isRunning;

        public VariableStopwatch(int interval = 0)
        {
            stopwatchCounter = interval;
        }

        public void Update(float timescale)
        {
            if (isRunning == false) return;

            float stopwatchElapse = Game.LastFrameTime * 1000;

            stopwatchCounter += (decimal)(stopwatchElapse * timescale);
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void Start()
        {
            isRunning = true;
        }

        public void Reset()
        {
            stopwatchCounter = 0;
        }

        public int Counter
        {
            get { return (int)stopwatchCounter; }
        }
    }
}
