using System;
using System.Collections.Generic;

namespace Common
{
    public interface IPlayroom
    {
        string CompetitionText { get; set; }
        DateTime GameStartingTime { get; set; }
        DateTime GameEndingTime { get; set; }
        DateTime TimeToWaitForOpponents { get; set; }
        int Place { get; set; }
        Player GetPlayer(string name);
        List<Player> Players { get; set; }
        bool Leave(string name);
        bool Join(Player currentPlayer, IRecievedInformationManager informationManager);
        void TrySetStartingTime();
    }
}