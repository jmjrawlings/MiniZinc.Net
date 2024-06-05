﻿namespace MiniZinc.Parser;

public enum TokenKind : sbyte
{
    ERROR,

    // Keywords
    ANNOTATION,
    ANN,
    ANONENUM,
    ANY,
    ARRAY,
    BOOL,
    CASE,
    CONSTRAINT,
    DEFAULT,
    DIFF,
    DIV,
    ELSE,
    ELSEIF,
    ENDIF,
    ENUM,
    FALSE,
    FLOAT,
    FUNCTION,
    IF,
    IN,
    INCLUDE,
    INT,
    INTERSECT,
    LET,
    LIST,
    MAXIMIZE,
    MINIMIZE,
    MOD,
    NOT,
    OF,
    OP,
    OPT,
    OUTPUT,
    PAR,
    PREDICATE,
    RECORD,
    SATISFY,
    SET,
    SOLVE,
    STRING,
    SUBSET,
    SUPERSET,
    SYMDIFF,
    TEST,
    THEN,
    TRUE,
    TUPLE,
    TYPE,
    UNION,
    VAR,
    WHERE,
    XOR,

    // Nodes
    IDENTIFIER,
    GENERIC,
    GENERIC_SEQUENCE,
    INT_LITERAL,
    FLOAT_LITERAL,
    STRING_LITERAL,
    LINE_COMMENT,
    BLOCK_COMMENT,
    INFIX_IDENTIFIER,

    // Binary Ops
    DOUBLE_ARROW,
    LEFT_ARROW,
    RIGHT_ARROW,
    DOWN_WEDGE,
    UP_WEDGE,
    LESS_THAN,
    GREATER_THAN,
    LESS_THAN_EQUAL,
    GREATER_THAN_EQUAL,
    EQUAL,
    NOT_EQUAL,
    DOT_DOT,
    PLUS,
    MINUS,
    STAR,
    SLASH,
    EXP,
    PLUS_PLUS,
    TUPLE_ACCESS,
    RECORD_ACCESS,
    TILDE_EQUALS,
    TILDE_PLUS,
    TILDE_MINUS,
    TILDE_STAR,
    OPEN_BRACKET,
    CLOSE_BRACKET,
    OPEN_PAREN,
    CLOSE_PAREN,
    OPEN_BRACE,
    CLOSE_BRACE,
    PERCENT,
    UNDERSCORE,
    COMMA,
    TILDE,
    BACKSLASH,
    FWDSLASH,
    COLON,
    COLON_COLON,
    PIPE,
    EMPTY,
    EOL,
    EOF,
}
