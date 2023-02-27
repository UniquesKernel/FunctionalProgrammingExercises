module ParserTests

open NUnit.Framework
open Parser

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``It should parse a simple score`` () =
    let score = "32.#d3 16-"
    let result = parse score
    
    let assertFirstToken token = failwith "test length: 32note extended, sound: D# 3octave"
    let assertSecondToken token = failwith "test length: 16 normal, sound: Pause"
         
    match result with
        | Choice1Of2 errorMsg -> Assert.Fail(errorMsg)
        | Choice2Of2 tokens ->
            Assert.AreEqual(2, List.length tokens)
            List.head tokens |> assertFirstToken
            List.item 1 tokens |> assertSecondToken

    


    
