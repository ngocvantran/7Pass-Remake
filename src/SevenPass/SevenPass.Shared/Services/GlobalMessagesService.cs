using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Caliburn.Micro;
using SevenPass.IO.Models;
using SevenPass.Messages;

namespace SevenPass.Services
{
    public class GlobalMessagesService : IHandleWithTask<DatabaseSupportMessage>,
        IHandleWithTask<DuplicateDatabaseMessage>,
        IHandle<CachedFileAccessMessage>
    {
        public async Task Handle(DatabaseSupportMessage message)
        {
            var msg = new MessageDialog(
                GetMessage(message.Format))
            {
                Title = "Database File Format",
            };

            await msg.ShowAsync();
        }

        public async Task Handle(DuplicateDatabaseMessage message)
        {
            var msg = new MessageDialog("The selected database file has already been registered. " +
                "7Pass will automatically use the latest version of the database, " +
                "you don't have to add the database again.")
            {
                Title = "Duplicate Database File",
            };

            await msg.ShowAsync();
        }

        public void Handle(CachedFileAccessMessage message)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var template = ToastNotificationManager
                .GetTemplateContent(ToastTemplateType.ToastText02);

            var element = template.GetElementsByTagName("text")[0];
            element.AppendChild(template.CreateTextNode("Source file not available!"));
            element = template.GetElementsByTagName("text")[1];
            element.AppendChild(template.CreateTextNode("Using cached database file."));

            var scheduledToast = new ToastNotification(template);
            notifier.Show(scheduledToast);
        }

        private static string GetMessage(FileFormats format)
        {
            switch (format)
            {
                case FileFormats.KeePass1x:
                    return "7Pass Remake does not support KeePass 1.x database files. " +
                        "Please consider converting it to KeePass 2.x format." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.NewVersion:
                    return "The selected database file is created by a newer " +
                        "version of KeePass that is not supported by 7Pass. " +
                        "7Pass should be updated soon to support this version." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.NotSupported:
                    return "The selected file is not supported " +
                        "by 7Pass, or not a KeePass database file." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.OldVersion:
                    return "The selected database file is too old, and is not supported by 7Pass. " +
                        "Please use KeePass 2 on desktop to migrate it to the current format." +
                        "\r\n\r\nFile not added to 7Pass";

                case FileFormats.PartialSupported:
                    return "The database is created/modified by a newer version of KeePass. " +
                        "Do not make changes to the database with 7Pass to avoid loss of data.";

                default:
                    return null;
            }
        }
    }
}