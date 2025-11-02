using System;
using System.Drawing;

namespace DateCreateRepair2
{
  public enum ReportType
  {
    Info,
    Success,
    Warning,
    Error,
    Detail
  }

  public class ProgressReport
  {
    // PŘIŘAZENA VÝCHOZÍ HODNOTA pro opravu varování CS8618
    public string Message { get; set; } = string.Empty;

    public ReportType Type { get; set; } = ReportType.Info;

    public int? CurrentProgress { get; set; }
    public int? TotalProgress { get; set; }

    public Color GetColor()
    {
      return Type switch
      {
        ReportType.Success => Color.Green,
        ReportType.Error => Color.Red,
        ReportType.Warning => Color.Orange,
        ReportType.Detail => Color.Gray,
        _ => Color.Black,
      };
    }
  }
}