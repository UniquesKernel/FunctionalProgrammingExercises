// import FParsec

open FParsec
open System.IO
open System.Text
open Microsoft.FSharp.Text

type Duration = float

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

type Octave = int

type Tone = {note: Note; octave: Octave} 
type RestTone = Rest

type Pitch = 
    | Tone of Tone
    | Rest of RestTone

type Token = {pitch: Pitch; duration: Duration}

let durationParser: Parser<float, unit> =
    let intParser: Parser<int, unit> = pint32
    let trailingDotParser: Parser<char, unit> = pchar '.'
    let multiplyBy1_5 (durationInt: int): float = float durationInt * 1.5
    let convertToFloat (durationInt: int): float = float durationInt

    let floatWithTrailingDotParser: Parser<float, unit> = 
        (intParser |>> multiplyBy1_5) .>> trailingDotParser

    let floatWithoutTrailingDotParser: Parser<float, unit> = 
        (intParser |>> convertToFloat)

    let durationParser': Parser<float, unit> = 
        attempt floatWithTrailingDotParser <|> 
        attempt floatWithoutTrailingDotParser

    durationParser'

let semitonesBetween (lower: Tone) (upper: Tone): int = 
    let offSets = [ A, 0; ASharp, 1; B, 2; C, 3; CSharp, 4; D, 5; DSharp, 6; E, 7; F, 8; FSharp, 9; G, 10; GSharp, 11
    ] 

    let noteOffSetMap = Map.ofList offSets

    let lowerOffSet = noteOffSetMap.[lower.note]
    let upperOffSet = noteOffSetMap.[upper.note]

    let semitones = (upper.octave - lower.octave + 1) * 12 + (upperOffSet - lowerOffSet) 

    semitones


let noteParser: Parser<Note, unit> = 
    // create a note parser that takes a string and returns a Note the strings are the note names

    let noteParser': Parser<Note, unit> =
        attempt (pchar '#' >>. (pchar 'A') >>. preturn Note.ASharp) <|>
        attempt (pchar 'A' >>. preturn Note.A) <|>
        attempt (pchar 'B' >>. preturn Note.B) <|>
        attempt (pchar '#' >>. followedBy (pchar 'C') >>. preturn Note.CSharp) <|>
        attempt (pchar 'C' >>. preturn Note.C) <|>
        attempt (pchar '#' >>. followedBy (pchar 'D') >>. preturn Note.DSharp) <|>
        attempt (pchar 'D' >>. preturn Note.D) <|>
        attempt (pchar 'E' >>. preturn Note.E) <|>
        attempt (pchar '#' >>. followedBy (pchar 'F') >>. preturn Note.FSharp) <|>
        attempt (pchar 'F' >>. preturn Note.F) <|>
        attempt (pchar '#' >>. followedBy (pchar 'G') >>. preturn Note.GSharp) <|>
        attempt (pchar 'G' >>. preturn Note.G)

    noteParser'

let octaveParser: Parser<Octave, unit> = 
    let intParser = pint32

    intParser


let toneParser: Parser<Tone, unit> = 
    let noteParser = noteParser
    let octaveParser = octaveParser

    let toneParser' = 
        noteParser .>>. octaveParser |>> fun (note, octave) -> {note = note; octave = octave}

    toneParser' 

let restParser: Parser<RestTone, unit> =
    let restParser' = 
        pchar '-' >>. preturn RestTone.Rest

    restParser'

let pitchParser: Parser<Pitch, unit> = 
    let toneParser = toneParser
    let restParser = restParser

    let pitchParser' = 
        attempt (toneParser |>> fun tone -> Pitch.Tone tone) <|>
        attempt (restParser |>> fun rest -> Pitch.Rest rest)

    pitchParser'

let tokenParser: Parser<Token, unit> =
    let pitchParser = pitchParser
    let durationParser = durationParser

    let tokenParser' = 
        pipe2 durationParser pitchParser (fun duration pitch -> {pitch = pitch; duration = duration})

    let tokenParser'' = 
        tokenParser' .>> spaces

    tokenParser''



let applyParser (input:string) (parser: Parser<'a, unit>) = 
    match run parser input with
    | Success(result, _, _) -> printfn "%A" result
    | Failure(errorMsg, _, _) -> failwith errorMsg

let applyTokenParser (input:string) = 
    match run tokenParser input with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith errorMsg

applyParser "2" durationParser // expect 2.0
applyParser "2." durationParser // expect 3.0

applyParser "A" noteParser // expect A
applyParser "#A" noteParser // expect ASharp

applyParser "-" restParser // expect Rest

applyParser "A2" pitchParser // expect Tone {note = A; octave = 2}
applyParser "#A2" pitchParser // expect Tone {note = ASharp; octave = 2}

applyParser "2A2" tokenParser // expect {pitch = Tone {note = A; octave = 2}; duration = 2.0}
applyParser "2#A2" tokenParser // expect {pitch = Tone {note = ASharp; octave = 2}; duration = 2.0}
applyParser "2-" tokenParser // expect {pitch = Rest RestTone.Rest; duration = 2.0}
applyParser "2.-" tokenParser // expect {pitch = Rest RestTone.Rest; duration = 3.0}
applyParser "2.A2" tokenParser // expect {pitch = Tone {note = A; octave = 2}; duration = 2.0}
applyParser "2.#A2" tokenParser // expect {pitch = Tone {note = ASharp; octave = 2}; duration = 2.0}
applyParser "2.E2" tokenParser 

printfn "------------------"

// apply tokenParser to a string of tokens 

let result = "2.A2 2.#A2 2.E2"

let resultList = result.Split(' ') |> Array.map (fun token -> applyTokenParser token)
resultList |> Array.iter (fun token -> printfn "%A" token)

printfn "------------------"

let lowerTone = {note = C; octave = 0}
let upperTone = {note = B; octave = 8}

let semitones = semitonesBetween lowerTone upperTone

printfn "%i" semitones

applyParser result (many1 tokenParser)
