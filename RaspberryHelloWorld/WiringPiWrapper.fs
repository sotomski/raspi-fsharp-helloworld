module RasPi

open System
open System.Runtime.InteropServices


module private WiringPiImports =
  [<DllImport( "libwiringPi.so", EntryPoint="wiringPiSetup", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern int WiringPiSetup();

  [<DllImport( "libwiringPi.so", EntryPoint="pinMode", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern void pinMode( int pin, int mode );

  [<DllImport( "libwiringPi.so", EntryPoint="digitalWrite", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern void digitalWrite( int pin, int state );

  [<DllImport( "libwiringPi.so", EntryPoint="digitalRead", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern int digitalRead( int pin );

  // Delays and timing functions
  [<DllImport( "libwiringPi.so", EntryPoint="millis", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern uint32 millis();

  [<DllImport( "libwiringPi.so", EntryPoint="micros", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern uint32 micros();

  [<DllImport( "libwiringPi.so", EntryPoint="delay", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern uint32 delayMilliseconds(uint32 howLong);

  [<DllImport( "libwiringPi.so", EntryPoint="delayMicroseconds", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern uint32 delayMicroseconds(uint32 howLong);

  // Attempts to shift the program to a higher priority and enables real-time scheduling.
  // Param: priority should be from 0 (default) - 99 (max prio). 
  // Returns 0 if operation was successful; -1 for error
  // It is possible to consult errno global variable for more information about the error.
  [<DllImport( "libwiringPi.so", EntryPoint="piHiPri", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern int piHiPri(int priority);

module PiTimer =
    [<Measure>] type ms // milliseconds
    [<Measure>] 
    type us = // microseconds
        static member perMillisecond = 1000<us/ms>

    let millisSinceSetup = int(WiringPiImports.millis()) * 1<ms>
    let microsSinceSetup = int(WiringPiImports.micros()) * 1<us>

    let delayMillis (howLong:int<ms>) = 
        let rawHowLong = uint32(howLong/ 1<ms>)
        WiringPiImports.delayMilliseconds(rawHowLong)

    let delayMicros (howLong:int<us>) = 
        let rawHowLong = uint32(howLong/ 1<us>)
        WiringPiImports.delayMicroseconds(rawHowLong)


type pinMode =
  | In
  | Out

type pinState =
  | High
  | Low

type pinId =
  | gpio14 = 15
  | wipi15 = 15
  | pin8 = 15
  | gpio23 = 4
  | wipi4 = 4
  | pin16 = 4
  | gpio17 = 0
  | wipi0 = 0
  | pin11 = 0
  | gpio24 = 5
  | wipi5 = 5
  | pin18 = 5
  | gpio18 = 1
  | wipi1 = 1
  | pin12 = 1
  | gpio8 = 10
  | wipi10 = 10
  | pin24 = 10

type GpioPin = { PinId:pinId; Mode:pinMode }

let wiringPiSetup() =
  let res = WiringPiImports.WiringPiSetup()
  if res <> 0 then
      failwith ("init failed with " + (res.ToString()))

  let prioResult = WiringPiImports.piHiPri(99)
  if prioResult <> 0 then
      failwith ("execution priority setup failed during initialization with " + prioResult.ToString())


let setPinMode (pin:pinId) mode =
  match mode with
    | In -> WiringPiImports.pinMode( int pin, 0 )
    | Out -> WiringPiImports.pinMode( int pin, 1 )


let digitalWrite (pin:pinId) state =
  match state with
    | High -> WiringPiImports.digitalWrite( int pin, 1 )
    | Low -> WiringPiImports.digitalWrite( int pin, 0 )


let digitalRead (pin:pinId) = 
    let readout = WiringPiImports.digitalRead( int pin )
    match readout with
    | 0 -> Low
    | 1 -> High
    | _ -> failwith ("unexpected readout from pin" + pin.ToString())

