
open System

// TODO 8 take input from file/cmd and save it to a file
[<EntryPoint>]
let main argv =
    let score = "32.#d3 16-"
    match Assembler.assembleToPackedStream score with
            | Choice2Of2 ms ->
                failwith "Save file"
            | Choice1Of2 err -> failwith err