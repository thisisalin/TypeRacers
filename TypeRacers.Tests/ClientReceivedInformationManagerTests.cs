using Common;
using Server;
using System;
using System.Collections.Generic;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class ClientReceivedInformationManagerTests
    {
        private Player player;
        private GameInfo gameInfo;
        private ClientReceivedInformationManager communicator;

        [Fact]
        public void FirstConnectionReceivedMessage()
        {
            player = new Player(new FakeTypeRacersClient());
            gameInfo = new GameInfo();
            player.SetPlayroom(gameInfo);
            communicator = new ClientReceivedInformationManager(player, gameInfo);
            var waitingTime = DateTime.UtcNow;
            player.Write(new GameMessage("This is the text", waitingTime, DateTime.MinValue, DateTime.MinValue));
            communicator.ManageReceivedData();

            Assert.Equal("This is the text", gameInfo.CompetitionText);
            Assert.False(gameInfo.TimeToWaitForOpponents.Equals(DateTime.MinValue));
        }

        [Fact]
        public void GameStatusReceivedMessage()
        {
            player = new Player(new FakeTypeRacersClient());
            gameInfo = new GameInfo();
            player.SetPlayroom(gameInfo);
            communicator = new ClientReceivedInformationManager(player, gameInfo);
            var waitingTime = DateTime.UtcNow;
            player.Write(new GameMessage("This is the text", waitingTime, DateTime.MinValue, DateTime.MinValue));
            communicator.ManageReceivedData();

            Player opponent = new Player(new FakeTypeRacersClient())
            {
                Name = "Ana"
            };
            var opponents = new List<Player> { opponent };
            player.Write(new OpponentsMessage(opponents, DateTime.MinValue, DateTime.MinValue, "Bianca", false, 0));
            communicator.ManageReceivedData();
            Assert.Single(gameInfo.Players);
        }
    }
}