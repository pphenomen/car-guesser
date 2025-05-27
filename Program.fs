let connectionString = "Server=192.168.1.22;Database=robot;User=root;Password=allhailklab"

open FSharp.Data.Sql

type sql = SqlDataProvider<ConnectionString = connectionString, DatabaseVendor = Common.DatabaseProviderTypes.POSTGREESQL>
let ctx = sql.GetDataContext()


let getTop10Cars() =
    query {
        for session in ctx.``[akinator_cars].[game_sessions]`` do
        where (session.GuessedCharacter.IsSome)  // Успешные сессии, где угадали автомобиль
        groupBy session.GuessedCharacter into g
        sortByDescending (g.Count())
        select (g.Key, g.Count())
        take 10
    } |> Seq.toList

let getSuccessRate() =
    let total = query { for session in ctx.``[akinator_cars].[game_sessions]`` do count }
    let correct = query { 
        for session in ctx.``[akinator_cars].[game_sessions]`` do 
        where (session.GuessedCharacter.IsSome) 
        count 
    }
    float correct / float total