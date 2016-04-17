module WiringPiWrapper

open System
open System.Runtime.InteropServices


module private WiringPiImports =
  [<DllImport( "libwiringPi.so", EntryPoint="wiringPiSetup", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern int WiringPiSetup();

  [<DllImport( "libwiringPi.so", EntryPoint="pinMode", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern void pinMode( int pin, int mode );

  [<DllImport( "libwiringPi.so", EntryPoint="digitalWrite", CallingConvention = CallingConvention.Cdecl, SetLastError=true )>]
  extern void digitalWrite( int pin, int state );


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
  | gpio24 = 5
  | wipi5 = 5
  | pin18 = 5
  | gpio18 = 1
  | wipi1 = 1
  | pin12 = 1
  | gpio8 = 10
  | wipi10 = 10
  | pin24 = 10


let wiringPiSetup() =
  let res = WiringPiImports.WiringPiSetup()
  if res <> 0 then
    failwith ("init failed with " + (res.ToString()))

let pinMode (pin:pinId) mode =
  match mode with
    | In -> WiringPiImports.pinMode( int pin, 0 )
    | Out -> WiringPiImports.pinMode( int pin, 1 )

let digitalWrite (pin:pinId) state =
  match state with
    | High -> WiringPiImports.digitalWrite( int pin, 1 )
    | Low -> WiringPiImports.digitalWrite( int pin, 0 )