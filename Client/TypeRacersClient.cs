﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        public delegate void OpponentsChangedEventHandler(List<Player> updatedOpponents);

        public event OpponentsChangedEventHandler OpponentsChanged;

        public GameInfo gameInfo;

        public TypeRacersClient(Player player)
        {
            Player = player;
            gameInfo = new GameInfo();
            Player.SetPlayroom(gameInfo);
            StartServerCommunication();
        }

        public Player Player { get; set; }

        public void StartServerCommunication()
        {
            Thread writeThread = new Thread(Write);
            Thread readThread = new Thread(Read);
            writeThread.Start();
            readThread.Start();
        }

        private void Read()
        {
            while (true)
            {
                var data = Player.Read();
                if (Player.FirstTimeConnecting)
                {
                    gameInfo.SetGameInfo(data);
                    Player.FirstTimeConnecting = false;
                }
                else
                {
                    SetGameStatus(data);
                }

                Thread.Sleep(3000);
            }
        }

        private void SetGameStatus(string data)
        {
            var infos = data.Split('%').ToList();
            infos.Remove("#");
            foreach (var i in infos)
            {
                if (i.StartsWith("!"))
                {
                    var rank = i.Split('/');
                    Player.Finnished = Convert.ToBoolean(rank.FirstOrDefault().Substring(1));
                    Player.Place = int.Parse(rank.LastOrDefault());
                    infos.Remove(i);
                    break;
                }
            }
            gameInfo.SetOpponentsAndTimers(infos);
        }

        private void Write()
        {
            while (true)
            {
                Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name));
                OnOpponentsChanged(Player.Playroom.Players);
                Thread.Sleep(3000);
            }
        }

        public void NameClient(string username)
        {
            Player.Name = username;
        }
        protected void OnOpponentsChanged(List<Player> opponents)
        {
            if (opponents != null && OpponentsChanged != null)
            {
                OpponentsChanged(opponents);
            }
        }
    }
}