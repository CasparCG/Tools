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
using System.Xml.Linq;

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
            var extraTypes = new Type[2]{typeof(decklinkConsumer), typeof(AbstractConsumer)};

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using(var writer = doc.CreateWriter())
            {
                new XmlSerializer(typeof(configuration), extraTypes).Serialize(writer, config, namespaces);
            }

            doc.Element("configuration").Add(
                new XElement("controllers",
                    new XElement("tcp",
                        new XElement[2]
                        {
                            new XElement("port", 5220),
                            new XElement("protocol", "AMCP")
                        })));

            using (var writer = new XmlTextWriter("casparcg.config", new UTF8Encoding(false, false))) // No BOM
            {
                doc.Save(writer);
            }
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
                else if (listBox2.SelectedItem.GetType() == typeof(bluefishConsumer))
                {
                    consumereditorcontrol = new bluefishConsumerControl((bluefishConsumer)listBox2.SelectedItem);
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

        private void button1_Click(object sender, EventArgs e)
        {
            var d = new systemaudioConsumer();

            ((BindingList<AbstractConsumer>)listBox2.DataSource).Add(d);
            refreshconsumerpanel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var d = new bluefishConsumer();

            ((BindingList<AbstractConsumer>)listBox2.DataSource).Add(d);
            refreshconsumerpanel();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                datapathTextBox.Text = fd.SelectedPath;
            }
            fd.Dispose();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                logpathTextBox.Text = fd.SelectedPath;
            }
            fd.Dispose();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mediapathTextBox.Text = fd.SelectedPath;
            }
            fd.Dispose();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                templatepathTextBox.Text = fd.SelectedPath;
            }
            fd.Dispose();
        }
    }
}
