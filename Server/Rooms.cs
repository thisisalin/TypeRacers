using Common;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class Rooms
    {
        public readonly List<Playroom> playrooms;

        public Rooms()
        {
            playrooms = new List<Playroom>
            {
                new Playroom(new ServerGeneratedText())
            };
        }

        public void AllocatePlayroom(Player player)
        {
            var communicator = new ServerReceivedInformationManager(player, playrooms.Last());

            if (!playrooms.Any(p => p.Join(player, communicator)))
            {
                CreateNewPlayroom();
                communicator = new ServerReceivedInformationManager(player, playrooms.Last());
                playrooms.Last().Join(player, communicator);
            }
        }

        public int GetNumberOfPlayrooms()
        {
            return playrooms.Count;
        }

        private void CreateNewPlayroom()
        {
            var newPlayroom = new Playroom(new ServerGeneratedText());
            playrooms.Add(newPlayroom);
        }
    }
}