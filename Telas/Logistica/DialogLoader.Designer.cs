namespace GerarCargaDez.Telas.Logistica
{
    partial class DialogLoader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogLoader));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.preLoader1 = new GerarCargaDez.Telas.Logistica.PreLoader();
            this.timerLoaderForm = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 6;
            this.bunifuElipse1.TargetControl = this;
            // 
            // preLoader1
            // 
            this.preLoader1.BackColor = System.Drawing.Color.White;
            this.preLoader1.Location = new System.Drawing.Point(45, 36);
            this.preLoader1.Name = "preLoader1";
            this.preLoader1.Size = new System.Drawing.Size(201, 170);
            this.preLoader1.TabIndex = 0;
            // 
            // timerLoaderForm
            // 
            this.timerLoaderForm.Enabled = true;
            this.timerLoaderForm.Tick += new System.EventHandler(this.TimerLoaderForm_Tick);
            // 
            // DialogLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(293, 264);
            this.Controls.Add(this.preLoader1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLoader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DialogLoader";
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private PreLoader preLoader1;
        private System.Windows.Forms.Timer timerLoaderForm;
    }
}