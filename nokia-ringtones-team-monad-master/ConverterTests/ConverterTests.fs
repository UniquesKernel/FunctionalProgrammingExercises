module ConverterTests

open NUnit.Framework
open System.IO

open SampleGenerator
open WavePacker

[<TestFixture>]
type ``When packing an audio file `` () =

    let getFile milliseconds =
        generateSamples milliseconds 440.
        |> Array.ofSeq
        |> pack
        |> (fun ms ->
            ms.Seek(0L, SeekOrigin.Begin) |> ignore
            ms)

    [<Test>]
    member this.``the stream should start with RIFF`` () =
        let file = getFile 2000.
        let bucket = Array.zeroCreate 4
        file.Read(bucket, 0, 4) |> ignore
        let first4Char = System.Text.Encoding.ASCII.GetString(bucket)
        Assert.AreEqual("RIFF", first4Char)
    
       
    [<Test>]
    member this.``the size is correct`` () =
        let formatOverhead = 44.
        let audioLength = [2000.; 50.; 1500.; 3000.] // Comma will yield a list of one tuple
        let files = List.zip audioLength (List.map getFile audioLength)
        
        let assertLegnth (length, file: MemoryStream) =
            Assert.AreEqual((length/1000.) * 44100. * 2. + formatOverhead, file.Length)

        List.iter assertLegnth files
    