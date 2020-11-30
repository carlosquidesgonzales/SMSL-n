using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DesignPatternCmsInlupp.Builders
{
    public class RapportElement
    {
        public string Name, Text;
        public List<RapportElement> Elements = new List<RapportElement>();
        public RapportElement()
        {

        }
        public RapportElement(string name, string text)
        {
            Name = name;
            Text = text;
        }
        private string ToStringImpl()
        {
            
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Text))
            {
                if(!string.IsNullOrWhiteSpace(Name))
                {
                    sb.Append(Name);
                    sb.Append(Text);
                }
                else
                    sb.Append(Text);

            }
            foreach (var e in Elements)
                sb.Append(e.ToStringImpl());
            return sb.ToString();
        }
        public override string ToString()
        {
            return ToStringImpl();
        }
    }
}