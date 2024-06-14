using Microsoft.AspNetCore.SignalR;

namespace RemoteJob;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        if (message == "start")
        {
            await JobTest.ExecuteProcessAsync(Notifier);
            return;

            async Task<string> Notifier(string text)
            {
                await Clients.All.SendAsync("ReceiveMessage", user, text, message);
                return text;
            }
        }

        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}