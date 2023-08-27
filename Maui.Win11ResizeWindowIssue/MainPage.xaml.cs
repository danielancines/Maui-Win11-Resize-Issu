using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Maui.Win11ResizeWindowIssue
{
    public partial class MainPage : ContentPage
    {
        [DllImport("USER32.dll", ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        internal static extern HWND SetParent(HWND hWndChild, HWND hWndNewParent);

        [DllImport("USER32.dll", ExactSpelling = true, EntryPoint = "SetWindowLongW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SupportedOSPlatform("windows5.0")]
        internal static extern int SetWindowLong(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, int dwNewLong);
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            var newWindow = new Window(new ContentPage()) {  Width = 200, Height = 200 };
            Application.Current.OpenWindow(newWindow);

            (newWindow.Handler.PlatformView as MauiWinUIWindow).ExtendsContentIntoTitleBar = false;
            var appWindow = (newWindow.Handler.PlatformView as MauiWinUIWindow).GetAppWindow();
            if (appWindow.Presenter is OverlappedPresenter presenter)
                presenter.SetBorderAndTitleBar(true, true);

            var childHandle = new HWND((newWindow.Handler.PlatformView as MauiWinUIWindow).GetWindowHandle());
            var parentHandle = new HWND((this.Window.Handler.PlatformView as MauiWinUIWindow).GetWindowHandle());

            SetParent(childHandle, parentHandle);
            SetWindowLong(childHandle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, -20);
        }
    }

    internal enum WINDOW_LONG_PTR_INDEX
    {
        GWL_EXSTYLE = -20,
        GWLP_HINSTANCE = -6,
        GWLP_HWNDPARENT = -8,
        GWLP_ID = -12,
        GWL_STYLE = -16,
        GWLP_USERDATA = -21,
        GWLP_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWL_ID = -12,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
        GWL_HWNDPARENT = -8,
    }

    internal readonly partial struct HWND : IEquatable<HWND>
    {
        internal readonly IntPtr Value;

        internal HWND(IntPtr value) => this.Value = value;

        internal static HWND Null => default;

        internal bool IsNull => Value == default;

        public static implicit operator IntPtr(HWND value) => value.Value;

        public static explicit operator HWND(IntPtr value) => new HWND(value);

        public static bool operator ==(HWND left, HWND right) => left.Value == right.Value;

        public static bool operator !=(HWND left, HWND right) => !(left == right);

        public bool Equals(HWND other) => this.Value == other.Value;

        public override bool Equals(object obj) => obj is HWND other && this.Equals(other);

        public override int GetHashCode() => this.Value.GetHashCode();
    }
}