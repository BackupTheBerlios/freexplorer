using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Threading;

namespace Wizou.LIRC
{
    class LIRCServer
    {
        private TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8765);
        private ArrayList sockets = new ArrayList(1);

        public void Start()
        {
            if (tcpListener.Server.IsBound) return;
            tcpListener.Start();
            Thread serverThread = new Thread(new ThreadStart(ThreadLoop));
            serverThread.Start();
        }

        public void Stop()
        {
            if (!tcpListener.Server.IsBound) return;
            foreach (Socket socket in sockets)
                socket.Close();
            sockets.Clear();
            tcpListener.Stop();
        }

        public bool Active
        {
            set
            {
                if (value) 
                    Start(); 
                else 
                    Stop();
            }
        }


        private void ThreadLoop()
        {
            while (true)
            {
                Socket socket;
                try
                {
                    socket = tcpListener.AcceptSocket();
                }
                catch (SocketException e)
                {
                    if ((e.SocketErrorCode == SocketError.Interrupted) || (e.SocketErrorCode == SocketError.InvalidArgument))
                        return; // arrêt du serveur
                    throw;
                }
                sockets.Add(socket);
            }
        }

        public int Connections
        {
            get
            {
                foreach (Socket socket in sockets)
                    if (!socket.Connected)
                        sockets.Remove(socket);
                return sockets.Count;
            }
        }

        public void KeyPressed(string key)
        {
            int index = 0;
            while (index < sockets.Count)
            {
                Socket socket = (Socket) sockets[index++];
                string line = "0000000000000000 00 " + key + " freebox\n";
                try
                {
                    socket.Send(Encoding.Default.GetBytes(line));
                }
                catch (SocketException)
                {
                    if (!socket.Connected)
                        sockets.RemoveAt(--index);
                }
            }
        }
    }
}
