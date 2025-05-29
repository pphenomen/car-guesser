namespace AkinatorQueries

open FSharp.Data.Sql
open System.Threading.Tasks
open System.Linq

module Queries =
    type Sql =
        SqlDataProvider<
            ConnectionString = "Host=192.168.1.22;Database=carguesser;Username=robot;Password=allhailklab",
            DatabaseVendor = Common.DatabaseProviderTypes.POSTGRESQL,
            UseOptionTypes = Common.NullableColumnType.OPTION
        >

    let private ctx : Sql.dataContext = Sql.GetDataContext()

    let getTop10CarsAsync() : Task<List<string * int>> =
        async {
            let result =
                query {
                    for session in ctx.Public.GameSessions do
                        where (session.GuessedCar.IsSome)
                        groupBy session.GuessedCar.Value into g
                        select (g.Key, g.Count()) // Используем Count() здесь
                }
                |> Seq.toList
                |> List.sortByDescending snd
                |> List.truncate 10

            return result
        }
        |> Async.StartAsTask

    let getSuccessRateAsync() : Task<double> =
        async {
            let total =
                query {
                    for session in ctx.Public.GameSessions do
                        count
                }

            let correct =
                query {
                    for session in ctx.Public.GameSessions do
                        where (session.GuessedCar.IsSome)
                        count
                }

            if total = 0 then 
                return 0.0
            else 
                return float correct / float total
        }
        |> Async.StartAsTask

    let getAverageSessionsPerUserAsync() : Task<double> =
        async {
            let totalSessions =
                query {
                    for session in ctx.Public.GameSessions do
                        count
                }

            let totalUsersWithSessions =
                query {
                    for session in ctx.Public.GameSessions do
                        groupBy session.UserId into g
                        count
                }

            if totalUsersWithSessions = 0 then 
                return 0.0
            else 
                return float totalSessions / float totalUsersWithSessions
        }
        |> Async.StartAsTask

    let getMostFrequentQuestionAsync() : Task<int option> =
        async {
            let result =
                query {
                    for answer in ctx.Public.SessionAnswers do
                        groupBy answer.QuestionNumber into g
                        select (g.Key, g.Count())
                }
                |> Seq.toList
                |> List.sortByDescending snd
                |> List.tryHead

            match result with
            | Some (questionNumber, _) -> return Some questionNumber
            | None -> return None
        }
        |> Async.StartAsTask

    let getLiverpoolCountAsync() : Task<int> =
        async {
            let count =
                query {
                    for session in ctx.Public.GameSessions do
                    where (session.OwnerClub.Value = "Liverpool")
                    count
                }
            return count
        }
        |> Async.StartAsTask