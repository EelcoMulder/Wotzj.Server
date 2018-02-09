namespace Wotzj.Api.Database

open FSharp.Data.Sql
open FSharp.Data.Sql.Transactions

module DatabaseContext =
    [<Literal>]
    let ConnectionString = "Server=tcp:wotzjapi.database.windows.net,1433;Initial Catalog=wotzj;Persist Security Info=False;User ID=wotzjadmin;Password=Bdh8DQ$jH0@O0wRr;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    type SeriesSqlProvider = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString>
    let getContext = 
        SeriesSqlProvider.GetDataContext( { Timeout = System.TimeSpan.MaxValue; IsolationLevel = IsolationLevel.DontCreateTransaction})