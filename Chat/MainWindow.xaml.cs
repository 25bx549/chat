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

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // https://stackoverflow.com/questions/33680398/c-sharp-wpf-how-to-simply-update-ui-from-another-class-thread
        public static Button StatusButton;
        public static TextBlock TB_local;

        


        public MainWindow()
        {

            InitializeComponent();

            this.Title = "Secret Chat";
            

            StatusButton = Button_CxnState;
            //TB_local = TextBox_enterMessage;
            TB_local = TextBlock_messages;

            TB_local.Text = "ja ja ja";

        }



        public void initiate_tcp()
        {


            //  collect config settings from UI to determine which of server/client to run...

            if ( Radio_tcp.IsChecked == true && Checkbox_tcp_client.IsChecked == true )
            {
                tcpClass tcpInstance = new tcpClass( "client", tcp_client_ip_addr_port_string.Text.ToString() );
                tcpInstance.initiate_connection_tcp_client();


                //Button_CxnState.Background = Brushes.Green;



            }
            else if (Radio_tcp.IsChecked == true && Checkbox_tcp_server.IsChecked == true)
            {
                tcpClass tcpInstance = new tcpClass( "server", tcp_client_ip_addr_port_string.Text.ToString() );

                //Button_CxnState.Background = Brushes.Green;

                tcpInstance.initiate_connection_tcp_server();


                


            }


        }


        class tcpClass
        {

            //  data
            string role { get; set; }
            string client_ip_and_port {  get; set; }

            string client_ip;
            string client_port;

            string server_port { get; set; }
            



            public tcpClass(string role, string client_ip_and_port )
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
                System.Net.IPAddress ip = System.Net.IPAddress.Parse( client_ip );
                


                //System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint( ip, 1000);
                System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, Convert.ToInt32( client_port) );



                Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(ipEndPoint);
                listener.Listen( 1000 );

                //  wait for connection to be established...
                for (; ; )
                {

                    Console.WriteLine(" Next iteration...");

                    bool result_read = listener.Poll(1000, SelectMode.SelectRead);
                    //bool result_write = listener.Poll(1000, SelectMode.SelectWrite);

                    bool result_connected = listener.Connected;


                    if (result_read == true) 
                    //if ( ( result_read == true ) && ( result_write == true ) )
                    {
                        //Console.WriteLine("socket Is Readble and Writable!");
                        Console.WriteLine("socket Is Readble!");


                        //App.Current.Dispatcher.Invoke(delegate {
                        //    Button_CxnState.Background = Brushes.Green;
                        //});


                        StatusButton.Background = Brushes.Green;
                        StatusButton.Content = "Connected!";

                        TB_local.Text = "ARGH!!!";
                        
                        //Button_CxnState.Background = Brushes.Green;




                        break;
                    }

                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for(; ; )
                    {

                        NOW = DateTime.Now;

                        if ( NOW >= NOW_PLUS) {

                            Console.WriteLine(" Next delay iteration...");

                            break; 
                        }
                        //else
                        //{
                        //    Thread.Sleep(4000);
                        //}

                    }

                }

                var handler = await listener.AcceptAsync();
                
                
                while (true)
                {
                    // Receive message.
                    var buffer = new byte[1_024];


                    var receiveArgs = new SocketAsyncEventArgs();
                    receiveArgs.SetBuffer(buffer,0,1024);


                    //var received = await handler.ReceiveAsync(receiveArgs);
                    var received = handler.ReceiveAsync(receiveArgs);


                    handler.Receive(buffer, SocketFlags.None );

                    //var response = Encoding.UTF8.GetString(buffer, 0, received);
                    var response = Encoding.UTF8.GetString(buffer);



                    Console.WriteLine( " printing response: " + response);
                    Console.WriteLine( " printing buffr: " + buffer);


                    TB_local.Text = response;

                    




                    //_local. = response;

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

                return;
            }




            public bool initiate_connection_tcp_client()
            {


                //System.Net.IPAddress ip = System.Net.IPAddress.Parse("192.168.0.54");
                System.Net.IPAddress ip = System.Net.IPAddress.Parse( client_ip );



                //System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, 1000);
                System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, Convert.ToInt32(client_port ) );



                Socket client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                var handler = client.ConnectAsync(ipEndPoint);






                for (; ; )
                {

                    Console.WriteLine(" Next iteration...");

                    //bool result_read = client.Poll(1000, SelectMode.SelectRead);
                    //bool result_write = listener.Poll(1000, SelectMode.SelectWrite);

                    bool result_connected = client.Connected;


                    if (result_connected == true)
                    //if ( ( result_read == true ) && ( result_write == true ) )
                    {
                        //Console.WriteLine("socket Is Readble and Writable!");
                        Console.WriteLine("socket Is Connected!");


                        //App.Current.Dispatcher.Invoke(delegate {
                        //    Button_CxnState.Background = Brushes.Green;
                        //});


                        StatusButton.Background = Brushes.Green;
                        StatusButton.Content = "Connected!";



                        //Button_CxnState.Background = Brushes.Green;




                        break;
                    }

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


                
                while (true)
                {
                    // Send message.
                    var message = "Hi friends 👋!<|EOM|>";

                    var messageBytes = Encoding.UTF8.GetBytes(message);

                    var sendArgs = new SocketAsyncEventArgs();

                    client.Send(messageBytes, SocketFlags.None);

                    

                    //_ = await client.SendAsync(messageBytes, SocketFlags.None);
                    //_ = client.SendAsync(messageBytes, SocketFlags.None);

                    Console.WriteLine($"Socket client sent message: \"{message}\"");





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
                    */


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























                return true;
            }

            public bool transmit_message()
            {




                return true;
            }


        }



                       

        
        private void Button_Click_Intitiate_tcp(object sender, RoutedEventArgs e)
        {

            Console.WriteLine(" launching App");

            initiate_tcp();




        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {





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
