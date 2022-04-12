using System;
using System.Threading.Tasks;
using HACC.Components;
using HACC.Enumerations;
using HACC.Extensions;
using HACC.Models.Canvas.WebGL;
using Microsoft.AspNetCore.Components;

namespace HACC.Models.Canvas.Test.ClientSide.Pages;

public partial class WebGL : ComponentBase
{
    private const string VS_SOURCE = "attribute vec3 aPos;" +
                                     "attribute vec3 aColor;" +
                                     "varying vec3 vColor;" +
                                     "void main() {" +
                                     "gl_Position = vec4(aPos, 1.0);" +
                                     "vColor = aColor;" +
                                     "}";

    private const string FS_SOURCE = "precision mediump float;" +
                                     "varying vec3 vColor;" +
                                     "void main() {" +
                                     "gl_FragColor = vec4(vColor, 1.0);" +
                                     "}";

    protected BECanvas _canvas;
    private WebGLContext _context;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        this._context = await this._canvas.CreateWebGLAsync(attributes: new WebGLContextAttributes
        {
            PowerPreference = WebGLContextAttributes.POWER_PREFERENCE_HIGH_PERFORMANCE,
        });

        await this._context.ClearColorAsync(red: 0,
            green: 0,
            blue: 0,
            alpha: 1);
        await this._context.ClearAsync(mask: BufferBits.COLOR_BUFFER_BIT);

        var program = await this.InitProgramAsync(gl: this._context,
            vsSource: VS_SOURCE,
            fsSource: FS_SOURCE);

        var vertexBuffer = await this._context.CreateBufferAsync();
        await this._context.BindBufferAsync(target: BufferType.ARRAY_BUFFER,
            buffer: vertexBuffer);

        var vertices = new[]
        {
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
        };
        await this._context.BufferDataAsync(target: BufferType.ARRAY_BUFFER,
            data: vertices,
            usage: BufferUsageHint.STATIC_DRAW);

        await this._context.VertexAttribPointerAsync(index: 0,
            size: 3,
            type: DataType.FLOAT,
            normalized: false,
            stride: 6 * sizeof(float),
            offset: 0);
        await this._context.VertexAttribPointerAsync(index: 1,
            size: 3,
            type: DataType.FLOAT,
            normalized: false,
            stride: 6 * sizeof(float),
            offset: 3 * sizeof(float));
        await this._context.EnableVertexAttribArrayAsync(index: 0);
        await this._context.EnableVertexAttribArrayAsync(index: 1);

        await this._context.UseProgramAsync(program: program);

        await this._context.DrawArraysAsync(mode: Primitive.TRIANGLES,
            first: 0,
            count: 3);
    }

    private async Task<WebGLProgram> InitProgramAsync(WebGLContext gl, string vsSource, string fsSource)
    {
        var vertexShader = await this.LoadShaderAsync(gl: gl,
            type: ShaderType.VERTEX_SHADER,
            source: vsSource);
        var fragmentShader = await this.LoadShaderAsync(gl: gl,
            type: ShaderType.FRAGMENT_SHADER,
            source: fsSource);

        var program = await gl.CreateProgramAsync();
        await gl.AttachShaderAsync(program: program,
            shader: vertexShader);
        await gl.AttachShaderAsync(program: program,
            shader: fragmentShader);
        await gl.LinkProgramAsync(program: program);

        await gl.DeleteShaderAsync(shader: vertexShader);
        await gl.DeleteShaderAsync(shader: fragmentShader);

        if (!await gl.GetProgramParameterAsync<bool>(program: program,
                pname: ProgramParameter.LINK_STATUS))
        {
            var info = await gl.GetProgramInfoLogAsync(program: program);
            throw new Exception(message: "An error occured while linking the program: " + info);
        }

        return program;
    }

    private async Task<WebGLShader> LoadShaderAsync(WebGLContext gl, ShaderType type, string source)
    {
        var shader = await gl.CreateShaderAsync(type: type);

        await gl.ShaderSourceAsync(shader: shader,
            source: source);
        await gl.CompileShaderAsync(shader: shader);

        if (!await gl.GetShaderParameterAsync<bool>(shader: shader,
                pname: ShaderParameter.COMPILE_STATUS))
        {
            var info = await gl.GetShaderInfoLogAsync(shader: shader);
            await gl.DeleteShaderAsync(shader: shader);
            throw new Exception(message: "An error occured while compiling the shader: " + info);
        }

        return shader;
    }
}