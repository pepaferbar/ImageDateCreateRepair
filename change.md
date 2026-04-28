# Změny - 28.04.2026
- **Cíl:** Omezení velikosti logu v UI na 3000 řádků kvůli stabilitě při velkých dávkách souborů.
- **Soubory:**
  - `MainForm.cs`: Přidán limit logu (`MaxLogLines = 3000`) a průběžné odmazávání nejstarších řádků před přidáním nové zprávy.
  - `MainForm.Designer.cs`: Přidán `chkFixHeicDate` checkbox (ve výchozím stavu vypnut).
  - `MainForm.cs`: Robustnější aktualizace progress baru při změně rozsahu fází.
  - `ImageProcessor.cs`: Implementována volitelná oprava datumu u HEIC souborů (zelený výpis) ve Fázi 1, přidán explicitní reset progresu pro každou fázi a započítán progres i pro Fázi 3.
