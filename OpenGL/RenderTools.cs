using System;
using System.Collections.Generic;
using System.Drawing;
using Toolbox.Core.Imaging;

namespace Toolbox.Core
{
    public class RenderTools
    {
        public static STGenericTexture defaulttex;
        public static STGenericTexture defaultTex
        {
            get
            {
                if (defaulttex == null)
                {
                    defaulttex = new GenericBitmapTexture(Properties.Resources.DefaultTexture);
                    defaulttex.LoadOpenGLTexture();
                    defaulttex.RenderableTex.Bind();
                }
                return defaulttex;
            }
            set
            {
                defaulttex = value;
            }
        }

        public static void DisposeTextures()
        {
            defaultTex = null;
           // uvTestPattern = null;
          //  brdfPbr = null;
          //  diffusePbr = null;
           // specularPbr = null;
        }
    }
}
