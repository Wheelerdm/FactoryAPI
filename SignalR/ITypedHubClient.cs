using FactoryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactoryAPI.SignalR
{
    public interface ITypedHubClient
    {
        Task BroadcastMessage(string type, FactoryNode factoryNode);
    }
}
