public static class Templates {
    // if = "(\t)\n"
    public const string IF = Operators.OPENING_PARENTHESIS + Operators.TAB + Operators.CLOSING_PARENTHESIS,
        WHILE =  Operators.OPENING_PARENTHESIS + Operators.TAB + Operators.CLOSING_PARENTHESIS,
        FOR = Operators.EQUALS + Operators.TAB + Operators.END_LINE + Operators.NEW_LINE + Operators.END_LINE + Operators.TAB + Operators.END_LINE + Operators.NEW_LINE + Operators.END_LINE + Operators.TAB + Operators.CLOSING_PARENTHESIS;
}

public class Template {
    public string key;
    public string template_regex;

    public Template(string key, string template_regex) {
        this.key = key;
        this.template_regex = template_regex;
    }
}