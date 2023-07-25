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

using CommunityToolkit.Mvvm;

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
using System.Windows.Resources;
using System.Windows.Markup;
using Application = System.Windows.Application;
using System.Reflection.Emit;

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
        public static TextBox TB_local;

        //  booleans for tracking the role specified by the user
        public bool tcp;
        public bool tcp_client;
        public bool tcp_server;


        public TcpListener server = null;

        public static Socket client = null;

        public tcpClass tcpInstance;


        //  the following are to run send_message() and receive_message() at the same time, for bi-directional messaging
        System.Threading.Thread t1;

        System.Threading.Thread t2;



        public MainWindow()
        {

            InitializeComponent();

            this.Title = "System.Net.Sockets - Run as a client or server for raw messaging with another endpoint!";

            //  the following to make the UI controls accessible/updateable 
            StatusButton = Button_CxnState;

            TextBox_Msg = TextBox_enterMessage;

            TB_local = TextBox_messages;
            

        }





        //  This is Program Entry Point based on Button Click to establish tcp connection 
        public void initiate_tcp()
        {
            //  collect config settings from UI to determine which of server/client to run...
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
            
            //  now execute based on user-defined tcp role 
            if (tcp == true && tcp_client == true)
            {
                tcpInstance = new tcpClass("client", tcp_client_ip_addr_port_string.Text.ToString());

                tcpInstance.initiate_connection_tcp_client();

            }
            else if (tcp == true && tcp_server == true)
            {
                tcpInstance = new tcpClass("server", tcp_client_ip_addr_port_string.Text.ToString());

                tcpInstance.initiate_connection_tcp_server();

            }

        }





        class UIControl : MainWindow
        {


            public static void ChangeButtonName(string text)
            {
                App.Current.Dispatcher.Invoke(delegate {

                    StatusButton.Background = Brushes.Green;
                    StatusButton.Content = "Connected!";
                    
                });
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

            public TcpClient client = null;

            public TcpListener server = null;



            public Int32 port = 1000;

            public IPAddress localAddr = IPAddress.Parse("192.168.0.54");

            public Byte[] bytes = new byte[256];

            public String data = null;



            


            public tcpClass(string role, string client_ip_and_port)
            {
                this.role = role;
                this.client_ip_and_port = client_ip_and_port;
                this.server_port = server_port;

                string[] ip_port_elements = client_ip_and_port.Split(',');
                client_ip = ip_port_elements[0];

                IPAddress iPAddress = IPAddress.Parse(ip_port_elements[0]);

                client_port = ip_port_elements[1];

                Console.WriteLine("client_ip: " + client_ip + " client_port: " + client_port);


            }


            ~tcpClass() { }



            //  methods
            public async void initiate_connection_tcp_server()
            {

                //UIControl.ChangeButtonName("Updated from another CLASS.");

                //Int32 port = 1000;
                port = 1000;
                //IPAddress localAddr = IPAddress.Parse("192.168.0.54");
                localAddr = IPAddress.Parse("192.168.0.54");

                server = new TcpListener(localAddr, port);

                server.Start();
                
                bytes = new byte[256];


                Console.WriteLine(" Waiting for a connection.");

                //  Perform a blocking call to accept requests.
                //  You can also use server. AcceptSocket() here.
                //tcpClient client = server.AcceptTcpClient();
                client = server.AcceptTcpClient();


                TB_local.Text = "Initializing...";

                Console.WriteLine("socket Is Connected!");

                StatusButton.Background = Brushes.Green;
                StatusButton.Content = "Connected!";

                Console.WriteLine("StatusButton should have updated to GREEN by now.");

                String data = null;



                return;
            }




            public bool initiate_connection_tcp_client()
            {

                System.Net.IPAddress ip = System.Net.IPAddress.Parse(client_ip);

                System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, Convert.ToInt32(client_port));

                client = new TcpClient(client_ip, Convert.ToInt32(client_port));

                StatusButton.Background = Brushes.Green;

                StatusButton.Content = "Connected!";

                Console.WriteLine("socket Is Connected!");

                Console.WriteLine("StatusButton should have updated to GREEN by now.");

                return true;
            }

            public void send_message()
            {


                Console.WriteLine(" tcp_client - inside tcpClass->send_msg");

                NetworkStream stream = client.GetStream();

                //var message = "Hi friends 👋!<|EOM|> ";

                string message = null;

                Application.Current.Dispatcher.Invoke(() => {

                    message = TextBox_Msg.Text;

                });
                



                Byte[] data = Encoding.UTF8.GetBytes(message);

                stream.Write(data, 0, data.Length); 

                Console.WriteLine(" Just sent msg: " + message);

                return;
            }



            public void receive_message( )
            {

                Console.WriteLine(" tcp_server - inside tcpClass->receive_msg");

                String data = null;

                //  Get a stream object for reading and writing
                //NetworkStream stream = client.GetStream();
                NetworkStream stream = client.GetStream();

                int i;

                //  Loop to receive all the data send by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //  Translate data bytes to ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(" Received: {0}", data);

                    //  Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);


                    //  note: there is a particular reason why this works yet other methods fail. Need to understand why this is the case. 
                    Application.Current.Dispatcher.Invoke(() => {
                        
                        TB_local.AppendText(data + Environment.NewLine);
                        
                    });
                    
                }

                return;
            }


        }



        //  this works. Need to understand why, versus all the other options that fail 
        //Application.Current.Dispatcher.Invoke( () =>  {
            // Code to run on the GUI thread.
        //});




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

            initiate_tcp();

        }

        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {

            if ( tcp_client == true )
            {

                Console.WriteLine(" tcp_client - initiating tcpClass->send_message()");

                t1 = new Thread(tcpInstance.send_message);

                t1.Start();

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

        private void Button_Click_tcp_server_begin_listening(object sender, RoutedEventArgs e)
        {


            Console.WriteLine(" tcp_server - initiating tcpClass->receive_message()");

            t2 = new Thread(tcpInstance.receive_message);

            t2.Start();


        }

        private void Button_Click_Exit_Application(object sender, RoutedEventArgs e)
        {

            System.Windows.Application.Current.Shutdown();

        }

        private void TextBox_messages_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Clear_Log_Click(object sender, RoutedEventArgs e)
        {


            TB_local.Clear();

            //Application.Current.Dispatcher.Invoke(() => {

                //TB_local.AppendText(data + Environment.NewLine);

            //});

        }
    }
}




