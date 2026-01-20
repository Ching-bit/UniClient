using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Framework.Common;

public class UniViewModel : ObservableObject
{
    public Control? View { get; set; }

    public virtual void OnLoaded(object? sender, RoutedEventArgs e)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(View);
        if (_lastTopLevel != topLevel)
        {
            _lastTopLevel = topLevel;
            _notificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(View))
            {
                Position = NotificationPosition.TopCenter,
                MaxItems = 1
            };
        }
    }
    
    protected void ShowNotification(string message, NotificationType notificationType)
    {
        Notification notification = new()
        {
            Message = ResourceHelper.FindStringResource(message)
        };
        _notificationManager?.Show(notification, type: notificationType);
    }
    
    private WindowNotificationManager? _notificationManager;
    private TopLevel? _lastTopLevel;
}