using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.Audio;

using MetalLib;
using MetalLib.Pencil.Gaming;
using MetalLib.CDS;
using Pencil.Gaming.MathUtils;

namespace cyberpunkgamejamgame
{
    public class Text
    {
        public string _Text, Name;
        public Vector2 Position;
        public float Size;
        public bool OriginLeft;

        public Text(string text, string name, Vector2 position, float size, bool originLeft = false)
        {
            Name = name;
            _Text = text;
            Position = position;
            Size = size;
            OriginLeft = originLeft;
        }
    }

    class FontHandler
    {
        public static List<Text> TextList = new List<Text>();
        public static void Draw()
        {
            int textureWidth = ContentManager.GetTexture("font").Width,
                textureHeight = ContentManager.GetTexture("font").Height;
            float charWidth = textureWidth / 26f,
                charHeight = textureHeight / 5f;
            float texCoordWidth = charWidth / textureWidth,
                texCoordHeight = charHeight / textureHeight;

            foreach (Text t in TextList)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                ContentManager.GetTexture("font").Bind();
                GL.Color4(Color4.White);

                GL.PushMatrix();
                if (t.OriginLeft)
                {
                    GL.Translate(t.Position.X - (GetStringLength(t._Text) * t.Size), t.Position.Y, 0);
                }
                else
                {
                    GL.Translate(t.Position.X, t.Position.Y, 0);
                }
                GL.Begin(BeginMode.Quads);
                {
                    float bufferCharPositionX = 0f;
                    for (int a = 0; a < t._Text.Length; a++)
                    {
                        char ch = t._Text[a];
                        int Xindex = 0;
                        int Yindex = 0;
                        if (char.IsLetter(ch) && char.IsUpper(ch))
                        {
                            Xindex = (int)ch - 65;
                            Yindex = 0;
                        }
                        else
                            if (char.IsLetter(ch) && char.IsLower(ch))
                            {
                                Xindex = (int)ch - 97;
                                Yindex = 1;
                            }
                            else
                                if (char.IsNumber(ch))
                                {
                                    Xindex = 10 + ((int)ch - 48);
                                    Yindex = 3;
                                }
                                else
                                {
                                    switch (ch)
                                    {
                                        case '!':
                                            Xindex = 0;
                                            Yindex = 3;
                                            break;
                                        case '"':
                                            Xindex = 1;
                                            Yindex = 3;
                                            break;
                                        case '%':
                                            Xindex = 2;
                                            Yindex = 3;
                                            break;
                                        case '&':
                                            Xindex = 3;
                                            Yindex = 3;
                                            break;
                                        case '`':
                                            Xindex = 4;
                                            Yindex = 3;
                                            break;
                                        case '[':
                                            Xindex = 5;
                                            Yindex = 3;
                                            break;
                                        case ']':
                                            Xindex = 6;
                                            Yindex = 3;
                                            break;
                                        case '+':
                                            Xindex = 7;
                                            Yindex = 3;
                                            break;
                                        case '-':
                                            Xindex = 8;
                                            Yindex = 3;
                                            break;
                                        case '/':
                                            Xindex = 9;
                                            Yindex = 3;
                                            break;
                                        case ':':
                                            Xindex = 20;
                                            Yindex = 3;
                                            break;
                                        case '=':
                                            Xindex = 21;
                                            Yindex = 3;
                                            break;
                                        case '<':
                                            Xindex = 22;
                                            Yindex = 3;
                                            break;
                                        case '>':
                                            Xindex = 23;
                                            Yindex = 3;
                                            break;
                                        case '?':
                                            Xindex = 25;
                                            Yindex = 3;
                                            break;

                                        case '@':
                                            Xindex = 0;
                                            Yindex = 4;
                                            break;
                                        case '\\':
                                            Xindex = 1;
                                            Yindex = 4;
                                            break;
                                        case '\'':
                                            Xindex = 2;
                                            Yindex = 4;
                                            break;
                                        case '~':
                                            Xindex = 3;
                                            Yindex = 4;
                                            break;
                                        case '^':
                                            Xindex = 4;
                                            Yindex = 4;
                                            break;
                                        case ',':
                                            Xindex = 5;
                                            Yindex = 4;
                                            break;
                                        case '.':
                                            Xindex = 6;
                                            Yindex = 4;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                        if (ch != ' ')
                        {
                            if (t.OriginLeft)
                            {
                                GL.TexCoord2(texCoordWidth * Xindex, texCoordHeight * Yindex);
                                GL.Vertex2((bufferCharPositionX) * t.Size, -charHeight / 2);

                                GL.TexCoord2(texCoordWidth * (Xindex + 1), texCoordHeight * Yindex);
                                GL.Vertex2((bufferCharPositionX + charWidth) * t.Size, -charHeight / 2);

                                GL.TexCoord2(texCoordWidth * (Xindex + 1), texCoordHeight * (Yindex + 1));
                                GL.Vertex2((bufferCharPositionX + charWidth) * t.Size, (-charHeight / 2) + charHeight * t.Size);

                                GL.TexCoord2(texCoordWidth * Xindex, texCoordHeight * (Yindex + 1));
                                GL.Vertex2((bufferCharPositionX) * t.Size, (-charHeight / 2) + charHeight * t.Size);
                            }
                            else
                            {
                                GL.TexCoord2(texCoordWidth * Xindex, texCoordHeight * Yindex);
                                GL.Vertex2((bufferCharPositionX - ((GetStringLength(t._Text) * charWidth) / 2)) * t.Size, -charHeight / 2);

                                GL.TexCoord2(texCoordWidth * (Xindex + 1), texCoordHeight * Yindex);
                                GL.Vertex2((bufferCharPositionX + charWidth - ((GetStringLength(t._Text) * charWidth) / 2)) * t.Size, -charHeight / 2);

                                GL.TexCoord2(texCoordWidth * (Xindex + 1), texCoordHeight * (Yindex + 1));
                                GL.Vertex2((bufferCharPositionX + charWidth - ((GetStringLength(t._Text) * charWidth) / 2)) * t.Size, (-charHeight / 2) + charHeight * t.Size);

                                GL.TexCoord2(texCoordWidth * Xindex, texCoordHeight * (Yindex + 1));
                                GL.Vertex2((bufferCharPositionX - ((GetStringLength(t._Text) * charWidth) / 2)) * t.Size, (-charHeight / 2) + charHeight * t.Size);
                            }
                            bufferCharPositionX += charWidth;
                        }
                        else
                        {
                            bufferCharPositionX += charWidth / 2;
                        }
                    }
                }
                GL.End();

                GL.PopMatrix();

                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.Blend);
            }


        }

        public static float GetStringLength(string s)
        {
            float length = 0f;
            for (int a = 0; a < s.Length; a++)
            {
                if (s[a] == ' ')
                {
                    length += 0.5f;
                }
                else
                {
                    length++;
                }
            }
            return length;
        }

        public static void AddText(Text text)
        {
            TextList = TextList.Where(x => x.Name != text.Name).ToList();
            TextList.Add(text);
        }

        public static void RemoveText(string name)
        {
            TextList = TextList.Where(x => x.Name != name).ToList();
        }

        public static Text GetText(string name)
        {
            return TextList.First(x => x.Name == name);
        }


    }
}
