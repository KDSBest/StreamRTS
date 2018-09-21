using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class SchedulerFrameInformation
    {
        /// <summary>
        /// Gets or sets the frame number.
        /// </summary>
        /// <value>
        /// The frame number.
        /// </value>
        public int FrameNumber { get; private set; }

        /// <summary>
        /// Gets the player count.
        /// </summary>
        /// <value>
        /// The player count.
        /// </value>
        public int PlayerCount { get; private set; }

        /// <summary>
        /// Gets or sets the player commands.
        ///
        /// Key: PlayerId
        /// </summary>
        /// <value>
        /// The player commands.
        /// </value>
        private Dictionary<int, PlayerCommand> playerCommands;

        public SchedulerFrameInformation(int frameNumber, int playerCount)
        {
            FrameNumber = frameNumber;
            PlayerCount = playerCount;
            playerCommands = new Dictionary<int, PlayerCommand>();
        }

        public void AddPlayerCommand(int playerId, PlayerCommand playerCommand)
        {
            if (playerCommands.ContainsKey(playerId))
            {
                Debug.LogWarning("Player " + playerId + " has given multiple PlayerCommands for Frame " + this.FrameNumber);
                return;
            }

            playerCommands.Add(playerId, playerCommand);
        }

        public Dictionary<int, PlayerCommand> GetPlayerCommands()
        {
            // TODO: cloning shouldn't be necessary
            return playerCommands.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is frame ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is frame ready; otherwise, <c>false</c>.
        /// </value>
        public bool IsFrameReady
        {
            get { return playerCommands.Count >= PlayerCount; }
        }
    }
}