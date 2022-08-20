using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGuessCodeBot
{

    public class CodeReference
    {
        public string Language;
        public string ReferenceURL;

        public CodeReference(string language, string url)
        {
            this.Language = language;
            this.ReferenceURL = url;
        }
    }

    public class CodeReferences : List<CodeReference>
    {
        public CodeReferences()
        {
            GetCodeReferences();
        }

        public List<CodeReference> GetCodeReferences()
        {
            this.Add(new CodeReference("C#", "https://raw.githubusercontent.com/relativemodder/CodeGuessStorage/main/1.cs"));
            this.Add(new CodeReference("C#", "https://raw.githubusercontent.com/relativemodder/CodeGuessStorage/main/2.cs"));

            this.Add(new CodeReference("Python", "https://raw.githubusercontent.com/relativemodder/CodeGuessStorage/main/3.py"));
            this.Add(new CodeReference("Python", "https://raw.githubusercontent.com/relativemodder/CodeGuessStorage/main/4.py"));



            this.Add(new CodeReference("PHP", "https://raw.githubusercontent.com/relativemodder/CodeGuessStorage/main/5.php"));


            return this;
        }

    }
}
