using GLFW;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using UGXP.Core.Components;
using UGXP.Game;
using UGXP.Util;

namespace UGXP.Core.Render;
internal class GLContext : Context
{
    private class BindingsContext : IBindingsContext {
        public delegate IntPtr GetProcAddressHandler(string procName);
        public GetProcAddressHandler handler;
        public BindingsContext(GetProcAddressHandler handler) {
            this.handler = handler;
        }

        public IntPtr GetProcAddress(string procName) {
            return handler.Invoke(procName);
        }
    }

    public static GLContext Instance { get; private set; }

    public GLContext(GameProcess game) : base(game) {
        Instance = this;
    }

    public override void CreateWindow(string name, GameSettings.FullscreenMode fullScreen, bool vSync, Vector2 dimensions) {
        base.CreateWindow(name, fullScreen, vsync, dimensions);

        Glfw.Init();

        Glfw.WindowHint(Hint.ContextVersionMajor, 4);
        Glfw.WindowHint(Hint.ContextVersionMinor, 4);

        Glfw.WindowHint(Hint.Samples, 8);
        Glfw.WindowHint(Hint.RedBits, 8);
        Glfw.WindowHint(Hint.GreenBits, 8);
        Glfw.WindowHint(Hint.BlueBits, 8);
        Glfw.WindowHint(Hint.AlphaBits, 8);
        Glfw.WindowHint(Hint.DepthBits, 24);
        Glfw.WindowHint(Hint.StencilBits, 0);

        GLFW.Monitor monitor = GLFW.Monitor.None;

        if (fullScreen == GameSettings.FullscreenMode.FULLSCREEN)
            monitor = Glfw.PrimaryMonitor;
        else if (fullScreen == GameSettings.FullscreenMode.BORDERLESS)
            Glfw.WindowHint(Hint.Decorated, false);
        else
            Glfw.WindowHint(Hint.Decorated, true);

        window = Glfw.CreateWindow((int) dimensions.x, (int) dimensions.y, name, monitor, Window.None);

        Glfw.MakeContextCurrent(window);
        Glfw.SwapInterval(vSync ? 1 : 0);

        GL.LoadBindings(new BindingsContext(Glfw.GetProcAddress));

        GL.Viewport(0, 0, (int) windowSize.x, (int) windowSize.y);

        // callbacks
        BindCallbacks();
    }

    private void BindCallbacks() {
        keyCallback = (_, key, scanCode, state, mods) => {
            if (state == GLFW.InputState.Press || state == GLFW.InputState.Repeat) {
                anyKeyDown = true;
                keyPressedCount++;
            }
            else
                keyPressedCount--;

            keys[(int)key] = (InputState) state;
        };
        mouseButtonCallback = (_, button, state, _) => mouseButtons[(int) button] = (InputState) state;
        mouseCallback = (_, x, y) => {
            if (cursorLocked) return;

            mousePosition = new Vector2((float) x, (float) y);
        };
        sizeCallback = (window, width, height) => {
            windowSize = new Vector2(width, height);
            GL.Viewport(0, 0, width, height);

            Camera.Main.RefreshUnitSize();
        };

        Glfw.SetKeyCallback(window, keyCallback);
        Glfw.SetMouseButtonCallback(window, mouseButtonCallback);
        Glfw.SetCursorPositionCallback(window, mouseCallback);
        Glfw.SetWindowSizeCallback(window, sizeCallback);
    }

