namespace TiebaLoopBan
{
    partial class ChaZhao
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_zhuXianZhangHao = new System.Windows.Forms.TextBox();
            this.textBox_touXiangID = new System.Windows.Forms.TextBox();
            this.textBox_tiebaName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_shangYiGe = new System.Windows.Forms.Button();
            this.button_xiaYiGe = new System.Windows.Forms.Button();
            this.label_chaXunJieGuo = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "主显账号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "头像ID";
            // 
            // textBox_zhuXianZhangHao
            // 
            this.textBox_zhuXianZhangHao.Location = new System.Drawing.Point(76, 76);
            this.textBox_zhuXianZhangHao.Name = "textBox_zhuXianZhangHao";
            this.textBox_zhuXianZhangHao.Size = new System.Drawing.Size(106, 21);
            this.textBox_zhuXianZhangHao.TabIndex = 2;
            this.textBox_zhuXianZhangHao.TextChanged += new System.EventHandler(this.textBox_zhuXianZhangHao_TextChanged);
            // 
            // textBox_touXiangID
            // 
            this.textBox_touXiangID.Location = new System.Drawing.Point(76, 49);
            this.textBox_touXiangID.Name = "textBox_touXiangID";
            this.textBox_touXiangID.Size = new System.Drawing.Size(129, 21);
            this.textBox_touXiangID.TabIndex = 1;
            this.textBox_touXiangID.TextChanged += new System.EventHandler(this.textBox_touXiangID_TextChanged);
            // 
            // textBox_tiebaName
            // 
            this.textBox_tiebaName.Location = new System.Drawing.Point(76, 22);
            this.textBox_tiebaName.Name = "textBox_tiebaName";
            this.textBox_tiebaName.Size = new System.Drawing.Size(106, 21);
            this.textBox_tiebaName.TabIndex = 0;
            this.textBox_tiebaName.TextChanged += new System.EventHandler(this.textBox_tiebaName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "贴吧名";
            // 
            // button_shangYiGe
            // 
            this.button_shangYiGe.Enabled = false;
            this.button_shangYiGe.Location = new System.Drawing.Point(19, 140);
            this.button_shangYiGe.Name = "button_shangYiGe";
            this.button_shangYiGe.Size = new System.Drawing.Size(90, 30);
            this.button_shangYiGe.TabIndex = 3;
            this.button_shangYiGe.Text = "查找上一个";
            this.button_shangYiGe.UseVisualStyleBackColor = true;
            this.button_shangYiGe.Click += new System.EventHandler(this.button_shangYiGe_Click);
            // 
            // button_xiaYiGe
            // 
            this.button_xiaYiGe.Enabled = false;
            this.button_xiaYiGe.Location = new System.Drawing.Point(115, 140);
            this.button_xiaYiGe.Name = "button_xiaYiGe";
            this.button_xiaYiGe.Size = new System.Drawing.Size(90, 30);
            this.button_xiaYiGe.TabIndex = 4;
            this.button_xiaYiGe.Text = "查找下一个";
            this.button_xiaYiGe.UseVisualStyleBackColor = true;
            this.button_xiaYiGe.Click += new System.EventHandler(this.button_xiaYiGe_Click);
            // 
            // label_chaXunJieGuo
            // 
            this.label_chaXunJieGuo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_chaXunJieGuo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_chaXunJieGuo.Location = new System.Drawing.Point(12, 111);
            this.label_chaXunJieGuo.Name = "label_chaXunJieGuo";
            this.label_chaXunJieGuo.Size = new System.Drawing.Size(200, 22);
            this.label_chaXunJieGuo.TabIndex = 8;
            this.label_chaXunJieGuo.Text = "查找到0个结果丨当前是第0个";
            this.label_chaXunJieGuo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Cursor = System.Windows.Forms.Cursors.Help;
            this.label8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(188, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 24;
            this.label8.Text = "？";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 25;
            this.label4.Text = "吧";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChaZhao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 191);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label_chaXunJieGuo);
            this.Controls.Add(this.button_xiaYiGe);
            this.Controls.Add(this.button_shangYiGe);
            this.Controls.Add(this.textBox_tiebaName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_touXiangID);
            this.Controls.Add(this.textBox_zhuXianZhangHao);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(240, 230);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(240, 230);
            this.Name = "ChaZhao";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查找";
            this.Load += new System.EventHandler(this.ChaZhao_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_zhuXianZhangHao;
        private System.Windows.Forms.TextBox textBox_touXiangID;
        private System.Windows.Forms.TextBox textBox_tiebaName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_shangYiGe;
        private System.Windows.Forms.Button button_xiaYiGe;
        private System.Windows.Forms.Label label_chaXunJieGuo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
    }
}