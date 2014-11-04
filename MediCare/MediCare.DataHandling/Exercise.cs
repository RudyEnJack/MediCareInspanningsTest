using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCare.DataHandling
{
    public class Exercise
    {
        private DateTime startOfTest;
        private DateTime currentTime;

        private DateTime startOfTimedTest; //This test starts when the hearthrate is over 140 and takes 6 minutes.

        private int timeRunningInSeconds;
        private int timeRunningSinceStartOfTimedTest;

        private int powerLevel;

        private bool warmingUp;
        private bool isMale;

        public Exercise(bool genderIsMale)
        {
            init();
            this.isMale = genderIsMale;
        }

        public void init()
        {
            powerLevel = 50;
            warmingUp = true;
        }

        /// <summary>
        /// Sets the start of the exercise. should be called before the getPowerLevel() method
        /// </summary>
        public void start()
        {
            startOfTest = DateTime.Now;
        }

        /// <summary>
        /// This method returns the powerlevel the bike should be set to at the time the method is called.
        /// the start() method should have been called before asking the powerlevel of the bike.
        /// </summary>
        /// <returns>Powerlevel in int</returns>
        public int getPowerLevel(int heartRate)
        {
            //updates the timers
            update();

            // If this value is changed the bikeSimulator has to be adjusted accordingly!
            int mintesForMeasurement = 1; // this is the ammount of minutes it takes for the timed testing. should be 6 for real measurements

            if (timeRunningSinceStartOfTimedTest == (mintesForMeasurement * 60)) //after 6 minutes of TimedTesting.
            {
                return -1;
            }

            //checks if the heartrate of 120 has been reached if yes. stop the warming up
            if (heartRate > 120 && warmingUp)
            {
                warmingUp = false;
                this.startOfTimedTest = DateTime.Now;
            }

            //during the warmingup the power keeps increasing. after the warming up it stays
            if (warmingUp)
            {
                if (isMale)
                {
                    powerLevel = (((int)(timeRunningInSeconds / 30)) * 50) + 50;
                }
                else
                {
                    powerLevel = (((int)(timeRunningInSeconds / 30)) * 25) + 50;
                }
            }

            return powerLevel;
        }

        public int getTimeRunning()
        {
            update();

            return timeRunningInSeconds;
        }

        public void setIsMale(bool isMale)
        {
            this.isMale = isMale;
        }

        private void update()
        {
            currentTime = DateTime.Now;
            timeRunningInSeconds =(int)( (currentTime.Ticks - startOfTest.Ticks) / TimeSpan.TicksPerSecond );
            timeRunningSinceStartOfTimedTest = (int)((currentTime.Ticks - startOfTimedTest.Ticks) / TimeSpan.TicksPerSecond);
            Console.WriteLine("Timerunningsincestartoftimedtest is: " + timeRunningSinceStartOfTimedTest);
        }
    }
}
