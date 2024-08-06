﻿namespace MiniZinc.Parser;

public enum TokenKind : sbyte
{
    ERROR,

    // Keywords
    KEYWORD_ANNOTATION,
    KEYWORD_ANN,
    KEYWORD_ANONENUM,
    KEYWORD_ANY,
    KEYWORD_ARRAY,
    KEYWORD_BOOL,
    KEYWORD_CASE,
    KEYWORD_CONSTRAINT,
    KEYWORD_DEFAULT,
    KEYWORD_DIFF,
    KEYWORD_DIV,
    KEYWORD_ELSE,
    KEYWORD_ELSEIF,
    KEYWORD_ENDIF,
    KEYWORD_ENUM,
    KEYWORD_FALSE,
    KEYWORD_FLOAT,
    KEYWORD_FUNCTION,
    KEYWORD_IF,
    KEYWORD_IN,
    KEYWORD_INCLUDE,
    KEYWORD_INT,
    KEYWORD_INTERSECT,
    KEYWORD_LET,
    KEYWORD_LIST,
    KEYWORD_MAXIMIZE,
    KEYWORD_MINIMIZE,
    KEYWORD_MOD,
    KEYWORD_NOT,
    KEYWORD_OF,
    KEYWORD_OP,
    KEYWORD_OPT,
    KEYWORD_OUTPUT,
    KEYWORD_PAR,
    KEYWORD_PREDICATE,
    KEYWORD_RECORD,
    KEYWORD_SATISFY,
    KEYWORD_SET,
    KEYWORD_SOLVE,
    KEYWORD_STRING,
    KEYWORD_SUBSET,
    KEYWORD_SUPERSET,
    KEYWORD_SYMDIFF,
    KEYWORD_TEST,
    KEYWORD_THEN,
    KEYWORD_TRUE,
    KEYWORD_TUPLE,
    KEYWORD_TYPE,
    KEYWORD_UNION,
    KEYWORD_VAR,
    KEYWORD_WHERE,
    KEYWORD_XOR,

    // Nodes
    IDENTIFIER, // x
    IDENTIFIER_GENERIC, // $T
    IDENTIFIER_GENERIC_SEQUENCE, // $$T
    IDENTIFIER_INFIX, // `max`
    INT_LITERAL,
    FLOAT_LITERAL,
    STRING_LITERAL,
    LINE_COMMENT, // %
    BLOCK_COMMENT, // /*  */

    // Binary Ops
    BI_IMPLICATION,
    REVERSE_IMPLICATION,
    FORWARD_IMPLICATION,
    DISJUNCTION,
    CONJUNCTION,
    LESS_THAN,
    GREATER_THAN,
    LESS_THAN_EQUAL,
    GREATER_THAN_EQUAL,
    EQUAL,
    NOT_EQUAL,
    CLOSED_RANGE,
    OPEN_RANGE,
    RIGHT_OPEN_RANGE,
    LEFT_OPEN_RANGE,
    PLUS,
    MINUS,
    TIMES,
    DIVIDE,
    EXPONENT,
    PLUS_PLUS,
    TUPLE_ACCESS,
    RECORD_ACCESS,
    TILDE_EQUALS,
    TILDE_PLUS,
    TILDE_MINUS,
    TILDE_TIMES,
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
    COLON,
    COLON_COLON,
    PIPE,
    EMPTY,
    EOL,
    EOF,
}
