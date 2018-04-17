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


namespace Buoi2_client
{
    public partial class FormClient : Form
    {
        byte[] data = new byte[1024];
        string input, stringData;
        IPEndPoint ipep;
        Socket server;
        int recv;
        string Data;
        string mess = "client: ";

        public FormClient()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void buttonexit_Click(object sender, EventArgs e)
        {
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            this.Close();
        }

        

        private void buttonketnoi_Click(object sender, EventArgs e)
        {
            ipep = new IPEndPoint(IPAddress.Parse(textBox2.Text), 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            //server.BeginConnect(ipep, new AsyncCallback(Connectserver), server);
        }

        public void Connectserver (IAsyncResult I)
        {
            Socket sock = ((Socket)I.AsyncState);
            sock.EndConnect(I);
            
           
        }

        private void SendData(IAsyncResult iar)
        {
            input = textBox1.Text;
            Socket server = (Socket)iar.AsyncState;
            int sent = server.EndSend(iar);
            
            if (listBoxclient.InvokeRequired)
            {
                listBoxclient.Invoke(new MethodInvoker(delegate {
                    listBoxclient.Items.Add("client: " + input);
                }));
                
            }
        }

        private void ReceiveData(IAsyncResult i)
        {
            string rec = Encoding.ASCII.GetString(data, 0, ((Socket)i.AsyncState).EndReceive(i));
            if (listBoxclient.InvokeRequired)
            {
                listBoxclient.Invoke(new MethodInvoker(delegate {
                    listBoxclient.Items.Add("server: "+ rec);
                }));
            }
        }

        private void buttonsend_Click(object sender, EventArgs e)
        {
            input = textBox1.Text;
            data = new byte[1024];
            data = Encoding.ASCII.GetBytes(input);
            server.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), server);
            data = new byte[1024];
            server.BeginReceive(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ReceiveData), server);


        }

    }
}
