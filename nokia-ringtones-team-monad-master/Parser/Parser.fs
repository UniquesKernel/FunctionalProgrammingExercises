module Parser

open FParsec

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

let noteToNumber note = 
    match note with
        | A -> 1
        | ASharp -> 2
        | B -> 3
        | C -> 4
        | CSharp -> 5
        | D -> 6
        | DSharp -> 7
        | E -> 8
        | F -> 9
        | FSharp -> 10
        | G -> 11
        | GSharp -> 12
    
let durationParser: Parser<float, unit> =
    let intParser: Parser<int, unit> = pint32
    let trailingDotParser: Parser<char, unit> = pchar '.'
    let multiplyBy1_5 (durationInt: int): float = float durationInt * 1.5
    let convertToFloat (durationInt: int): float = float durationInt

    let floatWithTrailingDotParser: Parser<float, unit> = 
        (intParser |>> multiplyBy1_5) .>> followedBy trailingDotParser

    let floatWithoutTrailingDotParser: Parser<float, unit> = 
        (intParser |>> convertToFloat)

    let durationParser': Parser<float, unit> = 
        attempt floatWithTrailingDotParser <|> floatWithoutTrailingDotParser

    durationParser'

let noteParser: Parser<Note, unit> =
    let noteParser': Parser<Note, unit> = 
        attempt (pchar '#' >>. (pchar 'A') >>. preturn Note.ASharp) <|>
        attempt (pchar 'A' >>. preturn Note.A) <|>
        attempt (pchar 'B' >>. preturn Note.B) <|>
        attempt (pchar '#' >>. (pchar 'C') >>. preturn Note.CSharp) <|>
        attempt (pchar 'C' >>. preturn Note.C) <|>
        attempt (pchar '#' >>. (pchar 'D') >>. preturn Note.DSharp) <|>
        attempt (pchar 'D' >>. preturn Note.D) <|>
        attempt (pchar 'E' >>. preturn Note.E) <|>
        attempt (pchar '#' >>. (pchar 'F') >>. preturn Note.FSharp) <|>
        attempt (pchar 'F' >>. preturn Note.F) <|>
        attempt (pchar '#' >>. (pchar 'G') >>. preturn Note.GSharp) <|>
        attempt (pchar 'G' >>. preturn Note.G)

    noteParser'

let octaveParser: Parser<Octave, unit> =
    let intParser = pint32

    intParser

let toneParser: Parser<Tone, unit> = 
    let noteParser: Parser<Note, unit> = noteParser
    let octaveParser: Parser<Octave, unit> = octaveParser

    let toneParser': Parser<Tone, unit> = 
        noteParser .>>. octaveParser |>> (fun (note, octave) -> {note = note; octave = octave})

    toneParser'

let restParser: Parser<RestTone, unit> = 
    let restParser': Parser<RestTone, unit> = 
        pchar '-' >>. preturn RestTone.Rest

    restParser'

let pitchParser: Parser<Pitch, unit> = 
    let toneParser: Parser<Tone, unit> = toneParser
    let restParser: Parser<RestTone, unit> = restParser

    let pitchParser': Parser<Pitch, unit> = 
        attempt (toneParser |>> fun tone -> Pitch.Tone tone) <|>
        attempt (restParser |>> fun rest -> Pitch.Rest rest)

    pitchParser'

let tokenParser: Parser<Token, unit> = 
    let pitchParser: Parser<Pitch, unit> = pitchParser
    let durationParser: Parser<float, unit> = durationParser

    let tokenParser' =
        pipe2 durationParser pitchParser (fun duration pitch -> {pitch = pitch; duration = duration})

    let tokenParser'' = 
        tokenParser' .>> spaces

    tokenParser''
    
let pScore: Parser<Token list, unit> = 
    let tokenParser: Parser<Token, unit> = tokenParser

    many1 tokenParser
    //failwith "Implement here" // TODO 2 builder parser
    

let parse (input: string): Choice<string, Token list> =
    match run pScore input with
    | Failure(errorMsg,_,_)-> Choice1Of2(errorMsg)
    | Success(result,_,_) -> Choice2Of2(result)

// Helper function to test parsers
let test (p: Parser<'a, unit>) (str: string): unit =
    match run p str with
    | Success(result, _, _) ->  printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg


// TODO 3 calculate duration from token.
// bpm = 120 (bpm = beats per minute)
// 'Duration in seconds' * 1000 * 'seconds per beat' (if extended *1.5)
// Whole note: 4 seconds
// Half note: 2 seconds
// Quarter note: 1 second
// Eight note: 1/2 second
// Sixteenth note 1/4 second
// thirty-second note: 1/8
let durationFromToken (token: Token): float =
    let secondsPerBeat = 60.0 / 120.0
    token.duration * secondsPerBeat * 1000.0

// TODO 4 calculate overall index of octave
// note index + (#octave-1) * 12
let overallIndex (note, octave) = 
    let octaveIndex = noteToNumber note + (octave - 1) * 12
    octaveIndex
    
    // failwith "Implement here"

// TODO 5 calculate semitones between to notes*octave
// [A; A#; B; C; C#; D; D#; E; F; F#; G; G#]
// overallIndex upper - overallIndex lower
let semitonesBetween (lower: Tone) (upper: Tone): int = 
    let offSets = [
        A, 0; ASharp, 1; B, 2;
        C, 3; CSharp, 4; D, 5;
        DSharp, 6; E, 7; F, 8;
        FSharp, 9; G, 10; GSharp, 11
    ]
    
    let noteOffSetMap = Map.ofList offSets

    let lowerOffset = noteOffSetMap.[lower.note]
    let upperOffset = noteOffSetMap.[upper.note]

    let semitones = (upper.octave - lower.octave) * 12 + (upperOffset - lowerOffset)
    semitones

// TODO 6
// For a tone frequency formula can be found here: http://www.phy.mtu.edu/~suits/NoteFreqCalcs.html
// 220 * 2^(1/12) ^ semitonesBetween (A1, Token.pitch) 
let frequency (token: Token): float = failwith "Implement here"
