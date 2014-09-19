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

namespace cyberpunkgamejamgame
{
    public class World
    {
        public float ambient = 1f;
        public GameGrid gameGrid, collisionGameGrid;
        public List<Light> lightList = new List<Light>();
        public List<Enemy> EnemyList = new List<Enemy>();
        public List<Projectile> projectileList = new List<Projectile>();
        public List<Particle> particleList = new List<Particle>();
        public List<TileFog> tileFogList = new List<TileFog>();
        public Player player;
        public TerminalGame terminalGame;
        public bool inTerminalGame = false;

        public string levelCaptionTop = "I like kittens!";
        public string levelCaptionBottom = "I like bacon!";
        public float screenShake = 0f;

        public int map = 0;
        public bool incrementMap = false;
        public bool restartMap = false;
        public bool levelComplete = false;

        public void Construct(int w, int h)
        {
            gameGrid = new GameGrid(w, h);
            collisionGameGrid = new GameGrid(w, h,"temp", true, 32, 32 );
            Program.cameraPosition = new Vector2(-800, 0);
        }

        public void Build(Window window)
        {
            Light.CollisionGrids = new List<GameGrid>();
            Light.CollisionGrids.Add(collisionGameGrid);

            foreach (GameObject g in gameGrid.Grid)
            {
                if (g != null)
                    tileFogList.Add(new TileFog(g.Position, g));
            }
            foreach (GameObject g in collisionGameGrid.Grid)
            {
                if (g != null)
                    tileFogList.Add(new TileFog(g.Position, g));
            }
            int j = 0;
            lightList.ForEach(x =>
            {
                j++;
                x.Restart();
                string dots = string.Empty;
                for (int i = 0; i < j; i++)
                    dots += ".";

                window.PrepDraw();
                FontHandler.AddText(new Text("LOADING NEXT LEVEL...", "loading", new Vector2(400, 300), 1f));
                FontHandler.AddText(new Text("MAKING CYBERPUNK STUFF", "loading2", new Vector2(400, 350), 0.8f));
                FontHandler.AddText(new Text(dots, "loading3", new Vector2(400, 400), 0.8f));
                FontHandler.Draw();
                window.EndDraw();

                FontHandler.TextList = new List<Text>();
            });
            window.PrepDraw();
            FontHandler.AddText(new Text("LOADING NEXT LEVEL...", "loading", new Vector2(400, 300), 1f));
            FontHandler.AddText(new Text("ADDING CYBERNESS...", "loading2", new Vector2(400, 350), 0.8f));
            FontHandler.Draw();
            window.EndDraw();
            FontHandler.TextList = new List<Text>();
            Program.cameraPosition = new Vector2(-800, 0);
            player.checkMapDelay = 2.5f;

            
        }

        public void Update(float delta)
        {
            if (player.checkMapDelay > 0 && !levelComplete)
            {
                if ((float)GameUtils.GetDistance(Program.cameraPosition, new Vector2(80, 80)) > 2)
                    Program.cameraPosition = GameUtils.MoveTowards(Program.cameraPosition, new Vector2(80, 80), (float)GameUtils.GetDistance(Program.cameraPosition, new Vector2(80, 80)) / 25f);
                else
                    Program.cameraPosition = new Vector2(80, 80);
                
            }
            else
            {
                if (GameUtils.GetDistance(Program.cameraPosition, new Vector2(80, 80)) > 32)
                {
                    player.checkMapDelay = 2.5f;
                }
            }
            float viewDist = 255f * 1.5f;
            foreach (TileFog t in tileFogList)
            {
                if (GameUtils.GetDistance(player.Position, t.position) < viewDist)
                {
                    float newalpha = (viewDist - (float)GameUtils.GetDistance(player.Position, t.position)) / viewDist;
                    if (newalpha > t.alpha)
                    {
                        if (GameUtils.GetDistance(player.Position, t.position) < 32 || Ray.FoundTile(player.Position, t.parent, this, 32f, viewDist))
                        {
                            t.alpha = newalpha;
                        }
                    }
                }
            }
            if (inTerminalGame && terminalGame.active)
            {
                terminalGame.Update(this, delta);
            }
            else
            {
                if (screenShake > 0 && player.checkMapDelay <= 0)
                {
                    Random rand = new Random();
                    Program.cameraPosition.X += -5f + (float)rand.NextDouble() * 10f;
                    Program.cameraPosition.Y += -5f + (float)rand.NextDouble() * 10f;
                    screenShake -= 0.1f;
                    if (screenShake <= 0)
                    {
                        Program.cameraPosition = new Vector2(80, 80);
                    }
                }
                projectileList = projectileList.Where(x => !x.Remove).ToList();
                player.Update(delta, this);
                if (!player.dead)
                {
                    particleList = particleList.Where(x => !x.remove).ToList();
                    particleList.ForEach(x => x.Update(delta));

                    EnemyList = EnemyList.Where(x => !x.remove).ToList();
                    projectileList.ForEach(x => x.Update(this, delta));

                    EnemyList.ForEach(x => { x.Update(delta, player, this); });
                    lightList.ForEach(x =>
                    {
                        x.direction += x.rotationSpeed * delta;
                    });
                }
            }
        }

