namespace MiniZinc

open System
open System.Collections.Generic
open System.Text
        
module Lexer =

    type Token =
        | TError  = 0
        // Values
        | TWord  = 1
        | TInt   = 2
        | TFloat = 3
        | TString = 4
        | TLineComment = 5
        | TBlockComment = 6
        // Keywords
        | KAnnotation  = 10 
        | KAnn         = 11
        | KAny         = 12
        | KArray       = 13 
        | KBool        = 14
        | KCase        = 15 
        | KConstraint  = 16 
        | KDefault     = 17
        | KDiff        = 18
        | KDiv         = 19
        | KElse        = 20
        | KElseif      = 21
        | KEndif       = 22
        | KEnum        = 23
        | KFalse       = 24
        | KFloat       = 25
        | KFunction    = 26
        | KIf          = 27
        | KIn          = 28
        | KInclude     = 29 
        | KInt         = 30
        | KIntersect   = 31 
        | KLet         = 32
        | KList        = 33
        | KMaximize    = 34
        | KMinimize    = 35
        | KMod         = 36
        | KNot         = 37
        | KOf          = 38
        | KOp          = 39
        | KOpt         = 40
        | KOutput      = 41
        | KPar         = 42
        | KPredicate   = 43 
        | KRecord      = 44
        | KSatisfy     = 45
        | KSet         = 46
        | KSolve       = 47
        | KString      = 48
        | KSubset      = 49
        | KSuperset    = 50
        | KSymdiff     = 51
        | KTest        = 52
        | KThen        = 53
        | KTrue        = 54     
        | KTuple       = 55
        | KType        = 56
        | KUnion       = 57       
        | KVar         = 58
        | KWhere       = 59
        | KXor         = 60
        // Operators
        | TDoubleArrow      = 70 // <->
        | TLeftArrow        = 71 // <-
        | TRightArrow       = 72 // -> 
        | TDownWedge        = 73 // \/                
        | TUpWedge          = 74 // /\
        | TLessThan         = 75 // <
        | TGreaterThan      = 76 // >
        | TLessThanEqual    = 77 // <=
        | TGreaterThanEqual = 78 // >=
        | TEqual            = 79 // =
        | TDotDot           = 80 // ..
        | TPlus             = 81 // +
        | TMinus            = 82 // -
        | TStar             = 83 // *
        | TSlash            = 84 // /
        | TTildeEquals      = 85 // ~=
        | TTildePlus        = 86 // ~+
        | TTildeMinus       = 87 // ~-
        | TTildeStar        = 88 // ~*
        // Symbol
        | TLeftBracket     = 100 // [ 
        | TRightBracket    = 101 // ]
        | TLeftParen       = 102 // (
        | TRightParen      = 103 // )
        | TLeftBrace       = 102 // {
        | TRightBrace      = 103 // }
        | TDot             = 104 // .
        | TTilde           = 105 // ~
        | TBackSlash       = 106 // \                
        | TForwardSlash    = 107 // /
        
    [<Struct>]
    type Lexeme =
        { mutable Kind    : Token
        ; mutable Line    : int64
        ; mutable Column  : int64
        ; mutable Start   : int64
        ; mutable End     : int64 }

    type LexResult =          
        { Source    : string
        ; StartTime : DateTime
        ; EndTime   : DateTime
        ; Duration  : TimeSpan
        ; mutable Lexemes  : ResizeArray<Lexeme>
        ; mutable Strings : ResizeArray<string>
        ; mutable Ints    : ResizeArray<int>
        ; mutable Floats  : ResizeArray<float>
        ; mutable Error   : string }            

    val lexFile: encoding: Encoding -> file: string -> LexResult

    val lexString: mzn: string -> LexResult
        
    val lexStream: encoding: Encoding -> stream: IO.Stream -> LexResult
