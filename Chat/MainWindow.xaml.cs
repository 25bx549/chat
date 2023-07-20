using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net;
//using static System.Net.Mime.MediaTypeNames;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Chat
{


    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // https://stackoverflow.com/questions/33680398/c-sharp-wpf-how-to-simply-update-ui-from-another-class-thread
        public static Button StatusButton;
        public static TextBox TextBox_Msg;
        public static TextBlock TB_local;


        public bool tcp;
        public bool tcp_client;
        public bool tcp_server;

        public static Socket listener = null;
        public static Socket client = null;

        public tcpClass tcpInstance;

        System.Threading.Thread t1;
        //Thread t2;

        //public static Socket handler = null;


        public MainWindow()
        {

            InitializeComponent();

            this.Title = "System.Net.Sockets - Run as a client or server for raw messaging with another endpoint!";


            StatusButton = Button_CxnState;
            TextBox_Msg = TextBox_enterMessage;
            //TB_local = TextBox_enterMessage;
            TB_local = TextBlock_messages;

            //TB_local.Text = "ja ja ja";



            










        }



        public void initiate_tcp()
        {



            //bool tcp;
            //bool tcp_client;
            //bool tcp_server;

            //  deal with GUI controls of Main thread
            System.Windows.Application.Current.Dispatcher.Invoke( DispatcherPriority.Normal, (ThreadStart)delegate 
            {
                if (Radio_tcp.IsChecked == true && Checkbox_tcp_client.IsChecked == true)
                {
                    tcp = true;
                    tcp_client = true;
                }
                else if (Radio_tcp.IsChecked == true && Checkbox_tcp_server.IsChecked == true)
                {
                    tcp = true;
                    tcp_server = true;
                }
            });


            //  collect config settings from UI to determine which of server/client to run...

            //if (Radio_tcp.IsChecked == true && Checkbox_tcp_client.IsChecked == true)
            if (tcp == true && tcp_client == true)
            {
                //tcpClass tcpInstance = new tcpClass("client", tcp_client_ip_addr_port_string.Text.ToString());
                tcpInstance = new tcpClass("client", tcp_client_ip_addr_port_string.Text.ToString());
                tcpInstance.initiate_connection_tcp_client();



            }
            //else if (Radio_tcp.IsChecked == true && Checkbox_tcp_server.IsChecked == true)
            else if (tcp == true && tcp_server == true)
            {
                //tcpClass tcpInstance = new tcpClass("server", tcp_client_ip_addr_port_string.Text.ToString());
                tcpInstance = new tcpClass("server", tcp_client_ip_addr_port_string.Text.ToString());
                tcpInstance.initiate_connection_tcp_server();




            }


        }


        public class tcpClass
        {

            //  data
            string role { get; set; }
            string client_ip_and_port { get; set; }

            string client_ip;
            string client_port;

            string server_port { get; set; }

            bool result_read;
            bool result_write;

            public Socket handler = null;

            public tcpClass(string role, string client_ip_and_port)
            {
                this.role = role;
                this.client_ip_and_port = client_ip_and_port;
                this.server_port = server_port;

                string[] ip_port_elements = client_ip_and_port.Split(',');
                client_ip = ip_port_elements[0];
                client_port = ip_port_elements[1];

                Console.WriteLine("client_ip: " + client_ip + " client_port: " + client_port);


            }


            ~tcpClass() { }



            //  methods
            public async void initiate_connection_tcp_server()
            {
                //System.Net.IPEndPoint ipEndPoint = new( ipAddress, 11_000);


                //System.Net.IPAddress ip = System.Net.IPAddress.Parse("192.168.0.54");
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(client_ip);



                //System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint( ip, 1000);
                System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, Convert.ToInt32(client_port));



                //Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen(1000);

                //  wait for connection to be established...
                for (; ; )
                {

                    Console.WriteLine(" Starting next connection attempt iteration...");




                    //  this is necessary as for some reason listener.Connected does not seem to work for a tcp server role 
                    result_read = listener.Poll(1000, SelectMode.SelectRead);
                    Console.WriteLine(" result_read: " + result_read);
                    // for some reason SelectMode.SelectWrite does not return true after socket is set to a listen state
                    result_write = listener.Poll(1000, SelectMode.SelectWrite);
                    Console.WriteLine(" result_write: " + result_write);



                    //  skip this as it does not seem to work for a tcp server role
                    //bool result_connected = listener.Connected;


                    if (result_read == true)
                    //if ( (result_read == true) && (result_write == true) )
                    //if ( result_connected == true)
                    {
                        //Console.WriteLine("socket Is Readble and Writable!");
                        //Console.WriteLine("socket Is Readble!");
                        Console.WriteLine("socket Is Connected!");

                        StatusButton.Background = Brushes.Green;

                        StatusButton.Content = "Connected!";

                        TB_local.Text = "Initializing...";


                        Console.WriteLine("StatusButton should have updated by now.");

                        break;
                    }

                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for (; ; )
                    {

                        NOW = DateTime.Now;

                        if (NOW >= NOW_PLUS) {

                            Console.WriteLine(" Delaying loop for 5 seconds, before next connect attempt iteration...");

                            break;
                        }
                        //else
                        //{
                        //    Thread.Sleep(4000);
                        //}

                    }

                }


                Console.WriteLine(" Now setting-up listener.AcceptAsync()");

                //var handler = await listener.AcceptAsync();
                handler = await listener.AcceptAsync();




                /*

                while (true)
                {
                    // Receive message.
                    var buffer = new byte[1_024];
                    var receiveArgs = new SocketAsyncEventArgs();
                    receiveArgs.SetBuffer(buffer, 0, 1024);

                    //var received = handler.ReceiveAsync(receiveArgs);
                    var received = handler.Receive(buffer);

                    var response = Encoding.UTF8.GetString(buffer);

                    Console.WriteLine(" printing response: " + response);

                    Console.WriteLine(" printing buffr: " + buffer);

                    TB_local.Text = response;
                    //TextBox_Msg.Text = response;



                    break;

                    /*
                    var eom = "<|EOM|>";
                    if (response.IndexOf(eom) > -1 )
                    {
                        Console.WriteLine($"Socket server received message: \"{response.Replace(eom, "")}\"");

                        var ackMessage = "<|ACK|>";
                        var echoBytes = Encoding.UTF8.GetBytes(ackMessage);

                        handler.SendAsync(echoBytes, 0);

                        Console.WriteLine(
                            $"Socket server sent acknowledgment: \"{ackMessage}\"");

                        break;
                    }
                    */
                //break;


                /*
                DateTime NOW = DateTime.Now;
                DateTime NOW_PLUS = NOW.AddSeconds(5);
                for (; ; )
                {

                    NOW = DateTime.Now;

                    if (NOW >= NOW_PLUS)
                    {

                        Console.WriteLine(" Next delay iteration...");

                        break;
                    }
                    //else
                    //{
                    //    Thread.Sleep(4000);
                   // }

                }



            }

               */



                //  this is necessary as for some reason listener.Connected does not seem to work for a tcp server role 
                result_read = listener.Poll(1000, SelectMode.SelectRead);
                Console.WriteLine(" result_read: " + result_read);
                // for some reason SelectMode.SelectWrite does not return true after socket is set to a listen state
                result_write = listener.Poll(1000, SelectMode.SelectWrite);
                Console.WriteLine(" result_write: " + result_write);




                return;
            }




            public bool initiate_connection_tcp_client()
            {

                System.Net.IPAddress ip = System.Net.IPAddress.Parse(client_ip);

                System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, Convert.ToInt32(client_port));

                //Socket client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);





                //var handler = client.ConnectAsync(ipEndPoint);
                //handler = client.ConnectAsync(ipEndPoint);


                client.Connect(ipEndPoint);




                for (; ; )
                {

                    Console.WriteLine(" Starting next connection attempt iteration...");

                    bool result_connected = client.Connected;

                    if (result_connected == true)
                    {

                        StatusButton.Background = Brushes.Green;

                        StatusButton.Content = "Connected!";

                        Console.WriteLine("socket Is Connected!");

                        Console.WriteLine("StatusButton should have updated by now.");

                        break;
                    }

                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for (; ; )
                    {

                        NOW = DateTime.Now;

                        if (NOW >= NOW_PLUS)
                        {

                            Console.WriteLine(" Delaying loop for 5 seconds, before next connect attempt iteration...");

                            break;
                        }
                        else
                        {
                            Thread.Sleep(4000);
                        }

                    }

                }





                /*


                int counter = 0;
                while (true)
                {

                    counter++;
                    // Send message.
                    var message = "Hi friends 👋!<|EOM|> " + counter;

                    var messageBytes = Encoding.UTF8.GetBytes(message);

                    var sendArgs = new SocketAsyncEventArgs();

                    client.Send(messageBytes, SocketFlags.None);



                    //_ = await client.SendAsync(messageBytes, SocketFlags.None);
                    //_ = client.SendAsync(messageBytes, SocketFlags.None);

                    Console.WriteLine($"Socket client sent message: \"{message}\"");

                    break;



                    /*
                    // Receive ack.
                    var buffer = new byte[1_024];
                    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    if (response == "<|ACK|>")
                    {
                        Console.WriteLine(
                            $"Socket client received acknowledgment: \"{response}\"");
                        break;
                    }
                    


                    // Sample output:
                    //     Socket client sent message: "Hi friends 👋!<|EOM|>"
                    //     Socket client received acknowledgment: "<|ACK|>"



                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for (; ; )
                    {

                        NOW = DateTime.Now;

                        if (NOW >= NOW_PLUS)
                        {

                            Console.WriteLine(" Next delay iteration...");

                            break;
                        }
                        else
                        {
                            Thread.Sleep(4000);
                        }

                    }


                    
                }



                */













                return true;
            }

            public bool send_message()
            {


                Console.WriteLine(" tcp_client - inside tcpClass->send_msg");

                var message = "Hi friends 👋!<|EOM|> ";

                var messageBytes = Encoding.UTF8.GetBytes(message);

                var sendArgs = new SocketAsyncEventArgs();

                //Console.WriteLine(" client (Socket) is valid? " + client. );

                client.Send(messageBytes, SocketFlags.None);

                Console.WriteLine(" Just sent msg: " + message);

                return true;
            }



            public bool receive_message()
            {

                Console.WriteLine(" tcp_server - inside tcpClass->receive_msg");

                while (true)
                {
                    // Receive ack.
                    var buffer = new byte[1_024];
                    //var received = await client.ReceiveAsync(buffer, SocketFlags.None);


                    //  the following generating "Object not set to an instance of an object." Need to figure out why.  
                    var received = client.Receive(buffer, SocketFlags.None);


                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    //if (response == "<|ACK|>")
                    //{
                    //    Console.WriteLine(
                    //        $"Socket client received acknowledgment: \"{response}\"");
                    //    break;
                    //}

                    Console.WriteLine(" printing response: " + response);
                    Console.WriteLine(" printing buffer: " + buffer);

                    TB_local.Text = response;
                    //TextBox_Msg.Text = response;

                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for (; ; )
                    {

                        NOW = DateTime.Now;

                        if (NOW >= NOW_PLUS)
                        {
                            Console.WriteLine(" Delaying loop for 5 seconds, before next connect attempt iteration...");
                            break;
                        }
                        else
                        {
                            Thread.Sleep(4000);
                        }
                    }
                    //break;

                }

                    return true;
            }







        }






        //////////////////
        //  TESTING SECTION - NOT RELATED TO THIS APP AT ALL (IGNORE)! 
        /// /////////////


        class classForUnitTesting
        {

            //  data
            public int age { get; set; }
            public string name { get; set; }

            //  methods

            public classForUnitTesting(int Age, string Name)
            {
                age = Age;

                name = Name;
            }

            ~classForUnitTesting() { }

            public int changeAge()
            {

                int newAge = age + 26;

                age = newAge;

                return newAge;
            }

            public bool printLatestAge()
            {

                Console.WriteLine(" latest age: " + age);

                return true;
            }

        }


        [TestClass]
        public class testClass 
        {

            classForUnitTesting CFU;

            [TestInitialize]
            public void Initialize()
            {
                //classForUnitTesting CFU = new classForUnitTesting(25, "david ");
                CFU = new classForUnitTesting(25, "david ");
                CFU.changeAge();

                

            }
            
            [TestMethod]
            public void checkAssertions()
            {

                Assert.AreEqual(50, CFU.age);

                return;
            }



        }

        //////////////////
        //  END OF TESTING SECTION - NOT RELATED TO THIS APP AT ALL (IGNORE)! 
        /// /////////////












        private void Button_Click_Intitiate_tcp(object sender, RoutedEventArgs e)
        {

            Console.WriteLine(" launching App");

            //Thread t1 = new Thread(initiate_tcp);
            //t1 = new Thread(initiate_tcp);
            //t1.Start();

            initiate_tcp();




            if (tcp_server == true)
            {
                Console.WriteLine(" tcp_server - initiating tcpClass->receive_msg");

                tcpInstance.receive_message();

            }


        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {



            if ( tcp_client == true )
            {

                Console.WriteLine(" tcp_client - initiating tcpClass->send_msg");

                tcpInstance.send_message();


            }

            



        }


        private void RadioButton_Checked_tcp(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked_udp(object sender, RoutedEventArgs e)
        {

        }

        private void Checkbox_tcp_client_checked(object sender, RoutedEventArgs e)
        {

        }

        private void Checkbox_tcp_server_checked(object sender, RoutedEventArgs e)
        {

        }

        public void Button_CxnState_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_enterMessage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
