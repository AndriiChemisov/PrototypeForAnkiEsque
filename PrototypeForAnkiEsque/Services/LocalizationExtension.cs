using System;
using System.Windows;
using System.Windows.Data;
using PrototypeForAnkiEsque.Resources;

namespace PrototypeForAnkiEsque.Services
{
    public class LocalizationExtension : Binding
    {
        public LocalizationExtension() : base()
        {
            // Default constructor.
        }

        public LocalizationExtension(string name) : base()
        {
            // Set the path for localized values
            this.Source = Strings.ResourceManager;
            this.Path = new PropertyPath($"[{name}]");
        }
    }
}
