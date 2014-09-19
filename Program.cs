using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MetalLib;
using MetalLib.Pencil.Gaming;
using MetalLib.GameWorld;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Audio;
using System.Threading;


namespace cyberpunkgamejamgame
{
    class Program
    {
        public static Vector2 cameraPosition = new Vector2(80, 80);
        public static Sound fire, ouch, boom, hacking, complete, hit;
        public static Sound music;

        public static string GetAppPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        static void _LoadAudio()
        {
            music = new Sound(GameUtils.GetAppPath() + "\\content\\audio\\music.ogg");
            music.Gain = 0.0f;
            music.Play();
            music.Looping = true;
        }


        static void Main(string[] args)
        {
            Window2D window = new Window2D(800, 600, true, "Infiltration - Cyberpunk GameJam - By Metaldemon", 800, 600);

            ContentManager.LoadTexture(GetAppPath() + "\\content\\Floor.png", "floor");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Wall.png", "wall");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Guy.png", "player");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Turret.png", "turret");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Particle.png", "particle");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Terminal.png", "terminal");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\FloorOff.png", "off");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\FloorOn.png", "on");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Door1.png", "door1");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Door2.png", "door2");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\font.png", "font");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\terminalscreen.png", "terminalscreen");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\FloorV.png", "floorv");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\FloorH.png", "floorh");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Maskwall.png", "mask");
            ContentManager.LoadTexture(GetAppPath() + "\\content\\Rail.png", "rail");

            fire = new Sound(GetAppPath() + "\\content\\audio\\fire.wav");
            ouch = new Sound(GetAppPath() + "\\content\\audio\\ouch.wav");
            boom = new Sound(GetAppPath() + "\\content\\audio\\boom.wav");
            hacking = new Sound(GetAppPath() + "\\content\\audio\\hacking.wav");
            complete = new Sound(GetAppPath() + "\\content\\audio\\complete.wav");
            hit = new Sound(GetAppPath() + "\\content\\audio\\hit.wav");
            fire.Gain = ouch.Gain = boom.Gain = hacking.Gain = complete.Gain = hit.Gain = 0.5f;

            if (music == null)
            {
                FontHandler.AddText(new Text("Loading game...", "loading", new Vector2(400, 300), 1f));
                window.PrepDraw();
                FontHandler.Draw();
                window.EndDraw();
                FontHandler.TextList = new List<Text>();

                new Thread(_LoadAudio).Start();
            }
        restart:
            World world = Loader.LoadMap("\\content\\maps\\map00.txt", window);
            world.map = 00;
            double totalTime = 0.0;
            double delta = 0.0;
            while (window.IsOpen)
            {
                Glfw.SwapInterval(false);
                totalTime += Glfw.GetTime();

                delta = (float)Glfw.GetTime();

                Glfw.SetTime(0.0);

                world.Update((float)delta / 3);

                if (music != null && music.Gain < 0.6f)
                {
                    music.Gain += 0.0001f;
                }
                window.PrepDraw();

                if (world.incrementMap || world.restartMap)
                {
                    int newmap = world.map;
                    if (world.incrementMap) newmap++;
                    world.map = newmap;
                    world.incrementMap = false;

                    if (newmap == 15)
                    {
                        while (window.IsOpen)
                        {
                            int ypos = 100;
                            FontHandler.AddText(new Text("THE WINNER IS YOU", "yay1", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text("YOU COMPLETED THE GAME IN:", "yay2", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text(Math.Round(totalTime, 2) + " SECONDS", "yay3", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text("THANK YOU FOR PLAYING THIS GAME", "yay4", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text("NOW PLAY OTHER CYBERPUNK GAMEJAM GAMES", "yay5", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text("-METALDEMON", "yay6", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            FontHandler.AddText(new Text("PS: PRESS SPACE TO PLAY AGAIN", "yay7", new Vector2(400, ypos), 0.8f)); ypos += 75;
                            window.PrepDraw();
                            FontHandler.Draw();
                            window.EndDraw();
                            FontHandler.TextList = new List<Text>();
                            if (Input.GetState(0).Keyboard[Key.Space])
                            {
                                goto restart;
                            }
                        }
                        Environment.Exit(0);
                    }

                    if (newmap < 10)
                        world = Loader.LoadMap("\\content\\maps\\map0" + newmap + ".txt", window);
                    else
                        world = Loader.LoadMap("\\content\\maps\\map" + newmap + ".txt", window);
                    world.map = newmap;
                    world.incrementMap = false;

                    continue;
                }

                GL.PushMatrix();
                {
                    GL.Translate(cameraPosition.X, cameraPosition.Y, 0);

                    world.Draw();
                }
                GL.PopMatrix();


                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.OneMinusDstColor, BlendingFactorDest.One);
                GL.Color4(0, 0, 0, 0.0f);
                GL.Begin(BeginMode.TriangleFan);
                GL.Vertex2(400, 300);
                GL.Color4(1f - (world.player.health / 255f), 0, 0, 0.2f);
                GL.Vertex2(0, 0);
                GL.Vertex2(800, 0);
                GL.Vertex2(800, 600);
                GL.Vertex2(0, 600);
                GL.Vertex2(0, 0);
                GL.End();

                GL.Disable(EnableCap.Blend);

                FontHandler.Draw();
                window.EndDraw();
            }
        }
    }
}
