using System;
using System.Diagnostics;
using System.Windows.Forms;
using TentacleSlicers.windows;
using System.Runtime.InteropServices;

namespace TentacleSlicers
{
    /// <summary>
    /// Point de départ de l'application.
    /// Des fonctions externes sont récupérées pour masquer la console.
    /// </summary>
    internal class Program
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SwHide = 0;

        /// <summary>
        /// Masque la console puis lance l'application.
        /// </summary>
        /// <param name="args"> Arguments non utilisés </param>
        [STAThread]
        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SwHide);

            Application.Run(new MainForm());
        }
    }
}
