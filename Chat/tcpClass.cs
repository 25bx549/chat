


using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Windows;

namespace Chat {


    public class tcpClass
    {


        //  data
        string role { get; set; }
        string client_ip_and_port { get; set; }

        string client_ip;

        string client_port;

        //string server_port { get; set; }

        bool result_read;

        bool result_write;

        public Socket handler = null;

        public TcpClient client = null;

        public TcpListener server = null;

        public string IP_and_Port;

        public string[] IP_Port_columns;


        public Int32 port;  // = 1000;

        //public IPAddress localAddr = IPAddress.Parse("192.168.0.54");  // localhost
        public IPAddress localAddr = IPAddress.Parse("69.246.226.60");  // public IP

        public Byte[] bytes = new byte[256];

        public String data = null;

        public FlowDocument myFlowDoc = new FlowDocument();


        public tcpClass(string role, string client_ip_and_port)
        {

            this.role = role;

            this.client_ip_and_port = client_ip_and_port;
            //this.server_port = server_port;

            string[] ip_port_elements = client_ip_and_port.Split(',');

            client_ip = ip_port_elements[0];

            //IPAddress iPAddress = IPAddress.Parse(ip_port_elements[0]);

            client_port = ip_port_elements[1];

            Console.WriteLine("client_ip: " + client_ip + " client_port: " + client_port);


        }


        ~tcpClass() { Console.WriteLine(Environment.NewLine + DateTime.Now + " ~tcpClass() destructor called"); }



        //  methods
        //public async void initiate_connection_tcp_server()
        public void initiate_connection_tcp_server()
        {

            server = new TcpListener(IPAddress.Parse(client_ip), Convert.ToInt32(client_port));

            server.Start();

            bytes = new byte[256];

            Console.WriteLine(" Waiting for a connection (blocking call to server.AcceptTcpClient().");

            //  Perform a blocking call to accept requests.
            //  You can also use server. AcceptSocket() here.
            client = server.AcceptTcpClient();

            Console.WriteLine(Environment.NewLine + " socket Is Connected!");

            //StatusButton.Background = Brushes.Green;
            MainWindow.StatusButton.Background = Brushes.Green;


            //StatusButton.Content = "Connected!";
            MainWindow.StatusButton.Content = "Connected!";


            Console.WriteLine(Environment.NewLine + " StatusButton should have updated to GREEN by now.");

            String data = null;


            //    t1 = new Thread( runDispatcherTimer );

            //    t1.Start();

            return;
        }



        // public void runDispatcherTimer()
        // {
        // }


        public bool checkConnection()
        {

            bool isConnected = true;

            if (client.Connected == true)
            {

                Console.WriteLine(" Client IS connected.");

                return true;

            }
            else if (client.Connected == false)
            {

                Console.WriteLine(" Client is NOT connected.");

                return false;

            }

            return false;
        }





        public bool initiate_connection_tcp_client()
        {

            client = new TcpClient(client_ip, Convert.ToInt32(client_port));

            MainWindow.StatusButton.Background = Brushes.Green;

            MainWindow.StatusButton.Content = "Connected!";

            Console.WriteLine("socket Is Connected!");

            Console.WriteLine("StatusButton should have updated to GREEN by now.");

            return true;
        }



        public bool close_tcp_client_connection()
        {

            client.Close();

            return true;
        }


        public bool close_tcp_server_connection()
        {

            client.Close();

            return true;
        }






        public void send_message()
        {

            Console.WriteLine(" tcp_client - inside tcpClass->send_msg");

            NetworkStream stream = client.GetStream();

            // var message = "Hi friends 👋!<|EOM|> ";

            string message = null;

            Application.Current.Dispatcher.Invoke(() => {

                message = MainWindow.TextBox_Msg.Text;
                message = message.TrimEnd(new char[] { '\r', '\n' });

            });

            Byte[] data = Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);

            Console.WriteLine(" Just sent msg: " + message);

            //  Note: There is a particular reason why this works yet other methods fail. Need to understand why this is the case. 
            Application.Current.Dispatcher.Invoke(() => {

                //FlowDocument myFlowDoc = new FlowDocument();
                //Run myRun = new Run("This is a new line");
                message = " Sent msg: " + message;
                message = message.TrimEnd(new char[] { 'r', '\n' });
                Run myRun = new Run(message);
                Bold myBold = new Bold(myRun);
                Paragraph myParagraph = new Paragraph();
                myParagraph.Margin = new Thickness(0);
                myParagraph.Inlines.Add(myRun);
                myParagraph.Inlines.Add(myBold);

                if (this.role == "client")
                {
                    myParagraph.Foreground = Brushes.Red;
                }

                if (this.role == "server")
                {
                    myParagraph.Foreground = Brushes.Green;

                }

                myFlowDoc.Blocks.Add(myParagraph);
                // myFlowDoc.Blocks.Append(myParagraph);

                MainWindow.RTB_local.Document = myFlowDoc;

                // RTB_local.AppendText(Environment.NewLine + "Msg Sent: " + message  );

                // RTB_local.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightGreen );

            });

            return;
        }


        public void send_HB_message()
        {

            Console.WriteLine(" tcp_client - inside tcpClass->send_HB_message()");

            NetworkStream stream = client.GetStream();

            //var message = "Hi friends 👋!<|EOM|> ";

            string message = null;

            Application.Current.Dispatcher.Invoke(() => {

                message = "HB";

                message = message.TrimEnd(new char[] { '\r', '\n' });

            });

            Byte[] data = Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);

            Console.WriteLine(" Just sent msg: " + message);

            return;
        }




        public void receive_message()
        {

            Console.WriteLine(" tcp_server - inside tcpClass->receive_msg");

            String data = null;

            // Get a stream object for reading and writing. 
            // NetworkStream stream = client.GetStream();
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data send by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                //  Translate data bytes to ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                data = data.TrimEnd(new char[] { '\r', '\n' });
                data = " Recv'd msg: " + data;
                data = data.TrimEnd(new char[] { 'r', '\n' });
                Console.WriteLine(data);

                // Process the data sent by the client.
                // data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                if (data.Contains("HB") != true)
                {
                    // Note: there is a particular reason why this works yet other methods fail. Need to understand why this is the case. 
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        Run myRun = new Run(data);
                        Bold myBold = new Bold(myRun);
                        Paragraph myParagraph = new Paragraph();
                        myParagraph.Margin = new Thickness(0);
                        myParagraph.Inlines.Add(myRun);
                        myParagraph.Inlines.Add(myBold);


                        if (this.role == "client")
                        {
                            myParagraph.Foreground = Brushes.Green;
                        }


                        if (this.role == "server")
                        {
                            myParagraph.Foreground = Brushes.Red;
                        }

                        myFlowDoc.Blocks.Add(myParagraph);
                        // myFlowDoc.Blocks.Append(myParagraph);

                        MainWindow.RTB_local.Document = myFlowDoc;


                    });
                }

            }

            return;
        }


    }




}

