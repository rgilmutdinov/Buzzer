namespace Notifier.Forms.Notification
{
   partial class NotificationForm
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
         this._elementHost = new System.Windows.Forms.Integration.ElementHost();
         this._toolStrip = new System.Windows.Forms.ToolStrip();
         this._saveButton = new System.Windows.Forms.ToolStripButton();
         this._refreshButton = new System.Windows.Forms.ToolStripButton();
         this._toolStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // _elementHost
         // 
         this._elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this._elementHost.Location = new System.Drawing.Point(12, 28);
         this._elementHost.Name = "_elementHost";
         this._elementHost.Size = new System.Drawing.Size(528, 421);
         this._elementHost.TabIndex = 0;
         this._elementHost.Text = "elementHost1";
         this._elementHost.Child = null;
         // 
         // _toolStrip
         // 
         this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveButton,
            this._refreshButton});
         this._toolStrip.Location = new System.Drawing.Point(0, 0);
         this._toolStrip.Name = "_toolStrip";
         this._toolStrip.Size = new System.Drawing.Size(552, 25);
         this._toolStrip.TabIndex = 1;
         this._toolStrip.Text = "toolStrip1";
         // 
         // _saveButton
         // 
         this._saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._saveButton.Image = global::Notifier.Properties.Resources.SaveIcon;
         this._saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._saveButton.Name = "_saveButton";
         this._saveButton.Size = new System.Drawing.Size(23, 22);
         this._saveButton.ToolTipText = "Сохранить";
         this._saveButton.Click += new System.EventHandler(this.saveButtonClick);
         // 
         // _refreshButton
         // 
         this._refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this._refreshButton.Image = global::Notifier.Properties.Resources.RefreshIcon;
         this._refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this._refreshButton.Name = "_refreshButton";
         this._refreshButton.Size = new System.Drawing.Size(23, 22);
         this._refreshButton.Text = "toolStripButton1";
         this._refreshButton.Click += new System.EventHandler(this.refreshButtonClick);
         // 
         // NotificationForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.ClientSize = new System.Drawing.Size(552, 461);
         this.Controls.Add(this._toolStrip);
         this.Controls.Add(this._elementHost);
         this.Name = "NotificationForm";
         this.Text = "Оповещение";
         this._toolStrip.ResumeLayout(false);
         this._toolStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Integration.ElementHost _elementHost;
      private System.Windows.Forms.ToolStrip _toolStrip;
      private System.Windows.Forms.ToolStripButton _saveButton;
      private System.Windows.Forms.ToolStripButton _refreshButton;

   }
}