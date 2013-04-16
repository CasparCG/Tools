using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bespoke.Common.Osc;
using System.Net;
using System.Collections;

namespace OSCMonitor
{
    public partial class Form1 : Form
    {
        OscServer osc_server;
        String ipaddress;
        ArrayList oscmessages = new ArrayList();
        

        public Form1()
        {
            InitializeComponent();
            osc_server = new OscServer(Bespoke.Common.Net.TransportType.Tcp, IPAddress.Parse("127.0.0.1"), 5253);
            osc_server.MessageReceived += new OscMessageReceivedHandler(osc_server_MessageReceived);
            
            //Need to build in Connected/Disconnected/FailedConnect Event Handlers
            osc_server.FilterRegisteredMethods = false;

            dataGridView1.ColumnCount = 3;
           
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (osc_server.IsRunning)
            {
                osc_server.Stop();
                ipaddress = osc_server.IPAddress.ToString();
                toolStripStatusLabel1.Text = "Not Connected: " + ipaddress;
            }
            else {
                osc_server.Start();
                toolStripStatusLabel1.Text = "Connected: " + ipaddress;
            }

        }

        void osc_server_MessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            //make sure "value" doesnt already exist
            if (!(oscmessages.Contains(e.Message.Address)))
            {
                //since we've made it this far we can add it
                oscmessages.Add(e.Message.Address);

                switch (e.Message.Data.Length)
                {
                    case 1:
                        AddDataGridRow(new string[] { e.Message.Address, e.Message.Data[0].ToString() });
                        break;
                    case 2:
                        AddDataGridRow(new string[] { e.Message.Address, e.Message.Data[0].ToString(), e.Message.Data[1].ToString() });
                        break;
                    default:
                        break;
                }
             }
            else
            {
                UpdateDataGridRow(e.Message.Address, e.Message.Data); 
            }

        }

        delegate void AddDataGridDel(String[] info);
        public void AddDataGridRow(String[] info)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                dataGridView1.Rows.Add(info);
            }));
        }

        delegate void UpdateDataGridDel(String message, object[] data);
        public void UpdateDataGridRow(String message, object[] data)
        {
            int i;
           
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((String)dataGridView1.Rows[i].Cells[0].Value == message)
                {

                    this.Invoke(new MethodInvoker(delegate()
                    {
                        switch (data.Length)
                        {
                            case 1:
                                dataGridView1.Rows[i].Cells[1].Value = data[0].ToString();
                                break;
                            case 2:
                                dataGridView1.Rows[i].Cells[1].Value = data[0].ToString();
                                dataGridView1.Rows[i].Cells[2].Value = data[1].ToString();
                                break;
                            default:
                                break;
                        }
                    }));
                                       
                }
            }
                
           
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
           
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.Width = Form1.ActiveForm.Width;
            dataGridView1.Height = Form1.ActiveForm.Height + 47;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
    }
}
