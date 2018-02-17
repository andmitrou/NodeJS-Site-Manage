using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.Networking
{
    public class Utils
    {
      public static bool  ServerIsListening(string server, int port)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.Connect(server, port);
                }
                catch (SocketException)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
