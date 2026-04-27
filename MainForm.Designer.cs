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
      folderBrowserDialog = new FolderBrowserDialog();
      txtLog = new RichTextBox();
      chkConvertHeic = new CheckBox();
      chkFixHeicDate = new CheckBox();
      SuspendLayout();
      // 
      // txtPath
      // 
      txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtPath.Location = new Point(12, 13);
      txtPath.Name = "txtPath";
      txtPath.Size = new Size(653, 23);
      txtPath.TabIndex = 0;
      // 
      // btnBrowse
      // 
      btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnBrowse.Location = new Point(671, 13);
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
      btnStart.Location = new Point(752, 13);
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
      progressBar.Location = new Point(12, 71);
      progressBar.Name = "progressBar";
      progressBar.Size = new Size(815, 23);
      progressBar.TabIndex = 3;
      // 
      // txtLog
      // 
      txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtLog.BackColor = Color.LightGray;
      txtLog.BorderStyle = BorderStyle.FixedSingle;
      txtLog.Location = new Point(12, 100);
      txtLog.Name = "txtLog";
      txtLog.ReadOnly = true;
      txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;
      txtLog.Size = new Size(815, 362);
      txtLog.TabIndex = 4;
      txtLog.Text = "";
      // 
      // chkConvertHeic
      // 
      chkConvertHeic.AutoSize = true;
      chkConvertHeic.Checked = true;
      chkConvertHeic.CheckState = CheckState.Checked;
      chkConvertHeic.Location = new Point(12, 44);
      chkConvertHeic.Name = "chkConvertHeic";
      chkConvertHeic.Size = new Size(139, 19);
      chkConvertHeic.TabIndex = 5;
      chkConvertHeic.Text = "Převádět HEIC na JPG";
      chkConvertHeic.UseVisualStyleBackColor = true;
      // 
      // chkFixHeicDate
      // 
      chkFixHeicDate.AutoSize = true;
      chkFixHeicDate.Checked = false;
      chkFixHeicDate.CheckState = CheckState.Unchecked;
      chkFixHeicDate.Location = new Point(170, 44);
      chkFixHeicDate.Name = "chkFixHeicDate";
      chkFixHeicDate.Size = new Size(149, 19);
      chkFixHeicDate.TabIndex = 6;
      chkFixHeicDate.Text = "Opravit datum u HEIC";
      chkFixHeicDate.UseVisualStyleBackColor = true;
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(839, 474);
      Controls.Add(chkFixHeicDate);
      Controls.Add(chkConvertHeic);
      Controls.Add(txtLog);
      Controls.Add(progressBar);
      Controls.Add(btnStart);
      Controls.Add(btnBrowse);
      Controls.Add(txtPath);
      FormBorderStyle = FormBorderStyle.FixedSingle;
      Name = "MainForm";
      Text = "Image Date Create Repair";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox txtPath;
    private Button btnBrowse;
    private Button btnStart;
    private ProgressBar progressBar;
    private FolderBrowserDialog folderBrowserDialog;
    private RichTextBox txtLog;
    private CheckBox chkConvertHeic;
    private CheckBox chkFixHeicDate;
  }
}