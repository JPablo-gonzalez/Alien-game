module App.GameOver

open App.Types

let mostrarMenu x y =
    [
        GameOverCommand.NewGame,"Empezar de Nuevo"
        GameOverCommand.Exit,"Salir"
    ]
    |> Menu.mostrarMenu x y