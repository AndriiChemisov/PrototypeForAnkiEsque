using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Services
{

    // Interface for managing settings persistence
    public interface ISettingsManager
    {
        string GetSavedLanguage();
        void SaveLanguageSetting(string language);
    }
}
