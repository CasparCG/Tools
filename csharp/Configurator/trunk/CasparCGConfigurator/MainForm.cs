using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace CasparCGConfigurator
{
    public partial class MainForm : Form
    {
        public configuration config;
        private ConsumerControlBase consumereditorcontrol;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            config = new configuration();
            loadOrCreateConfigFile();
            wirebindings();
            updatechannel();
        }

        private void loadOrCreateConfigFile()
        {
            if(System.IO.File.Exists("casparcg.config"))
            {
                DeSerializeConfig(System.IO.File.ReadAllText("casparcg.config"));
            }else{
                SerializeConfig();
            }
        }

        private void wirebindings()
        {
            pathsBindingSource.DataSource = config.paths;
            configurationBindingSource.DataSource = config;
            listBox1.DataSource = config.channels;
           
        }


        private void SerializeConfig()
        {
            var extraTypes = new Type[2];
            extraTypes[0] = typeof(decklinkConsumer);
            extraTypes[1] = typeof(AbstractConsumer);

            var x = new XmlSerializer(typeof(configuration), extraTypes);

            var st = new StringWriter();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            x.Serialize(st, config, namespaces);
            var doc = new XmlDocument();
            doc.LoadXml(st.ToString());

            var xc = doc.SelectSingleNode("configuration");
            var xe = doc.CreateElement("controllers");
            xe.InnerXml = "<tcp><port>5250</port><protocol>AMCP</protocol></tcp>";
            xc.AppendChild(xe);

            doc.Save("casparcg.config");

        }


        private void DeSerializeConfig(string text)
        {
            var x = new XmlSerializer(typeof(configuration));

            var reader = new StringReader(text);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            config = (configuration)x.Deserialize(reader);
            wirebindings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            config.channels.AddNew();
            updatechannel();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatechannel();
        }

        private void updatechannel()
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                comboBox1.Enabled = true;
                listBox2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button7.Enabled = true;
                listBox2.DataSource = ((channel)listBox1.SelectedItem).consumers;
                comboBox1.SelectedItem = ((channel)listBox1.SelectedItem).videomode;

            }
            else
            {
                comboBox1.Enabled = false;
                listBox2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button7.Enabled = false;
                listBox2.DataSource = null;
                comboBox1.SelectedItem = null;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var d = new decklinkConsumer();
            
            ((BindingList<AbstractConsumer>)listBox2.DataSource).Add(d);

            refreshconsumerpanel();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var d = new screenConsumer();

            ((BindingList<AbstractConsumer>)listBox2.DataSource).Add(d);
            refreshconsumerpanel();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                ((channel)listBox1.SelectedItem).videomode = comboBox1.SelectedItem.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                config.channels.Remove((channel)listBox1.SelectedItem);
            }
            updatechannel();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count > 0)
            {
                ((channel)listBox1.SelectedItem).consumers.Remove((AbstractConsumer)listBox2.SelectedItem);
            }
            refreshconsumerpanel();
        }

        private void showXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerializeConfig();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshconsumerpanel();
        }

        private void refreshconsumerpanel()
        {
            if (consumereditorcontrol != null)
            {
                consumereditorcontrol.Dispose();
            }
            panel1.Controls.Clear();
            consumereditorcontrol = null;

            if (listBox2.SelectedItems.Count > 0)
            {
                if (listBox2.SelectedItem.GetType() == typeof(decklinkConsumer))
                {
                    consumereditorcontrol = new decklinkConsumerControl((decklinkConsumer)listBox2.SelectedItem);
                    panel1.Controls.Add(consumereditorcontrol);
                }
                else if (listBox2.SelectedItem.GetType() == typeof(screenConsumer))
                {
                    consumereditorcontrol = new screenConsumerControl((screenConsumer)listBox2.SelectedItem);
                    panel1.Controls.Add(consumereditorcontrol);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var res = System.Windows.Forms.MessageBox.Show("Do you want to save this configuration before exiting?", "CasparCG Configurator", MessageBoxButtons.YesNoCancel);
            if (res == System.Windows.Forms.DialogResult.Yes || res == System.Windows.Forms.DialogResult.OK)
            {
                SerializeConfig();
            }else if(res == System.Windows.Forms.DialogResult.No)
            {
            }else if(res == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true ;
            }

        }
    }
}
