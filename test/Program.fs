// import FParsec

open FParsec

// create a type of Duration that be either an integer or extended integer
type Duration = float

// create a parser that will parse Tone that can be a string of 
// either "A", "#A", "B", "C", "#C", "D", "#D", "E", "F", "#F", "G", "#G"

type Note = 
    | A
    | ASharp
    | B
    | C
    | CSharp
    | D
    | DSharp
    | E
    | F
    | FSharp
    | G
    | GSharp
    | None

type Octave = int

type Pitch = 
    | Pitch of Note * Octave
    | Rest

type Token = 
    | Token of Pitch * Duration

// take a string and parse it to a Duration

let durationParser (input: string): int =
    let floatLiteral : Parser<int, unit> = pint32
    let trailingDot : Parser<char, unit> = pchar '.'
    let multiplyBy1_5 (f : int) : int = f + 2

    let FloatWithDot = (floatLiteral |>> multiplyBy1_5) .>> followedBy (trailingDot) 

    let result = attempt FloatWithDot <|> floatLiteral

    match run result input with
    | Success (result, _, _) -> result
    | Failure (errMsg, _, _) -> failwith errMsg


let octaveParser (input: string): float = 
    match run pfloat input with
    | Success (result, _, _) -> result
    | Failure (errMsg, _, _) -> failwith errMsg


let result = durationParser "3"
let result2 = durationParser "3."

printfn "%A" result
printfn "%A" result2
