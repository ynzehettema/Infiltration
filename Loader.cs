using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MetalLib;
using MetalLib.Pencil.Gaming;
using MetalLib.GameWorld;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace cyberpunkgamejamgame
{
    class Loader
    {
        public static World LoadMap(string file, Window2D window)
        {
            World w = new World();

            List<Light> lightBuffer = new List<Light>();

            StreamReader sr = new StreamReader(GameUtils.GetAppPath() + file);

            w.Construct(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()));
            w.levelCaptionTop = sr.ReadLine();
            w.levelCaptionBottom = sr.ReadLine();
            string line = string.Empty;
            int lightcount = 0;
            while ((line = sr.ReadLine()) != "END")
            {
                if (line.StartsWith("light"))
                {
                    lightcount++;
                    string[] split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    lightBuffer.Add(new Light(new Vector2(0, 0), new Color4(byte.Parse(split[1]), byte.Parse(split[2]), byte.Parse(split[3]), 255), int.Parse(split[4]), split[5] == "true"));
                    lightBuffer[lightBuffer.Count - 1].isActivated = split[6] == "true";
                    if (split.Length > 7)
                    {
                        lightBuffer[lightBuffer.Count - 1].width = int.Parse(split[7]);
                        lightBuffer[lightBuffer.Count - 1].direction = int.Parse(split[8]);
                        lightBuffer[lightBuffer.Count - 1].rotationSpeed = int.Parse(split[9]);
                    }
                    FontHandler.AddText(new Text("LOADING NEXT LEVEL...", "loading", new Vector2(400, 300), 1f));
                    string dots = string.Empty;
                    for (int i = 0; i < lightcount; i++)
                        dots += ".";

                    FontHandler.AddText(new Text("PLUGGING IN THE LIGHTS", "loading2", new Vector2(400, 350), 0.8f));

                    FontHandler.AddText(new Text(dots, "loading3", new Vector2(400, 400), 0.8f));
                    window.PrepDraw();
                    FontHandler.Draw();
                    window.EndDraw();

                    FontHandler.TextList = new List<Text>();
                }
            }

            int x = 0, y = 0;
            while ((line = sr.ReadLine()) != "END" && line != null)
            {
                for (x = 0; x < line.Length; x++)
                {
                    if (char.IsNumber(line[x]))
                    {
                        byte nr = byte.Parse(line[x].ToString());
                        Light l = lightBuffer[nr];
                        l.Position = new Vector2(x * 32, y * 32);
                        w.lightList.Add(l);
                        w.gameGrid.AddTile("floor", x, y);
                        continue;
                    }
                    switch (line[x])
                    {
                        case '*':
                            w.collisionGameGrid.AddTile("wall", x, y);
                            break;
                        case 'V':
                            w.gameGrid.AddTile("floorv", x, y);
                            w.collisionGameGrid.AddTile("door1", x, y);
                            break;
                        case 'H':
                            w.gameGrid.AddTile("floorh", x, y);
                            w.collisionGameGrid.AddTile("door2", x, y);
                            break;
                        case 'P':
                            w.gameGrid.AddTile("floor", x, y);
                            w.player = new Player("player", new Vector2(x * 32, y * 32));
                            break;
                        case 'B':
                            w.gameGrid.AddTile("rail", x, y);
                            Light yellow2 = new Light(new Vector2(x * 32, y * 32), Color4.Yellow, 16f, false);
                            yellow2.anglestep = 36f;
                            w.lightList.Add(yellow2);
                            w.EnemyList.Add(new Enemy(new Vector2(x * 32, y * 32), w, true));
                            w.EnemyList[w.EnemyList.Count - 1].Rotation = 0f;
                            break;
                        case 'W':
                            w.gameGrid.AddTile("floor", x, y);
                            w.EnemyList.Add(new Enemy(new Vector2(x * 32, y * 32), w));
                            w.EnemyList[w.EnemyList.Count - 1].Rotation = 0f;
                            break;
                        case 'A':
                            w.gameGrid.AddTile("floor", x, y);
                            w.EnemyList.Add(new Enemy(new Vector2(x * 32, y * 32), w));
                            w.EnemyList[w.EnemyList.Count - 1].Rotation = -90f;
                            break;
                        case 'S':
                            w.gameGrid.AddTile("floor", x, y);
                            w.EnemyList.Add(new Enemy(new Vector2(x * 32, y * 32), w));
                            w.EnemyList[w.EnemyList.Count - 1].Rotation = 180f;
                            break;
                        case 'D':
                            w.gameGrid.AddTile("floor", x, y);
                            w.EnemyList.Add(new Enemy(new Vector2(x * 32, y * 32), w));
                            w.EnemyList[w.EnemyList.Count - 1].Rotation = 90f;
                            break;
                        case 'T':
                            w.collisionGameGrid.AddTile("terminal", x, y);

                            w.lightList.Add(new Light(new Vector2((x - 1) * 32, y * 32), Color4.Green, 25f, false));

                            w.lightList[w.lightList.Count - 1].anglestep = 36f;
                            w.lightList.Add(new Light(new Vector2((x + 1) * 32, y * 32), Color4.Green, 25f, false));
                            w.lightList[w.lightList.Count - 1].anglestep = 36f;
                            w.lightList.Add(new Light(new Vector2(x * 32, (y - 1) * 32), Color4.Green, 25f, false));
                            w.lightList[w.lightList.Count - 1].anglestep = 36f;
                            w.lightList.Add(new Light(new Vector2(x * 32, (y + 1) * 32), Color4.Green, 25f, false));
                            w.lightList[w.lightList.Count - 1].anglestep = 36f;
                            break;
                        case 'O':
                            w.gameGrid.AddTile("off", x, y);
                            Light red = new Light(new Vector2(x * 32, y * 32), Color4.Red, 25f, false);
                            red.anglestep = 36f;
                            w.lightList.Add(red);
                            break;
                        case 'R':
                            w.gameGrid.AddTile("rail", x, y);
                            Light yellow = new Light(new Vector2(x * 32, y * 32), Color4.Yellow, 16f, false);
                            yellow.anglestep = 36f;
                            w.lightList.Add(yellow);
                            break;
                        case 'M':
                            w.collisionGameGrid.AddTile("mask", x, y);
                            break;

                        case 'E':
                            w.gameGrid.AddTile("floor", x, y);
                            Light red2 = new Light(new Vector2(x * 32, y * 32), Color4.Red, 50f, false);
                            red2.anglestep = 36f;
                            red2.isActivated = true;
                            Light green = new Light(new Vector2(x * 32, y * 32), Color4.Green, 50f, false);
                            green.anglestep = 36f;
                            green.isActivated = false;
                            w.lightList.Add(green);
                            w.lightList.Add(red2);
                            break;
                        case ' ':
                            w.gameGrid.AddTile("floor", x, y);
                            break;
                        default:
                            Console.WriteLine("Unrecognized block in map. wtf?");
                            break;
                    }
                }
                y++;
            }

            

            w.Build(window);
            return w;

        }
    }
}
