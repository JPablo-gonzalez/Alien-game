module App.Navigator
open System
open App.Types

type NavigatorState =
| ShowMainMenu
| ShowGame
| ShowPause
| ShowGameOver
| Terminated

type State = {
    NavigatorState: NavigatorState
    SavedGame: App.Game.State option 
}

let initState() =
    {
        NavigatorState = ShowMainMenu
        SavedGame = None
    }

let showMainMenu state =
    Console.Clear()
    Utils.displayMessageGigante 46 2 ConsoleColor.Green "Alien"
    Utils.displayMessageGigante 42 10 ConsoleColor.Red "Attack!"
    
    match MainMenu.mostrarMenu 60 20 with
    | GameCommand.NewGame ->
        {state with NavigatorState=ShowGame; SavedGame=None}
    
    | GameCommand.LoadGame ->
        match App.Guardado.gestionarCarga state.SavedGame with
        | Some juegoCargado -> 
            {state with NavigatorState=ShowGame; SavedGame = Some juegoCargado}
        | None -> 
            Utils.displayMessage 50 24 ConsoleColor.Red "⚠️ No hay ninguna partida guardada."
            System.Threading.Thread.Sleep(2000)
            state 

    | GameCommand.Exit ->
        {state with NavigatorState=Terminated}

let showGame state =
    Console.Clear()
    let (status, gameState) = App.Game.mostrarJuego state.SavedGame
    
    match status with
    | GamesStatus.Pause -> 
         {state with NavigatorState=ShowPause; SavedGame = Some gameState}
    | GamesStatus.GameOver ->
        {state with NavigatorState=ShowGameOver; SavedGame = None}

let showGameOver state =
    Console.Clear()
    Utils.displayMessageGigante 34 10 ConsoleColor.Red "Game Over"
    match GameOver.mostrarMenu 60 20 with
    | GameOverCommand.NewGame ->
        {state with NavigatorState=ShowGame; SavedGame=None}
    | GameOverCommand.Exit ->
        { state with NavigatorState=Terminated}

let showPause state =
    Console.Clear()
    Utils.displayMessageGigante 46 5 ConsoleColor.Magenta "Paused"
    match PauseMenu.mostrarMenu 60 15 with
    | PauseCommand.ContinueGame ->
        {state with NavigatorState=ShowGame}
    
    | PauseCommand.SaveGame ->
        App.Guardado.gestionarGuardado state.SavedGame
        
        {state with NavigatorState=Terminated}
        
    | PauseCommand.Exit ->
        {state with NavigatorState=Terminated}

let updateState state =
    match state.NavigatorState with
    | ShowMainMenu -> showMainMenu state
    | ShowGame -> showGame state
    | ShowPause -> showPause state
    | ShowGameOver -> showGameOver state
    | _ -> state

let rec mainLoop state =
    let newState = state |> updateState
    if newState.NavigatorState <> Terminated then
        mainLoop newState

let mostrarNavegador() =
    initState()
    |> mainLoop