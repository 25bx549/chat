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
using System.Timers;


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
        public static TextBox IP_and_port_string;
        public static TextBox TextBox_Msg;
        public static RichTextBox RTB_local;


        //  booleans for tracking the role specified by the user
        public bool tcp;
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

            TextBox_Msg = TextBox_enterMessage;

            RTB_local = RichTextBox_Messages;


            
            //  DispatcherTimer setup for checking state of tcp connection 
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Start();
            


            /*
            //  heartbeat timer event handler to ensure client remains connected (or detect that it is no longer connected) 
            System.Timers.Timer heartbeatTimer = new System.Timers.Timer(5000);
            //heartbeatTimer.Interval = 5000; //5 seconds
            heartbeatTimer.Elapsed += heartbeatTimer_Elapsed;
            heartbeatTimer.AutoReset = true;
            heartbeatTimer.Enabled = true;
            heartbeatTimer.Start();
            */


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


            ~tcpClass() { }



            //  methods
            //public async void initiate_connection_tcp_server()
            public void initiate_connection_tcp_server()
            {

                server = new TcpListener( IPAddress.Parse(client_ip) , Convert.ToInt32(client_port) );

                server.Start();
                
                bytes = new byte[256];

                Console.WriteLine(" Waiting for a connection (blocking call to server.AcceptTcpClient().");

                //  Perform a blocking call to accept requests.
                //  You can also use server. AcceptSocket() here.
                client = server.AcceptTcpClient();

                Console.WriteLine(Environment.NewLine + " socket Is Connected!");

                StatusButton.Background = Brushes.Green;

                StatusButton.Content = "Connected!";

                Console.WriteLine( Environment.NewLine + " StatusButton should have updated to GREEN by now.");

                String data = null;


                
                

                


                //    t1 = new Thread( runDispatcherTimer );

                //    t1.Start();

                return;
            }


            //public void runDispatcherTimer()
            //{

                
                

            //}

            public bool checkConnection() { 


              bool isConnected = true;

               if  (client.Connected == true)
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

                client = new TcpClient( client_ip, Convert.ToInt32(client_port));

                StatusButton.Background = Brushes.Green;

                StatusButton.Content = "Connected!";

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

                //var message = "Hi friends 👋!<|EOM|> ";

                string message = null;

                Application.Current.Dispatcher.Invoke(() => {
                    
                    message = TextBox_Msg.Text;
                    message = message.TrimEnd(new char[] { '\r', '\n' });

                });


                Byte[] data = Encoding.UTF8.GetBytes(message);



                stream.Write(data, 0, data.Length); 

                Console.WriteLine(" Just sent msg: " + message);
                

                //  note: there is a particular reason why this works yet other methods fail. Need to understand why this is the case. 
                Application.Current.Dispatcher.Invoke(() => {

                    
                    //FlowDocument myFlowDoc = new FlowDocument();
                    //Run myRun = new Run("This is a new line");
                    message = " Sent msg: " + message;
                    message = message.TrimEnd(new char[] { 'r', '\n' });
                    Run myRun = new Run( message );
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
                    //myFlowDoc.Blocks.Append(myParagraph);

                    RTB_local.Document = myFlowDoc;



                    //RTB_local.AppendText(Environment.NewLine + "Msg Sent: " + message  );

                    //RTB_local.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightGreen );







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
                    data = data.TrimEnd(new char[] { '\r', '\n' });
                    data = " Recv'd msg: " + data;
                    data = data.TrimEnd(new char[] { 'r', '\n' });
                    Console.WriteLine( data);

                    //  Process the data sent by the client.
                    //data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    if (data.Contains("HB") != true)
                    {
                        //  note: there is a particular reason why this works yet other methods fail. Need to understand why this is the case. 
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
                            //myFlowDoc.Blocks.Append(myParagraph);



                            RTB_local.Document = myFlowDoc;




                        });
                    }
                    
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

        private void Button_Click_tcp_server_begin_listening(object sender, RoutedEventArgs e)
        {


            Console.WriteLine(" tcp_server - initiating tcpClass->receive_message()");


            //t1 = new Thread(tcpInstance.send_message);

            //t1.Start();


            t2 = new Thread(this.tcpInstance.receive_message);

            t2.Start();






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




