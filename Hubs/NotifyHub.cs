using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ChartAPI.Hubs
{
    public class NotifyHub : Hub
    {
        // 這個方法可以讓前端呼叫（可選）
        public async Task JoinGroup(string connectionId)
        {
            await Groups.AddToGroupAsync(connectionId, "TaskGroup");
        }
    }
}
