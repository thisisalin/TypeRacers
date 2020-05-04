namespace Server
{
    public class ServerReceivedInformationManager
    {
        public ServerReceivedInformationManager()
        {
        }

        //public Player Player { get; set; }
        //public IPlayroom Playroom { get; set; }

        //public void StartCommunication()
        //{
        //    if (networkClient.IsConnected())
        //    {
        //        //first we read data from clients
        //        networkClient.Read(ReadCallback, buffer);
        //    }
        //    else
        //    {
        //        throw new IOException();
        //    }
        //}

        //private void WriteCallback(IAsyncResult ar)
        //{
        //    //when the write is done we reach here
        //    try
        //    {
        //        // Retrieve the stream from the state object.
        //        NetworkStream networkStream = (NetworkStream)ar.AsyncState;
        //        // Complete sending the data to the remote device.
        //        networkStream.EndWrite(ar);
        //        //after writing we read again, Player.Read contains networkclient.BeginRead()
        //        networkClient.Read(ReadCallback, buffer);
        //    }
        //    catch (IOException)
        //    {
        //        networkClient.Dispose();
        //    }
        //}

        //private void ReadCallback(IAsyncResult ar)
        //{
        //    //when the read is done we reach here
        //    try
        //    {
        //        NetworkStream networkStream = ar.AsyncState as NetworkStream;
        //        int bytesRead = networkStream.EndRead(ar);

        //        if (bytesRead > 0)
        //        {
        //            receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        //            if (receivedData.Contains("#"))
        //            {
        //                // All the data has been read from the stream
        //                receivedData = receivedData.Remove(receivedData.Length - 1);
        //                Console.WriteLine("Data received: " + receivedData);
        //                ProcessResults(receivedData);
        //            }
        //            else
        //            {
        //                // Not all data received. Get more.
        //                networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), networkStream);
        //            }
        //        }
        //    }
        //    catch (IOException)
        //    {
        //        networkClient.Dispose();
        //    }
        //}

        //private void ProcessResults(string data)
        //{
        //    Player.UpdateInfo(data);

        //    if (!Player.CheckIfLeft())
        //    {
        //        if (Player.FirstTimeConnecting || Player.CheckIfTriesToRestart())
        //        {
        //            SendGameInfo();
        //            Player.FirstTimeConnecting = false;
        //        }
        //        else
        //        {
        //            SendGamestatus();
        //        }
        //    }
        //}

        //private void SendGamestatus()
        //{
        //    Playroom.TrySetStartingTime();
        //    Player.TrySetRank();
        //    networkClient.Write(GetGameStatus(), WriteCallback);
        //    Console.WriteLine("sending opponents");
        //}

        //private void SendGameInfo()
        //{
        //    Playroom.TrySetStartingTime();
        //    networkClient.Write(GameMessage(), WriteCallback);
        //    Console.WriteLine("sending game info");
        //}

        //private IMessage GetGameStatus()
        //{
        //    return new OpponentsMessage(Playroom.Players, Playroom.GameStartingTime, Playroom.GameEndingTime, Player.Name, Player.Finnished, Player.Place);
        //}

        //private IMessage GameMessage()
        //{
        //    return new GameMessage(Playroom.CompetitionText, Playroom.TimeToWaitForOpponents, Playroom.GameStartingTime, Playroom.GameEndingTime);
        //}
    }
}