open System

let rec gcd a b =
    match b with
    | 0 -> a
    | _ -> gcd b (a % b)

let isPrime n =
    let rec check d =
        match d with
        | d when d * d > n -> true
        | d when n % d = 0 -> false
        | _ -> check (d + 1)
    match n with
    | n when n < 2 -> false
    | _ -> check 2

let getDivisors n =
    let rec loop d acc =
        match d > n with
        | true -> acc
        | false when n % d = 0 -> loop (d + 1) (d :: acc)
        | false -> loop (d + 1) acc
    loop 1 [] |> List.rev

let getPrimeDivisors n =
    getDivisors n
    |> List.filter (fun d -> isPrime d)

let multiply lst =
    let rec loop lst acc =
        match lst with
        | [] -> acc
        | x :: xs -> loop xs (acc * x)
    loop lst 1

let rad n =
    getPrimeDivisors n |> multiply

let isCoprimeTriple (a, b, c) =
    gcd a b = 1 && gcd a c = 1 && gcd b c = 1

let isAbcHit (a, b, c) =
    a < b &&
    a + b = c &&
    isCoprimeTriple (a, b, c) &&
    rad (a * b * c) < c

let abcHitsSum limit =
    [|1 .. limit - 1|]
    |> Array.collect (fun c ->
        [|1 .. c / 2|]
        |> Array.map (fun a ->
            let b = c - a
            (a, b, c)))
    |> Array.filter isAbcHit
    |> Array.map (fun (_, _, c) -> c)
    |> Array.sum

[<EntryPoint>]
let main argv = 

    let result = abcHitsSum 120000
    printfn "Сумма всех c (abc-hits < 1000): %d" result

    0