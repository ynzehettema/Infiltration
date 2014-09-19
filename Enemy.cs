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
    public class Enemy : GameObject
    {
        private Light l;
        public float health = 100f;
        private bool _remove;
        private World world;

        private float fireDelay = 0;
        private float maxFireDelay = 0.25f;
        public float fireDuration = 0;
        private float spread = 5f;

        private List<Vector2> nodeList = new List<Vector2>();
        private Vector2 target;

        private bool moves = false;

        new public bool remove
        {
            get { return _remove; }
            set
            {
                _remove = value;
                if (value)
                {
                    Program.boom.Play();
                    world.screenShake += 2f;
                    Random rand = new Random();
                    for (int a = 0; a < 120; a++)
                    {
                        world.particleList.Add(new Particle(Position, Color4.Yellow, rand));
                        world.particleList.Add(new Particle(Position, Color4.Gray, rand));
                        world.particleList.Add(new Particle(Position, Color4.DarkGray, rand));
                    }

                }
            }
        }

        public Enemy(Vector2 position, World world, string textureName = "turret")
            : base(textureName, position)
        {
            l = new Light(position, Color4.Red, 200, false);
            l.anglestep = 2f;
        }
        public Enemy(Vector2 position, World world, bool moves, string textureName = "turret")
            : base(textureName, position)
        {
            l = new Light(position, Color4.Red, 200, true);
            l.anglestep = 2f;
            this.moves = moves;
            if (moves)
            {
                health *= 2;
                maxFireDelay /= 8;
                target = position;
            }
        }

        public void Update(float delta, Player p, World world)
        {
            if (fireDelay > 0)
            {
                fireDelay -= delta;
            }
            this.world = world;
            remove = health <= 0f;
            if (fireDuration > 0)
            {
                fireDuration -= delta;
                if (fireDelay <= 0)
                {

                    if (moves)
                    {
                        fireDelay = maxFireDelay;
                        world.projectileList.Add(new Projectile(Position, Rotation + (-spread + ((float)new Random().NextDouble() * (spread * 2))), 40f, 1000, 1));
                    }
                }
            }
            if (GameUtils.GetDistance(Position, p.Position) < 200f)
            {
                if (Ray.FoundPlayer(Position, p.Position, world))
                {
                    l.anglestep = 3f;
                    l.Dynamic = true;
                    float angle = (float)GameUtils.GetRotation(Position, p.Position) + 90;
                    if ((float)Math.Abs(Rotation - angle) < l.width / 2)
                    {
                        Rotation = angle;
                        if (fireDelay <= 0)
                        {
                            fireDelay = maxFireDelay;
                            if (moves)
                            {
                                world.projectileList.Add(new Projectile(Position, Rotation + (-spread + ((float)new Random().NextDouble() * (spread * 2))), 40f, 1000, 1));
                            }
                            else
                            {

                                world.projectileList.Add(new Projectile(Position, Rotation + (-spread + ((float)new Random().NextDouble() * (spread * 2))), 95f, 1000, 1));
                            }
                        }
                    }
                }
                else
                {
                    if (l.Dynamic && !moves)
                    {
                        l.anglestep = 1f;
                        l.Dynamic = false;
                        l.Restart();
                    }
                }
            }
            l.direction = Rotation - 90;
            if (!moves)
                l.width = 35f;
            else
                l.width = 40;
            l.Position = Position;
            if (moves && fireDelay <= 0)
            {
                if (GameUtils.GetDistance(Position, target) < 2)
                {

                    nodeList.Add(target);
                again:
                    float closestRailDist = float.MaxValue;
                    GameObject closestRail = null;
                    foreach (GameObject g in world.gameGrid.Grid)
                    {
                        if (g != null && g.textureName == "rail" && !nodeList.Contains(g.Position))
                        {
                            float dist = (float)GameUtils.GetDistance(Position, g.Position);
                            if (dist <= 35 && dist < closestRailDist)
                            {
                                closestRail = g;
                                closestRailDist = dist;
                            }
                        }
                    }
                    if (closestRail != null)
                    {
                        target = closestRail.Position;
                    }
                    else
                    {
                        nodeList = new List<Vector2>();
                        goto again;
                    }
                }
                else
                {
                    Vector2 nextpos = GameUtils.MoveTowards(Position, target, 175 * delta);
                    if (GameUtils.GetDistance(Position, nextpos) < 8)
                    {
                        Position = nextpos;
                    }
                    Rotation = (float)GameUtils.GetRotation(Position, target) + 90;
                }
            }
        }

        new public void Draw()
        {
            this.color = new Color4(world.ambient, world.ambient, world.ambient, 1);
            base.Draw();
            if (!world.player.dead)
                l.Render();
        }
    }

}
