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
    writer.Write(Encoding.ASCII.GetBytes("RIFF"))
    writer.Write(36 + dataLength) // chunksize
    writer.Write(Encoding.ASCII.GetBytes("WAVE"))

    // data
    writer.Write(Encoding.ASCII.GetBytes("fmt "))
    writer.Write(16) // subchunk1size
    writer.Write(1) // audioformat
    writer.Write(1) // num channels
    writer.Write(44100) // sample rate
    writer.Write(44100 * 16 / 8) // byte rate
    writer.Write(2) // block align
    writer.Write(16) // bits per sample


    writer.Write(Encoding.ASCII.GetBytes("data"))
    writer.Write(dataLength) // subchunk2size

    for i in 0..Array.length d-1 do
        writer.Write(d.[i])

    stream

    
let write filename (ms: MemoryStream) =
    use fs = new FileStream(Path.Combine(__SOURCE_DIRECTORY__, filename), FileMode.Create) // use IDisposible
    ms.WriteTo(fs)
