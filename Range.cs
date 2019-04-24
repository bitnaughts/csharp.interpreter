/* Wrapper for scope starts/ends */
public class Range {
    public int start;
    public int end;
    public int return_to;

    public Range (int start, int end) {
        this.start = start;
        this.end = end;
        this.return_to = end;
    }
    public Range (int start, int end, int return_to) {
        this.start = start;
        this.end = end;
        this.return_to = return_to;
    }

    public static Range getScopeRange (string[] script, int start_line) {
        int end_line = start_line;

        int bracket_count = 1;
        while (bracket_count > 0) {
            end_line++;
            if (script[end_line].Contains (Operators.OPENING_BRACKET)) {
                bracket_count++;
            } else if (script[end_line] == Operators.CLOSING_BRACKET) {
                bracket_count--;
            }
        }
        return new Range (start_line, end_line);
    }

    public static Range returnTo (Range range, int return_to) {
        return new Range (range.start, range.end, return_to+1);
    }
    public override string ToString () {
        return "Range(" + start + " -> " + end + ")\n";
    }
}