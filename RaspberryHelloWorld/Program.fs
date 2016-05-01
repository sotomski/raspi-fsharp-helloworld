// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module MainModule

open RasPi
open System.Threading

[<EntryPoint>]
let main argv = 
    printfn "Setting up wiringPi"
    wiringPiSetup()

    printfn "Going into the loop"
    while true do
        printfn "Sleep..."
        Thread.Sleep 2500
        printfn "Getting the readout"
        let dht22 = Dht22.create pinId.gpio23
        Dht22.internalGetReadout dht22 |> ignore

    0 // return an integer exit code

