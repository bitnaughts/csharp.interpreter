public static class Operators {
    public const string EMPTY = "",
        SPACE = " ",
        DOT = ".",
        EQUALS = "=",
        INCREMENT = "++",
        DECREMENT = "--",
        ADDITION = "+=",
        SUBTRACTION = "-=",
        MULTIPLICATION = "*=",
        DIVISION = "/=",
        MODULO = "%=",
        EQUAL_TO = "==",
        NOT_EQUAL = "!=",
        GREATER_THAN = ">",
        GREATER_THAN_EQUAL = ">=",
        LESS_THAN = "<",
        LESS_THAN_EQUAL = "<=",
        AND = "&&",
        OR = "||",
        MODULUS = "%",
        TIMES = "*",
        DIVIDE = "/",
        ADD = "+",
        SUBTRACT = "-",
        OPENING_BRACKET = "{",
        CLOSING_BRACKET = "}",
        OPENING_PARENTHESIS = "(",
        CLOSING_PARENTHESIS = ")",
        END_LINE = ";",
        TAB = "\t",
        NEW_LINE = "\n";

    //public const string[] ALL_OPERATORS = { EQUALS, INCREMENT, DECREMENT, ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION, MODULO, EQUAL_TO, NOT_EQUAL, GREATER_THAN, GREATER_THAN_EQUAL, LESS_THAN, LESS_THAN_EQUAL, AND, OR, MODULUS, TIMES, DIVIDE, ADD, SUBTRACT };

    public const char END_LINE_CHAR = ';';


    /* For the Parser.simplify() function */
    public static readonly string[][] PEMDAS = {
        new string[] { MODULUS },
        new string[] { TIMES, DIVIDE },
        new string[] { ADD, SUBTRACT },
        new string[] { EQUAL_TO, NOT_EQUAL, GREATER_THAN, GREATER_THAN_EQUAL, LESS_THAN, LESS_THAN_EQUAL },
        new string[] { AND },
        new string[] { OR }
    };

    /* For the String.Split() function */
    public static readonly string[] SPLIT = new string[] { SPACE };

}