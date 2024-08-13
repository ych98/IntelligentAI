// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

namespace IntelligentAI.Components.Universal.DisplayGroup;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


public partial class CodeSnippet
{
    private ElementReference codeElement;

    private IJSObjectReference _jsModule = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string Language { get; set; } = "language-cshtml-razor";

    [Parameter]
    public string? Style { get; set; } = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("hljs.highlightElement", codeElement);
            await JSRuntime.InvokeVoidAsync("addCopyButton");
        }
    }
}
