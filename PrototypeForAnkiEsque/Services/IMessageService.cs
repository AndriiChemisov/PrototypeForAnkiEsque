using System.Windows;

namespace PrototypeForAnkiEsque.Services
{
    public interface IMessageService
    {
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxImage icon);
        MessageBoxResult ShowMessageWithButton(string message, string caption, MessageBoxImage icon, MessageBoxButton buttons);

    }
}
