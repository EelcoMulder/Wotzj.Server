namespace Wotzj.Api.Database

open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions

module DatabaseContext =
    [<Literal>]
    let WotzjConnectionString = @"Server=.\SQLExpress;Database=Series;Integrated Security=SSPI;"
    type SeriesSqlProvider = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, WotzjConnectionString>
    let getContext = 
        SeriesSqlProvider.GetDataContext( { Timeout = System.TimeSpan.MaxValue; IsolationLevel = IsolationLevel.DontCreateTransaction})