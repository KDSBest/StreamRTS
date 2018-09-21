using System;
using System.Collections.Generic;
using System.Text;

namespace Gameplay
{
    /// <summary>
    /// Takes all input from all Players and Simulate Frames
    /// </summary>
    public class Scheduler
    {
        public int PlayerCount { get; private set; }

        public Dictionary<int, SchedulerFrameInformation> Frames { get; set; }

        public Scheduler(int playerCount)
        {
            PlayerCount = playerCount;
            Frames = new Dictionary<int, SchedulerFrameInformation>();
        }

        private void EnsureFrameExists(int frameNumber)
        {
            if (!Frames.ContainsKey(frameNumber))
            {
                Frames.Add(frameNumber, new SchedulerFrameInformation(frameNumber, PlayerCount));
            }
        }

        public SchedulerFrameInformation GetFrame(int frameNumber)
        {
            EnsureFrameExists(frameNumber);

            return Frames[frameNumber];
        }

        public void Schedule(int playerId, PlayerCommand playerCommand, int frameNumber)
        {
            GetFrame(frameNumber).AddPlayerCommand(playerId, playerCommand);
        }
    }
}
