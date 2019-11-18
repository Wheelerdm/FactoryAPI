using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks; 

namespace FactoryAPI.SignalR
{
    public class NotifyHub:Hub<ITypedHubClient>
    {
        public async Task SendMessage(string user, string message)
        {
            
        }
    }
}
