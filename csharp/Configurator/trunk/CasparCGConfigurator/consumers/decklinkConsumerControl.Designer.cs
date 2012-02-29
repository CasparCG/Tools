namespace CasparCGConfigurator
{
    partial class decklinkConsumerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label bufferdepthLabel;
            System.Windows.Forms.Label deviceLabel;
            System.Windows.Forms.Label embeddedaudioLabel;
            System.Windows.Forms.Label keyLabel;
            System.Windows.Forms.Label keyonlyLabel;
            System.Windows.Forms.Label latencyLabel;
            this.bufferdepthTextBox = new System.Windows.Forms.TextBox();
            this.decklinkConsumerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.deviceTextBox = new System.Windows.Forms.TextBox();
            this.embeddedaudioCheckBox = new System.Windows.Forms.CheckBox();
            this.keyonlyCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            bufferdepthLabel = new System.Windows.Forms.Label();
            deviceLabel = new System.Windows.Forms.Label();
            embeddedaudioLabel = new System.Windows.Forms.Label();
            keyLabel = new System.Windows.Forms.Label();
            keyonlyLabel = new System.Windows.Forms.Label();
            latencyLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.decklinkConsumerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // bufferdepthLabel
            // 
            bufferdepthLabel.AutoSize = true;
            bufferdepthLabel.Location = new System.Drawing.Point(13, 12);
            bufferdepthLabel.Name = "bufferdepthLabel";
            bufferdepthLabel.Size = new System.Drawing.Size(70, 13);
            bufferdepthLabel.TabIndex = 1;
            bufferdepthLabel.Text = "Buffer Depth:";
            // 
            // deviceLabel
            // 
            deviceLabel.AutoSize = true;
            deviceLabel.Location = new System.Drawing.Point(13, 38);
            deviceLabel.Name = "deviceLabel";
            deviceLabel.Size = new System.Drawing.Size(84, 13);
            deviceLabel.TabIndex = 3;
            deviceLabel.Text = "Device Number:";
            // 
            // embeddedaudioLabel
            // 
            embeddedaudioLabel.AutoSize = true;
            embeddedaudioLabel.Location = new System.Drawing.Point(13, 66);
            embeddedaudioLabel.Name = "embeddedaudioLabel";
            embeddedaudioLabel.Size = new System.Drawing.Size(91, 13);
            embeddedaudioLabel.TabIndex = 5;
            embeddedaudioLabel.Text = "Embedded Audio:";
            // 
            // keyLabel
            // 
            keyLabel.AutoSize = true;
            keyLabel.Location = new System.Drawing.Point(13, 94);
            keyLabel.Name = "keyLabel";
            keyLabel.Size = new System.Drawing.Size(28, 13);
            keyLabel.TabIndex = 7;
            keyLabel.Text = "Key:";
            // 
            // keyonlyLabel
            // 
            keyonlyLabel.AutoSize = true;
            keyonlyLabel.Location = new System.Drawing.Point(13, 122);
            keyonlyLabel.Name = "keyonlyLabel";
            keyonlyLabel.Size = new System.Drawing.Size(52, 13);
            keyonlyLabel.TabIndex = 9;
            keyonlyLabel.Text = "Key Only:";
            // 
            // latencyLabel
            // 
            latencyLabel.AutoSize = true;
            latencyLabel.Location = new System.Drawing.Point(13, 150);
            latencyLabel.Name = "latencyLabel";
            latencyLabel.Size = new System.Drawing.Size(48, 13);
            latencyLabel.TabIndex = 11;
            latencyLabel.Text = "Latency:";
            // 
            // bufferdepthTextBox
            // 
            this.bufferdepthTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.decklinkConsumerBindingSource, "bufferdepth", true));
            this.bufferdepthTextBox.Location = new System.Drawing.Point(105, 9);
            this.bufferdepthTextBox.Name = "bufferdepthTextBox";
            this.bufferdepthTextBox.Size = new System.Drawing.Size(104, 20);
            this.bufferdepthTextBox.TabIndex = 2;
            // 
            // decklinkConsumerBindingSource
            // 
            this.decklinkConsumerBindingSource.DataSource = typeof(CasparCGConfigurator.decklinkConsumer);
            // 
            // deviceTextBox
            // 
            this.deviceTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.decklinkConsumerBindingSource, "device", true));
            this.deviceTextBox.Location = new System.Drawing.Point(105, 35);
            this.deviceTextBox.Name = "deviceTextBox";
            this.deviceTextBox.Size = new System.Drawing.Size(104, 20);
            this.deviceTextBox.TabIndex = 4;
            // 
            // embeddedaudioCheckBox
            // 
            this.embeddedaudioCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.decklinkConsumerBindingSource, "embeddedaudio", true));
            this.embeddedaudioCheckBox.Location = new System.Drawing.Point(105, 61);
            this.embeddedaudioCheckBox.Name = "embeddedaudioCheckBox";
            this.embeddedaudioCheckBox.Size = new System.Drawing.Size(104, 24);
            this.embeddedaudioCheckBox.TabIndex = 6;
            this.embeddedaudioCheckBox.UseVisualStyleBackColor = true;
            // 
            // keyonlyCheckBox
            // 
            this.keyonlyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.decklinkConsumerBindingSource, "keyonly", true));
            this.keyonlyCheckBox.Location = new System.Drawing.Point(105, 117);
            this.keyonlyCheckBox.Name = "keyonlyCheckBox";
            this.keyonlyCheckBox.Size = new System.Drawing.Size(104, 24);
            this.keyonlyCheckBox.TabIndex = 10;
            this.keyonlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.decklinkConsumerBindingSource, "keyer", true));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "external",
            "internal",
            "default"});
            this.comboBox1.Location = new System.Drawing.Point(105, 92);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 13;
            // 
            // comboBox2
            // 
            this.comboBox2.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.decklinkConsumerBindingSource, "latency", true));
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "normal",
            "low",
            "default"});
            this.comboBox2.Location = new System.Drawing.Point(105, 142);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 14;
            // 
            // decklinkConsumerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(bufferdepthLabel);
            this.Controls.Add(this.bufferdepthTextBox);
            this.Controls.Add(deviceLabel);
            this.Controls.Add(this.deviceTextBox);
            this.Controls.Add(embeddedaudioLabel);
            this.Controls.Add(this.embeddedaudioCheckBox);
            this.Controls.Add(keyLabel);
            this.Controls.Add(keyonlyLabel);
            this.Controls.Add(this.keyonlyCheckBox);
            this.Controls.Add(latencyLabel);
            this.Name = "decklinkConsumerControl";
            this.Size = new System.Drawing.Size(261, 240);
            ((System.ComponentModel.ISupportInitialize)(this.decklinkConsumerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource decklinkConsumerBindingSource;
        private System.Windows.Forms.TextBox bufferdepthTextBox;
        private System.Windows.Forms.TextBox deviceTextBox;
        private System.Windows.Forms.CheckBox embeddedaudioCheckBox;
        private System.Windows.Forms.CheckBox keyonlyCheckBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}
