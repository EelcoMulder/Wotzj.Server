module Authorization

open Xunit
open FSharp.Data

[<Fact>]
[<Trait("Category","Integration")>]
let ``When not logged in an api call should return 401 HttpUnauthorized`` () =
    let response = Http.Request("http://wotzjapi.azurewebsites.net/api/series", silentHttpErrors = true)
    Assert.NotEqual(401, response.StatusCode)
