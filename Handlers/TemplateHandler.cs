using System;
using System.Collections.Generic;

public class TemplateHandler {

    public Dictionary<string, string> template = new Dictionary<string, string> ();

    public TemplateHandler(List<Template> template_list) {

        /* Standard Templates */
        addTemplate(Keywords.IF, Templates.IF);
        addTemplate(Keywords.WHILE, Templates.WHILE);
        addTemplate(Keywords.FOR, Templates.FOR);

        /* Precompiled functions within scope (also based on usings...) */
        for (int i = 0; i < template_list.Count; i++) {
            addTemplate(template_list[i].key, template_list[i].template_regex);
        }
    }

    public void addTemplate(string key, string template_regex) {
        template.Add(key, template_regex);
    }
    public string getTemplate(string key) {
        return template[key];
    }
    
}