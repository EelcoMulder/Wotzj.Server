namespace Wotjz.Api
open System.Configuration
open Suave.OAuth
open System
open Suave

module Authentication = 

    type AuthenticationModel =
        {
            mutable name: string
            mutable logged_id: string
            mutable logged_in: bool
            mutable provider: string
            mutable providers: string[]
        }
    let oauthConfigs =
        let gitHubClientId = ConfigurationManager.AppSettings.Item("GitHub.ClientId")
        let gitHubClientSecret = ConfigurationManager.AppSettings.Item("GitHub.ClientSecret")
        defineProviderConfigs (function
            | "github" -> fun c ->
                {c with
                    client_id = gitHubClientId
                    client_secret = gitHubClientSecret }
            | _ -> id 
        )

    let buildRedirectUri (request: HttpRequest) = 
        let bb = UriBuilder (request.url)
        bb.Host <- request.host
        bb.Path <- "oalogin"
        bb.Port <- -1
        bb.Query <- ""

        bb.ToString()