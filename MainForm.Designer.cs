namespace DateCreateRepair2
{
  partial class MainForm
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
      txtPath = new TextBox();
      btnBrowse = new Button();
      btnStart = new Button();
      progressBar = new ProgressBar();
      txtLog = new TextBox();
      folderBrowserDialog = new FolderBrowserDialog();
      SuspendLayout();
      // 
      // txtPath
      // 
      txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtPath.Location = new Point(34, 23);
      txtPath.Name = "txtPath";
      txtPath.Size = new Size(603, 23);
      txtPath.TabIndex = 0;
      // 
      // btnBrowse
      // 
      btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnBrowse.Location = new Point(643, 23);
      btnBrowse.Name = "btnBrowse";
      btnBrowse.Size = new Size(75, 23);
      btnBrowse.TabIndex = 1;
      btnBrowse.Text = "...";
      btnBrowse.UseVisualStyleBackColor = true;
      btnBrowse.Click += btnBrowse_Click;
      // 
      // btnStart
      // 
      btnStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnStart.Location = new Point(724, 23);
      btnStart.Name = "btnStart";
      btnStart.Size = new Size(75, 23);
      btnStart.TabIndex = 2;
      btnStart.Text = "Start";
      btnStart.UseVisualStyleBackColor = true;
      btnStart.Click += btnStart_Click;
      // 
      // progressBar
      // 
      progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      progressBar.Location = new Point(34, 52);
      progressBar.Name = "progressBar";
      progressBar.Size = new Size(765, 23);
      progressBar.TabIndex = 3;
      // 
      // txtLog
      // 
      txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtLog.Location = new Point(34, 81);
      txtLog.Multiline = true;
      txtLog.Name = "txtLog";
      txtLog.ReadOnly = true;
      txtLog.ScrollBars = ScrollBars.Vertical;
      txtLog.Size = new Size(765, 381);
      txtLog.TabIndex = 4;
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(811, 474);
      Controls.Add(txtLog);
      Controls.Add(progressBar);
      Controls.Add(btnStart);
      Controls.Add(btnBrowse);
      Controls.Add(txtPath);
      Name = "MainForm";
      Text = "MainForm";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox txtPath;
    private Button btnBrowse;
    private Button btnStart;
    private ProgressBar progressBar;
    private TextBox txtLog;
    private FolderBrowserDialog folderBrowserDialog;
  }
}