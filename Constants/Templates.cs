public static class Templates {
    public const string[, ]  IF = [[Operators.OPENING_PARENTHESIS, Operators.CLOSING_PARENTHESIS]],
        WHILE = [[Operators.OPENING_PARENTHESIS, Operators.CLOSING_PARENTHESIS]],
        FOR = [[Operators.EQUALS, Operators.END_LINE], [Operators.END_LINE, Operators.END_LINE], [Operators.END_LINE, Operators.CLOSING_PARENTHESIS]];
}

public class Template {
    public string key;
    public string[,] template_array;

    public Template(string key, string[,] template_array) {
        this.key = key;
        this.template_array = template_array;
    }
}