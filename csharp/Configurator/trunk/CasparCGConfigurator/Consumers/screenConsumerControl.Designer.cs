namespace CasparCGConfigurator
{
    partial class ScreenConsumerControl
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
            System.Windows.Forms.Label aspectratioLabel;
            System.Windows.Forms.Label autodeinterlaceLabel;
            System.Windows.Forms.Label deviceLabel;
            System.Windows.Forms.Label keyonlyLabel;
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label stretchLabel;
            System.Windows.Forms.Label vsyncLabel;
            System.Windows.Forms.Label windowedLabel;
            this.screenConsumerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.autodeinterlaceCheckBox = new System.Windows.Forms.CheckBox();
            this.keyonlyCheckBox = new System.Windows.Forms.CheckBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.vsyncCheckBox = new System.Windows.Forms.CheckBox();
            this.windowedCheckBox = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            aspectratioLabel = new System.Windows.Forms.Label();
            autodeinterlaceLabel = new System.Windows.Forms.Label();
            deviceLabel = new System.Windows.Forms.Label();
            keyonlyLabel = new System.Windows.Forms.Label();
            nameLabel = new System.Windows.Forms.Label();
            stretchLabel = new System.Windows.Forms.Label();
            vsyncLabel = new System.Windows.Forms.Label();
            windowedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.screenConsumerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // aspectratioLabel
            // 
            aspectratioLabel.AutoSize = true;
            aspectratioLabel.Location = new System.Drawing.Point(13, 10);
            aspectratioLabel.Name = "aspectratioLabel";
            aspectratioLabel.Size = new System.Drawing.Size(71, 13);
            aspectratioLabel.TabIndex = 1;
            aspectratioLabel.Text = "Aspect Ratio:";
            // 
            // autodeinterlaceLabel
            // 
            autodeinterlaceLabel.AutoSize = true;
            autodeinterlaceLabel.Location = new System.Drawing.Point(13, 38);
            autodeinterlaceLabel.Name = "autodeinterlaceLabel";
            autodeinterlaceLabel.Size = new System.Drawing.Size(89, 13);
            autodeinterlaceLabel.TabIndex = 3;
            autodeinterlaceLabel.Text = "Auto Deinterlace:";
            // 
            // deviceLabel
            // 
            deviceLabel.AutoSize = true;
            deviceLabel.Location = new System.Drawing.Point(13, 66);
            deviceLabel.Name = "deviceLabel";
            deviceLabel.Size = new System.Drawing.Size(54, 13);
            deviceLabel.TabIndex = 5;
            deviceLabel.Text = "Device #:";
            // 
            // keyonlyLabel
            // 
            keyonlyLabel.AutoSize = true;
            keyonlyLabel.Location = new System.Drawing.Point(13, 94);
            keyonlyLabel.Name = "keyonlyLabel";
            keyonlyLabel.Size = new System.Drawing.Size(52, 13);
            keyonlyLabel.TabIndex = 7;
            keyonlyLabel.Text = "Key Only:";
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(13, 122);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(80, 13);
            nameLabel.TabIndex = 9;
            nameLabel.Text = "Window Name:";
            // 
            // stretchLabel
            // 
            stretchLabel.AutoSize = true;
            stretchLabel.Location = new System.Drawing.Point(13, 148);
            stretchLabel.Name = "stretchLabel";
            stretchLabel.Size = new System.Drawing.Size(44, 13);
            stretchLabel.TabIndex = 11;
            stretchLabel.Text = "Stretch:";
            // 
            // vsyncLabel
            // 
            vsyncLabel.AutoSize = true;
            vsyncLabel.Location = new System.Drawing.Point(13, 176);
            vsyncLabel.Name = "vsyncLabel";
            vsyncLabel.Size = new System.Drawing.Size(39, 13);
            vsyncLabel.TabIndex = 13;
            vsyncLabel.Text = "Vsync:";
            // 
            // windowedLabel
            // 
            windowedLabel.AutoSize = true;
            windowedLabel.Location = new System.Drawing.Point(13, 206);
            windowedLabel.Name = "windowedLabel";
            windowedLabel.Size = new System.Drawing.Size(61, 13);
            windowedLabel.TabIndex = 15;
            windowedLabel.Text = "Windowed:";
            // 
            // screenConsumerBindingSource
            // 
            this.screenConsumerBindingSource.DataSource = typeof(CasparCGConfigurator.ScreenConsumer);
            // 
            // autodeinterlaceCheckBox
            // 
            this.autodeinterlaceCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.screenConsumerBindingSource, "AutoDeinterlace", true));
            this.autodeinterlaceCheckBox.Location = new System.Drawing.Point(102, 33);
            this.autodeinterlaceCheckBox.Name = "autodeinterlaceCheckBox";
            this.autodeinterlaceCheckBox.Size = new System.Drawing.Size(104, 24);
            this.autodeinterlaceCheckBox.TabIndex = 4;
            this.autodeinterlaceCheckBox.UseVisualStyleBackColor = true;
            // 
            // keyonlyCheckBox
            // 
            this.keyonlyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.screenConsumerBindingSource, "KeyOnly", true));
            this.keyonlyCheckBox.Location = new System.Drawing.Point(102, 89);
            this.keyonlyCheckBox.Name = "keyonlyCheckBox";
            this.keyonlyCheckBox.Size = new System.Drawing.Size(104, 24);
            this.keyonlyCheckBox.TabIndex = 8;
            this.keyonlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // nameTextBox
            // 
            this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.screenConsumerBindingSource, "Name", true));
            this.nameTextBox.Location = new System.Drawing.Point(102, 119);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(104, 20);
            this.nameTextBox.TabIndex = 10;
            // 
            // vsyncCheckBox
            // 
            this.vsyncCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.screenConsumerBindingSource, "VSync", true));
            this.vsyncCheckBox.Location = new System.Drawing.Point(102, 171);
            this.vsyncCheckBox.Name = "vsyncCheckBox";
            this.vsyncCheckBox.Size = new System.Drawing.Size(104, 24);
            this.vsyncCheckBox.TabIndex = 14;
            this.vsyncCheckBox.UseVisualStyleBackColor = true;
            // 
            // windowedCheckBox
            // 
            this.windowedCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.screenConsumerBindingSource, "Windowed", true));
            this.windowedCheckBox.Location = new System.Drawing.Point(102, 201);
            this.windowedCheckBox.Name = "windowedCheckBox";
            this.windowedCheckBox.Size = new System.Drawing.Size(104, 24);
            this.windowedCheckBox.TabIndex = 16;
            this.windowedCheckBox.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.screenConsumerBindingSource, "AspectRatio", true));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "default",
            "4:3",
            "16:9"});
            this.comboBox1.Location = new System.Drawing.Point(102, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 17;
            // 
            // comboBox2
            // 
            this.comboBox2.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.screenConsumerBindingSource, "Stretch", true));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "none",
            "fill",
            "uniform",
            "uniform_to_fill"});
            this.comboBox2.Location = new System.Drawing.Point(102, 145);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 18;
            // 
            // comboBox3
            // 
            this.comboBox3.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.screenConsumerBindingSource, "Device", true));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBox3.Location = new System.Drawing.Point(102, 62);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 19;
            // 
            // ScreenConsumerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(aspectratioLabel);
            this.Controls.Add(autodeinterlaceLabel);
            this.Controls.Add(this.autodeinterlaceCheckBox);
            this.Controls.Add(deviceLabel);
            this.Controls.Add(keyonlyLabel);
            this.Controls.Add(this.keyonlyCheckBox);
            this.Controls.Add(nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(stretchLabel);
            this.Controls.Add(vsyncLabel);
            this.Controls.Add(this.vsyncCheckBox);
            this.Controls.Add(windowedLabel);
            this.Controls.Add(this.windowedCheckBox);
            this.Name = "ScreenConsumerControl";
            ((System.ComponentModel.ISupportInitialize)(this.screenConsumerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource screenConsumerBindingSource;
        private System.Windows.Forms.CheckBox autodeinterlaceCheckBox;
        private System.Windows.Forms.CheckBox keyonlyCheckBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.CheckBox vsyncCheckBox;
        private System.Windows.Forms.CheckBox windowedCheckBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;

    }
}
