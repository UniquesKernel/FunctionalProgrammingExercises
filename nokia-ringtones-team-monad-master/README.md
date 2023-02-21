# Assignment 1

This exercise is about parsing a simplified RTTL (Ring Tone Transfer Language). A file format that was developed by Nokia to transfer ringtones between phones.

The format specification can be found here: https://en.wikipedia.org/wiki/Ring_Tone_Transfer_Language

The language we are defining are:

A token is a pair with a duration and a pitch

Duration:
- Half note
- Quarter note
- Eigth note
- Sixteenth note
- Thirty-second note

Durations can be 'normal' or 'extended'

Notes:
A, A#, B, C, C#, D, D#, E, F, F#, G, G#

Octaves:
1, 2, 3

Pitch is either a rest or a tone consisting of a note and a octave 

*Read the whole assignment before starting to code*

## Part 1 - Parser

Here we need to create a parser that parses the files into a list of notes (Token list)

There are added tests in the project ParserTests - these should help you on your way, but they do not test all cases.

An examples of a score string could be

    1d2 4d2 4e2 2#f2 1g2 4a2 4a1 4d2 4#c2 8#f2 8#f2 8e2 4#f2 4e2 4#f2 4e2 8e2 4b2 4a2 8#f2 8#f2 8e2 4#f2 4e2 4d2 4d2 8e2 2e2 8#f2 8#f2 8e2 4#f2 4e2 4#f2 4e2 8e2 4b2 4a2 8#f2 8#f2 8e2 4#f2 4e2 4d2 4d2 8e2 2e2

Notice that '#' is before the note-letter

**a)** In Parser.fs Start be defining the types for the language defined above. (TODO 1)

**b)** In Parser.fs: pScore is the parser for a whole score. Create a suitable number of parsers, which you can use to compose the pScore parser. (TODO 2)

## Part 2 - Helper functions

**c)** In Parser.fs: We need a couple of helper functions (TODO 3-6) where we need ot calculate frequency and duration of token

## Part 3 - Create binary data

**d)** In WavePacker.fs: The data (now in a int16[]) should now be saved into a Stream in the correct format, so that it can be saved to a file. The format for this file is Wave (http://soundfile.sapp.org/doc/WaveFormat/) (TODO 7)

## Part 4 - Save files

**e)** In Program.fs: Load score string from cmd or file and save this to a file. (TODO 8)

**f)** Optional create ans Asp.Net project in F# and take input from a textarea and let wav file download

```f#
let cd = ContentDisposition(FileName = "ringring.wav", Inline=false)
this.Response.Headers.Add("Content-Disposition", StringValues(cd.ToString()))
ms.Position <- 0L
this.File(ms, "audio/x-wav")
```
