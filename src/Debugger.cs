using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RitoClient
{
    internal partial class Debugger
    {
        int Port;

        public string? FrontEndUrl { get; private set; }
        public string? WebSocketUrl { get; private set; }

        public DevTools DevTools;

        public Debugger(int port)
        {
            this.Port = port;
            this.DevTools = new DevTools();
        }

        public async Task Connect()
        {
            using var client = new HttpClient();

            while (true)
            {
                var url = $"http://localhost:{Port}/json";
                var json = await client.GetStringAsync(url);
                var list = JsonSerializer.Deserialize(json, DebuggerJsonSerializer.Default.ListDebuggerItem);
                if (list != null && list.Count > 0)
                {
                    var item = list!.Find(e => e.title == "Riot Client" && e.type == "page");

                    FrontEndUrl = item!.devtoolsFrontendUrl;
                    WebSocketUrl = item!.webSocketDebuggerUrl;

                    await DevTools.Connect(WebSocketUrl);
                    break;
                }

                await Task.Delay(100);
            }
        }

        record DebuggerItem(
            string title,
            string type,
            string url,
            string webSocketDebuggerUrl,
            string devtoolsFrontendUrl
        );

        [JsonSerializable(typeof(DebuggerItem))]
        [JsonSerializable(typeof(List<DebuggerItem>))]
        partial class DebuggerJsonSerializer : JsonSerializerContext
        {
        }

        public void OpenRemoteDevTools()
        {
            Utils.OpenUrl($"http://localhost:{Port}{FrontEndUrl}");
        }
    }
}
