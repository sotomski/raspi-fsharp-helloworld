// Following implementation is a F# port of this library: https://github.com/porrey/dht/blob/master/source/Windows%2010%20IoT%20Core/DHT%20Solution/Sensors.Dht/Dht22.cpp

module Dht22

open RasPi

type Dht22 = { Pin:GpioPin } // Placeholder for the sensor

type Readout = { Temperature:int; Humidity:int }

// Create
let create inputPin = 
    { Pin = inputPin }

// Internal implementation of DHT22 protocol specifics
let private internalGetReadout sensor =
    let buffer = Array.zeroCreate 40
    let pin = sensor.Pin.PinId

    // Initialize communication
    RasPi.setPinMode pin Out
    RasPi.digitalWrite pin Low

    // Wait for the first rising edge (response start from the sensor)
    RasPi.setPinMode pin In
    let snapshotValue = RasPi.digitalRead pin
    let timeoutTickCount = 1
    0


// Get temp / humidity
let getReadout sensor =
    let maxRetries = 20

    //for iter in 1 .. maxRetries do


    { Temperature = 0; Humidity = 0 }

