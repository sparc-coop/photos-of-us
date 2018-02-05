using System;

namespace Rotativa.NetCore.Options
{
    public class OptionFlag : Attribute
    {
        public string Name { get; private set; }

        public OptionFlag(string name)
        {
            this.Name = name;
        }
    }
}
