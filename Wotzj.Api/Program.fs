open Suave
open Suave.DotLiquid
open Suave.Filters
open Suave.Operators
open Suave.Successful
open System.IO
open Wotjz.Api.Authentication
open Wotzj.Api.Infrastructure
open WebServer
open Wotzj.Api.Api
open Wotzj.Api.BusinessLogic.Series

let templateDir = Directory.GetCurrentDirectory() + "/templates"
setTemplatesDir templateDir

[<EntryPoint>]
let main argv =

    let conf = 
        { defaultConfig with
                bindings = [ HttpBinding.create HTTP System.Net.IPAddress.Loopback <| getPort argv ] }

    let model = {
        name = "anonymous"; logged_id = ""; logged_in = false
        provider = ""
        providers = [|"GitHub"|]
        }

    let app =
        choose [
            path "/" >=> page "main.html" model

            warbler(fun ctx ->
                let authorizeRedirectUri = buildRedirectUri ctx.request in
                // Note: logon state for current user is stored in global variable, which is ok for demo purposes.
                // in your application you shoud store such kind of data to session data
                OAuth.authorize authorizeRedirectUri oauthConfigs
                    (fun loginData ->

                        model.logged_in <- true
                        model.logged_id <- sprintf "%s (name: %s)" loginData.Id loginData.Name
                        
                        Redirection.FOUND "/"
                    )
                    (fun () ->

                        model.logged_id <- ""
                        model.logged_in <- false

                        Redirection.FOUND "/"
                    )
                    (fun error -> OK <| sprintf "Authorization failed because of `%s`" error.Message)
                )

            OAuth.protectedPart
                (choose [
                    path "/api/series" 
                        >=> GET 
                        >=> choose
                            [ path "/api/series" >=> handleGetAll getSeries
                              pathScan "/api/series/%d" (fun id -> handleGet id getSerie) ]
                ])
                (RequestErrors.FORBIDDEN "You do not have access to this application part")

            (OK "Unreachable")
        ]

    startWebServer conf app
    0
