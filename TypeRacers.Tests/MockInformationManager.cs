﻿using Common;
using System;

namespace TypeRacersFacts
{
    public class MockInformationManager : IRecievedInformationManager
    {
        public Player Player { get; set; }
        public IPlayroom Playroom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void StartCommunication()
        {
            return;
        }
    }
}