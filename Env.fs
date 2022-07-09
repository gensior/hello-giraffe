module Env

open System

let private tryGetEnv key =
    Environment.GetEnvironmentVariable(key)
        |> Option.ofObj

let getPortOrDefault default' =
    let tryPort = tryGetEnv "PORT"
    tryPort |> Option.defaultValue default'

let getHostOrDefault (default':string) =
    let host = tryGetEnv "ASPNETCORE_URL"
    match host with
    | Some(h) ->
        h.Split(':')
        |> Array.rev
        |> Array.tail
        |> Array.rev
        |> String.concat ":"
    | None -> default'
