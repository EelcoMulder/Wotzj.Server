namespace Wotzj.Api

open Suave
open Suave.State.CookieStateStore

module Cookies = 

    let setSessionValue (key : string) (value : 'T) : WebPart =
      context (fun ctx ->
        match HttpContext.state ctx with
        | Some state ->
            state.set key value
        | _ ->
            never // fail
        )

    let getSessionValue (ctx : HttpContext) (key : string) : 'T option =
      match HttpContext.state ctx with
      | Some state ->
          state.get key
      | _ ->
          None

    /// This a convenience function that turns a None string result into an empty string
    let getStringSessionValue (ctx : HttpContext) (key : string) : string = 
      defaultArg (getSessionValue ctx key) ""