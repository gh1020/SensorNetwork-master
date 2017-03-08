using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;
using NetMQ;

namespace SensorNetwork.Common
{
    public class ZeroMQ
    {

        public static string ZMQRequest(string endpoint, string requestText)
        {
            string answer = "";
            using (NetMQContext context = NetMQContext.Create())
            {
                answer=Client(context, endpoint, requestText);
            }
            return answer;
        }


        static string Client(NetMQContext context, string endpoint, string requestText)
        {
            string answer = "";
            using (NetMQSocket clientSocket = context.CreateRequestSocket())
            {
                clientSocket.Connect(endpoint);

               // while (true)
               // {
                   // Console.WriteLine("Please enter your message:");
                  
                    clientSocket.Send(requestText);

                    answer = clientSocket.ReceiveString();

                  //  Console.WriteLine("Answer from server: {0}", answer);

                  //  if (message == "exit")
                  //  {
                  //      break;
                  //  }
               // }
            }
            return answer;
        }




        public static string ZMQRequest2(string endpoint, string requestText)
        {
            string strReply = "";//收到的回复信息
                                 // Create
                                 // using (var context = new ZContext())
                                 //  try
                                 //{
            var context = new ZContext();
            var requester = new ZSocket(context, ZSocketType.DEALER);
            // Connect
            //requester.Connect("tcp://202.103.209.2:6789");

            //var requester = new ZSocket(ZSocketType.REQ);
                requester.Connect(endpoint);
            // for (int n = 0; n < 10; ++n)
            // {


            //Console.WriteLine("ID:" + n.ToString() + " Sending {0}...", requestText);

            // Send
            requester.Send(new ZFrame(requestText));
            Console.WriteLine("Sending:"+ requestText);
          // Receive
          //using (ZFrame reply = requester.ReceiveFrame())
          //{
          //    Console.WriteLine(" Received: {0} {1}!", requestText, reply.ReadString());//数据量大的时候会被截断
          //}
          byte[] buffer = null;
            using (ZFrame reply = requester.ReceiveFrame())                     // Receive  data
            {
                StringBuilder sb = new StringBuilder();
                buffer = new byte[reply.Length];
                reply.Read(buffer, 0, buffer.Length);
                sb.Append(System.Text.Encoding.UTF8.GetString(buffer));
                strReply = sb.ToString();
                Console.WriteLine(" Received: {0}...", sb.ToString());
            }//end using        using (ZFrame reply = requester.ReceiveFrame())  

            //   }
            //  catch (Exception ex)
            //   {
            // log.Info($"ZMQ Request：{ ex.Message}");
            //  }
            return strReply;
        }
    }
}

