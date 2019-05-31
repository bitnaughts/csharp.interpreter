public class FunctionObject {

    public string return_type;
    public string name;
    public RangeObject range;
    VariableObject[] parameters;

    public FunctionObject (string return_type, string name, RangeObject range, VariableObject[] parameters) {
        this.return_type = return_type;
        this.name = name;
        this.range = range;
        this.parameters = parameters;
    }

    public override string ToString () {
        string output = return_type + " " + name;
        if (parameters != null) {
            for (int i = 0; i < parameters.Length; i++) {
                output += "-> -> Parameter(" + parameters[i].ToString () + ")\n";
            }
        }
        return output;
    }

}