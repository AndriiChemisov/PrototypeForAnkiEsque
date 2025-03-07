using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using PrototypeForAnkiEsque.Resources;

public class LocalizationExtension : Binding
{
    public LocalizationExtension(string name) : base($"[{name}]")
    {
        this.Source = Strings.ResourceManager;
    }
}

