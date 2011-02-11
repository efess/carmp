using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Geometry;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace CarMP.Graphics.Implementation.OpenTk
{
    public class OTKImage : OTKDrawableObjectBase, IImage, IDisposable
    {
        private string _path;
        private Rectangle _lastDrawDimension;
        private int _textureId;
        private Vbo _vbo;

        public Size Size { get; set; }

        internal int TextureId { get { return _textureId; } }

        internal OTKImage(string pImagePath)
        {
            if (string.IsNullOrEmpty(pImagePath))
                throw new ArgumentNullException("pImagePath");
            
            _path = pImagePath;
            _vbo = new Vbo();
            _vbo.VertexArray = new Vertex[4];
            _vbo.TexCoordArray = new TexCoord[4];
            Load();
        }

        internal void SetDimensions(Rectangle pImagePosition)
        {
            /// <summary>
            /// (FROM ENGINE) Draws a part of image with specified size.
            /// </summary>
            /// <param name="x">X position of left-upper corner.</param>
            /// <param name="y">Y position of left-upper corner.</param>
            /// <param name="w">Width of image.</param>
            /// <param name="h">Height of image.</param>
            /// <param name="imgX">X positon on image.</param>
            /// <param name="imgY">Y positon on image.</param>
            /// <param name="imgW">Width of image part to be drawn.</param>
            /// <param name="imgH">Height of image part to be drawn.</param>
            // Texture coordinates
            float u1 = 0.0f, u2 = 1.0f, v1 = 0.0f, v2 = 1.0f;

            if (!_lastDrawDimension.Equals(pImagePosition))
            {
                // Check if texture coordinates have changed
                if (_vbo.TexCoordArray[0].u != u1 || _vbo.TexCoordArray[1].u != u2 || _vbo.TexCoordArray[2].v != v1 || _vbo.TexCoordArray[0].v != v2)
                {
                    // Update TexCoordArray for all VertexArray
                    BuildTexCoords(u1, u2, v1, v2);
                }

                // Check if position coordinates have changed
                if (_vbo.VertexArray[0].x != pImagePosition.Left
                    || _vbo.VertexArray[2].y != pImagePosition.Top
                    || _vbo.VertexArray[0].y != pImagePosition.Bottom 
                    || _vbo.VertexArray[1].x != pImagePosition.Right)
                {
                    BuildVertices(pImagePosition.X, pImagePosition.Y, pImagePosition.Width, pImagePosition.Height);
                }

                _lastDrawDimension = pImagePosition;
            }
        }   


        protected override void DrawObject()
        {
            // Bind texture
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            // Draw VBO
            _vbo.Draw(_vbo.VertexArray.Length, BeginMode.Quads);

        }

        /// <summary>
        /// Builds texcoords for quad.
        /// </summary>
        /// <param name="u1">U1.</param>
        /// <param name="u2">U2.</param>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public void BuildTexCoords(float u1, float u2, float v1, float v2)
        {
            _vbo.TexCoordArray[0].u = u1;
            _vbo.TexCoordArray[0].v = v2;
            _vbo.TexCoordArray[1].u = u2;
            _vbo.TexCoordArray[1].v = v2;
            _vbo.TexCoordArray[2].u = u2;
            _vbo.TexCoordArray[2].v = v1;
            _vbo.TexCoordArray[3].u = u1;
            _vbo.TexCoordArray[3].v = v1;

            _vbo.CreateTexCoordVbo();
        }


        /// <summary>
        /// Builds vertices for quad.
        /// </summary>
        /// <param name="x">X pos.</param>
        /// <param name="y">Y pos.</param>
        /// <param name="w">Width.</param>
        /// <param name="h">Height.</param>
        private void BuildVertices(float x, float y, float w, float h)
        {
            _vbo.VertexArray[0].x = x;
            _vbo.VertexArray[0].y = y + h;
            _vbo.VertexArray[1].x = x + w;
            _vbo.VertexArray[1].y = y + h;
            _vbo.VertexArray[2].x = x + w;
            _vbo.VertexArray[2].y = y;
            _vbo.VertexArray[3].x = x;
            _vbo.VertexArray[3].y = y;

            _vbo.CreateVertexVbo();
        }

        private void Load()
        {
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(_path))
            {
                System.Drawing.Imaging.BitmapData bmpData =
                    bitmap.LockBits(
                        new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        bitmap.PixelFormat
                );

                Size = new Size(bitmap.Width, bitmap.Height);

                //// Declare an array to hold the bytes of the bitmap.
                //IntPtr ptr = bmpData.Scan0;

                //byte[] bytes = new byte[bmpData.Stride * bitmap.Height];

                // Generate texture
                GL.GenTextures(1, out _textureId);
                
                GL.BindTexture(TextureTarget.Texture2D, TextureId);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bitmap.UnlockBits(bmpData);

                // Setup filtering
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            }
        }

        public void Dispose()
        {
            GL.DeleteTextures(1, ref _textureId);
        }
    }
}
