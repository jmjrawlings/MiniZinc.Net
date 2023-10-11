namespace MiniZinc
    
  module Lexer =
    
    type TokenType =
        | Error = 0
        | Model = 1
        | LineComment = 2
        | BlockComment = 3
    
    [<Struct>]
    type Token =
        { Type  : TokenType
        ; Mzn   : string
        ; Start : FParsec.Position
        ; End   : FParsec.Position }

    