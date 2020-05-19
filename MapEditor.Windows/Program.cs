using System;

namespace MapEditor.Windows
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Environment.MapReader = new Mir.Ethernity.MapLibrary.Wemade.WemadeMapReader();
            Environment.CreateImageLibrary = (fileName) => new Mir.Ethernity.ImageLibrary.Zircon.ZirconImageLibrary(fileName);

            using (var game = new MapEditorGame())
                game.Run();
        }
    }
#endif
}
