// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
module MainModule

open RasPi
open System.Threading

[<EntryPoint>]
let main argv = 
    printfn "Setting up wiringPi"
    wiringPiSetup()

    printfn "Setting up the pin"
    setPinMode pinId.wipi0 Out

    printfn "Pin High"
    digitalWrite pinId.wipi0 High
    printfn "Sleep 1000"
    Thread.Sleep(1000)
    printfn "Pin Low"
    digitalWrite pinId.wipi0 Low
    printfn "Sleep 1000"
    Thread.Sleep(1000)
    printfn "Pin High"
    digitalWrite pinId.wipi0 High
    printfn "Sleep 1000"
    Thread.Sleep(1000)
    printfn "Pin Low"
    digitalWrite pinId.wipi0 Low
    printfn "Sleep 1000"
    Thread.Sleep(1000)
    printfn "Pin High"
    digitalWrite pinId.wipi0 High

    0 // return an integer exit code

