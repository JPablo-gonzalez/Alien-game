module App.Guardado

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open App.Game 

let rutaArchivo = "savegame.json"


type SpriteStateConverter() =
    inherit JsonConverter<SpriteState>()
    override _.Write(writer, value, options) =
        let str = 
            match value with
            | Vivo -> "Vivo"
            | Colisionado -> "Colisionado"
            | Dormido -> "Dormido"
        writer.WriteStringValue(str)

    override _.Read(reader, typeToConvert, options) =
        match reader.GetString() with
        | "Colisionado" -> Colisionado
        | "Dormido" -> Dormido
        | _ -> Vivo 

type ProgramStateConverter() =
    inherit JsonConverter<ProgramState>()
    override _.Write(writer, value, options) =
        let str = 
            match value with
            | Running -> "Running"
            | Terminated -> "Terminated"
        writer.WriteStringValue(str)

    override _.Read(reader, typeToConvert, options) =
        match reader.GetString() with
        | "Terminated" -> Terminated
        | _ -> Running

let obtenerOpciones() =
    let opciones = JsonSerializerOptions(WriteIndented = true)
    opciones.IncludeFields <- true 
    opciones.Converters.Add(SpriteStateConverter())
    opciones.Converters.Add(ProgramStateConverter())
    opciones


let escribirEnDisco (state: State) =
    let opciones = obtenerOpciones()
    let json = JsonSerializer.Serialize(state, opciones)
    File.WriteAllText(rutaArchivo, json)

let leerDeDisco () =
    if File.Exists(rutaArchivo) then
        try
            let json = File.ReadAllText(rutaArchivo)
            
            if String.IsNullOrWhiteSpace(json) then
                None
            else
                let opciones = obtenerOpciones()
                let state = JsonSerializer.Deserialize<State>(json, opciones)
                Some state
        with _ -> 
            None
    else
        None



let gestionarGuardado (estadoEnMemoria: State option) =
    match estadoEnMemoria with
    | Some juego -> escribirEnDisco juego
    | None -> ()

let gestionarCarga (estadoEnMemoria: State option) =
    match estadoEnMemoria with
    | Some juegoEnRam -> 
        Some juegoEnRam
    | None -> 
        leerDeDisco()