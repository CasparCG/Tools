namespace CasparCGConfigurator
{
    partial class bluefishConsumerControl
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
            System.Windows.Forms.Label deviceLabel;
            System.Windows.Forms.Label embeddedaudioLabel;
            System.Windows.Forms.Label keyonlyLabel;
            this.bluefishConsumerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.deviceTextBox = new System.Windows.Forms.TextBox();
            this.embeddedaudioCheckBox = new System.Windows.Forms.CheckBox();
            this.keyonlyCheckBox = new System.Windows.Forms.CheckBox();
            deviceLabel = new System.Windows.Forms.Label();
            embeddedaudioLabel = new System.Windows.Forms.Label();
            keyonlyLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bluefishConsumerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // bluefishConsumerBindingSource
            // 
            this.bluefishConsumerBindingSource.DataSource = typeof(CasparCGConfigurator.bluefishConsumer);
            // 
            // deviceLabel
            // 
            deviceLabel.AutoSize = true;
            deviceLabel.Location = new System.Drawing.Point(3, 9);
            deviceLabel.Name = "deviceLabel";
            deviceLabel.Size = new System.Drawing.Size(54, 13);
            deviceLabel.TabIndex = 1;
            deviceLabel.Text = "Device #:";
            // 
            // deviceTextBox
            // 
            this.deviceTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bluefishConsumerBindingSource, "device", true));
            this.deviceTextBox.Location = new System.Drawing.Point(95, 6);
            this.deviceTextBox.Name = "deviceTextBox";
            this.deviceTextBox.Size = new System.Drawing.Size(104, 20);
            this.deviceTextBox.TabIndex = 2;
            // 
            // embeddedaudioLabel
            // 
            embeddedaudioLabel.AutoSize = true;
            embeddedaudioLabel.Location = new System.Drawing.Point(3, 37);
            embeddedaudioLabel.Name = "embeddedaudioLabel";
            embeddedaudioLabel.Size = new System.Drawing.Size(91, 13);
            embeddedaudioLabel.TabIndex = 3;
            embeddedaudioLabel.Text = "Embedded Audio:";
            // 
            // embeddedaudioCheckBox
            // 
            this.embeddedaudioCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.bluefishConsumerBindingSource, "embeddedaudio", true));
            this.embeddedaudioCheckBox.Location = new System.Drawing.Point(95, 32);
            this.embeddedaudioCheckBox.Name = "embeddedaudioCheckBox";
            this.embeddedaudioCheckBox.Size = new System.Drawing.Size(104, 24);
            this.embeddedaudioCheckBox.TabIndex = 4;
            this.embeddedaudioCheckBox.UseVisualStyleBackColor = true;
            // 
            // keyonlyLabel
            // 
            keyonlyLabel.AutoSize = true;
            keyonlyLabel.Location = new System.Drawing.Point(3, 67);
            keyonlyLabel.Name = "keyonlyLabel";
            keyonlyLabel.Size = new System.Drawing.Size(52, 13);
            keyonlyLabel.TabIndex = 5;
            keyonlyLabel.Text = "Key Only:";
            // 
            // keyonlyCheckBox
            // 
            this.keyonlyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.bluefishConsumerBindingSource, "keyonly", true));
            this.keyonlyCheckBox.Location = new System.Drawing.Point(95, 62);
            this.keyonlyCheckBox.Name = "keyonlyCheckBox";
            this.keyonlyCheckBox.Size = new System.Drawing.Size(104, 24);
            this.keyonlyCheckBox.TabIndex = 6;
            this.keyonlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // bluefishConsumerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(deviceLabel);
            this.Controls.Add(this.deviceTextBox);
            this.Controls.Add(embeddedaudioLabel);
            this.Controls.Add(this.embeddedaudioCheckBox);
            this.Controls.Add(keyonlyLabel);
            this.Controls.Add(this.keyonlyCheckBox);
            this.Name = "bluefishConsumerControl";
            ((System.ComponentModel.ISupportInitialize)(this.bluefishConsumerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bluefishConsumerBindingSource;
        private System.Windows.Forms.TextBox deviceTextBox;
        private System.Windows.Forms.CheckBox embeddedaudioCheckBox;
        private System.Windows.Forms.CheckBox keyonlyCheckBox;

    }
}
