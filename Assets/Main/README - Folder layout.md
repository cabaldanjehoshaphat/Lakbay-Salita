# Assets/Main – folder layout

Purpose: where everything of the Lakbay Salita game lives. (Assets/Sub is the team's other work and is not part of this layout.)
Folder numbers follow the game order: 1 Menu, 2 Wordle, 3 Crossword, 4 Word Search, 5 Library.

| Folder | What is in it |
|---|---|
| `Scenes/1 Menu` | Main menu, Profile, language hub (3 Language_Selection_Menu), Library. `Archive` = older designs. |
| `Scenes/2 Wordle` | `Wordle - Start` (categories) and `Wordle - Play`. `Archive` = old design + the 30 level scenes from before categories. |
| `Scenes/3 Crossword` | `Crossword - Start`, `Crossword - Play`, `CrossWord - Generate`. `Archive/1st design`, `Archive/2nd design`. |
| `Scenes/4 Word Search` | `WordSearch - Start`, `WordSearch - Play`. |
| `Scripts/Core` | Shared code: player save (PlayerData, PlayerDatabase), StreakTracker, SceneNavigator, dictionary loading, LevelDataRegistry. |
| `Scripts/1 Menu` … `5 Library` | Code of each screen/game. `Editor` sub-folders hold the scene builder tools (editor only). |
| `Scripts/3 Crossword/Gameplay` | The crossword board, input and clue panel scripts (still used by the live game). |
| `Scripts/3 Crossword/Level Loader` | Older crossword level loader/generator. |
| `Scripts/Audio` | BackgroundMusic. |
| `Data/Wordle/Levels` | Category level JSON (7 categories x 6 levels per language). `Settings` = timer/text-style assets, `Legacy words` = the old one-word-per-file JSON. |
| `Data/Crossword/Puzzles` | Category puzzle JSON (7 categories x 3 sizes per language). `Settings`, `Legacy levels` as above. |
| `Data/Word Search/Puzzles` | Category puzzle JSON (7 categories x 3 puzzles per language). |
| `Data/Language Dictionary` | Library/validation dictionaries (cebuano, hiligaynon, tagalog). |
| `Data/Legacy` | Unused early word files, kept for reference. |
| `Prefabs`, `Sprites`, `Fonts`, `TextMesh Pro` | Art and UI pieces, grouped per game. |
| `Resources` | Must stay named `Resources` (loaded by name from code): `LevelDataRegistry.asset`, `Audio Soundtrack`, `Audio SFX`. Do not rename these folders or their files without changing the code. |

Rules of thumb
- Move or rename files only inside the Unity Project window (so the .meta files and links follow).
- New level data goes into `Data/<Game>/…` and is registered in `Resources/LevelDataRegistry.asset`.
