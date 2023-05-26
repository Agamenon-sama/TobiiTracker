
namespace EyeTracker
{
    partial class ControlPanel
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
            this.Recalibrate = new System.Windows.Forms.Button();
            this.textView = new System.Windows.Forms.Button();
            this.imageView = new System.Windows.Forms.Button();
            this.gbxFonts = new System.Windows.Forms.GroupBox();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.numFontSize = new System.Windows.Forms.NumericUpDown();
            this.lblFontOptions = new System.Windows.Forms.Label();
            this.btnApplyFonts = new System.Windows.Forms.Button();
            this.cbxFonts = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtSaveFileName = new System.Windows.Forms.TextBox();
            this.ToggleGazeRect = new System.Windows.Forms.Button();
            this.gbxFonts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // Recalibrate
            // 
            this.Recalibrate.Location = new System.Drawing.Point(13, 129);
            this.Recalibrate.Name = "Recalibrate";
            this.Recalibrate.Size = new System.Drawing.Size(75, 23);
            this.Recalibrate.TabIndex = 0;
            this.Recalibrate.Text = "Re&calibrate";
            this.Recalibrate.UseVisualStyleBackColor = true;
            this.Recalibrate.Click += new System.EventHandler(this.Recalibrate_Click);
            // 
            // textView
            // 
            this.textView.Location = new System.Drawing.Point(125, 129);
            this.textView.Name = "textView";
            this.textView.Size = new System.Drawing.Size(75, 23);
            this.textView.TabIndex = 1;
            this.textView.Text = "&Text view";
            this.textView.UseVisualStyleBackColor = true;
            this.textView.Click += new System.EventHandler(this.textView_Click);
            // 
            // imageView
            // 
            this.imageView.Location = new System.Drawing.Point(261, 129);
            this.imageView.Name = "imageView";
            this.imageView.Size = new System.Drawing.Size(75, 23);
            this.imageView.TabIndex = 2;
            this.imageView.Text = "&Image view";
            this.imageView.UseVisualStyleBackColor = true;
            this.imageView.Click += new System.EventHandler(this.imageView_Click);
            // 
            // gbxFonts
            // 
            this.gbxFonts.Controls.Add(this.lblFontSize);
            this.gbxFonts.Controls.Add(this.numFontSize);
            this.gbxFonts.Controls.Add(this.lblFontOptions);
            this.gbxFonts.Controls.Add(this.btnApplyFonts);
            this.gbxFonts.Controls.Add(this.cbxFonts);
            this.gbxFonts.Location = new System.Drawing.Point(13, 176);
            this.gbxFonts.Name = "gbxFonts";
            this.gbxFonts.Size = new System.Drawing.Size(323, 145);
            this.gbxFonts.TabIndex = 3;
            this.gbxFonts.TabStop = false;
            this.gbxFonts.Text = "Fonts";
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(6, 49);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(54, 13);
            this.lblFontSize.TabIndex = 4;
            this.lblFontSize.Text = "Font Size:";
            // 
            // numFontSize
            // 
            this.numFontSize.Location = new System.Drawing.Point(84, 47);
            this.numFontSize.Name = "numFontSize";
            this.numFontSize.Size = new System.Drawing.Size(233, 20);
            this.numFontSize.TabIndex = 3;
            this.numFontSize.Value = new decimal(new int[] {
            22,
            0,
            0,
            0});
            // 
            // lblFontOptions
            // 
            this.lblFontOptions.AutoSize = true;
            this.lblFontOptions.Location = new System.Drawing.Point(6, 22);
            this.lblFontOptions.Name = "lblFontOptions";
            this.lblFontOptions.Size = new System.Drawing.Size(70, 13);
            this.lblFontOptions.TabIndex = 2;
            this.lblFontOptions.Text = "Font Options:";
            // 
            // btnApplyFonts
            // 
            this.btnApplyFonts.Location = new System.Drawing.Point(248, 108);
            this.btnApplyFonts.Name = "btnApplyFonts";
            this.btnApplyFonts.Size = new System.Drawing.Size(69, 23);
            this.btnApplyFonts.TabIndex = 1;
            this.btnApplyFonts.Text = "Apply";
            this.btnApplyFonts.UseVisualStyleBackColor = true;
            this.btnApplyFonts.Click += new System.EventHandler(this.btnApplyFonts_Click);
            // 
            // cbxFonts
            // 
            this.cbxFonts.FormattingEnabled = true;
            this.cbxFonts.Location = new System.Drawing.Point(84, 19);
            this.cbxFonts.Name = "cbxFonts";
            this.cbxFonts.Size = new System.Drawing.Size(233, 21);
            this.cbxFonts.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(13, 327);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save Data";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtSaveFileName
            // 
            this.txtSaveFileName.Location = new System.Drawing.Point(95, 329);
            this.txtSaveFileName.Name = "txtSaveFileName";
            this.txtSaveFileName.Size = new System.Drawing.Size(235, 20);
            this.txtSaveFileName.TabIndex = 5;
            this.txtSaveFileName.Text = "save";
            // 
            // ToggleGazeRect
            // 
            this.ToggleGazeRect.Location = new System.Drawing.Point(13, 357);
            this.ToggleGazeRect.Name = "ToggleGazeRect";
            this.ToggleGazeRect.Size = new System.Drawing.Size(76, 23);
            this.ToggleGazeRect.TabIndex = 6;
            this.ToggleGazeRect.Text = "Gaze Rect";
            this.ToggleGazeRect.UseVisualStyleBackColor = true;
            this.ToggleGazeRect.Click += new System.EventHandler(this.ToggleGazeRect_Click);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ToggleGazeRect);
            this.Controls.Add(this.txtSaveFileName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbxFonts);
            this.Controls.Add(this.imageView);
            this.Controls.Add(this.textView);
            this.Controls.Add(this.Recalibrate);
            this.Name = "ControlPanel";
            this.Text = "ControlPanel";
            this.gbxFonts.ResumeLayout(false);
            this.gbxFonts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Recalibrate;
        private System.Windows.Forms.Button textView;
        private System.Windows.Forms.Button imageView;
        private System.Windows.Forms.GroupBox gbxFonts;
        private System.Windows.Forms.ComboBox cbxFonts;
        private System.Windows.Forms.Label lblFontOptions;
        private System.Windows.Forms.Button btnApplyFonts;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.NumericUpDown numFontSize;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtSaveFileName;
        private System.Windows.Forms.Button ToggleGazeRect;
    }
}