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
using MetalLib.GameWorld;

namespace cyberpunkgamejamgame
{
    public class TerminalGame
    {
        public bool active = true;
        
        public Sprite terminalScreen;
        private GameObject sender;
        private float state = 0;
        private float maxTime = 5f;
        private float time = 0f;
        private bool playedsound = false;

        public TerminalGame(GameObject sender)
        {
            this.sender = sender;
            terminalScreen = new Sprite("terminalscreen", new Vector2(400, 300) - new Vector2(80,80));
            terminalScreen.SetDimensions(0f, 0f);
        }

        private DateTime t1;

        public void Update(World world, float delta)
        {
            
            
            if (terminalScreen.Width < 677)
            {
                terminalScreen.Width += 8f * 1.97953216374f;
            }
            if (terminalScreen.Height < 342)
            {
                terminalScreen.Height += 8f;
            }
            if (terminalScreen.Width >= 677 && terminalScreen.Height >= 342)
            {
                if (!playedsound)
                {
                    Program.hacking.Play();
                    playedsound = true;
                    t1 = DateTime.Now;
                }
                time += delta * 3.125f;
                state = (55 / maxTime) * time;
                float ypos = 40f;

                if (state > 1) FontHandler.AddText(new Text("   ONACTIVATE", "line1", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 2) FontHandler.AddText(new Text(" [", "line2", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 3) FontHandler.AddText(new Text("         SET-POWER-OUTPUT [ DIRECTIONS.ALL, TRUE ]:", "line3", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 4) FontHandler.AddText(new Text("         SET-EXPLODER-DEVICE [ STATE-OFF ]:", "line4", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 5) FontHandler.AddText(new Text("         ACTIVATE [ BACON-GENERATOR-9000 ]:", "line5", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 6) FontHandler.AddText(new Text("         GOOGLE [ KITTENS ]:", "line6", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 7) FontHandler.AddText(new Text("         YAHOO [ NOTHING ]:", "line7", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 8) FontHandler.AddText(new Text("           SYSTEM-OUT-PRINTLINE [ CYBERPUNK IS AWESOME ]:", "line8", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 9) FontHandler.AddText(new Text("           SYSTEM-OUT-PRINTLINE [ GAMEJAMS ARE AWESOME ]:", "line9", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 10) FontHandler.AddText(new Text("         WHILE [ TRUE ] DO", "line10", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 11) FontHandler.AddText(new Text("         [", "line11", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 12) FontHandler.AddText(new Text("                SYSTEM-OUT-PRINTLINE [ YOU ARE AWESOME ]:", "line12", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 13) FontHandler.AddText(new Text("         ]", "line13", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 14) FontHandler.AddText(new Text(" ]", "line14", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 15) FontHandler.AddText(new Text("  TERMINAL: SYSTEM UNLOCKED", "line15", terminalScreen.Position - new Vector2(250, ypos), 0.5f, true)); ypos -= 20;
                if (state > 20)
                {
                    Console.WriteLine((DateTime.Now - t1).TotalMilliseconds);
                    Player.activateBlock(sender,world);
                    this.active = false;
                    FontHandler.TextList = new List<Text>();
                }
            }
        }

        public void Draw()
        {
            terminalScreen.Draw();
        }
    }
}
