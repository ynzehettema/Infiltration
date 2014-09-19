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

using System.Threading;

namespace cyberpunkgamejamgame
{
    public class Particle
    {
        private Vector2 position;
        private Color4 color;
        private float angle;
        private float speed;

        private float maxTime;
        private float time = 0f;
        private float size = 0f;

        public bool remove;

        public Particle(Vector2 position, Color4 color, Random random)
        {
            this.position = position;
            this.color = color;
            this.angle = (float)random.NextDouble() * 360;
            this.speed = 2f + (float)random.NextDouble() * 5f;
            size = 1f + (float)random.NextDouble() * 2.5f;
            maxTime = (float)Glfw.GetTime() * random.Next(10, 25);

        }
        public Particle(Vector2 position, Color4 color, float perpAngle, Random random)
        {
            this.position = position;
            this.color = color;
            float spread = 45f;
            this.angle = (perpAngle - 180 - spread) + (float)random.NextDouble() * spread * 2;
            this.speed = 2f + (float)random.NextDouble() * 5f;
            size = 1f + (float)random.NextDouble() * 2.5f;
            maxTime = (float)Glfw.GetTime() * random.Next(10, 25);
        }

        public void Update(float delta)
        {
            position = GameUtils.MoveAlongAngle(angle, position, speed);
            time += delta / 10;
            remove = time >= maxTime;
        }

        public void Draw()
        {
            GL.Color4(color);

            GL.PointSize(size);

            GL.Begin(BeginMode.Points);

            GL.Vertex2(position.X, position.Y);

            GL.End();
        }
    }

    public class Projectile : GameObject
    {
        private float damage, speed;
        private byte team;
        private bool _remove;
        private World world;
        private Vector2 initialpos;
        public bool Remove
        {
            get { return _remove; }
            set
            {
                _remove = value;
                if (value)
                {
                    world.screenShake = 0.5f;
                    Random rand = new Random();
                    for (int a = 0; a < rand.Next(5, 15); a++)
                    {
                        world.particleList.Add(new Particle(Position, Color4.Red, Rotation - 90, rand));
                    }
                }
            }
        }

        public Projectile(Vector2 position, float angle, float damage, float speed, byte team, string textureName = "particle")
            : base(textureName, position)
        {
            Program.fire.Play();
            this.damage = damage;
            this.team = team;
            this.speed = speed;
            this.Rotation = angle;
            initialpos = position;
        }

        public void Update(World world, float delta)
        {
            this.world = world;
            Position = GameUtils.MoveAlongAngle(Rotation - 90, Position, speed * delta);

            foreach (GameObject g in world.collisionGameGrid.Grid)
            {
                if (g != null && g.textureName != "null" && GameObject.Contains(Position, g))
                {
                    Remove = true;
                    break;
                }
            }

            if (team == 0)
            {
                foreach (GameObject g in world.EnemyList)
                {
                    if (g != null && GameObject.Contains(Position, g))
                    {
                        Remove = true;
                        ((Enemy)g).health -= damage;
                        ((Enemy)g).fireDuration = 0.1f;
                        g.Rotation = (float)GameUtils.GetRotation(g.Position, world.player.Position) + 90; break;
                        
                    }
                }
            }
            else
            {
                if (GameObject.Contains(Position, world.player))
                {
                    Remove = true;
                    world.player.health -= damage;
                }
                foreach (GameObject g in world.EnemyList)
                {
                    if (g != null && GameObject.Contains(Position, g) && GameUtils.GetDistance(g.Position, initialpos) > 16)
                    {
                        Remove = true;
                        ((Enemy)g).health -= damage * 5;
                    }
                }
            }
            if (Remove)
            {
                Program.hit.Play();
            }
        }
    }
}
