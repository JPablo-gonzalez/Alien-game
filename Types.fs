module App.Types


type GameCommand =
| NewGame
| LoadGame
| Exit

type PauseCommand =
| ContinueGame
| SaveGame
| Exit

type GameOverCommand=
| NewGame
| Exit

type GamesStatus =
| Pause
| GameOver

type GameState = {
    PlayerX: int
    PlayerY: int
    AlienLives: int
    Score: int
    Enemy: (int * int) list
}