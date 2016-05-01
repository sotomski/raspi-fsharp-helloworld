// Datasheet used as reference for implementation: https://cdn-shop.adafruit.com/datasheets/Digital+humidity+and+temperature+sensor+AM2302.pdf
// Following implementation is a F# port of this library: https://github.com/adafruit/Adafruit_Python_DHT

module Dht22

open System
open RasPi
open RasPi.PiTimer

type Dht22 = { Pin:RasPi.pinId } // Placeholder for the sensor

type Readout = { Temperature:float; Humidity:float }

// Create
let create inputPin = 
    { Pin = inputPin }

// Performs a simple analysis of raising and falling edges
// and tries calculate relative lengths of low and high portions of the signal.
let private calculateTempAndHumidity (edges: int array) =
    // Don't need lows for the calculation
    let onlyHighs = [| for i, x in Array.mapi (fun i x -> i, x) edges do
                        if i % 2 = 0 then yield x |]

    //printfn "Only highs: %A" onlyHighs

    let threshold = onlyHighs |> Array.averageBy (fun elem -> float elem)
    let binarySignal = onlyHighs |> Array.map (fun x -> if float x <= threshold then 0 else 1)

    //printfn "Binary signal: %A" binarySignal
   
    let humidity = 
        Array.sub binarySignal 0 16 
        |> Array.rev
        |> Array.fold (fun (index, decValue) el -> (index + 1, decValue + (el <<< index))) (0, 0)
        |> fun (_, h) -> float h / 10.0

    let temperature = 
        Array.sub binarySignal 16 16
        |> Array.rev
        |> Array.fold (fun (index, decValue) el -> (index + 1, decValue + (el <<< index))) (0, 0)
        |> fun (_, t) -> float t / 10.0

    // HERE IS A GOOD PLACE FOR RAILROAD ERROR HANDLING
    //let checkSum = binarySignal |> Array.sub 32 8

    { Temperature = temperature; Humidity = humidity }



// Internal implementation of DHT22 protocol specifics
let private internalGetReadout sensor =
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
    RasPi.PiTimer.delayMillis 2<ms>

    RasPi.digitalWrite pin High
    // Wait until sensor starts pulling up (according to spec min 100 < x < 120)
    // I am using 140 just to be sure
    RasPi.PiTimer.delayMicros 140<us>

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

    //printfn "End of time-critical operation"

    //printfn "Edges: %A" edges

    // First two readouts are communication protocol pulses (Sensor pull low and then pull up)
    Array.sub edges 2 (edges.Length-2) |> calculateTempAndHumidity


// Get temp / humidity
let getReadout sensor : Option<Readout> =
    let maxRetries = 3

    match internalGetReadout sensor with
    | r when r.Humidity > 0.0 && r.Temperature > 0.0 -> Some r
    | _ -> None
