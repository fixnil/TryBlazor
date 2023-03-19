using Masa.Blazor;
using Masa.Blazor.Extensions.Languages.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;

namespace TryBlazor.Pages;

public partial class Index
{
    private Type? _type;
    private MMonacoEditor? _editor;
    private readonly object _options = new
    {
        theme = "vs-dark",
        language = "razor",
        automaticLayout = true,
        value = "<p>Hello World!</p>"
    };

    private readonly List<string> _assemblies = new()
    {
        "netstandard",
        "System",
        "System.Collections",
        "System.Linq",
        "System.Runtime",
        "System.Linq.Expressions",
        "System.Net.Http.Json",
        "System.Private.CoreLib",
        "Microsoft.AspNetCore.Components",
        "Microsoft.AspNetCore.Components.Web",
        "OneOf",
        "Masa.Blazor",
        "BlazorComponent",
        "FluentValidation",
        "FluentValidation.DependencyInjectionExtensions",
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            RazorCompile.Initialized(
                await this.GetReferenceAsync(),
                this.GetRazorExtension());

            this.StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task RunCode()
    {
        if (_editor is not null)
        {
            var code = await _editor.GetValue();

            _type = RazorCompile.CompileToType(new CompileRazorOptions
            {
                Code = code,
            });

            this.StateHasChanged();
        }
    }

    private async Task<List<PortableExecutableReference>?> GetReferenceAsync()
    {
        var references = new List<PortableExecutableReference>();

        foreach (var assembly in _assemblies)
        {
            try
            {
                await using var stream = await this.HttpClient!
                    .GetStreamAsync($"_framework/{assembly}.dll");

                if (stream.Length > 0)
                {
                    references?.Add(MetadataReference.CreateFromStream(stream));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        return references;
    }

    private List<RazorExtension> GetRazorExtension()
    {
        var extensions = new List<RazorExtension>();

        foreach (var asm in typeof(Index).Assembly.GetReferencedAssemblies())
        {
            extensions.Add(new AssemblyExtension(asm.FullName, AppDomain.CurrentDomain.Load(asm.FullName)));
        }

        return extensions;
    }
}