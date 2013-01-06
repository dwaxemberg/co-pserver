using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Worker
{
    public partial class Manager : Form
    {
        public Manager()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //addnpc
            Interfaces.INpc npc = new Network.GamePackets.NpcSpawn();
            npc.UID = uint.Parse(textBox1.Text);
            if (!ServerBase.Kernel.Maps[ushort.Parse(textBox6.Text)].Npcs.ContainsKey(npc.UID))
            {
                npc.Kind = (Enums.NpcKind)byte.Parse(textBox2.Text);
                npc.X = ushort.Parse(textBox4.Text);
                npc.Y = ushort.Parse(textBox5.Text);
                npc.MapID = ushort.Parse(textBox6.Text);
                npc.Model = ushort.Parse(textBox7.Text);
                npc.Facing = (Enums.ConquerAngle)0;
                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.INSERT);
                cmd.Insert("cq_npc").Insert("id", npc.UID).Insert("name", textBox3.Text).Insert("type", (ushort)npc.Kind).Insert("lookface", (int)npc.Model).Insert("cellx", npc.X).Insert("celly", npc.Y).Insert("mapid", npc.MapID);
                cmd.Execute();
                npc.Model = (ushort)(npc.Model / 10);
                ServerBase.Kernel.Maps[ushort.Parse(textBox6.Text)].AddNpc(npc);
                WriteLine("Successfully added new npc " + npc.UID);
            }
            else
            {
                WriteLine("The map " + textBox6.Text + " already contains \n an npc with id " + npc.UID);
            }
        }

        public void WriteLine(string value)
        {
            richTextBox1.AppendText(value + "\n");
        }

        public void Clear()
        {
            richTextBox1.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //removenpc
            Interfaces.INpc npc = new Network.GamePackets.NpcSpawn();
            npc.UID = uint.Parse(textBox1.Text);
            if (ServerBase.Kernel.Maps[ushort.Parse(textBox6.Text)].Npcs.ContainsKey(npc.UID))
            {
                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.DELETE);
                cmd.Delete("cq_npc", "id", npc.UID);
                cmd.Execute();
                ServerBase.Kernel.Maps[ushort.Parse(textBox6.Text)].Npcs.SafeRemove(npc.UID);
                WriteLine("Successfully removed npc " + npc.UID);
            }
            else
            {
                WriteLine("The map " + textBox6.Text + " already doesn't contains \n an npc with id " + npc.UID);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Interfaces.INpc npc = new Network.GamePackets.NpcSpawn();
            npc.UID = uint.Parse(textBox8.Text);
            if (ServerBase.Kernel.Maps[ushort.Parse(textBox6.Text)].Npcs.ContainsKey(npc.UID))
            {
                Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                cmd.Update("cq_npc").Set("direction", textBox9.Text).Where("id", npc.UID);
                cmd.Execute();
                ServerBase.Kernel.Maps[ushort.Parse(textBox8.Text)].Npcs[npc.UID].Facing = (Enums.ConquerAngle)ushort.Parse(textBox9.Text);
                WriteLine("Successfully updated npc " + npc.UID);
            }
            else
            {
                WriteLine("The map " + textBox6.Text + " doesn't contains \n an npc with id " + npc.UID);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Local_Actions.Action_State Action = new Local_Actions.Action_State();
            Action.ID = uint.Parse(textBox10.Text);
            Action.ID_Next = uint.Parse(textBox11.Text);
            Action.ID_Next_Fail = uint.Parse(textBox12.Text);
            Action.Type = textBox13.Text;
            Action.Data_Type = textBox14.Text;
            Action.Param = textBox15.Text;
            Database.ActionTable.InsertAction(Action, uint.Parse(textBox16.Text));
            richTextBox3.AppendText("Inserted new action... Check it!\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox10.Text = textBox11.Text = textBox12.Text = textBox13.Text = textBox14.Text = textBox15.Text = textBox16.Text = "";
            richTextBox3.Clear();
        }
    }
}
