using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCare.DataHandling
{
    class Exercise
    {
        private DateTime startOfTest;
        private DateTime currentTime;

        private int timeRunningInSeconds;

        private Dictionary<int, int> powerLevelAtTime;

        public Exercise()
        {
            startOfTest = DateTime.Now;
        }

        public void init()
        {
            powerLevelAtTime = new Dictionary<int, int>();

            powerLevelAtTime.Add(0, 25); //At time 0 the power level should be set to 25
            powerLevelAtTime.Add(20, 50); //at time 20 the power level should be set to 50
            powerLevelAtTime.Add(60, 100);
            powerLevelAtTime.Add(120, 200);
            powerLevelAtTime.Add(180, 100);
            powerLevelAtTime.Add(240, 250);
            powerLevelAtTime.Add(300, 100);
            powerLevelAtTime.Add(360, 50);
            powerLevelAtTime.Add(420, 25);
            powerLevelAtTime.Add(480, 0);
        }

        public int getPowerLevel()
        {
            update();

            foreach (KeyValuePair<int,int> pair in powerLevelAtTime)
            {
                if(pair.Key < timeRunningInSeconds)
                {
                    return pair.Value;
                }
            }

            return -1;
        }

        public void update()
        {
            currentTime = DateTime.Now;
            timeRunningInSeconds =(int)( (currentTime.Ticks - startOfTest.Ticks) / TimeSpan.TicksPerSecond );
        }
    }
}
