using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class FormServer : Form
    {
        IPEndPoint ipep;
        Socket newsock;
        Socket client;
        int recv;
        IPEndPoint clientep;
        byte[] data = new byte[1024];
        string welcome;
        string mess = "Server: ";
        string input;
        int flag = 1;
        string stringData;
        int connections = 0;
        public FormServer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {       
            try
            {
                ipep = new IPEndPoint(IPAddress.Any, 9999);
                newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                newsock.Bind(ipep);
                newsock.Listen(10);
                while (true) {
                    if (newsock.Poll(1000000, SelectMode.SelectRead))
                    {
                        Thread newthread = new Thread(new ThreadStart(HandleConnection));
                        newthread.Start();
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void HandleConnection()
        {
            client = newsock.AcceptTcpClient();

            Thread recei = new Thread(new ThreadStart(ReceiveData));
            recei.Start();
            int recv; byte[] data = new byte[1024];
            NetworkStream ns = new NetworkStream(client);
            connections++;
            //listBoxserver.Items.Add("New client accepted: "+ connections+ "active connections");
            if (listBoxserver.InvokeRequired)
            {
                listBoxserver.Invoke(new MethodInvoker(delegate {
                    listBoxserver.Items.Add("New client accepted: " + connections + "active connections");
                }));
            }
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            ns.Write(data, 0, data.Length);
            while (true)
            {
                data = new byte[1024];
                recv = ns.Read(data, 0, data.Length);
                if (recv == 0)
                    break;

                ns.Write(data, 0, recv);
            }
            ns.Close();
            client.Close();
            connections--;
            //Console.WriteLine("Client disconnected: {0} active connections", connections);
        }
        private void AcceptClient(IAsyncResult i)
        {
            client = ((Socket)i.AsyncState).EndAccept(i);
        }

        private void SendData()
        {
            input = textBox1.Text;
            //Socket client = (Socket)iar.AsyncState;
            //int sent = client.EndSend(iar);
            
            if (listBoxserver.InvokeRequired)
            {
                listBoxserver.Invoke(new MethodInvoker(delegate {
                    listBoxserver.Items.Add("server: " + input);
                }));
            }

        }

        private void ReceiveData()
        {
            //.string rec = Encoding.ASCII.GetString(data, 0, ((Socket)i.AsyncState).EndReceive(i));
            while (true)
            {
                if (listBoxserver.InvokeRequired)
                {
                    listBoxserver.Invoke(new MethodInvoker(delegate
                    {
                        //listBoxserver.Items.Add("client: " + rec);
                    }));
                }
            }
        }
        private void buttonsend_Click(object sender, EventArgs e)
        {
            input = textBox1.Text;
            textBox1.Text = "";
            data = Encoding.ASCII.GetBytes(input);
            //client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            data = new byte[1024];
            //client.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), client);
            
        }
        private void buttonexit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
