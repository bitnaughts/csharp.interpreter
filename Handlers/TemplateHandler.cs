using System;
using System.Collections.Generic;

public class TemplateHandler {

    public Dictionary<string, string[,]> template = new Dictionary<string, string[,]> ();

    public TemplateHandler(List<Template> template_list) {

        /* Standard Templates */
        addTemplate(Keyword.IF, Template.IF);
        addTemplate(Keyword.WHILE, Template.WHILE);
        addTemplate(Keyword.FOR, Template.FOR);

        /* Precompiled functions within scope (also based on usings...) */
        for (int i = 0; i < template_list.Count; i++) {
            addTemplate(template_list[i].key, template_list[i].template_array)
        }
    }

    public void addTemplate(string key, string[,] template_array) {
        template.Add(key, template_array);
    }
    public string[,] getTemplate(string key) {
        return template[key];
    }
    
}