namespace Notifier
{
   public partial class MainForm
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
         this._menuStrip = new System.Windows.Forms.MenuStrip();
         this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this._notificationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this._contractsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this._menuStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // _menuStrip
         // 
         this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creditsToolStripMenuItem});
         this._menuStrip.Location = new System.Drawing.Point(0, 0);
         this._menuStrip.Name = "_menuStrip";
         this._menuStrip.Size = new System.Drawing.Size(284, 24);
         this._menuStrip.TabIndex = 1;
         this._menuStrip.Text = "menuStrip1";
         // 
         // creditsToolStripMenuItem
         // 
         this.creditsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._notificationToolStripMenuItem,
            this._contractsToolStripMenuItem});
         this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
         this.creditsToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
         this.creditsToolStripMenuItem.Text = "Кредиты";
         // 
         // _notificationToolStripMenuItem
         // 
         this._notificationToolStripMenuItem.Name = "_notificationToolStripMenuItem";
         this._notificationToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
         this._notificationToolStripMenuItem.Text = "Оповещение";
         this._notificationToolStripMenuItem.Click += new System.EventHandler(this.notificationToolStripMenuItemClick);
         // 
         // _contractsToolStripMenuItem
         // 
         this._contractsToolStripMenuItem.Name = "_contractsToolStripMenuItem";
         this._contractsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
         this._contractsToolStripMenuItem.Text = "Контракты";
         this._contractsToolStripMenuItem.Click += new System.EventHandler(this.contractsToolStripMenuItemClick);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 262);
         this.Controls.Add(this._menuStrip);
         this.IsMdiContainer = true;
         this.MainMenuStrip = this._menuStrip;
         this.Name = "MainForm";
         this.Text = "Notifier";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.Shown += new System.EventHandler(this.mainFormShown);
         this._menuStrip.ResumeLayout(false);
         this._menuStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip _menuStrip;
      private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem _notificationToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem _contractsToolStripMenuItem;

   }
}