        public void Draw()
        {
            if (player.checkMapDelay > 0 || levelComplete)
            {
                FontHandler.AddText(new Text(levelCaptionTop, "levelcaptiontop", Program.cameraPosition - new Vector2(80, 80) + new Vector2(400, 25), 0.8f));
                FontHandler.AddText(new Text(levelCaptionBottom, "levelcaptionbottom", Program.cameraPosition - new Vector2(80, 80) + new Vector2(400, 575), 0.8f));
            }
            else
            {
                if (!player.dead)
                {
                    FontHandler.AddText(new Text(levelCaptionTop, "levelcaptiontop", new Vector2(400, 25), 0.8f));
                    FontHandler.AddText(new Text(levelCaptionBottom, "levelcaptionbottom", new Vector2(400, 575), 0.8f));
                }
            }
            gameGrid.Draw();
            foreach (GameObject g in gameGrid.Grid)
            {
                if (g != null)
                    g.color = new Color4(ambient, ambient, ambient, 1);
            }
            foreach (GameObject g in collisionGameGrid.Grid)
            {
                if (g != null)
                    g.color = new Color4(ambient, ambient, ambient, 1);
            }

            if (!player.dead)
                player.Draw();
            EnemyList.ForEach(x => x.Draw());
            if (!player.dead)
            {
                projectileList.ForEach(x => x.Draw());

                particleList.ForEach(x => x.Draw());


                lightList.ForEach(x => x.Render());
            }

            collisionGameGrid.Draw();
            if (!player.dead)
            {
                tileFogList.ForEach(x => x.Draw());
            }
            if (player.dead)
                player.Draw();
            if (terminalGame != null && terminalGame.active)
            {
                terminalGame.Draw();
            }
        }
    }

    public class TileFog
    {
        public Vector2 position;
        public float alpha = 0.5f;
        public GameObject parent;
        public TileFog(Vector2 position, GameObject parent)
        {
            this.position = position;
            this.parent = parent;
        }

        public void Draw()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);


            GL.PushMatrix();
            GL.Translate(position.X, position.Y, 0);

            GL.Begin(BeginMode.Quads);
            {
                //GL.Color4(0.5f, 0.5f, 0.5f, 0.5f);
                GL.Color4(alpha, alpha, alpha, 0.5f);
                GL.Vertex2(-16, -16);
                GL.Vertex2(16, -16);
                GL.Vertex2(16, 16);
                GL.Vertex2(-16, 16);
            }
            GL.End();

            GL.Disable(EnableCap.Blend);
            GL.PopMatrix();

        }
    }

    public class Ray
    {
        public static bool FoundPlayer(Vector2 position, Vector2 player, World world, float precision = 2f, float maxDistance = 200)
        {
            float distance = 0f;
            float angle = (float)GameUtils.GetRotation(position, player);
            while (distance < maxDistance)
            {
                position = GameUtils.MoveAlongAngle(angle, position, precision);
                foreach (GameObject g in world.collisionGameGrid.Grid)
                {
                    if (g != null && g.textureName != "null")
                    {
                        if (GameObject.Contains(position, g))
                            return false;
                    }
                }
                distance += precision;
                if (GameUtils.GetDistance(position, player) < 32) return true;
            }

            return false;
        }

        public static bool FoundTile(Vector2 position, GameObject target, World world, float precision, float maxDistance)
        {
            float distance = 0f;
            float angle = (float)GameUtils.GetRotation(position, target.Position);
            while (distance < maxDistance)
            {
                position = GameUtils.MoveAlongAngle(angle, position, precision);
                foreach (GameObject g in world.collisionGameGrid.Grid)
                {
                    if (g != null && g.textureName != "null")
                    {
                        if (GameObject.Contains(position, g))
                        {
                            if (g != target)
                                return false;
                            else
                                return true;
                        }
                    }
                }
                foreach (GameObject g in world.gameGrid.Grid)
                {
                    if (g != null && g.textureName != "null")
                    {
                        if (GameObject.Contains(position, g))
                        {
                            if (g == target)
                            { return true; }
                        }
                    }
                }
                distance += precision;
            }

            return false;
        }
    }
}
