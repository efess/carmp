using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace CarMP.Graphics.Implementation.OpenTk
{
    public class Vbo
    {
        private Vertex[] _vertexArray;
        public Vertex[] VertexArray
        {
            get { return _vertexArray;}
            set { _vertexArray = value;}
        }

        private TexCoord[] _texCoordArray;
        public TexCoord[] TexCoordArray 
        { 
            get{return _texCoordArray;  }
            set { _texCoordArray = value; }
        }

        private uint[] vboIds = new uint[ 2 ];

        public void CreateVertexVbo()
        {
            // Delete old VBO
            if (vboIds[0] != 0) GL.DeleteBuffers(1, ref vboIds[0]);

            // VBO for vertices
            GL.GenBuffers(1, out vboIds[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboIds[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_vertexArray.Length * 2 * sizeof(float)), _vertexArray, BufferUsageHint.StaticDraw);
        
        }

        public void CreateTexCoordVbo()
        {
            // Delete old VBO
            if (vboIds[1] != 0) GL.DeleteBuffers(1, ref vboIds[1]);

            // VBO for texcoords
            GL.GenBuffers(1, out vboIds[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboIds[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_texCoordArray.Length * 2 * sizeof(float)), _texCoordArray, BufferUsageHint.StaticDraw);
        }

        public void Draw(int lenght, BeginMode mode)
        {
            if (true)
            {
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.TextureCoordArray);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vboIds[0]);
                GL.VertexPointer(2, VertexPointerType.Float, 0, IntPtr.Zero);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vboIds[1]);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);

                GL.DrawArrays(mode, 0, lenght);

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.TextureCoordArray);
            }
            // Use immediate mode (Check for vbo support ...)
            else
            {
                GL.Begin(mode);

                for (int i = 0; i < lenght; i++)
                {
                    GL.TexCoord2(_texCoordArray[i].u, _texCoordArray[i].v);
                    GL.Vertex2(_vertexArray[i].x, _vertexArray[i].y);
                }

                GL.End();
            }
        }
    }
}
