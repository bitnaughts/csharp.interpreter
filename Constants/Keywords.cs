public static class Keywords {
     
    /* 
        See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/statement-keywords
        for an explicit breakdown of this heirarchy.
    */
    public static class Type {
           public static class Value {
                public const string BOOLEAN = "bool", BYTE = "byte", CHAR = "char", DOUBLE = "double", FLOAT = "float", INTEGER = "int";
           }
           public static class Reference {
                public const string CLASS = "class", INTERFACE = "interface", OBJECT = "object", STRING = "string";   
           }
           public const string VOID = "void";
    }
    public static class Modifier {
        public static class Access {
            public const string PRIVATE = "private", PROTECTED = "protected", PUBLIC = "public";
        }
        public const string CONST = "const", STATIC = "static";
    }
    public static class Statement {
        public static class Selection {
            public const string IF = "if", ELSE = "else", SWITCH = "switch", CASE = "case";
        }
        public static class Iteration {
            public const string DO = "do", WHILE = "while", FOR = "for", FOREACH = "foreach", IN = "in";
        } 
        public static class Jump {
            public const string BREAK = "break", CONTINUE = "continue", GOTO = "goto", RETURN = "return";
        } 
    }
    public static class Method {
        public const string PARAMS = "params", IN = "in", REF = "ref", OUT = "out";
    }
    public static class Namespace {
        public const string USING = "using";   
    }
    public static class Operator {
        public const string NEW = "new";
    }
    public static class Access {
        public const string BASE = "base", THIS = "this";
    }
    public static class Literal {
        public const string NULL = "null", TRUE = "true", FALSE = "false", DEFAULT = "default";
    }
 }
