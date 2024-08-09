using Markdig;
using Markdig.Extensions.GenericAttributes;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IntelligentAI.Components.Universal;

public partial class MarkdownSection : FluentComponentBase
{
    private Lazy<Task<IJSObjectReference>> moduleTask;
    private IJSObjectReference _jsModule;
    private bool _markdownChanged = false;
    private string? _content;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Markdown content 
    /// </summary>
    [Parameter]
    public string? Content
    {
        get => _content;
        set
        {
            if (_content is not null && !_content.Equals(value))
            {
                _markdownChanged = true;
            }
            _content = value;
        }
    }

    [Parameter]
    public EventCallback OnContentConverted { get; set; }

    [Parameter]
    public bool DisplayMode { get; set; }

    public MarkupString HtmlContent { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // import code for highlighting code blocks
            moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/IntelligentAI.Components/Universal/MarkdownSection.razor.js").AsTask());

            _jsModule = await moduleTask.Value;
        }

        if (firstRender || _markdownChanged)
        {
            _markdownChanged = false;

            // create markup from markdown source
            HtmlContent = await MarkdownToMarkupStringAsync();

            StateHasChanged();

            // notify that content converted from markdown 
            if (OnContentConverted.HasDelegate)
            {
                await OnContentConverted.InvokeAsync();
            }

            await _jsModule.InvokeVoidAsync("highlight");

            await _jsModule.InvokeVoidAsync("addCopyButton");
        }

    }

    /// <summary>
    /// Converts markdown, provided in Content or from markdown file stored as a static asset, to MarkupString for rendering.
    /// </summary>
    /// <returns>MarkupString</returns>
    private async Task<MarkupString> MarkdownToMarkupStringAsync()
    {
        string? markdown = Content;

        return ConvertToMarkupString(markdown);
    }

    private static MarkupString ConvertToMarkupString(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var builder = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Use<MarkdownSectionPreCodeExtension>();

            var pipeline = builder.Build();

            // Convert markdown string to HTML
            var html = Markdown.ToHtml(value, pipeline);

            // Return sanitized HTML as a MarkupString that Blazor can render
            return new MarkupString(html);
        }

        return new MarkupString();
    }
}
internal class MarkdownSectionPreCodeExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        var htmlRenderer = renderer as TextRendererBase<HtmlRenderer>;
        if (htmlRenderer == null)
        {
            return;
        }

        var originalCodeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
        if (originalCodeBlockRenderer != null)
        {
            htmlRenderer.ObjectRenderers.Remove(originalCodeBlockRenderer);
        }

        htmlRenderer.ObjectRenderers.AddIfNotAlready(new MarkdownSectionPreCodeRenderer(
                new MarkdownSectionPreCodeRendererOptions
                {
                    PreTagAttributes = "{.snippet .hljs-copy-wrapper}",
                })
            );
    }
}

/// <summary>
/// Modified version of original markdig CodeBlockRenderer
/// </summary>
/// <see href="https://github.com/xoofx/markdig/blob/master/src/Markdig/Renderers/Html/CodeBlockRenderer.cs"/>
internal class MarkdownSectionPreCodeRenderer : HtmlObjectRenderer<CodeBlock>
{
    private HashSet<string>? _blocksAsDiv;
    private readonly MarkdownSectionPreCodeRendererOptions? _options;

    public MarkdownSectionPreCodeRenderer(MarkdownSectionPreCodeRendererOptions? options)
    {
        _options = options;
    }

    public bool OutputAttributesOnPre { get; set; }

    /// <summary>
    /// Gets a map of fenced code block infos that should be rendered as div blocks instead of pre/code blocks.
    /// </summary>
    public HashSet<string> BlocksAsDiv => _blocksAsDiv ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    protected override void Write(HtmlRenderer renderer, CodeBlock obj)
    {
        renderer.EnsureLine();

        if (_blocksAsDiv is not null && (obj as FencedCodeBlock)?.Info is string info && _blocksAsDiv.Contains(info))
        {
            var infoPrefix = (obj.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            // We are replacing the HTML attribute `language-mylang` by `mylang` only for a div block
            // NOTE that we are allocating a closure here

            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<div")
                        .WriteAttributes(obj.TryGetAttributes(),
                            cls => cls.StartsWith(infoPrefix, StringComparison.Ordinal) ? cls.Substring(infoPrefix.Length) : cls)
                        .Write('>');
            }

            renderer.WriteLeafRawLines(obj, true, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</div>");
            }
        }
        else
        {
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<pre");

                WritePreAttributes(renderer, obj, _options?.PreTagAttributes);

                renderer.Write("><code");

                WriteCodeAttributes(renderer, obj, _options?.CodeTagAttributes);

                renderer.Write('>');
            }

            renderer.WriteLeafRawLines(obj, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</code></pre>");
            }
        }

        renderer.EnsureLine();
    }

    private void WritePreAttributes(HtmlRenderer renderer, CodeBlock obj, string? preGenericAttributes)
    {
        HtmlAttributes? orig = null;

        if (OutputAttributesOnPre)
        {
            orig = obj.TryGetAttributes();
        }

        WriteElementAttributes(renderer, orig, preGenericAttributes);
    }

    private void WriteCodeAttributes(HtmlRenderer renderer, CodeBlock obj, string? codeGenericAttributes)
    {
        HtmlAttributes? orig = null;

        if (!OutputAttributesOnPre)
        {
            orig = obj.TryGetAttributes();
        }

        WriteElementAttributes(renderer, orig, codeGenericAttributes);
    }
    static private void WriteElementAttributes(HtmlRenderer renderer, HtmlAttributes? fromCodeBlock, string? genericAttributes)
    {
        // origin code block had no attributes
        fromCodeBlock ??= new HtmlAttributes();

        // append if any additional attributes provided
        var ss = new StringSlice(genericAttributes);
        if (!ss.IsEmpty && GenericAttributesParser.TryParse(ref ss, out var attributes))
        {
            if (fromCodeBlock != null)
            {
                if (attributes.Classes != null)
                {
                    foreach (var a in attributes.Classes)
                    {
                        fromCodeBlock.AddClass(a);
                    }
                }
                if (attributes.Properties != null)
                {
                    foreach (var pr in attributes.Properties)
                    {
                        fromCodeBlock.AddProperty(pr.Key, pr.Value!);
                    }
                }
            }
        }

        //
        renderer.WriteAttributes(fromCodeBlock);
    }
}

internal class MarkdownSectionPreCodeRendererOptions
{
    /// <summary>
    /// html attributes for Tag element in markdig generic attributes format
    /// </summary>
    public string? PreTagAttributes;
    /// <summary>
    /// html attributes for Code element in markdig generic attributes format
    /// </summary>
    public string? CodeTagAttributes;
}