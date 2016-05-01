// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module MainModule

open RasPi
open System
open System.Threading

[<EntryPoint>]
let main argv = 
    printfn "Setting up wiringPi"
    wiringPiSetup()

    let dht22 = Dht22.create pinId.gpio23

    printfn "Going into the loop"
    while true do
        Thread.Sleep 2500
        match Dht22.getReadout dht22 with
        | Some r -> printfn "%A Temp: %A     Humidity: %A" DateTime.Now r.Temperature r.Humidity
        | None -> printfn "%A Error reading the sensor" DateTime.Now

    0 // return an integer exit code

