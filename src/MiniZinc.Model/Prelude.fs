namespace MiniZinc


[<AutoOpen>]
module Prelude =
    let a = ()


module Map =
    let withKey f xs =
        xs
        |> Seq.map (fun x -> (f x), x)
        |> Map


module Result =
    let get (result: Result<'ok, 'err>) =
        match result with
        | Ok ok -> ok
        | Error err -> failwithf "%A" err
        
    let ofSeq (results : Result<'ok, 'err> seq) =
        let oks = ResizeArray()
        let errs = ResizeArray()
        for result in results do
            match result with
            | Ok v -> oks.Add v
            | Error err -> errs.Add err
        match errs.Count with
        | 0 -> Ok (Seq.toList oks)
        | _ -> Error (Seq.toList errs)
