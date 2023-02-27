open System
open System.IO
open System.Collections.Generic

open Parser
open FParsec
open WavePacker

// TODO 8 take input from file/cmd and save it to a file
[<EntryPoint>]
let main argv =
    let scoreString = argv.[0] // get score string from command line
    // parse score string using Parser.pScore
    let score = fun scoreString -> 
        match run pScore scoreString with
        | Success(result, _, _) -> result
        | Failure(errorMsg, _, _) -> failwith errorMsg

    
    let scoreFile = "score.txt"
    File.WriteAllText(scoreFile, (score scoreString).ToString())

    0 
