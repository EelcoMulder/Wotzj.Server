open Suave
open Suave.DotLiquid
open Suave.Filters
open Suave.OAuth
open Suave.Operators
open Suave.Successful
open System.IO
open System.Configuration

type AppModel =
    {
        mutable name: string
        mutable logged_id: string
        mutable logged_in: bool
        mutable provider: string
        mutable providers: string[]
    }
let templateDir = Directory.GetCurrentDirectory() + "/templates"
setTemplatesDir templateDir

let oauthConfigs =
    let gitHubClientId = ConfigurationManager.AppSettings.Item("GitHub.ClientId")
    let gitHubClientSecret = ConfigurationManager.AppSettings.Item("GitHub.ClientSecret")
    defineProviderConfigs (function
        | "github" -> fun c ->
            {c with
                client_id = gitHubClientId
                client_secret = gitHubClientSecret }
        | _ -> id   // we do not provide secret keys for other oauth providers
    )
let appWebPart =
  choose
    [ GET >=> choose
        [ path "/api/series" >=> OK ("HAI") ] ]
let getPort (argv: string[]) =
  match Array.length argv with
    | 0 -> 8080us
    | _ -> argv.[0] |> uint16

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
                let authorizeRedirectUri = buildLoginUrl ctx in
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
                    path "/api/series" >=> GET >=> OK "You've accessed protected part!"
                ])
                (RequestErrors.FORBIDDEN "You do not have access to that application part (/protected)")

            // we'll never get here
            (OK "Hello World!")
        ]

    startWebServer conf app
    0
