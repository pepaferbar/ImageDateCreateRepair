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

      // Propojení logování v RichTextBoxu (pokud byste ho použili)
      // Ale pro TextBox musíme logiku napsat ručně.
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        txtPath.Text = folderBrowserDialog.SelectedPath;
      }
    }

    // Klíčová metoda - spouští zpracování asynchronně
    private async void btnStart_Click(object sender, EventArgs e)
    {
      string path = txtPath.Text;
      if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
      {
        MessageBox.Show("Prosím, vyberte platný adresář.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // --- UI Zamknutí ---
      SetControlsEnabled(false);
      txtLog.Clear();
      progressBar.Value = 0;

      // --- Příprava na hlášení stavu ---
      // 'Progress<T>' zajistí, že hlášení z 'ImageProcessor'
      // se vždy vykoná na UI vlákně.
      var progress = new Progress<ProgressReport>(report =>
      {
        // Aktualizace logu
        if (!string.IsNullOrWhiteSpace(report.Message))
        {
          // Přidáme text do logu s barvou
          AppendLog(report.Message, report.GetColor());
        }

        // Aktualizace ProgressBaru
        if (report.TotalProgress.HasValue)
        {
          progressBar.Maximum = report.TotalProgress.Value;
        }
        if (report.CurrentProgress.HasValue)
        {
          progressBar.Value = report.CurrentProgress.Value;
        }
      });

      // --- Spuštění logiky na pozadí ---
      try
      {
        var processor = new ImageProcessor(path);

        // Spustíme těžkou práci na jiném vlákně, aby UI nezamrzlo
        await Task.Run(() => processor.ProcessFiles(progress));

        AppendLog("--- HOTOVO ---", Color.Blue);
      }
      catch (Exception ex)
      {
        AppendLog($"FATÁLNÍ CHYBA: {ex.Message}", Color.Red);
      }
      finally
      {
        // --- UI Odemknutí ---
        SetControlsEnabled(true);
        progressBar.Value = 0; // Vynulovat
      }
    }

    // Pomocná metoda pro zamknutí/odemknutí UI
    private void SetControlsEnabled(bool enabled)
    {
      btnStart.Enabled = enabled;
      btnBrowse.Enabled = enabled;
      txtPath.Enabled = enabled;
    }

    // Pomocná metoda pro barevné logování
    // Poznámka: Obyčejný TextBox nepodporuje barvy.
    // Pro barvy nahraďte 'TextBox' za 'RichTextBox' a odkomentujte:
    private void AppendLog(string message, Color color)
    {
      /*
      // Pro RichTextBox
      txtLog.SelectionStart = txtLog.TextLength;
      txtLog.SelectionLength = 0;
      txtLog.SelectionColor = color;
      txtLog.AppendText(message + Environment.NewLine);
      txtLog.SelectionColor = txtLog.ForeColor; // Vrátit barvu
      */

      // Pro obyčejný TextBox (bez barev)
      txtLog.AppendText(message + Environment.NewLine);
      txtLog.ScrollToCaret(); // Automatické rolování
    }

  }
}