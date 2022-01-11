using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
namespace EarWorm.Code
{
    
    public static class Util
    {
        static IJSRuntime s_JS;
        public static void Init(IJSRuntime js)
		{
            s_JS = js;
		}
        public async static void Log(string s)
        {
            var thread = System.Threading.Thread.CurrentThread.ManagedThreadId;
            await s_JS.InvokeVoidAsync("console.log", $"[{thread}] {s}");

        }

        public async static ValueTask<string> ReadStorage(string key)
		{
            return await s_JS.InvokeAsync<string>("localStorage.getItem", key);
		}

        public async static void WriteStorage(string key, string value)
		{
            await s_JS.InvokeVoidAsync("localStorage.setItem", key, value);
		}

        public static IJSRuntime JS { get { return s_JS; } }
    }
}
