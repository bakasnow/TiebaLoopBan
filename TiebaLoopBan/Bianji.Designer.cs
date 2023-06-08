namespace TiebaLoopBan
{
    partial class BianJi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BianJi));
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label_fengJinShiChang = new System.Windows.Forms.Label();
            this.button_day_type1 = new System.Windows.Forms.Button();
            this.button_day_type2 = new System.Windows.Forms.Button();
            this.button_day_type3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button_ziDong_huoQu = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_ziDong_neiRong = new System.Windows.Forms.TextBox();
            this.comboBox_ziDong_leiXing = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_zhuXianZhangHao = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_touXiang = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_tiebaName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox_touXiang = new System.Windows.Forms.PictureBox();
            this.textBox_yongHuMing = new System.Windows.Forms.TextBox();
            this.label_yongHuMing = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_touXiang)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(352, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 30);
            this.button1.TabIndex = 11;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(290, 17);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(132, 21);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "添加日期";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "结束日期";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(290, 44);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(132, 21);
            this.dateTimePicker2.TabIndex = 7;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // label_fengJinShiChang
            // 
            this.label_fengJinShiChang.ForeColor = System.Drawing.Color.Red;
            this.label_fengJinShiChang.Location = new System.Drawing.Point(206, 112);
            this.label_fengJinShiChang.Name = "label_fengJinShiChang";
            this.label_fengJinShiChang.Size = new System.Drawing.Size(140, 12);
            this.label_fengJinShiChang.TabIndex = 11;
            this.label_fengJinShiChang.Text = "封禁时长：0天";
            this.label_fengJinShiChang.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_day_type1
            // 
            this.button_day_type1.Location = new System.Drawing.Point(290, 71);
            this.button_day_type1.Name = "button_day_type1";
            this.button_day_type1.Size = new System.Drawing.Size(40, 23);
            this.button_day_type1.TabIndex = 8;
            this.button_day_type1.Text = "30天";
            this.button_day_type1.UseVisualStyleBackColor = true;
            this.button_day_type1.Click += new System.EventHandler(this.button_day_type1_Click);
            // 
            // button_day_type2
            // 
            this.button_day_type2.Location = new System.Drawing.Point(336, 71);
            this.button_day_type2.Name = "button_day_type2";
            this.button_day_type2.Size = new System.Drawing.Size(40, 23);
            this.button_day_type2.TabIndex = 9;
            this.button_day_type2.Text = "1年";
            this.button_day_type2.UseVisualStyleBackColor = true;
            this.button_day_type2.Click += new System.EventHandler(this.button_day_type2_Click);
            // 
            // button_day_type3
            // 
            this.button_day_type3.Location = new System.Drawing.Point(382, 71);
            this.button_day_type3.Name = "button_day_type3";
            this.button_day_type3.Size = new System.Drawing.Size(40, 23);
            this.button_day_type3.TabIndex = 10;
            this.button_day_type3.Text = "长期";
            this.button_day_type3.UseVisualStyleBackColor = true;
            this.button_day_type3.Click += new System.EventHandler(this.button_day_type3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.button_ziDong_huoQu);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBox_ziDong_neiRong);
            this.groupBox1.Controls.Add(this.comboBox_ziDong_leiXing);
            this.groupBox1.Location = new System.Drawing.Point(154, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 147);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "获取用户信息";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 25;
            this.label10.Text = "内容";
            // 
            // button_ziDong_huoQu
            // 
            this.button_ziDong_huoQu.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ziDong_huoQu.Location = new System.Drawing.Point(210, 111);
            this.button_ziDong_huoQu.Name = "button_ziDong_huoQu";
            this.button_ziDong_huoQu.Size = new System.Drawing.Size(70, 30);
            this.button_ziDong_huoQu.TabIndex = 2;
            this.button_ziDong_huoQu.Text = "获取";
            this.button_ziDong_huoQu.UseVisualStyleBackColor = true;
            this.button_ziDong_huoQu.Click += new System.EventHandler(this.button_ziDong_huoQu_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "类型";
            // 
            // textBox_ziDong_neiRong
            // 
            this.textBox_ziDong_neiRong.Location = new System.Drawing.Point(45, 43);
            this.textBox_ziDong_neiRong.Multiline = true;
            this.textBox_ziDong_neiRong.Name = "textBox_ziDong_neiRong";
            this.textBox_ziDong_neiRong.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_ziDong_neiRong.Size = new System.Drawing.Size(235, 58);
            this.textBox_ziDong_neiRong.TabIndex = 1;
            // 
            // comboBox_ziDong_leiXing
            // 
            this.comboBox_ziDong_leiXing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ziDong_leiXing.FormattingEnabled = true;
            this.comboBox_ziDong_leiXing.Items.AddRange(new object[] {
            "用户名",
            "个人主页链接",
            "头像图片链接",
            "贴吧号"});
            this.comboBox_ziDong_leiXing.Location = new System.Drawing.Point(45, 17);
            this.comboBox_ziDong_leiXing.Name = "comboBox_ziDong_leiXing";
            this.comboBox_ziDong_leiXing.Size = new System.Drawing.Size(100, 20);
            this.comboBox_ziDong_leiXing.TabIndex = 0;
            this.comboBox_ziDong_leiXing.SelectedIndexChanged += new System.EventHandler(this.comboBox_ziDong_leiXing_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textBox_yongHuMing);
            this.groupBox3.Controls.Add(this.label_yongHuMing);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textBox_zhuXianZhangHao);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.button_day_type3);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.button_day_type2);
            this.groupBox3.Controls.Add(this.textBox_touXiang);
            this.groupBox3.Controls.Add(this.button_day_type1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label_fengJinShiChang);
            this.groupBox3.Controls.Add(this.textBox_tiebaName);
            this.groupBox3.Controls.Add(this.dateTimePicker2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.dateTimePicker1);
            this.groupBox3.Location = new System.Drawing.Point(12, 165);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(428, 139);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "封禁信息";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Cursor = System.Windows.Forms.Cursors.Help;
            this.label8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(193, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "？";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // textBox_zhuXianZhangHao
            // 
            this.textBox_zhuXianZhangHao.Location = new System.Drawing.Point(67, 71);
            this.textBox_zhuXianZhangHao.Name = "textBox_zhuXianZhangHao";
            this.textBox_zhuXianZhangHao.Size = new System.Drawing.Size(120, 21);
            this.textBox_zhuXianZhangHao.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "头像ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(193, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "吧";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_touXiang
            // 
            this.textBox_touXiang.Location = new System.Drawing.Point(67, 44);
            this.textBox_touXiang.Name = "textBox_touXiang";
            this.textBox_touXiang.Size = new System.Drawing.Size(120, 21);
            this.textBox_touXiang.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "贴吧名";
            // 
            // textBox_tiebaName
            // 
            this.textBox_tiebaName.Location = new System.Drawing.Point(67, 17);
            this.textBox_tiebaName.Name = "textBox_tiebaName";
            this.textBox_tiebaName.Size = new System.Drawing.Size(120, 21);
            this.textBox_tiebaName.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox_touXiang);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(136, 147);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "用户头像";
            // 
            // pictureBox_touXiang
            // 
            this.pictureBox_touXiang.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_touXiang.Location = new System.Drawing.Point(6, 17);
            this.pictureBox_touXiang.Name = "pictureBox_touXiang";
            this.pictureBox_touXiang.Size = new System.Drawing.Size(124, 124);
            this.pictureBox_touXiang.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_touXiang.TabIndex = 25;
            this.pictureBox_touXiang.TabStop = false;
            // 
            // textBox_yongHuMing
            // 
            this.textBox_yongHuMing.Enabled = false;
            this.textBox_yongHuMing.Location = new System.Drawing.Point(67, 98);
            this.textBox_yongHuMing.Name = "textBox_yongHuMing";
            this.textBox_yongHuMing.Size = new System.Drawing.Size(120, 21);
            this.textBox_yongHuMing.TabIndex = 24;
            // 
            // label_yongHuMing
            // 
            this.label_yongHuMing.AutoSize = true;
            this.label_yongHuMing.Location = new System.Drawing.Point(20, 101);
            this.label_yongHuMing.Name = "label_yongHuMing";
            this.label_yongHuMing.Size = new System.Drawing.Size(41, 12);
            this.label_yongHuMing.TabIndex = 25;
            this.label_yongHuMing.Text = "用户名";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "主显账号";
            // 
            // BianJi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 315);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(468, 354);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(468, 354);
            this.Name = "BianJi";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BianJi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Bianji_FormClosing);
            this.Load += new System.EventHandler(this.Bianji_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_touXiang)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label_fengJinShiChang;
        private System.Windows.Forms.Button button_day_type1;
        private System.Windows.Forms.Button button_day_type2;
        private System.Windows.Forms.Button button_day_type3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_ziDong_neiRong;
        private System.Windows.Forms.ComboBox comboBox_ziDong_leiXing;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_zhuXianZhangHao;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_touXiang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_tiebaName;
        private System.Windows.Forms.Button button_ziDong_huoQu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox_touXiang;
        private System.Windows.Forms.TextBox textBox_yongHuMing;
        private System.Windows.Forms.Label label_yongHuMing;
        private System.Windows.Forms.Label label1;
    }
}