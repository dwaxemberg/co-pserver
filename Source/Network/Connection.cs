using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Conquer_Online_Server.Network
{
    public delegate void ConnectionArrived(StateObj StO);
    public delegate void DataArrived(StateObj StO, byte[] Buffer);
    public delegate void Disconnection(StateObj StO);


    public class StateObj
    {
        public byte[] Data;
        public Socket Sock;
        public object Wrapper;
    }

    public class Connection
    {
        Socket Listener;
        ConnectionArrived ConnHandler;
        DataArrived DataHandler;
        Disconnection DCHandler;
        private int MaxBuffer = 0;
        public void SetConnHandler(ConnectionArrived Method)
        {
            ConnHandler = Method;
        }
        public void SetDataHandler(DataArrived Method)
        {
            DataHandler = Method;
        }
        public void SetDCHandler(Disconnection Method)
        {
            DCHandler = Method;
        }
        public void Close()
        {
            Listener.Close();
        }
        public void Listen(int Port)
        {
            try
            {
                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint IEP = new IPEndPoint(IPAddress.Any, Port);
                Listener.Bind(IEP);
                Listener.Listen(100);
                MaxBuffer = 5024;
            }
            catch (Exception Exc) { Console.WriteLine(Exc); }

            Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
        }
        void WaitConnections(IAsyncResult Res)
        {
            try
            {
                StateObj S = null;
                try
                {
                    S = (StateObj)Res.AsyncState;
                    S.Sock = Listener.EndAccept(Res);
                    S.Data = new byte[MaxBuffer];
                }
                catch
                {
                    Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                    return;
                }
                if (ConnHandler != null)
                    ConnHandler.Invoke(S);

                Listener.BeginAccept(new AsyncCallback(WaitConnections), new StateObj());
                S.Sock.BeginReceive(S.Data, 0, MaxBuffer, SocketFlags.None, new AsyncCallback(WaitData), S);
            }
            catch { }
        }
        unsafe void WaitData(IAsyncResult Res)
        {
            try
            {
                StateObj S = (StateObj)Res.AsyncState;
                SocketError SE;
                try
                {
                    if (S.Sock.Connected)
                    {
                        uint DataLen = (uint)S.Sock.EndReceive(Res, out SE);

                        if (SE == SocketError.Success && DataLen != 0)
                        {
                            //byte[] RData = new byte[DataLen];
                            //fixed (byte* p = RData, p2 = S.Data)
                            //    Native.memcpy(p, p2, DataLen);

                            byte[] received = new byte[DataLen];
                            for (UInt16 x = 0; x < DataLen; x++)
                            {
                                received[x] = S.Data[x];
                            }


                            if (DataHandler != null)
                                DataHandler.Invoke(S, received);

                            S.Sock.BeginReceive(S.Data, 0, MaxBuffer, SocketFlags.None, new AsyncCallback(WaitData), S);
                        }
                        else if (DCHandler != null)
                            DCHandler.Invoke(S);
                    }
                    else if (DCHandler != null)
                        DCHandler.Invoke(S);
                }
                catch
                {
                    if (DCHandler != null)
                        DCHandler.Invoke(S);
                }
            }
            catch (Exception Exc) { Console.WriteLine(Exc); }
        }
    }
}
