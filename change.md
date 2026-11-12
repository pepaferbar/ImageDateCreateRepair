# Změny - 12.11.2026
- **Cíl:** Konfigurace repa – identita a publikace exe, ikona, české README, licence a formátování.
- **Soubory:**
  - `DateCreateRepair2.csproj`: `AssemblyName`/`Product` = ImageDateCreateRepair, `AssemblyTitle`, `Version` 1.0.0, `Authors`, `Description`, `Copyright`, `ApplicationIcon`; publikace `win-x64`, `SelfContained=false`, `PublishSingleFile`, `IncludeNativeLibrariesForSelfExtract`; v Release `DebugType=embedded`.
  - `app.ico`: Nová ikona aplikace (fotka + kalendář, 16–256 px).
  - `MainForm.cs`: Ikona okna převzatá z exe.
  - `MainForm.Designer.cs`: Titulek okna změněn na „Image DataCreate Repair“.
  - `README.md`: Přepsáno česky – požadavky, použití, volby, fáze zpracování, složky `_heic`/`_notsupport`, upozornění na zálohu, sestavení.
  - `docs/screenshot.png`: Snímek hlavního okna pro README.
  - `LICENSE`: MIT.
  - `.editorconfig`: Odsazení 2 mezery, UTF-8, CRLF.
  - `.gitattributes`: `* text=auto`, CRLF pro sln/csproj, binární obrázky.

# Změny - 28.04.2026
- **Cíl:** Omezení velikosti logu v UI na 3000 řádků kvůli stabilitě při velkých dávkách souborů.
- **Soubory:**
  - `MainForm.cs`: Přidán limit logu (`MaxLogLines = 3000`) a průběžné odmazávání nejstarších řádků před přidáním nové zprávy.
  - `MainForm.Designer.cs`: Přidán `chkFixHeicDate` checkbox (ve výchozím stavu vypnut).
  - `MainForm.cs`: Robustnější aktualizace progress baru při změně rozsahu fází.
  - `ImageProcessor.cs`: Implementována volitelná oprava datumu u HEIC souborů (zelený výpis) ve Fázi 1, přidán explicitní reset progresu pro každou fázi a započítán progres i pro Fázi 3.
