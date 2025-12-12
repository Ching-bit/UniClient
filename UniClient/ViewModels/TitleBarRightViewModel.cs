using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Control.Basic;
using Framework.Common;
using Plugin.AppEnv;
using Plugin.Log;
using UniClient.Models;
using Ursa.Controls;

namespace UniClient;

public partial class TitleBarRightViewModel : UniViewModel
{
    [RelayCommand] private async Task Setting()
    {
        ConfirmDialogResult? result =
            await Dialog.ShowCustomModal<SettingDialog, SettingDialogViewModel, ConfirmDialogResult>(
                new SettingDialogViewModel(),
                options: new DialogOptions
                {
                    // Title = ResourceHelper.FindStringResource("R_STR_SETTING", string.Empty),
                    Mode = DialogMode.Info,
                    CanDragMove = true,
                    IsCloseButtonVisible = true,
                    CanResize = false
                });
        if (true != result?.IsConfirmed ||
            result.ReturnParameter is not SettingModel settingModel)
        {
            return;
        }
        settingModel.SyncToGlobalSetting();
    }

    [RelayCommand] private async Task Connections()
    {
        ConfirmDialogResult? result =
            await Dialog.ShowCustomModal<ConnectionsDialog, ConnectionsDialogViewModel, ConfirmDialogResult>(
                new ConnectionsDialogViewModel(),
                options: new DialogOptions
                {
                    Mode = DialogMode.Info,
                    CanDragMove = true,
                    IsCloseButtonVisible = true,
                    CanResize = false
                });
        if (true != result?.IsConfirmed ||
            result.ReturnParameter is not ObservableCollection<ConnConfModel> connConfs)
        {
            return;
        }

        string? selectedConnName = connConfs.FirstOrDefault(x => x.IsSelected)?.Name;
        if (null == selectedConnName)
        {
            Global.Get<ILog>().Error(LogModule.DIALOG, "No connection selected");
            return; // impossible
        }

        if (selectedConnName != Global.Get<IGlobalSetting>().SelectedConn &&
            null != Global.Get<IAppEnv>().User)
        {
            MessageDialog messageDialog = new MessageDialog
            {
                Message = ResourceHelper.FindStringResource("R_STR_CHANGE_CONN_NOTICE")
            };
            ConfirmDialogResult? reconnectConfirmResult =
                await Dialog.ShowCustomModal<ConfirmDialogResult>(
                    messageDialog, new ConfirmDialogViewModel(),
                    options: new DialogOptions
                    {
                        Mode = DialogMode.Info,
                        CanDragMove = true,
                        IsCloseButtonVisible = true,
                        CanResize = false
                    });
            if (true != reconnectConfirmResult?.IsConfirmed)
            {
                return;
            }
        }

        Global.Get<IGlobalSetting>().SelectedConn = selectedConnName;
        ConnConf? selectedConn = SystemConfig.ConnConfDict[ConnConfType.Trade]
            .FirstOrDefault(x => x.Name == selectedConnName);
        if (null == selectedConn)
        {
            return; // impossible
        }
        WeakReferenceMessenger.Default.Send(new ConnectionChangedMessage(selectedConn));
    }
    
}