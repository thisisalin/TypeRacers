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
            if (!playrooms.Any(p => p.Join(player)))
            {
                CreateNewPlayroom();
                playrooms.Last().Join(player);
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