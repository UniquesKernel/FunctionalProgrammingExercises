module WavePacker

open System.IO
open System.Text

// TODO 7 write data to stream
// http://soundfile.sapp.org/doc/WaveFormat/
// subchuncksize 16
// audioformat: 1s
// num channels: 1s
// sample rate: 44100
// byte rate: sample rate *16/8
// block origin: 2s
// bits per sample: 16s
let pack (d: int16[]) =
    let stream = new MemoryStream()
    let writer = new BinaryWriter(stream, Encoding.ASCII)
    let dataLength = Array.length d*2
    
    // RIFF
    failwith "Implement here"
    
    // data
    failwith "Implement here"
    stream
    
let write filename (ms: MemoryStream) =
    use fs = new FileStream(Path.Combine(__SOURCE_DIRECTORY__, filename), FileMode.Create) // use IDisposible
    ms.WriteTo(fs)
