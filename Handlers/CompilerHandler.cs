using System.Collections.Generic;

public class CompilerHandler {
    /*
        Needs to determine::
            - Where the main function is
            - What Listeners to add (list)
            - What Function Objects to add (list)
            - What Variable Objects to add (list) to base scope...
     */

    public int main_function_line;

    public List<string> handlers;
    public List<Template> template_list;

    public FunctionHandler function_handler;
    public ScopeObject base_scope;
    // public List<VariableObject> variables;

    public CompilerHandler (string[] script) {
        handlers = new List<string> ();
        template_list = new List<Template> ();
        base_scope = new ScopeObject (new RangeObject(-1, 1000), false);
        function_handler = new FunctionHandler ();

        compile (script);

        base_scope.setRange (RangeObject.getScopeRange (script, main_function_line));
    }

    public void compile (string[] script) {
        int scope_depth = 0;

        for (int i = 0; i < script.Length; i++) {
            string line = script[i];
            string[] line_parts;

            /* Manage current depth of scope (relevant for public variables, function declarations) */

            switch (scope_depth) {
                case 0:
                    /* Outside Class declaration, for imports */
                    line_parts = line.Split (' ');
                    switch (line_parts[0]) {
                        case Keywords.Namespace.USING:
                            handlers.Add (line_parts[1].Substring (0, line_parts[1].Length - 1));
                            break;
                    }
                    break;
                case 1:
                    /* Inside Class declaration, but not inside any functions */
                    line_parts = line.Split (' ');
                    switch (line_parts[0]) {
                        case Keywords.Type.VOID:
                        case Keywords.Type.Value.BOOLEAN:
                        case Keywords.Type.Value.INTEGER:
                        case Keywords.Type.Value.FLOAT:
                        case Keywords.Type.Reference.STRING:
                            if (line.Contains (Operators.OPENING_PARENTHESIS)) {
                                /* Function declaration, e.g. "void sum (int x, int y) {" */
                                // string[] parameters = line.Split(",");
                                // if (parameters.Length == 0) {
                                //     Template function_template = new Template(line_parts[1], null);   
                                // }
                                // if (parameters.Length == 1) {

                                // }
                                Template function_template = new Template(line_parts[1], null);
                                template_list.Add(function_template);

                                function_handler.declareFunction (line, RangeObject.getScopeRange (script, i));
                            } else {
                                /* Variable declaration, e.g. "int y;" */
                                base_scope.variable_handler.declareVariable (line);
                            }
                            break;
                        case Keywords.Modifier.STATIC:
                            /* the Main function is the only static function allowed */
                            main_function_line = i;
                            break;
                    }
                    break;
            }
            if (line.Contains (Operators.OPENING_BRACKET)) scope_depth++;
            if (line.Contains (Operators.CLOSING_BRACKET)) scope_depth--;
        }
    }
    public override string ToString () {
        string output = function_handler.ToString ();
        output += "\n" + main_function_line + "\n";
        return output;
    }
}