    public int[] CreateTexVerts(float[] vertices, uint[] indices, ShaderProgram shader, BufferUsageHint drawType = BufferUsageHint.StaticDraw) {
        int vao, vbo, ebo;

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, drawType);

        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, drawType);

        shader.Use();

        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        shader.SetInt("slot", 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        shader.Unbind();

        return new int[] { vao, vbo, ebo };
    }

    public int[] CreateVerts(float[] vertices, ShaderProgram shader, uint[] indices = null, BufferUsageHint drawType = BufferUsageHint.StaticDraw) {
        int vao, vbo;
        int ebo = 0;

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, drawType);

        if (indices != null) {
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, drawType);
        }

        shader.Use();

        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        shader.Unbind();

        return new int[] { vao, vbo, ebo };
    }

    public void DisposeBuffers(int[] buffers) {
        GL.DeleteVertexArray(buffers[0]);
        GL.DeleteBuffer(buffers[1]);
        if (buffers.Length > 2 && buffers[2] > 0)
            GL.DeleteBuffer(buffers[2]);
    }

    public void DrawArrayLines(int vao, int count = 2) {
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, count);
        GL.BindVertexArray(0);
    }

    public void DrawLines(int vao, uint[] indices) {
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void DrawTriangles(int vao, uint[] indices) {
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

    }

    public override void Run() {
        // perform gl setup
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // main loop
        Glfw.Time = 0.0D;
        do {
            HandleCursorLock();

            if (vsync || Time.time - lastFrameTime > 1000 / targetFrameRate) {
                lastFrameTime = Time.time;

                frameCount++;
                if (Time.time - lastFPSTime > 1000) {
                    lastFPS = (int)(frameCount / ((Time.time - lastFPSTime) / 1000.0f));
                    lastFPSTime = Time.time;
                    frameCount = 0;
                }
                game.Step();
                soundSystem.Step();

                ResetHitCounters();

                Display();

                Time.newFrame();
                Glfw.PollEvents();
            }
        } while (!Glfw.WindowShouldClose(window));

        // the process is being terminated
        Close();
    }

    public override void Display() {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        Color bgColor = (Color) Camera.Main.background;
        GL.ClearColor(bgColor[0], bgColor[1], bgColor[2], bgColor[3]);

        game.Render();
        
		// Swap the back buffer with the front buffer
		Glfw.SwapBuffers(window);
    }

    public override void Close() {
        soundSystem.Deinit();
        
        Glfw.SetWindowShouldClose(window, true);
        Glfw.Terminate();

        game.Close();

        Environment.Exit(0);
    }

    public override void SetScissor(Vector2 position, Vector2 size) {
        // TODO
        //if (width == WindowSize.instance.width && height == WindowSize.instance.height)
        //{
        //    //GL.Disable(GL.SCISSOR_TEST);
        //    GL.Disable(EnableCap.ScissorTest);
        //}
        //else
        //{
        //    //GL.Enable(GL.SCISSOR_TEST);
        //    GL.Enable(EnableCap.ScissorTest);
        //}

        //GL.Scissor(
        //    (int)(x * _realToLogicWidthRatio),
        //    (int)(y * _realToLogicHeightRatio),
        //    (int)(width * _realToLogicWidthRatio),
        //    (int)(height * _realToLogicHeightRatio)
        //);
    }

    private void HandleCursorLock() {
        if (cursorLocked)
            Glfw.SetCursorPosition(window, mousePosition.x, mousePosition.y);
        
        //if (cursorLockedInWindow) {
        //    if (mousePosition.x < 1) {
        //        Glfw.SetCursorPosition(window, 1, mousePosition.y);
        //        mousePosition.x = 1;
        //    }
        //    if (mousePosition.y < 1) {
        //        Glfw.SetCursorPosition(window, mousePosition.x, 1);
        //        mousePosition.y = 1;
        //    }

        //    if (mousePosition.x > windowSize.x - 1) {
        //        Glfw.SetCursorPosition(window, windowSize.x - 1, mousePosition.y);
        //        mousePosition.x = windowSize.x - 1;
        //    }
        //    if (mousePosition.y > windowSize.y - 1) {
        //        Glfw.SetCursorPosition(window, mousePosition.x, windowSize.y - 1);
        //        mousePosition.y = windowSize.y - 1;
        //    }
        //}
    }

    public override void ShowCursor() {
        Glfw.SetInputMode(window, InputMode.Cursor, (int) CursorMode.Normal);
    }
    public override void HideCursor(bool lockInWindow = false) {
        Glfw.SetInputMode(window, InputMode.Cursor, lockInWindow ? (int) CursorMode.Disabled : (int) CursorMode.Hidden);
    }

    public override void UnlockCursor() {
        cursorLockedInWindow = false;
        cursorLocked = false;
    }

    // locking cursor in window doesn't seem to work with glfw, will look further into it another time...
    public override void LockCursor(bool inWindow = false) {
        if (inWindow) {
            cursorLockedInWindow = true;
        }
        else
            cursorLocked = true;
    }
}
