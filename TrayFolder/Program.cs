using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TrayFolder
{
    internal class Program
    {
        private const int ERROR_SUCCESS = 0;
        private const int ERROR_INVALILD_COMMAND_LINE = 1;
        private const int ERROR_BAD_ARGUMENTS = 2;
        private const int ERROR_UNKNOWN_ERROR = 3;

        static NotifyIcon notifyIcon = new NotifyIcon();

        /// <summary>
        /// Tray Folder
        /// </summary>
        /// <param name="args">폴더 경로</param>
        /// <see cref="https://stackoverflow.com/questions/751944/net-console-application-in-system-tray"/>
        /// <returns></returns>
        static int Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 제목에 버전을 표시한다.
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(String.Format("Tray Folder (v{0}.{1}.{2:#0}.{3:#0})\n", version.Major, version.Minor, version.Build, version.Revision));

            int len = args.Length;
            if (len == 0)
            {
                Console.WriteLine("폴더 경로를 입력해주세요.\n사용법: TrayFolder.exe [폴더 경로]");
                MessageBox.Show("폴더 경로를 입력해주세요.\n사용법: TrayFolder.exe [폴더 경로]", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERROR_INVALILD_COMMAND_LINE;
            }

            try
            {
                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine(String.Format("\"{0}\" 폴더가 존재하지 않습니다.", args[0]));
                    MessageBox.Show(String.Format("\"{0}\" 폴더가 존재하지 않습니다.", args[0]), "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ERROR_BAD_ARGUMENTS;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("\"{0}\"은 올바른 폴더 경로가 아닙니다.\n{1}", args[0], ex.Message));
                MessageBox.Show(String.Format("\"{0}\" 폴더가 존재하지 않습니다.\n{1}", args[0], ex.Message), "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ERROR_BAD_ARGUMENTS;
            }

            notifyIcon.DoubleClick += (s, e) =>
            {
            };
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            var contextMenu = new ContextMenuStrip();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(args[0]);
            foreach (System.IO.FileInfo fileInfo in di.GetFiles())
            {
                Icon? icon = Icon.ExtractAssociatedIcon(fileInfo.FullName);
                contextMenu.Items.Add(fileInfo.Name, (icon != null) ? icon.ToBitmap() : null, (s, e) => {
                    ProcessStartInfo psi = new ProcessStartInfo(fileInfo.FullName);
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                });
            }
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });
            notifyIcon.ContextMenuStrip = contextMenu;

            Console.WriteLine("Running!");

            // Standard message loop to catch click-events on notify icon
            // Code after this method will be running only after Application.Exit()
            Application.Run();

            notifyIcon.Visible = false;

            return ERROR_SUCCESS;
        }
    }
}