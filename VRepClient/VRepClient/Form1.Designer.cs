﻿namespace VRepClient
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bt_tcp_test = new System.Windows.Forms.Button();
            this.rtb_tcp = new System.Windows.Forms.RichTextBox();
            this.rtb_tcp2 = new System.Windows.Forms.RichTextBox();
            this.btsend = new System.Windows.Forms.Button();
            this.KukaPotButton = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.VrepAdapter = new System.Windows.Forms.Button();
            this.YoubotAdapter = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Drive = new System.Windows.Forms.Button();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(165, 78);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(305, 4);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(279, 35);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(418, 70);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(166, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(324, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "RobLocData";
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(11, 583);
            this.Button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(100, 35);
            this.Button2.TabIndex = 4;
            this.Button2.Text = "PotField";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(418, 95);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(166, 22);
            this.textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(418, 149);
            this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(166, 22);
            this.textBox3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(308, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "TargetDirection";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(332, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "RoboDirect";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(418, 181);
            this.textBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(166, 22);
            this.textBox4.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(385, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Phi";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(418, 243);
            this.textBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(166, 22);
            this.textBox5.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(323, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "DistToTarget";
            // 
            // bt_tcp_test
            // 
            this.bt_tcp_test.Location = new System.Drawing.Point(23, 78);
            this.bt_tcp_test.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bt_tcp_test.Name = "bt_tcp_test";
            this.bt_tcp_test.Size = new System.Drawing.Size(115, 35);
            this.bt_tcp_test.TabIndex = 13;
            this.bt_tcp_test.Text = "TCPTest";
            this.bt_tcp_test.UseVisualStyleBackColor = true;
            this.bt_tcp_test.Click += new System.EventHandler(this.bt_tcp_test_Click);
            // 
            // rtb_tcp
            // 
            this.rtb_tcp.Location = new System.Drawing.Point(20, 398);
            this.rtb_tcp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rtb_tcp.Name = "rtb_tcp";
            this.rtb_tcp.Size = new System.Drawing.Size(260, 66);
            this.rtb_tcp.TabIndex = 14;
            this.rtb_tcp.Text = "";
            this.rtb_tcp.TextChanged += new System.EventHandler(this.rtb_tcp_TextChanged);
            // 
            // rtb_tcp2
            // 
            this.rtb_tcp2.Location = new System.Drawing.Point(20, 478);
            this.rtb_tcp2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rtb_tcp2.Name = "rtb_tcp2";
            this.rtb_tcp2.Size = new System.Drawing.Size(260, 50);
            this.rtb_tcp2.TabIndex = 15;
            this.rtb_tcp2.Text = "";
            // 
            // btsend
            // 
            this.btsend.Location = new System.Drawing.Point(11, 548);
            this.btsend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btsend.Name = "btsend";
            this.btsend.Size = new System.Drawing.Size(100, 35);
            this.btsend.TabIndex = 17;
            this.btsend.Text = "Send";
            this.btsend.UseVisualStyleBackColor = true;
            this.btsend.Click += new System.EventHandler(this.btsend_Click);
            // 
            // KukaPotButton
            // 
            this.KukaPotButton.Location = new System.Drawing.Point(11, 623);
            this.KukaPotButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KukaPotButton.Name = "KukaPotButton";
            this.KukaPotButton.Size = new System.Drawing.Size(115, 47);
            this.KukaPotButton.TabIndex = 18;
            this.KukaPotButton.Text = "KukaPotButton";
            this.KukaPotButton.UseVisualStyleBackColor = true;
            this.KukaPotButton.Click += new System.EventHandler(this.KukaPotButton_Click);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(418, 121);
            this.textBox6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(166, 22);
            this.textBox6.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(297, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 17);
            this.label6.TabIndex = 20;
            this.label6.Text = "controlCommand";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(418, 215);
            this.textBox7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(166, 22);
            this.textBox7.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(386, 218);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 17);
            this.label7.TabIndex = 22;
            this.label7.Text = "FX";
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(20, 162);
            this.tb_ip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(115, 22);
            this.tb_ip.TabIndex = 23;
            this.tb_ip.Text = "192.168.88.25";
            this.tb_ip.TextChanged += new System.EventHandler(this.tb_ip_TextChanged);
            // 
            // VrepAdapter
            // 
            this.VrepAdapter.Location = new System.Drawing.Point(165, 121);
            this.VrepAdapter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.VrepAdapter.Name = "VrepAdapter";
            this.VrepAdapter.Size = new System.Drawing.Size(115, 36);
            this.VrepAdapter.TabIndex = 24;
            this.VrepAdapter.Text = "VrepAdapter";
            this.VrepAdapter.UseVisualStyleBackColor = true;
            this.VrepAdapter.Click += new System.EventHandler(this.VrepAdapter_Click);
            // 
            // YoubotAdapter
            // 
            this.YoubotAdapter.Location = new System.Drawing.Point(20, 121);
            this.YoubotAdapter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.YoubotAdapter.Name = "YoubotAdapter";
            this.YoubotAdapter.Size = new System.Drawing.Size(115, 36);
            this.YoubotAdapter.TabIndex = 25;
            this.YoubotAdapter.Text = "YoubotAdapter";
            this.YoubotAdapter.UseVisualStyleBackColor = true;
            this.YoubotAdapter.Click += new System.EventHandler(this.YoubotAdapter_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(20, 312);
            this.richTextBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(257, 82);
            this.richTextBox2.TabIndex = 26;
            this.richTextBox2.Text = "";
            this.richTextBox2.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);
            // 
            // Drive
            // 
            this.Drive.Location = new System.Drawing.Point(97, 36);
            this.Drive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Drive.Name = "Drive";
            this.Drive.Size = new System.Drawing.Size(109, 38);
            this.Drive.TabIndex = 28;
            this.Drive.Text = "Drive";
            this.Drive.UseVisualStyleBackColor = true;
            this.Drive.UseWaitCursor = true;
            this.Drive.Click += new System.EventHandler(this.Drive_Click);
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(50, 218);
            this.textBox8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(72, 22);
            this.textBox8.TabIndex = 29;
            this.textBox8.Text = "1";
            this.textBox8.TextChanged += new System.EventHandler(this.textBox8_TextChanged);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(179, 218);
            this.textBox9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(72, 22);
            this.textBox9.TabIndex = 30;
            this.textBox9.Text = "1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 17);
            this.label8.TabIndex = 31;
            this.label8.Text = "X=";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(148, 220);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 17);
            this.label9.TabIndex = 32;
            this.label9.Text = "Y=";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(599, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(850, 850);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 41;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 286);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(134, 17);
            this.label10.TabIndex = 42;
            this.label10.Text = "Одометрия робота";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1461, 830);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox9);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.Drive);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.YoubotAdapter);
            this.Controls.Add(this.VrepAdapter);
            this.Controls.Add(this.tb_ip);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.KukaPotButton);
            this.Controls.Add(this.btsend);
            this.Controls.Add(this.rtb_tcp2);
            this.Controls.Add(this.rtb_tcp);
            this.Controls.Add(this.bt_tcp_test);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Button2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bt_tcp_test;
        private System.Windows.Forms.RichTextBox rtb_tcp;
        private System.Windows.Forms.RichTextBox rtb_tcp2;
        private System.Windows.Forms.Button btsend;
        private System.Windows.Forms.Button KukaPotButton;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.Button VrepAdapter;
        private System.Windows.Forms.Button YoubotAdapter;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button Drive;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label10;
    }
}

