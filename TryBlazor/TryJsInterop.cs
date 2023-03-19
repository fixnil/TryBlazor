using BlazorComponent.JSInterop;
using Microsoft.JSInterop;

namespace TryBlazor;

public class TryJsInterop : JSModule
{
    public TryJsInterop(IJSRuntime js)
        : base(js, "./_content/TryBlazor/tryJsInterop.js")
    {
    }

    public async ValueTask Init()
    {
        await this.InvokeVoidAsync("init");
    }

    public async ValueTask SetStorage(string key, string value)
    {
        await this.InvokeVoidAsync("setStorage", key, value);
    }

    public async ValueTask<string?> GetStorage(string key)
    {
        return await this.InvokeAsync<string?>("getStorage", key);
    }

    public async ValueTask DelStorage(string key)
    {
        await this.InvokeVoidAsync("delStorage", key);
    }

    public async ValueTask ClearStorage()
    {
        await this.InvokeVoidAsync("clearStorage");
    }

    public async ValueTask AddCommand<T>(IJSObjectReference id, int keybinding, DotNetObjectReference<T> dotNetObjectReference, string method) where T : class
    {
        await this.InvokeVoidAsync("addCommand", id, keybinding, dotNetObjectReference, method);
    }
}
