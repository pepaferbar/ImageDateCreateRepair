using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DateCreateRepair2
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        txtPath.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
      string path = txtPath.Text;
      if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
      {
        MessageBox.Show("Prosím, vyberte platný adresář.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      SetControlsEnabled(false);
      txtLog.Clear();
      progressBar.Value = 0;

      var progress = new Progress<ProgressReport>(report =>
      {
        if (!string.IsNullOrWhiteSpace(report.Message))
        {
          // TATO METODA JE NYNÍ AKTUALIZOVÁNA
          AppendLog(report.Message, report.GetColor());
        }

        if (report.TotalProgress.HasValue)
        {
          progressBar.Maximum = report.TotalProgress.Value;
        }
        if (report.CurrentProgress.HasValue)
        {
          progressBar.Value = report.CurrentProgress.Value;
        }
      });

      try
      {
        var processor = new ImageProcessor(path);

        await Task.Run(() => processor.ProcessFiles(progress));

        AppendLog("--- HOTOVO ---", Color.Blue);
      }
      catch (Exception ex)
      {
        AppendLog($"FATÁLNÍ CHYBA: {ex.Message}", Color.Red);
      }
      finally
      {
        SetControlsEnabled(true);
        progressBar.Value = 0;
      }
    }

    private void SetControlsEnabled(bool enabled)
    {
      btnStart.Enabled = enabled;
      btnBrowse.Enabled = enabled;
      txtPath.Enabled = enabled;
    }

    // ---------- UPRAVENÁ METODA ----------
    // Tato metoda nyní plně využívá RichTextBox pro barevný výpis.
    private void AppendLog(string message, Color color)
    {
      // Zkontrolujeme, jestli operaci nevoláme z jiného vlákna
      if (txtLog.InvokeRequired)
      {
        // Pokud ano, převoláme ji bezpečně na UI vlákno
        txtLog.Invoke(new Action(() => AppendLog(message, color)));
      }
      else
      {
        // Toto je kód pro RichTextBox
        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = color; // Nastavíme barvu
        txtLog.AppendText(message + Environment.NewLine);
        txtLog.SelectionColor = txtLog.ForeColor; // Vrátíme barvu na výchozí
        txtLog.ScrollToCaret(); // Automatické rolování
      }
    }
  }
}