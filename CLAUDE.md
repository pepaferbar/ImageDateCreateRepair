# ImageDateCreateRepair – architektura

*Aktualizováno: 13. 11. 2026 skillem `/architektura`. Zdroj pravdy je kód – při rozporu platí kód.*

## Účel
Windows WinForms aplikace: ve vybrané složce nastaví fotkám **datum změny** (`LastWriteTime`, ne datum vytvoření) podle data pořízení z EXIF, volitelně převede HEIC na JPG a nepodporované soubory odklidí. Uživatelská dokumentace je v `README.md`.

## Vstupní body
- `Program.cs` – `Main`, spustí `MainForm` (alias `Application = System.Windows.Forms.Application` kvůli kolizi s Magick.NET, CS0104).
- `MainForm.cs` – výběr složky, checkboxy `convertHeic` (výchozí zapnuto) a `chkFixHeicDate` (výchozí vypnuto), `btnStart_Click` spustí `ImageProcessor.ProcessFiles` přes `Task.Run`, průběh přes `IProgress<ProgressReport>`.

## Struktura
| Cesta | Obsah |
|---|---|
| `ImageProcessor.cs` | Celé zpracování ve 3 fázích, každá `Parallel.ForEach`, počítadla přes `Interlocked`, na konci souhrn do logu |
| `ExifData.cs` | `DateTaken()` přes MetadataExtractor: `DateTimeOriginal` z ExifSubIfd → z IFD0 → záloha `DateTime` z IFD0 |
| `HeicConverter.cs` | `ConvertHeicToJpg()` přes `MagickImage` (volá se s kvalitou 95) |
| `ProgressReport.cs` | `ReportType` (Info/Success/Warning/Error/Detail) + barva řádku logu; prázdná zpráva s `Current/TotalProgress` jen posouvá progress bar |
| `MainForm.Designer.cs`, `MainForm.resx` | Návrh okna |
| `DateCreateRepair2.csproj` / `.sln` | Projekt (název souborů zůstal po původním jménu) |
| `app.ico`, `docs/screenshot.png`, `LICENSE` (MIT), `.editorconfig` (2 mezery, CRLF) | |

## Průběh zpracování (`ProcessFiles`)
1. Vytvoří `_heic` a `_notsupport` ve vybrané složce (vždy, i když zůstanou prázdné). Bere jen soubory přímo ve složce, ne podsložky.
2. **Fáze 1 – HEIC:** volitelně `SetLastWriteTime` podle EXIF; při konverzi vznikne `.jpg` vedle originálu (existující JPG se nepřepíše, ale počítá se jako „převedeno“) a originál se přesune do `_heic`.
3. **Fáze 2 – JPG/JPEG:** seznam se načte znovu (zahrne i nově převedené JPG); datum se nastaví, jen když se liší.
4. **Fáze 3:** vše kromě `.jpg/.jpeg/.heic` (ze seznamu z bodu 1) se přesune do `_notsupport`; existující cíl se nepřepisuje.

## Závislosti mimo složku
- NuGet: `Magick.NET-Q16-AnyCPU` 14.9.1 (HEIC → JPG), `MetadataExtractor` 2.9.0 (EXIF).
- Běh vyžaduje .NET 8 Desktop Runtime (není self-contained).

## Nasazení / build
- `net8.0-windows`, `AssemblyName` = `ImageDateCreateRepair` (namespace zůstává `DateCreateRepair2`), verze 1.0.0.
- `dotnet publish DateCreateRepair2.csproj -c Release` → jeden exe (`PublishSingleFile`, `win-x64`, `SelfContained=false`, `IncludeNativeLibrariesForSelfExtract`), v Release `DebugType=embedded`.
- Výstup: `bin\Release\net8.0-windows\win-x64\publish\ImageDateCreateRepair.exe`. Instalátor (`.iss`) není.

## Záměrná omezení a pasti
- Mění se `LastWriteTime` (datum změny), ne `CreationTime` – název projektu je zavádějící, tak to má být (README).
- Log v UI je omezen na `MaxLogLines = 3000` (stabilita u velkých dávek) – neodstraňovat.
- `Report()` se volá z paralelních vláken; UI aktualizace jdou přes `Progress<T>` (synchronizační kontext formuláře).
- Aplikace soubory přesouvá a vytváří nové – žádné „undo“.
