using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Toolbox.Core
{
    public static class Runtime
    {
        /// <summary>
        /// Enable or disable vsync used in 3D and 2D editors.
        /// </summary>
        public static bool EnableVSync = true;

        /// <summary>
        /// Toggles usage of the OpenGL api used in 3D and 2D editors.
        /// </summary>
        public static bool UseOpenGL = true;

        /// <summary>
        /// Determines the state of OpenGL being loaded or not.
        /// </summary>
        public static bool OpenTKInitialized = false;

        /// <summary>
        /// Determines the type of renderer to use for OpenGL.
        /// </summary>
        public static bool UseLegacyGL = false;

        /// <summary>
        /// Toggles model rendering in 3D view.
        /// </summary>
        public static bool RenderModels = true;

        private static string executableDir;

        /// <summary>
        /// The directory the program is located in.
        /// </summary>
        public static string ExecutableDir
        {
            get {
                if (executableDir == null)
                    executableDir = FindExecutableDir();

                return executableDir; }
            set
            {
                executableDir = value;
            }
        }

        private static string FindExecutableDir()
        {
           return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// The level of compression used for YAZ0 from 1 - 9.
        /// </summary>
        public static int Yaz0CompressionLevel;

        /// <summary>
        /// Toggles drag and drop for the main window form.
        /// </summary>
        public static bool EnableDragDrop = true;

        public static float BonePointSize { get; set; } = 0.1f;

        public static int SelectedBoneIndex { get; set; } = -1;

        public static float Preview3DScale { get; set; } = 1.0f;

        public static bool DisplayBones { get; set; } = true;

        public static List<ModelContainer> ModelContainers = new List<ModelContainer>();

        //GUI based editors
        //While multiple GUI frameworks can be used, they should still share the same editors

        public class GUI
        {
            /// <summary>
            /// Determines to always maximize mdi windows when loaded.
            /// </summary>
            public static bool MaximizeMdiWindow = true;

            /// <summary>
            /// Determines the state the window is currently in and to load as on boot.
            /// </summary>
            public static bool MaximizeWindow = false;

            /// <summary>
            /// The default extension used for the image when exporting.
            /// </summary>
            public static string DefaultImageExt = ".png";
        }

        public class BackgroundSettings
        {
            public static Color backgroundGradientTop = Color.FromArgb(255, 26, 26, 26);
            public static Color backgroundGradientBottom = Color.FromArgb(255, 77, 77, 77);
        }

        public class GridSettings
        {
            public static float CellSize = 1.0f;
            public static uint CellAmount = 10;
            public static Color color = Color.FromArgb(90, 90, 90);
        }

        public class ObjectEditor
        {
            public static int EditorSelectedIndex;

            public static bool OpenInActiveEditor = true;
        }

        public class ImageEditor
        {
            public static Color CustomPicureBoxBGColor = Color.DarkCyan;

            public static PictureBoxBG pictureBoxStyle = PictureBoxBG.Checkerboard;

            public static bool PreviewGammaFix = false;

            public static bool ShowPropertiesPanel = true;
            public static bool DisplayVertical = false;


            public static bool DisplayAlpha = true;
            public static bool UseComponetSelector = true;

            public static bool EnableImageZoom = true;
            public static bool EnablePixelGrid = false;

            public enum PictureBoxBG
            {
                Checkerboard,
                Black,
                White,
                Custom,
            }
        }
    }
}
