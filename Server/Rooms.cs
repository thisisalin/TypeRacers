﻿using System.Collections.Generic;
using System.Linq;
using System;
using Common;

namespace Server
{
    public class Rooms
    {
        private readonly List<IPlayroom<Player>> playrooms;

        public Rooms()
        {
            playrooms = new List<IPlayroom<Player>>
            {
                new Playroom()
            };
        }

        public int GetNumberOfPlayrooms()
        {
            return playrooms.Count;
        }

        public void AllocatePlayroom(Player player)
        {
            while (true)
            {
                var dataRead = player.Read();
                dataRead.Remove(dataRead.Length - 1);

                var nameAndInfo = dataRead.Split('$');
                var infos = nameAndInfo.FirstOrDefault()?.Split('&');
                player.Name = nameAndInfo.LastOrDefault();
                Console.WriteLine(dataRead);

                ManagePlayerReceivedData(player, infos);
            }
        }

        private void ManagePlayerReceivedData(Player player, string[] infos)
        {
            player.FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
            if (player.FirstTimeConnecting)
            {
                SetPlayroom(player);
                player.Write(new GameMessage(player.Playroom.CompetitionText, player.Playroom.TimeToWaitForOpponents, player.Playroom.GameStartingTime, player.Playroom.GameEndingTime));
                Console.WriteLine("sending game info");
            }
            else
            {
                player.Playroom.TrySetGameStartingTime();
                player.TrySetRank();
                player.Write(new OpponentsMessage(player.Playroom.Players, player.Playroom.GameStartingTime, player.Playroom.GameEndingTime, player.Name, player.Finnished, player.Place));
                Console.WriteLine("sending opponents");
            }
        }

        private void SetPlayroom(Player player)
        {
            if(!playrooms.Any(p => p.Join(player)))
            {
                CreateNewPlayroom();
                playrooms.Last().Join(player);
            }
            player.Playroom.TrySetGameStartingTime();
        }

        public bool PlayerIsNew(Player player)
        {
            return !playrooms.Any(x => x.IsInPlayroom(player.Name));
        }

        private void CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
        }
    }
}