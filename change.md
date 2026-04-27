# Změny - 27.04.2026
- **Cíl:** Vylepšení konverze HEIC a přidání možnosti opravy datumu i u HEIC originálů.
- **Soubory:**
  - `MainForm.Designer.cs`: Přidán `chkFixHeicDate` checkbox (ve výchozím stavu vypnut).
  - `MainForm.cs`: Robustnější aktualizace progress baru při změně rozsahu fází.
  - `ImageProcessor.cs`: Implementována volitelná oprava datumu u HEIC souborů (zelený výpis) ve Fázi 1, přidán explicitní reset progresu pro každou fázi a započítán progres i pro Fázi 3.
