public static class Operators {
    /* 
        See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/
        for an explicit breakdown of this heirarchy.
    */
    public static class Arithmetic {
        public const string MULTIPLICATION = "*", DIVISION = "/", REMAINDER = "%", ADDITION = "+", SUBTRACTION = "-";   
        public static class Unary {
            public const string INCREMENT = "++", DECREMEMT = "--";
        }
        public static class Compound {
           public const string MULTIPLICATION = "*=", DIVISION = "/=", REMAINDER = "%=", ADDITION = "+=", SUBTRACTION = "-=";
        }
    }
    public static class Boolean {
        public const string NEGATION = "!", AND = "&&", OR = "||", EQUALITY = "==", INEQUALITY = "!=";
    }
    public static class Comparison {
        public const string LESS_THAN = "<", GREATER_THAN = ">", LESS_THAN_OR_EQUAL = "<=", GREATER_THAN_OR_EQUAL = ">=";
    }
    public static class Member {
        public const string ACCESS = ".", INDEXER_OPENING = "[", INDEXER_CLOSING = "]", INVOCATION = "()";
    }
    public const string EMPTY = "",
        SPACE = " ",
        COMMA = ",",
        DOT = ".",
        EQUALS = "=",
        OPENING_BRACKET = "{",
        CLOSING_BRACKET = "}",
        OPENING_PARENTHESIS = "(",
        CLOSING_PARENTHESIS = ")",
        END_LINE = ";",
        TAB = "\t",
        NEW_LINE = "\n";
    public const char END_LINE_CHAR = ';';


    /* For the Parser.simplify() function */
    public static readonly string[][] PEMDAS = {
        new string[] { Arithmetic.MULTIPLICATION, Arithmetic.DIVISION, Arithmetic.REMAINDER },
        new string[] { Arithmetic.ADDITION, Arithmetic.SUBTRACTION },
        new string[] { Boolean.EQUALITY, Boolean.INEQUALITY, Comparison.GREATER_THAN, Comparison.GREATER_THAN_OR_EQUAL, Comparison.LESS_THAN, Comparison.LESS_THAN_OR_EQUAL },
        new string[] { Boolean.AND },
        new string[] { Boolean.OR }
    };

    /* For the String.Split() function */
    public static readonly string[] SPLIT = new string[] { SPACE };

    /* For the SubstringHandler.Split() function */
    public static readonly string[] SPLIT_PARAMETERS = new string[] { OPENING_PARENTHESIS, END_LINE, COMMA, CLOSING_PARENTHESIS };

}
