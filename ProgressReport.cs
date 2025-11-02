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

        // UPRAVENO: Oranžová nahrazena tmavší zlatou pro lepší čitelnost
        ReportType.Warning => Color.DarkGoldenrod,

        // UPRAVENO: Změněno na šedou pro odlišení od Info
        ReportType.Detail => Color.Gray,

        _ => Color.Black,
      };
    }
  }
}