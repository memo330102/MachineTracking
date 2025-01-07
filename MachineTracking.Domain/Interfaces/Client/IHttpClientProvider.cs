using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Domain.Interfaces.Client
{
    public interface IHttpClientProvider
    {
        Task<HttpResponseMessage> RequestPost(string url, object data);
        Task<HttpResponseMessage> RequestGet(string url);
        Task<HttpResponseMessage> RequestGetById(string url, int id);
        Task<HttpResponseMessage> RequestGetByStringId(string url, string id);
        Task<HttpResponseMessage> RequestGetByObject(string url, object data);
    }
}
