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
    public class Player : GameObject
    {
        private float _health = 255f;
        private float maxHealth = 255f;
        public float health
        {
            get { return _health; }
            set
            {
                if (value < _health)
                {
                    Program.ouch.Play();
                    world.screenShake = 3f;
                }
                _health = value;

            }
        }

        public bool dead = false;

        private World world;
        public float checkMapDelay = 2.5f;

        public Player(string textureName, Vector2 position)
            : base(textureName, position)
        {
            this.hitboxDimensions *= .8f;
        }
        private bool IsKeyPressed(char key)
        {
            return Input.GetState(0).Keyboard[key] && !Input.GetState(1).Keyboard[key];
        }

        public void Update(float delta, World world)
        {
            if (checkMapDelay > 0)
                checkMapDelay -= delta;
            if (health < maxHealth)
            {
                if (!dead)
                {
                    health += 0.25f;
                }
                else
                {
                    health += 2.5f;
                }

            }
            if (health <= 0)
            {
                dead = true;
            }
            if (dead)
            {
                
                FontHandler.TextList = new List<Text>();
                FontHandler.AddText(new Text("YOU MADE MY DAY, CYBERPUNK", "dead", new Vector2(420, 250), 0.75f));
                FontHandler.AddText(new Text("TRY AGAIN", "dead2", new Vector2(400, 350), 0.75f));
                FontHandler.AddText(new Text("PRESS SPACE", "dead3", new Vector2(400, 450), 0.75f));
                if (world.ambient > 0)
                {
                    world.ambient -= 0.01f;
                }
                this.Rotation = 0;
                if (GameUtils.GetDistance(Position, new Vector2(400, 300) - Program.cameraPosition) > 2)
                    Position = GameUtils.MoveTowards(Position, new Vector2(400, 300) - Program.cameraPosition, 150 * delta);
                else
                {
                    Position = new Vector2(400, 300) - Program.cameraPosition;
                }
                if (Input.GetState(0).Keyboard[Key.Space] && !Input.GetState(1).Keyboard[Key.Space])
                {
                    world.restartMap = true;
                }
            }
            this.world = world;



            if (IsKeyPressed('E'))
            {
                foreach (GameObject g in world.collisionGameGrid.Grid)
                {
                    if (g != null)
                    {
                        if (g.textureName == "terminal" && GameUtils.GetDistance(g.Position, Position) < 40)
                        {
                            world.terminalGame = new TerminalGame(g);
                            world.inTerminalGame = true;
                        }
                    }
                }
            }
            if (!dead)
            {
                this.Rotation = (float)GameUtils.GetRotation(this.Position, Input.GetState(0).MousePosition - new Vector2(80, 80)) + 90;

                Vector2 bufferposition = Position;


                if (Input.GetState(0).Mouse.LeftButton && !Input.GetState(1).Mouse.LeftButton)
                {
                    float spread = 2.5f;
                    world.projectileList.Add(new Projectile(Position, Rotation + (-spread + ((float)new Random().NextDouble() * (spread * 2))), 10f, 1200, 0));
                }

                if (Input.GetState(0).Keyboard['W'])
                {
                    //this.sprite.Position = GameUtils.MoveTowards(this.sprite.Position, Input.GetState(0).MousePosition - new Vector2(80, 80), 150 * delta);
                    bufferposition += GameUtils.MoveAlongAngle(-90, Position, 450 * delta) - Position;
                }
                if (Input.GetState(0).Keyboard['S'])
                {
                    //this.sprite.Position = GameUtils.MoveTowards(this.sprite.Position, Input.GetState(0).MousePosition - new Vector2(80, 80), -(150 * delta));
                    bufferposition += GameUtils.MoveAlongAngle(90, Position, 450 * delta) - Position;
                }
                if (Input.GetState(0).Keyboard['A'])
                {
                    bufferposition += GameUtils.MoveAlongAngle(180, Position, 450 * delta) - Position;
                }
                if (Input.GetState(0).Keyboard['D'])
                {
                    bufferposition += GameUtils.MoveAlongAngle(0, Position, 450 * delta) - Position;
                }
                if (GameUtils.GetDistance(Position, bufferposition) < 16)
                {
                    if (!CheckCollision(bufferposition, world))
                    {
                        Position = bufferposition;
                    }
                    else
                    {
                        if (!CheckCollision(new Vector2(bufferposition.X, Position.Y), world))
                        {
                            Position = new Vector2(bufferposition.X, Position.Y);
                        }
                        else
                        {
                            if (!CheckCollision(new Vector2(Position.X, bufferposition.Y), world))
                            {
                                Position = new Vector2(Position.X, bufferposition.Y);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

            if (world.levelComplete)
            {
                Program.cameraPosition = GameUtils.MoveTowards(Program.cameraPosition, new Vector2(880, 80), (float)GameUtils.GetDistance(Program.cameraPosition, new Vector2(79, 80)) / 25f);
            
                if (world.ambient > 0)
                {
                    world.ambient -= 0.01f;
                }
                foreach (TileFog t in world.tileFogList)
                {
                    if (t.alpha > 0)
                    {
                        t.alpha -= 0.01f;
                    }
                }
                FontHandler.TextList = new List<Text>();
                FontHandler.AddText(new Text("LEVEL COMPLETE", "yay!",  new Vector2(400,300), 1f));
                
                if ((float)GameUtils.GetDistance(Program.cameraPosition, new Vector2(880, 80)) < 32)
                {
                    world.incrementMap = true;
                }
                
            }
            if (checkMapDelay > 2.5f)
            {
                checkMapDelay = 2.5f;
            }
            if (!dead && checkMapDelay <= 0 && !world.incrementMap && !world.levelComplete)
            {
                foreach (GameObject g2 in world.gameGrid.Grid)
                {
                    if (g2 != null && GameObject.Contains(Position, g2))
                    {
                        return;
                    }
                }
                world.levelComplete = true;
                Program.complete.Play();
            }
        }

        private bool CheckCollision(Vector2 position, World world)
        {
            GameObject buffer = new GameObject("player", position);
            buffer.hitboxDimensions = this.hitboxDimensions;

            foreach (GameObject g in world.collisionGameGrid.Grid)
            {
                if (g != null && g.textureName != "null")
                    if (GameObject.Intersects(buffer, g))
                        return true;
            }

            return false;
        }

        public static void activateBlock(GameObject g, World world)
        {
            if (g.textureName == "off")
            {
                g.textureName = "on";
                foreach (Light l in world.lightList)
                {
                    if (l.color == Color4.Red && l.Position == g.Position)
                    {
                        l.color = Color4.Green;
                    }
                }
            }
            if (g.textureName.StartsWith("door"))
            {
                g.textureName = "null";
            }
            foreach (Light l in world.lightList)
            {
                if (GameUtils.GetDistance(g.Position, l.Position) <= 32)
                {
                    l.isActivated = !l.isActivated;
                }
            }
            foreach (GameObject g2 in world.gameGrid.Grid)
            {
                if (g2 != null && g2 != g && g2.textureName == "off" && GameUtils.GetDistance(g.Position, g2.Position) <= 32)
                {
                    activateBlock(g2, g, world);
                }
            }
            foreach (GameObject g2 in world.collisionGameGrid.Grid)
            {
                if (g2 != null && g2 != g && (g2.textureName.StartsWith("door") || g2.textureName.StartsWith("mask")) && GameUtils.GetDistance(g.Position, g2.Position) <= 32)
                {
                    activateBlock(g2, g, world);
                }
            }
        }
        public static void activateBlock(GameObject g, GameObject sender, World world)
        {
            if (g.textureName == "off")
            {
                g.textureName = "on";
                foreach (Light l in world.lightList)
                {
                    if (l.color == Color4.Red && l.Position == g.Position)
                    {
                        l.color = Color4.Green;
                    }
                }
            }
            if (g.textureName.StartsWith("door"))
            {
                g.textureName = "null";
            }
            foreach (Light l in world.lightList)
            {
                if (GameUtils.GetDistance(g.Position, l.Position) <= 32)
                {
                    l.isActivated = !l.isActivated;
                }
            }
            foreach (GameObject g2 in world.gameGrid.Grid)
            {
                if (g2 != null && g2 != g && g2 != sender && g2.textureName == "off" && GameUtils.GetDistance(g.Position, g2.Position) <= 32)
                {
                    activateBlock(g2, g, world);
                }
            }
            foreach (GameObject g2 in world.collisionGameGrid.Grid)
            {
                if (g2 != null && g2 != g && g2 != sender && (g2.textureName.StartsWith("door") || g2.textureName.StartsWith("mask")) && GameUtils.GetDistance(g.Position, g2.Position) <= 32)
                {
                    activateBlock(g2, g, world);
                }
            }
        }
    }
}
