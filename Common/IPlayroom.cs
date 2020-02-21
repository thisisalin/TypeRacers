﻿using System;
using System.Collections.Generic;

namespace Common
{
    public interface IPlayroom<Player>
    {
        string CompetitionText { get; set; }
        List<Player> Players { get; set; }
        Player GetPlayer(string name);

        DateTime GameStartingTime { get; set; }
        DateTime GameEndingTime { get; set; }
        DateTime TimeToWaitForOpponents { get; set; }
        int Place { get; set; }

        void SetGameInfo(string v);
        void SetOpponentsAndTimers(string v);
    }
}