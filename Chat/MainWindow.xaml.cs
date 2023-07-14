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

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {

            InitializeComponent();

        }



        public void initiate_tcp()
        {


            //  collect config settings from UI to determine which of server/client to run...

            if ( Radio_tcp.IsChecked == true && Checkbox_tcp_client.IsChecked == true )
            {
                tcpClass tcpInstance = new tcpClass( "client", tcp_client_ip_addr_port_string.Text.ToString(), tcp_server_port_string.Text.ToString() );
                tcpInstance.initiate_connection_tcp_client();




            }
            else if (Radio_tcp.IsChecked == true && Checkbox_tcp_server.IsChecked == true)
            {
                tcpClass tcpInstance = new tcpClass( "server", tcp_client_ip_addr_port_string.Text.ToString(), tcp_server_port_string.Text.ToString());

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


            public tcpClass(string role, string client_ip_and_port, string server_port)
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
                listener.Listen(1000);


                

                for (; ; )
                {

                    

                    bool result = listener.Poll(1000, SelectMode.SelectRead);

                    bool result_connected = listener.Connected;


                    if ( result == true )
                    {
                       Console.WriteLine("socketIsReadble!");

                       


                    }
                    //if (result_connected == true)
                    //{
                    //    Console.WriteLine("socketIsConnected!");
                    //}


                    DateTime NOW = DateTime.Now;
                    DateTime NOW_PLUS = NOW.AddSeconds(5);
                    for(; ; )
                    {
                        if ( NOW >= NOW_PLUS) { break; }
                    }

                    

                }
                
                






                //var handler = await listener.AcceptAsync();


                /*
                
                while (true)
                {
                    // Receive message.
                    var buffer = new byte[1_024];

                    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);

                    var response = Encoding.UTF8.GetString(buffer, 0, received);

                    var eom = "<|EOM|>";
                    if (response.IndexOf(eom) > -1 )
                    {
                        Console.WriteLine(
                            $"Socket server received message: \"{response.Replace(eom, "")}\"");

                        var ackMessage = "<|ACK|>";
                        var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                        handler.SendAsync(echoBytes, 0);
                        Console.WriteLine(
                            $"Socket server sent acknowledgment: \"{ackMessage}\"");

                        break;
                    }




                }
                */




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

        private void Button_CxnState_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
