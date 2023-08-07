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
using System.Net.Http;
//using static System.Net.Mime.MediaTypeNames;
using System.Timers;


using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Resources;
using System.Windows.Markup;
using Application = System.Windows.Application;
using System.Reflection.Emit;
using System.Web.UI.WebControls;






namespace Chat
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // https://stackoverflow.com/questions/33680398/c-sharp-wpf-how-to-simply-update-ui-from-another-class-thread
        public static System.Windows.Controls.Button StatusButton;
        public static System.Windows.Controls.TextBox IP_and_port_string;
        public static System.Windows.Controls.Label label_public_IP_and_port_string;
        //public static System.Windows.Controls.Label label_private_IP_and_port_string;
        public static System.Windows.Controls.TextBlock TextBlock_private_IP_and_port_string;
        public static System.Windows.Controls.TextBox TextBox_Msg;
        public static RichTextBox RTB_local;


        //  booleans for tracking the role specified by the user
        //public bool tcp;
        public bool tcp_client;

        public bool tcp_server;


        public TcpListener server = null;

        public static Socket client = null;

        public tcpClass tcpInstance = null;


        //  the following are to run send_message() and receive_message() at the same time, for bi-directional messaging
        public static System.Threading.Thread t1;

        public System.Threading.Thread t2;

        public System.Timers.Timer heartbeatTimer = null;

        public bool CxnStatus = false; 


        //  this is the intitial message in TextBox
        // "&quot;Hi friends 👋!&lt;|EOM|&gt;&quot;"

        public MainWindow()
        {

            InitializeComponent();

            this.Title = "  Chat v1.0"; 

            //  the following to make the UI controls accessible/updateable 
            StatusButton = Button_CxnState;

            IP_and_port_string = tcp_client_ip_addr_port_string;

            label_public_IP_and_port_string = Label_public_ip_and_port;

            //label_private_IP_and_port_string = Label_private_ip_and_port;

            TextBlock_private_IP_and_port_string = TextBlock_private_ip_and_port;


            TextBox_Msg = TextBox_enterMessage;

            RTB_local = RichTextBox_Messages;


            
            //  DispatcherTimer setup for checking state of tcp connection 
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Start();

            /*  skipping this for the DispatcherTimer instead, but leaving stub code here for now...
            //  heartbeat timer event handler to ensure client remains connected (or detect that it is no longer connected) 
            System.Timers.Timer heartbeatTimer = new System.Timers.Timer(5000);
            //heartbeatTimer.Interval = 5000; //5 seconds
            heartbeatTimer.Elapsed += heartbeatTimer_Elapsed;
            heartbeatTimer.AutoReset = true;
            heartbeatTimer.Enabled = true;
            heartbeatTimer.Start();
            */


            get_and_display_public_ip();

            List<string> localIP = GetLocalIPAddress();

            string string_for_label = "Your Available Private IP Addresses: ";

            foreach ( string IP in localIP )
            {
                Console.WriteLine(" localIP: " + IP.ToString());

                string_for_label = string_for_label + " " + IP.ToString();
            }

            
            //label_private_IP_and_port_string.Content = string_for_label;
            TextBlock_private_ip_and_port.Text = string_for_label;

        }


        static async Task get_and_display_public_ip()
        {


            HttpClient sharedClient = new HttpClient();

            HttpResponseMessage response = await sharedClient.GetAsync("https://api.ipify.org");

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);

            Console.WriteLine(responseBody);

            IP_and_port_string.Text = responseBody + ",1000";

            label_public_IP_and_port_string.Content = "Your Public IP Address: " + responseBody;


        }

        public List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            List<string> tempList = new List<string>();

            foreach (var ip in host.AddressList)
            {

                Console.WriteLine(" current IP: " + ip);
                
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine(" Adding IP: " + ip.ToString() + " to List<string>");
                    tempList.Add(ip.ToString());
                }

            }
            //throw new Exception("No network adapters with an IPv4 address in the system!");

            return tempList;
        }






        //  This is Program Entry Point based on Button Click to establish tcp connection 
        public void initiate_tcp()
        {

            //  collect config settings from UI to determine which of server/client to run...
            //if (Radio_tcp.IsChecked == true && Checkbox_tcp_client.IsChecked == true)
            if (Checkbox_tcp_client.IsChecked == true)
            {
                //tcp = true;
                tcp_client = true;
            }
            //else if (Radio_tcp.IsChecked == true && Checkbox_tcp_server.IsChecked == true)
            else if (Checkbox_tcp_server.IsChecked == true)
            {
                //tcp = true;
                tcp_server = true;
            }
            
            //  now execute based on user-defined tcp role 
            //if (tcp == true && tcp_client == true)
            if (tcp_client == true)
            {
                tcpInstance = new tcpClass("client", tcp_client_ip_addr_port_string.Text.ToString());

                tcpInstance.initiate_connection_tcp_client();

            }
            //else if (tcp == true && tcp_server == true)
            else if (tcp_server == true)
            {
                tcpInstance = new tcpClass("server", tcp_client_ip_addr_port_string.Text.ToString());

                tcpInstance.initiate_connection_tcp_server();

            }


        }










        //  this works. Need to understand why, versus all the other options that fail 
        //Application.Current.Dispatcher.Invoke( () =>  {
            // Code to run on the GUI thread.
        //});



















        //  EVENT HANDLERS



        
        //  the following to check the state of the tcp unicast connection every five seconds
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            Console.WriteLine(Environment.NewLine + DateTime.Now + " Firing dispatcherTimer");

            bool result = false;

            if (tcpInstance != null) {

                result = tcpInstance.checkConnection();

                Console.WriteLine(Environment.NewLine + DateTime.Now + " tcp Connection Status: " + result);


                if ( result == false )
                {

                    StatusButton.Background = Brushes.Red;

                    StatusButton.Content = "Disconnected!";

                    Console.WriteLine("socket Disconnected!");

                    Console.WriteLine("StatusButton should have updated to RED.");

                }
                
                try
                {
                    tcpInstance.send_HB_message();
                }
                catch( Exception f)
                {
                    Console.WriteLine(f.Message);
                }

            }

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
        





        /*
        //  The following for sending/receiving heartbeat messages between tcp client and tcp server 
        void heartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (tcp_server == true)
            {
                try
                {
                    tcpInstance.send_HB_message();

                }
                catch (Exception f)
                {

                    Console.WriteLine(f.Message); //Check what is the exception

                    //Fa
                    //il to send message - client disconnected.
                    //Invoke((MethodInvoker)delegate //prevent cross-thread exception
                    //{

                    //    ClientIPLabel.Text = "(No Clients Connected)";

                    //});

                    Console.WriteLine(Environment.NewLine + DateTime.Now + " heartbeatTimer.Stop()");


                    System.Timers.Timer timer = (System.Timers.Timer)sender; // Get the timer that fired the event

                    timer.Stop(); // Stop the timer that fired the event


                    

                    //heartbeatTimer.AutoReset = false;

                    //client.Disconnect();

                    //server.Stop();



                }

            }

        }
        */


        private void Button_Click_Intitiate_tcp(object sender, RoutedEventArgs e)
        {

            Console.WriteLine(" launching App");
            
            initiate_tcp();

            Console.WriteLine(" initiating tcpClass->receive_message()");

            t2 = new Thread(this.tcpInstance.receive_message);

            t2.Start();

        }


        private void Button_Click_Send(object sender, RoutedEventArgs e)
        {

            //if ( tcp_client == true )
            //{

                Console.WriteLine(" tcp_client - initiating tcpClass->send_message()");

                //t1 = new Thread(tcpInstance.send_message);

                //t1.Start();



                tcpInstance.send_message();


                TextBox_enterMessage.Clear();

                Keyboard.Focus(TextBox_enterMessage);

                //t2 = new Thread(tcpInstance.receive_message);

                //t2.Start();



            //}

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
       

        private void Button_Click_Exit_Application(object sender, RoutedEventArgs e)
        {

            if ( tcp_client == true)
            {

                tcpInstance.close_tcp_client_connection();

                System.Windows.Application.Current.Shutdown();

            }

            else if ( tcp_server == true )
            {


                tcpInstance.close_tcp_server_connection();
                
                System.Windows.Application.Current.Shutdown();

            }

            else
            {
                System.Windows.Application.Current.Shutdown();

            }



        }

       

        private void Button_Clear_Log_Click(object sender, RoutedEventArgs e)
        {


            //TB_local.Clear();

            RTB_local.Document.Blocks.Clear();

            //Application.Current.Dispatcher.Invoke(() => {

            //TB_local.AppendText(data + Environment.NewLine);

            //});

        }

        private void RichTextBox_Messages_TextChanged(object sender, TextChangedEventArgs e)
        {




        }

    }

}




