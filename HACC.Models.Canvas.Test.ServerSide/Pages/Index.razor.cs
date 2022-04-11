using System.Threading.Tasks;
using HACC.Components;
using HACC.Extensions;
using HACC.Models.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;

namespace HACC.Models.Canvas.Test.ServerSide.Pages;

public partial class Index : ComponentBase
{
    protected WebConsole _webConsole;
    private Canvas2DContext _context;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        this._context = await this._webConsole.CreateCanvas2DAsync();
        await this._context.SetFillStyleAsync(value: "green");

        await this._context.FillRectAsync(x: 10,
            y: 100,
            width: 100,
            height: 100);

        await this._context.SetFontAsync(value: "48px serif");
        await this._context.StrokeTextAsync(text: "Hello Blazor!!!",
            x: 10,
            y: 100);
    }
}