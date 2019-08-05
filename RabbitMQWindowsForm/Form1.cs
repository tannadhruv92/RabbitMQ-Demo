using RabbitMQClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabbitMQWindowsForm
{
    public partial class Form1 : Form
    {
        static RabbitMQ rabbitMQ = null;

        public int WithoutRefresh
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["RedisKey"]);
            }
        }

        public int WithRefresh
        {
            get
            {
                ConfigurationManager.RefreshSection("appSettings");
                return Convert.ToInt32(ConfigurationManager.AppSettings["RedisKey"]);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rabbitMQ = new RabbitMQ();
            rabbitMQ.OnConnectionBlocked(onConnectionBlcked);
            rabbitMQ.OnConnectionShutDown(onConnectionBlcked);
            rabbitMQ.channel = rabbitMQ.GetChannel();
            rabbitMQ.ReciveMessage("Dhruv", rabbitMQ.channel, OnMessageRecived);
        }

        private static void onConnectionBlcked(object arg1, object arg2)
        {
            MessageBox.Show(Newtonsoft.Json.JsonConvert.SerializeObject(arg2));
        }

        private static void OnMessageRecived(string obj, ulong deliveryTag)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() => ShowPopUpAndAckMessage(obj, deliveryTag));
            }
            catch (Exception)
            {

            }
        }

        private static void ShowPopUpAndAckMessage(string obj, ulong deliveryTag)
        {
            MessageBox.Show(obj);
            rabbitMQ.AckMessage(rabbitMQ.channel, deliveryTag);
        }

        private void Button1_Click(object sender, EventArgs e) => MessageBox.Show(WithoutRefresh.ToString());

        private void Button2_Click(object sender, EventArgs e) => MessageBox.Show(WithRefresh.ToString());
    }
}
