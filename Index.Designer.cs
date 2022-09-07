namespace VGACheck
{
    partial class Index
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Index));
            this.label_vga = new System.Windows.Forms.Label();
            this.label_auto = new System.Windows.Forms.Label();
            this.bw_auto = new System.ComponentModel.BackgroundWorker();
            this.group_kvm = new System.Windows.Forms.GroupBox();
            this.text_baud = new System.Windows.Forms.TextBox();
            this.label_kvm = new System.Windows.Forms.Label();
            this.button_kvm = new System.Windows.Forms.Button();
            this.combo_port = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serial_kvm = new System.IO.Ports.SerialPort(this.components);
            this.button_screen = new System.Windows.Forms.Button();
            this.combo_kvm = new System.Windows.Forms.ComboBox();
            this.button_mac = new System.Windows.Forms.Button();
            this.button_image_range = new System.Windows.Forms.Button();
            this.button_image_interval = new System.Windows.Forms.Button();
            this.button_main = new System.Windows.Forms.Button();
            this.group_xml = new System.Windows.Forms.GroupBox();
            this.label_kvmnum = new System.Windows.Forms.Label();
            this.textBox_status = new System.Windows.Forms.TextBox();
            this.bw_manual = new System.ComponentModel.BackgroundWorker();
            this.group_vga = new System.Windows.Forms.GroupBox();
            this.textBox_timer = new System.Windows.Forms.TextBox();
            this.label_timer = new System.Windows.Forms.Label();
            this.combo_rel = new System.Windows.Forms.ComboBox();
            this.label_rel = new System.Windows.Forms.Label();
            this.button_vga = new System.Windows.Forms.Button();
            this.group_auto = new System.Windows.Forms.GroupBox();
            this.button_auto = new System.Windows.Forms.Button();
            this.pictureBoxShowMain = new System.Windows.Forms.PictureBox();
            this.label_checking = new System.Windows.Forms.Label();
            this.label_clock = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.group_kvm.SuspendLayout();
            this.group_xml.SuspendLayout();
            this.group_vga.SuspendLayout();
            this.group_auto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShowMain)).BeginInit();
            this.SuspendLayout();
            // 
            // label_vga
            // 
            this.label_vga.AutoSize = true;
            this.label_vga.Font = new System.Drawing.Font("Calibri", 16F);
            this.label_vga.Location = new System.Drawing.Point(106, 22);
            this.label_vga.Name = "label_vga";
            this.label_vga.Size = new System.Drawing.Size(91, 27);
            this.label_vga.TabIndex = 5;
            this.label_vga.Text = "VGA OFF";
            this.label_vga.Click += new System.EventHandler(this.label_vga_Click);
            // 
            // label_auto
            // 
            this.label_auto.AutoSize = true;
            this.label_auto.Font = new System.Drawing.Font("Calibri", 13F);
            this.label_auto.ForeColor = System.Drawing.Color.Gray;
            this.label_auto.Location = new System.Drawing.Point(97, 33);
            this.label_auto.Name = "label_auto";
            this.label_auto.Size = new System.Drawing.Size(129, 22);
            this.label_auto.TabIndex = 7;
            this.label_auto.Text = "Automation OFF";
            // 
            // bw_auto
            // 
            this.bw_auto.WorkerSupportsCancellation = true;
            this.bw_auto.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_auto_DoWork);
            this.bw_auto.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_auto_RunWorkerCompleted);
            // 
            // group_kvm
            // 
            this.group_kvm.BackColor = System.Drawing.SystemColors.Control;
            this.group_kvm.Controls.Add(this.text_baud);
            this.group_kvm.Controls.Add(this.label_kvm);
            this.group_kvm.Controls.Add(this.button_kvm);
            this.group_kvm.Controls.Add(this.combo_port);
            this.group_kvm.Controls.Add(this.label2);
            this.group_kvm.Controls.Add(this.label1);
            this.group_kvm.Font = new System.Drawing.Font("Calibri", 12F);
            this.group_kvm.Location = new System.Drawing.Point(7, 140);
            this.group_kvm.Name = "group_kvm";
            this.group_kvm.Size = new System.Drawing.Size(275, 135);
            this.group_kvm.TabIndex = 10;
            this.group_kvm.TabStop = false;
            this.group_kvm.Text = "KVM";
            this.group_kvm.Enter += new System.EventHandler(this.group_kvm_Enter);
            // 
            // text_baud
            // 
            this.text_baud.Enabled = false;
            this.text_baud.Location = new System.Drawing.Point(80, 56);
            this.text_baud.Name = "text_baud";
            this.text_baud.Size = new System.Drawing.Size(171, 27);
            this.text_baud.TabIndex = 3;
            this.text_baud.TextChanged += new System.EventHandler(this.text_baud_TextChanged);
            // 
            // label_kvm
            // 
            this.label_kvm.AutoSize = true;
            this.label_kvm.Font = new System.Drawing.Font("Calibri", 16F);
            this.label_kvm.Location = new System.Drawing.Point(119, 93);
            this.label_kvm.Name = "label_kvm";
            this.label_kvm.Size = new System.Drawing.Size(94, 27);
            this.label_kvm.TabIndex = 11;
            this.label_kvm.Text = "KVM OFF";
            // 
            // button_kvm
            // 
            this.button_kvm.Enabled = false;
            this.button_kvm.Image = global::VGACheck.Properties.Resources.btncheckoff;
            this.button_kvm.Location = new System.Drawing.Point(4, 90);
            this.button_kvm.Name = "button_kvm";
            this.button_kvm.Size = new System.Drawing.Size(94, 35);
            this.button_kvm.TabIndex = 4;
            this.button_kvm.UseVisualStyleBackColor = true;
            this.button_kvm.Click += new System.EventHandler(this.button_kvm_Click);
            // 
            // combo_port
            // 
            this.combo_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_port.Enabled = false;
            this.combo_port.FormattingEnabled = true;
            this.combo_port.Location = new System.Drawing.Point(80, 24);
            this.combo_port.Name = "combo_port";
            this.combo_port.Size = new System.Drawing.Size(171, 27);
            this.combo_port.TabIndex = 2;
            this.combo_port.SelectedIndexChanged += new System.EventHandler(this.combo_port_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 15F);
            this.label2.Location = new System.Drawing.Point(2, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Baud";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 15F);
            this.label1.Location = new System.Drawing.Point(2, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port";
            // 
            // button_screen
            // 
            this.button_screen.Enabled = false;
            this.button_screen.Font = new System.Drawing.Font("Calibri", 11F);
            this.button_screen.Location = new System.Drawing.Point(172, 89);
            this.button_screen.Name = "button_screen";
            this.button_screen.Size = new System.Drawing.Size(92, 31);
            this.button_screen.TabIndex = 7;
            this.button_screen.Text = "Single Shot";
            this.button_screen.UseVisualStyleBackColor = true;
            this.button_screen.Click += new System.EventHandler(this.button_screen_Click);
            // 
            // combo_kvm
            // 
            this.combo_kvm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_kvm.Enabled = false;
            this.combo_kvm.Font = new System.Drawing.Font("Calibri", 15F);
            this.combo_kvm.FormattingEnabled = true;
            this.combo_kvm.Location = new System.Drawing.Point(305, 232);
            this.combo_kvm.Name = "combo_kvm";
            this.combo_kvm.Size = new System.Drawing.Size(274, 32);
            this.combo_kvm.TabIndex = 6;
            this.combo_kvm.SelectedIndexChanged += new System.EventHandler(this.combo_kvm_SelectedIndexChanged);
            // 
            // button_mac
            // 
            this.button_mac.Enabled = false;
            this.button_mac.Font = new System.Drawing.Font("Calibri", 9F);
            this.button_mac.Location = new System.Drawing.Point(42, 56);
            this.button_mac.Name = "button_mac";
            this.button_mac.Size = new System.Drawing.Size(92, 31);
            this.button_mac.TabIndex = 9;
            this.button_mac.Text = "Fixture";
            this.button_mac.UseVisualStyleBackColor = true;
            this.button_mac.Click += new System.EventHandler(this.button_mac_Click);
            // 
            // button_image_range
            // 
            this.button_image_range.Enabled = false;
            this.button_image_range.Font = new System.Drawing.Font("Calibri", 9F);
            this.button_image_range.Location = new System.Drawing.Point(172, 21);
            this.button_image_range.Name = "button_image_range";
            this.button_image_range.Size = new System.Drawing.Size(92, 31);
            this.button_image_range.TabIndex = 11;
            this.button_image_range.Text = "Check Range";
            this.button_image_range.UseVisualStyleBackColor = true;
            this.button_image_range.Click += new System.EventHandler(this.button_image_range_Click);
            // 
            // button_image_interval
            // 
            this.button_image_interval.Enabled = false;
            this.button_image_interval.Font = new System.Drawing.Font("Calibri", 9F);
            this.button_image_interval.Location = new System.Drawing.Point(172, 55);
            this.button_image_interval.Name = "button_image_interval";
            this.button_image_interval.Size = new System.Drawing.Size(92, 31);
            this.button_image_interval.TabIndex = 10;
            this.button_image_interval.Text = "Check Color";
            this.button_image_interval.UseVisualStyleBackColor = true;
            this.button_image_interval.Click += new System.EventHandler(this.button_image_interval_Click);
            // 
            // button_main
            // 
            this.button_main.Enabled = false;
            this.button_main.Font = new System.Drawing.Font("Calibri", 9F);
            this.button_main.Location = new System.Drawing.Point(42, 22);
            this.button_main.Name = "button_main";
            this.button_main.Size = new System.Drawing.Size(92, 31);
            this.button_main.TabIndex = 8;
            this.button_main.Text = "Main";
            this.button_main.UseVisualStyleBackColor = true;
            this.button_main.Click += new System.EventHandler(this.button_main_Click);
            // 
            // group_xml
            // 
            this.group_xml.BackColor = System.Drawing.SystemColors.Control;
            this.group_xml.Controls.Add(this.button_screen);
            this.group_xml.Controls.Add(this.button_main);
            this.group_xml.Controls.Add(this.button_image_interval);
            this.group_xml.Controls.Add(this.button_mac);
            this.group_xml.Controls.Add(this.button_image_range);
            this.group_xml.Font = new System.Drawing.Font("Calibri", 12F);
            this.group_xml.Location = new System.Drawing.Point(305, 3);
            this.group_xml.Name = "group_xml";
            this.group_xml.Size = new System.Drawing.Size(274, 135);
            this.group_xml.TabIndex = 16;
            this.group_xml.TabStop = false;
            this.group_xml.Text = "XML info";
            this.group_xml.Enter += new System.EventHandler(this.group_xml_Enter);
            // 
            // label_kvmnum
            // 
            this.label_kvmnum.AutoSize = true;
            this.label_kvmnum.Font = new System.Drawing.Font("Calibri", 12F);
            this.label_kvmnum.Location = new System.Drawing.Point(595, 3);
            this.label_kvmnum.Name = "label_kvmnum";
            this.label_kvmnum.Size = new System.Drawing.Size(49, 19);
            this.label_kvmnum.TabIndex = 17;
            this.label_kvmnum.Text = "(KVM)";
            this.label_kvmnum.Click += new System.EventHandler(this.label_kvmnum_Click);
            // 
            // textBox_status
            // 
            this.textBox_status.Font = new System.Drawing.Font("Calibri", 9.25F);
            this.textBox_status.Location = new System.Drawing.Point(7, 281);
            this.textBox_status.MaxLength = 10000;
            this.textBox_status.Multiline = true;
            this.textBox_status.Name = "textBox_status";
            this.textBox_status.ReadOnly = true;
            this.textBox_status.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_status.Size = new System.Drawing.Size(572, 403);
            this.textBox_status.TabIndex = 0;
            this.textBox_status.TabStop = false;
            this.textBox_status.WordWrap = false;
            this.textBox_status.TextChanged += new System.EventHandler(this.textBox_status_TextChanged);
            // 
            // bw_manual
            // 
            this.bw_manual.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_manual_DoWork);
            this.bw_manual.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_manual_RunWorkerCompleted);
            // 
            // group_vga
            // 
            this.group_vga.BackColor = System.Drawing.SystemColors.Control;
            this.group_vga.Controls.Add(this.textBox_timer);
            this.group_vga.Controls.Add(this.label_timer);
            this.group_vga.Controls.Add(this.combo_rel);
            this.group_vga.Controls.Add(this.label_rel);
            this.group_vga.Controls.Add(this.label_vga);
            this.group_vga.Controls.Add(this.button_vga);
            this.group_vga.Font = new System.Drawing.Font("Calibri", 12F);
            this.group_vga.Location = new System.Drawing.Point(7, 3);
            this.group_vga.Name = "group_vga";
            this.group_vga.Size = new System.Drawing.Size(275, 135);
            this.group_vga.TabIndex = 19;
            this.group_vga.TabStop = false;
            this.group_vga.Text = "VGA";
            this.group_vga.Enter += new System.EventHandler(this.group_vga_Enter);
            // 
            // textBox_timer
            // 
            this.textBox_timer.Font = new System.Drawing.Font("Calibri", 14F);
            this.textBox_timer.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox_timer.Location = new System.Drawing.Point(0, 99);
            this.textBox_timer.Name = "textBox_timer";
            this.textBox_timer.Size = new System.Drawing.Size(269, 30);
            this.textBox_timer.TabIndex = 25;
            this.textBox_timer.Text = "Wait";
            this.textBox_timer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_timer.Visible = false;
            this.textBox_timer.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label_timer
            // 
            this.label_timer.AutoSize = true;
            this.label_timer.Font = new System.Drawing.Font("Calibri", 16F);
            this.label_timer.Location = new System.Drawing.Point(12, 22);
            this.label_timer.Name = "label_timer";
            this.label_timer.Size = new System.Drawing.Size(0, 27);
            this.label_timer.TabIndex = 8;
            this.label_timer.Visible = false;
            // 
            // combo_rel
            // 
            this.combo_rel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_rel.Enabled = false;
            this.combo_rel.FormattingEnabled = true;
            this.combo_rel.Location = new System.Drawing.Point(107, 58);
            this.combo_rel.Name = "combo_rel";
            this.combo_rel.Size = new System.Drawing.Size(153, 27);
            this.combo_rel.TabIndex = 7;
            this.combo_rel.Visible = false;
            this.combo_rel.SelectedIndexChanged += new System.EventHandler(this.combo_rel_SelectedIndexChanged);
            // 
            // label_rel
            // 
            this.label_rel.AutoSize = true;
            this.label_rel.Font = new System.Drawing.Font("Calibri", 12F);
            this.label_rel.Location = new System.Drawing.Point(2, 64);
            this.label_rel.Name = "label_rel";
            this.label_rel.Size = new System.Drawing.Size(78, 19);
            this.label_rel.TabIndex = 6;
            this.label_rel.Text = "Resolution";
            // 
            // button_vga
            // 
            this.button_vga.Enabled = false;
            this.button_vga.Image = global::VGACheck.Properties.Resources.btncheckoff;
            this.button_vga.Location = new System.Drawing.Point(4, 19);
            this.button_vga.Name = "button_vga";
            this.button_vga.Size = new System.Drawing.Size(94, 35);
            this.button_vga.TabIndex = 1;
            this.button_vga.UseVisualStyleBackColor = true;
            this.button_vga.Click += new System.EventHandler(this.button_vga_Click);
            // 
            // group_auto
            // 
            this.group_auto.BackColor = System.Drawing.SystemColors.Control;
            this.group_auto.Controls.Add(this.button_auto);
            this.group_auto.Controls.Add(this.label_auto);
            this.group_auto.Font = new System.Drawing.Font("Calibri", 12F);
            this.group_auto.Location = new System.Drawing.Point(305, 140);
            this.group_auto.Name = "group_auto";
            this.group_auto.Size = new System.Drawing.Size(274, 70);
            this.group_auto.TabIndex = 20;
            this.group_auto.TabStop = false;
            this.group_auto.Text = "Automation Test";
            this.group_auto.Enter += new System.EventHandler(this.group_auto_Enter);
            // 
            // button_auto
            // 
            this.button_auto.Enabled = false;
            this.button_auto.Image = global::VGACheck.Properties.Resources.btncheckoff;
            this.button_auto.Location = new System.Drawing.Point(4, 24);
            this.button_auto.Name = "button_auto";
            this.button_auto.Size = new System.Drawing.Size(94, 38);
            this.button_auto.TabIndex = 5;
            this.button_auto.UseVisualStyleBackColor = true;
            this.button_auto.Click += new System.EventHandler(this.button_auto_Click);
            // 
            // pictureBoxShowMain
            // 
            this.pictureBoxShowMain.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBoxShowMain.Location = new System.Drawing.Point(599, 25);
            this.pictureBoxShowMain.Name = "pictureBoxShowMain";
            this.pictureBoxShowMain.Size = new System.Drawing.Size(881, 680);
            this.pictureBoxShowMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxShowMain.TabIndex = 2;
            this.pictureBoxShowMain.TabStop = false;
            this.pictureBoxShowMain.Click += new System.EventHandler(this.pictureBoxShowMain_Click);
            this.pictureBoxShowMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxShowMain_MouseDoubleClick);
            // 
            // label_checking
            // 
            this.label_checking.AutoSize = true;
            this.label_checking.Font = new System.Drawing.Font("Calibri", 10F);
            this.label_checking.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label_checking.Location = new System.Drawing.Point(1154, 37);
            this.label_checking.Name = "label_checking";
            this.label_checking.Size = new System.Drawing.Size(148, 34);
            this.label_checking.TabIndex = 23;
            this.label_checking.Text = "Automation checking...\r\nPlease wait few seconds!";
            this.label_checking.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_checking.Visible = false;
            this.label_checking.Click += new System.EventHandler(this.label_checking_Click);
            // 
            // label_clock
            // 
            this.label_clock.AutoSize = true;
            this.label_clock.Location = new System.Drawing.Point(4, 687);
            this.label_clock.Name = "label_clock";
            this.label_clock.Size = new System.Drawing.Size(93, 14);
            this.label_clock.TabIndex = 24;
            this.label_clock.Text = "Automation OFF";
            this.label_clock.Click += new System.EventHandler(this.label_clock_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Index
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 717);
            this.Controls.Add(this.label_clock);
            this.Controls.Add(this.combo_kvm);
            this.Controls.Add(this.label_checking);
            this.Controls.Add(this.group_auto);
            this.Controls.Add(this.group_vga);
            this.Controls.Add(this.textBox_status);
            this.Controls.Add(this.group_kvm);
            this.Controls.Add(this.group_xml);
            this.Controls.Add(this.label_kvmnum);
            this.Controls.Add(this.pictureBoxShowMain);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Index";
            this.Text = "(title)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Index_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Index_closed);
            this.Load += new System.EventHandler(this.Index_Load);
            this.group_kvm.ResumeLayout(false);
            this.group_kvm.PerformLayout();
            this.group_xml.ResumeLayout(false);
            this.group_vga.ResumeLayout(false);
            this.group_vga.PerformLayout();
            this.group_auto.ResumeLayout(false);
            this.group_auto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShowMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBoxShowMain;
        private System.Windows.Forms.Label label_vga;
        private System.Windows.Forms.Button button_auto;
        private System.Windows.Forms.Label label_auto;
        private System.ComponentModel.BackgroundWorker bw_auto;
        private System.Windows.Forms.GroupBox group_kvm;
        private System.Windows.Forms.Label label_kvm;
        private System.Windows.Forms.Button button_kvm;
        private System.Windows.Forms.ComboBox combo_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort serial_kvm;
        private System.Windows.Forms.TextBox text_baud;
        private System.Windows.Forms.Button button_screen;
        private System.Windows.Forms.Button button_mac;
        private System.Windows.Forms.ComboBox combo_kvm;
        private System.Windows.Forms.Button button_image_range;
        private System.Windows.Forms.Button button_image_interval;
        private System.Windows.Forms.Button button_main;
        private System.Windows.Forms.GroupBox group_xml;
        private System.Windows.Forms.Label label_kvmnum;
        private System.Windows.Forms.TextBox textBox_status;
        private System.ComponentModel.BackgroundWorker bw_manual;
        private System.Windows.Forms.GroupBox group_vga;
        private System.Windows.Forms.Label label_rel;
        private System.Windows.Forms.GroupBox group_auto;
        private System.Windows.Forms.ComboBox combo_rel;
        private System.Windows.Forms.Label label_checking;
        private System.Windows.Forms.Label label_clock;
        private System.Windows.Forms.Label label_timer;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox textBox_timer;
        private System.Windows.Forms.Button button_vga;
    }
}

