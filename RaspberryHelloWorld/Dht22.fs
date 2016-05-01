// Following implementation is a F# port of this library: https://github.com/porrey/dht/blob/master/source/Windows%2010%20IoT%20Core/DHT%20Solution/Sensors.Dht/Dht22.cpp

module Dht22

open System
open RasPi
open RasPi.PiTimer

type Dht22 = { Pin:RasPi.pinId } // Placeholder for the sensor

type Readout = { Temperature:int; Humidity:int }

// Create
let create inputPin = 
    { Pin = inputPin }

// Performs a simple analysis of raising and falling edges
// and tries calculate relative lengths of low and high portions of the signal.
let calculatePulses edges =
    // Don't need lows for the calculation
    let onlyHighs = [| for i, x in Array.mapi (fun i x -> i, x) edges do
                        if i % 2 = 0 then yield x |]

    printfn "Only highs: %A" onlyHighs

    //let lengths = 
    //    onlyHighs
    //    |> Array


// Internal implementation of DHT22 protocol specifics
let  internalGetReadout sensor =
    let pin = sensor.Pin
    let magicTimeoutValue = 32000
    let dhtPulseCount = 41
    // Prepare memory
    let edges : int array = Array.zeroCreate (dhtPulseCount * 2)
    let indexes = [| for i in 0 .. (edges.Length/2 - 1)  -> i * 2 |]

    // Initialize communication with DHT22
    RasPi.setPinMode pin Out

    // From this point on the operations should be as real-time as possible
    RasPi.setPriority Max |> ignore

    RasPi.digitalWrite pin Low
    RasPi.PiTimer.delayMillis 1<ms>

    RasPi.digitalWrite pin High
    //RasPi.PiTimer.delayMicros 20<us>

    RasPi.setPinMode pin In

    // Reading the actual lows and highs
    for i in indexes do
        // Count how long pin is low
        while (RasPi.digitalRead pin = High) && (edges.[i] < magicTimeoutValue) do
            edges.[i] <- edges.[i] + 1

        // Count how long pin is high
        while (RasPi.digitalRead pin = Low) && (edges.[i] < magicTimeoutValue) do
            edges.[i+1] <- edges.[i+1] + 1

    RasPi.setPriority Default |> ignore

    printfn "End of time-critical operation"

    printfn "Edges: %A" edges

    let pulses = calculatePulses edges

    0


// Get temp / humidity
let getReadout sensor =
    let maxRetries = 20

    //for iter in 1 .. maxRetries do


    { Temperature = 0; Humidity = 0 }

