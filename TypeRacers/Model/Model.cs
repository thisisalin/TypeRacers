﻿using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {       
        //returns the text sent from the server
        public string TextFromServer
        {
            get
            {
                return new TypeRacersClient()?.GetMessageFromServer();
            }
        }
    }
}