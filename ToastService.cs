using Aarati_s_Journal.Interfaces;

namespace Aarati_s_Journal.Services;

public class ToastService : IToastService
{
    public Task ShowAsync(string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current!.MainPage!.DisplayAlert("Aarati's Journal", message, "OK");
        });
        return Task.CompletedTask;
    }
}
