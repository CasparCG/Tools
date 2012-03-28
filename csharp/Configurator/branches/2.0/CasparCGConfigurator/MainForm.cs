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
        public configuration config = new configuration();
        private ConsumerControlBase consumerEditorControl;
        private AbstractConsumer lastConsumer;
        public List<String> availableDecklinkIDs = new List<string>();
        
        public MainForm()
        {
            this.InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("casparcg.config"))
                DeSerializeConfig(System.IO.File.ReadAllText("casparcg.config"));
            else
                System.Windows.Forms.MessageBox.Show("A 'casparcg.config' file was not found in the same directory as this application.  One is now being generated.","CasparCG Configurator",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                SerializeConfig();
            this.WireBindings();
            this.Updatechannel();
            this.SetToolTips();
        }

        private void WireBindings()
        {
            this.pathsBindingSource.DataSource = this.config.Paths;
            this.flashBindingSource.DataSource = this.config.Flash;
            this.configurationBindingSource.DataSource = this.config;
            this.listBox1.DataSource = this.config.Channels;           
        }

        private void SerializeConfig()
        {
            var extraTypes = new Type[1]{typeof(AbstractConsumer)};

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
                            new XElement("port", 5250),
                            new XElement("protocol", "AMCP")
                        })));

            doc.Add(new XComment(CasparCGConfigurator.Properties.Resources.configdoc.ToString()));

            using (var writer = new XmlTextWriter("casparcg.config", new UTF8Encoding(false, false))) // No BOM
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
        }
        
        private void DeSerializeConfig(string text)
        {
            var x = new XmlSerializer(typeof(configuration));

            using (var reader = new StringReader(text))
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                try
                {
                    this.config = (configuration)x.Deserialize(reader);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("There was an error reading the current 'casparcg.config' file.  A new one will be generated.","CasparCG Configurator", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    this.config = new configuration();
                }
            }
            this.WireBindings();
        }

        private void RefreshConsumerPanel()
        {
            if (lastConsumer != listBox2.SelectedItem)
            {

                this.panel1.Controls.Clear();
                if (consumerEditorControl != null)
                    consumerEditorControl.Dispose();

                this.consumerEditorControl = null;

                if (listBox2.SelectedItems.Count > 0)
                {
                    if (listBox2.SelectedItem.GetType() == typeof(DecklinkConsumer))
                    {
                        this.consumerEditorControl = new DecklinkConsumerControl(listBox2.SelectedItem as DecklinkConsumer,config.AvailableDecklinkIDs);
                        this.panel1.Controls.Add(consumerEditorControl);
                    }
                    else if (listBox2.SelectedItem.GetType() == typeof(ScreenConsumer))
                    {
                        this.consumerEditorControl = new ScreenConsumerControl(listBox2.SelectedItem as ScreenConsumer);
                        this.panel1.Controls.Add(consumerEditorControl);
                    }
                    else if (listBox2.SelectedItem.GetType() == typeof(BluefishConsumer))
                    {
                        this.consumerEditorControl = new BluefishConsumerControl(listBox2.SelectedItem as BluefishConsumer,config.AvailableBluefishIDs);
                        this.panel1.Controls.Add(consumerEditorControl);
                    }
                }
            }
            lastConsumer = (AbstractConsumer)listBox2.SelectedItem;
        }

        private void Updatechannel()
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                this.comboBox1.Enabled = true;
                this.listBox2.Enabled = true;
                this.button4.Enabled = true;
                this.button5.Enabled = true;
                this.button7.Enabled = true;
                this.button1.Enabled = true;
                this.button2.Enabled = true;
                this.listBox2.DataSource = ((Channel)listBox1.SelectedItem).Consumers;
                this.comboBox1.SelectedItem = ((Channel)listBox1.SelectedItem).VideoMode;
            }
            else
            {
                this.comboBox1.Enabled = false;
                this.listBox2.Enabled = false;
                this.button4.Enabled = false;
                this.button5.Enabled = false;
                this.button7.Enabled = false;
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.listBox2.DataSource = null;
                this.comboBox1.SelectedItem = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.config.Channels.AddNew();
            this.Updatechannel();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Updatechannel();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            (listBox2.DataSource as BindingList<AbstractConsumer>).Add(new DecklinkConsumer(config.AvailableDecklinkIDs));

            RefreshConsumerPanel();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            (listBox2.DataSource as BindingList<AbstractConsumer>).Add(new ScreenConsumer());
            this.RefreshConsumerPanel();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)            
                (listBox1.SelectedItem as Channel).VideoMode = comboBox1.SelectedItem.ToString();            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)            
                this.config.Channels.Remove((Channel)listBox1.SelectedItem);
            
            this.Updatechannel();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count > 0)            
                (listBox1.SelectedItem as Channel).Consumers.Remove(listBox2.SelectedItem as AbstractConsumer);
            
            this.RefreshConsumerPanel();
        }

        private void showXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SerializeConfig();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshConsumerPanel();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var res = System.Windows.Forms.MessageBox.Show("Do you want to save this configuration before exiting?", "CasparCG Configurator", MessageBoxButtons.YesNoCancel);
            if (res == System.Windows.Forms.DialogResult.Yes || res == System.Windows.Forms.DialogResult.OK)
                SerializeConfig();
            //else if(res == System.Windows.Forms.DialogResult.No)           
            else if(res == System.Windows.Forms.DialogResult.Cancel)            
                e.Cancel = true; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (listBox2.DataSource as BindingList<AbstractConsumer>).Add(new SystemAudioConsumer());
            RefreshConsumerPanel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (listBox2.DataSource as BindingList<AbstractConsumer>).Add(new BluefishConsumer(config.AvailableBluefishIDs));
            RefreshConsumerPanel();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            setTextboxFilepath(datapathTextBox);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            setTextboxFilepath(logpathTextBox);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            setTextboxFilepath(mediapathTextBox);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            setTextboxFilepath(templatepathTextBox);
        }

        private void setTextboxFilepath(TextBox control)
        {
            using (var fd = new FolderBrowserDialog())
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var p = fd.SelectedPath;
                    control.Text = fd.SelectedPath + (p.EndsWith("\\") ? "" : "\\");
                }
            }
        }

        private void SetToolTips()
        {
            var toolTip = new ToolTip();
            //toolTip.SetToolTip(this.##CONTROL##, "##Tooltip text##");
            toolTip.SetToolTip(this.pipelineTokensComboBox, "This will set the mixer buffer depth.");
        }

    }
}
