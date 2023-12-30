using System.Net.Sockets;

namespace Pico.Twitch
{
    public class TwitchChatHandle : IDisposable
    {
        //BE SURE TO GIVE YOUR BOT'S TWITCH ACCOUNT MODERATOR
        //SOME FUNCTIONS MAY NOT WORK OTHERWISE
        //OAuth token can be retrieved from https://twitchapps.com/tmi/

        bool exit = false;
        string channel;

        TcpClient tcpClient;
        StreamReader inputStream;
        StreamWriter outputStream;

        Thread pingThread;

        /// <summary>
        /// Allows you to read and write Twitch chat messages.
        /// </summary>
        /// <param name="oauth">OAuth token.</param>
        /// <param name="channel">Streamer's channel.</param>
        public TwitchChatHandle(string oauth, string channel)
        {
            this.channel = channel;

            tcpClient = new TcpClient("irc.twitch.tv", 6667);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine($"PASS {oauth}");
            outputStream.WriteLine($"NICK bot");
            outputStream.WriteLine($"USER bot 8 * :bot");
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();


            pingThread = new Thread(() => AutoPing());
            pingThread.Start();
        }

        /// <summary>
        /// Returns the next Twitch chat entry.
        /// </summary>
        /// <returns></returns>
        public TwitchChatEntry Read()
        {
            string read;
            while (true)
            {
                if (exit) return null;

                read = TcpRead();

                if (read == null)
                {
                    Thread.Sleep(100);  //CPU says thank you :)
                    continue;
                }

                if (!read.Contains("PRIVMSG")) continue;

                break; //Valid chat entry data
            }

            //Parse TwitchChatEntry from read

            //Get sender's name
            int startIndex = read.IndexOf(":") + 1;
            int endIndex = read.IndexOf("!");
            int length = endIndex - startIndex;
            string chatSender = read.Substring(startIndex, length);

            //Get chat message
            startIndex = read.IndexOf(":", 1) + 1;
            string chatMessage = read.Substring(startIndex);

            return new TwitchChatEntry(chatSender,chatMessage);
        }

        /// <summary>
        /// Writes a message to chat.
        /// </summary>
        /// <param name="chatMessage">Message to write.</param>
        public bool Write(string chatMessage)
        {
            return TcpWrite($":bot!bot@bot.tmi.twitch.tv PRIVMSG #{channel} :{chatMessage}");
        }

        string TcpRead()
        {
            try
            {
                return inputStream.ReadLine();
            }
            catch
            {
                return null;

            }
        }

        bool TcpWrite(string text)
        {
            try
            {
                outputStream.WriteLine(text);
                outputStream.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        void AutoPing()     //Prevents getting disconnected from server-side   
        {
            while (true)
            {
                if (exit) return;

                //Send ping to server
                var success = TcpWrite("PING irc.twitch.tv");

                //Check if ping failed
                if (!success)
                {
                    Thread.Sleep(100);
                    continue;   //Try again
                }
                //Ping successful

                //Wait for 5 minutes
                //Check if exit every second
                for (int i = 0; i < 300; i++)
                {
                    if (exit) return;
                    Thread.Sleep(1000);         
                    i++;
                }
            }
        }

        public void Dispose()
        {
            exit = true;
            pingThread.Join();
            inputStream.Dispose();
            outputStream.Dispose();
            tcpClient.Dispose();
        }

    }
    public class TwitchChatEntry
    {
        public readonly string Sender;
        public readonly string Message;
        public TwitchChatEntry(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
