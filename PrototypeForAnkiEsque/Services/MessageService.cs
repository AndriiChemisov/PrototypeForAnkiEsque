using System.Windows;
// This service is used to display messages to the user. The service is used in the following way:
// var result = _messageService.ShowMessage("Message", "Caption", MessageBoxImage.Information);
// MessageBoxResult is the result of the user's interaction with the message box.
// MessageBoxResult is an enum with the following values: None, OK, Cancel, Yes, No.
// MessageBoxImage is an enum with the following values: None, Hand, Stop, Error, Question, Exclamation, Warning, Asterisk, Information.
// MessageBoxButton is an enum with the following values: OK, OKCancel, YesNo, YesNoCancel, AbortRetryIgnore, RetryCancel.
// Simple explanation: The service is used to display messages to the user as well as prompt them for a choice and then return this choice in the ShowMessageWithButton method.
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
