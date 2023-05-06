namespace MiniZinc

open FParsec

module Parse =

    let ws = spaces
    let ws1 = spaces1
    let s x = pstring x
    let sws x = s x >>. ws
    let wss x = ws >>. s x |>> ignore
    let wssws x = ws >>. s x .>> ws
    let chr x = pchar x
    let cws c = chr c .>> ws
    let wsc c = ws >>. chr c
    let c char = pchar char
    let wscws c = ws >>. chr c .>> ws
    let btwn a b p = between (cws a) (c b) (p .>> ws)
    let notimpl<'a> : 'a = failwith "not implemented"
    let optl p = (opt p) |>> Option.defaultValue []

    let sep = ws
    let (.>>>) a b = a .>> sep .>> b
    let (>>>.) a b = a >>. sep >>. b
    let (.>>>.) a b = a .>> sep .>>. b
    let opt_or p x = (opt p) |>> Option.defaultValue x
