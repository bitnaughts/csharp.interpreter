/* Wrapper for scope starts/ends */
public class RangeObject {
    public int start;
    public int end;
    public int return_to;

    //what index of parts[] called this function? for inline function calls...
    public int return_to_index;

    public RangeObject (int start, int end) {
        this.start = start;
        this.end = end;
        this.return_to = end;
    }
    public RangeObject (int start, int end, int return_to) {
        this.start = start;
        this.end = end;
        this.return_to = return_to;
    }

    public static RangeObject getScopeRange (string[] script, int start_line) {
        int end_line = start_line;

        int bracket_count = 1;
        while (bracket_count > 0 && end_line < script.Length) {
            end_line++;
            if (script[end_line].Contains (Operators.OPENING_BRACKET)) {
                bracket_count++;
            } else if (script[end_line] == Operators.CLOSING_BRACKET) {
                bracket_count--;
            }
        }
        return new RangeObject (start_line, end_line);
    }

    public static RangeObject returnTo (RangeObject range, int return_to) {
        return new RangeObject (range.start, range.end, return_to+1);
    }
    public override string ToString () {
        return "Range(" + start + " -> " + end + ")\n";
    }
}