using System.Windows;

namespace PrototypeForAnkiEsque.Services
{
    public class MessageService : IMessageService
    {
        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        public MessageBoxResult ShowMessageWithButton(string message, string caption, MessageBoxImage icon, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, caption, buttons, icon);
        }
    }
}
