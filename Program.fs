open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.EndpointRouting
open Giraffe.ViewEngine

let indexView =
    html [] [
        head [] [
            title [] [ str "Giraffe Example" ]
        ]
        body [] [
            h1 [] [ str "I |> F#" ]
            p [ _class "some-css-class"; _id "someId" ] [
                str "Hello World from the Giraffe View Engine"
            ]
        ]
    ]

let sayHelloNameHandler (name:string) : HttpHandler =
    fun (next:HttpFunc) (ctx:HttpContext) ->
        task {
            let msg = $"Hello {name}, how are you?"
            return! json {| Response = msg |} next ctx
        }

let endpoints =
    [
        GET [
            route "/" (htmlView indexView)
            route "/api" (json {| Response = "Hello world!!" |})
            routef "/api/%s" sayHelloNameHandler    
        ]
    ]

let notFoundHandler =
    "Not Found"
    |> text
    |> RequestErrors.notFound

let configureApp (appBuilder : IApplicationBuilder) =
    appBuilder
        .UseRouting()
        .UseGiraffe(endpoints)
        .UseGiraffe(notFoundHandler)

let configureServices (services : IServiceCollection) =
    services
        .AddRouting()
        .AddGiraffe()
        |> ignore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    configureServices builder.Services
    
    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    configureApp app

    let port = 
        match Environment.GetEnvironmentVariable("PORT") with 
        | null -> "8080"
        | _ -> Environment.GetEnvironmentVariable("PORT")
    app.Run(sprintf "http://0.0.0.0:%s" port)
    0