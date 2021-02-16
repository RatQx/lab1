using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Laboras1
{
    public partial class Form1 : System.Web.UI.Page
    {
        ScorpoinContainer Scorpio = new ScorpoinContainer();
        protected void Page_Load(object sender, EventArgs e)
        {
            Label2.Visible = true;
            TextBox2.Visible = true ;
            Scorpio = ReadData(Server.MapPath("App_Data//Data.txt"));
            
            PrintData(Scorpio);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label2.Visible = true;
            TextBox2.Visible = true;
            for(int i=0;i<Scorpio.num;i++)
            {
                Scorpio.FindScorpion(i,Scorpio.Get(i));
                PrintResults(Scorpio.Get(i));
            }
            
            
            
        }
        /// <summary>
        /// Reads data of scorpion from App_Data//Data.txt file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ScorpoinContainer ReadData(string file)
        {
            ScorpoinContainer scorpions = new ScorpoinContainer();
            string[] lines = File.ReadAllLines(file);
            scorpions.num = int.Parse(lines[0]);
            lines = lines.Skip(1).ToArray();
            foreach (var symbols in lines)
            {
                char[] sym = symbols.ToCharArray();
                char[] locations = symbols.ToCharArray();
                Array.Clear(locations, 0,locations.Length);
                List<int> leg = new List<int>();
                scorpions.Add(new Scorpion(sym, locations,0,0,0,leg));
            }
            return scorpions;
        }
        private void PrintData(ScorpoinContainer scorpion)
        {
            string data = "";
            for(int i = 0 ; i < scorpion.num ; i++)
            {
                char[] sym = scorpion.Get(i).Symbol;
                Console.WriteLine(scorpion.Get(i));
                int count = 0;
                for(int j = 0 ; j < scorpion.num ; j ++)
                {
                    count++;
                    data += sym[j]+ " ";
                }
                data += '\n';
            }
            TextBox1.Text = data;
        }
        public void PrintResults(Scorpion scorpion)
        {
            string results = "";
            results += string.Format("Figura yra skorpionas!");
            results += string.Format("Geluonis yra {0} taskas \n", scorpion.Stinger);
            results += string.Format("Liemuo yra {0} taskas \n", scorpion.Waist);
            results += string.Format("Uodega yra {0} taskas \n", scorpion.Tail);
            int temp = 0;
            foreach(var sym in scorpion.Legs)
            {
                results += string.Format("{0} Koja yra {1} taskas \n", temp, sym);
                temp++;
            }
            TextBox2.Text = results;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }
    }
    public class Scorpion
    {
        public char[] Symbol { get; set; }           //Symbol in specific line.
        public char[] Locations { get; set; }       //If location fits.
        public int Stinger { get; set; }
        public int Waist { get; set; }
        public int Tail { get; set; }
        public List<int> Legs = new List<int>();
        /// <summary>
        /// Konstruktorius su parametrais.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="locations"></param>
        public Scorpion(char[] symbol, char[] locations,int stinger, int waist, int tail, List<int> legs)
        {
            Symbol = symbol;
            Locations = locations;
            Stinger = stinger;
            Waist = waist;
            Tail = tail;
            Legs = legs;
        }
    }
    public class ScorpoinContainer
    {
        private Scorpion[] scorpions { get; set; }
        public int num;
        public int Count { get; set; }
        private int Capacity;
        public ScorpoinContainer(int capacity = 2)
        {
            Capacity = capacity;
            scorpions = new Scorpion[capacity];
        }
        public void EnsureCapacity(int mincapacity)
        {
            if (mincapacity > Capacity)
            {
                Scorpion[] temp = new Scorpion[mincapacity];
                for (int i = 0; i < Count; i++)
                {
                    temp[i] = scorpions[i];
                }
                Capacity = mincapacity;
                scorpions = temp;
            }
        }
        public void Add(Scorpion scorpion)
        {
            if (Count == Capacity)
            {
                EnsureCapacity(Capacity * 2);
            }
            this.scorpions[Count++] = scorpion;
        }
        public Scorpion[] getAllScorpions()
        {
            return this.scorpions;
        }
        public Scorpion Get(int index)
        {
            return scorpions[index];
        }
        /// <summary>
        /// Gets nearest part neibours and returns it in int array.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int[] GetNearOnes(int index)
        {
            List<int> neighbors = new List<int>();
            for (int i = 0; i < this.num; i++)
            {
                if (index == i)
                    continue;
                char row = this.Get(i).Symbol[index];
                if (row == '+')
                    neighbors.Add(i);
            }
            return neighbors.ToArray<int>();
        }
        /// <summary>
        /// Finds all scorpion parts and returns full scorpions. If scorpion isn't full returns null.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="scorpion"></param>
        /// <returns></returns>
        public Scorpion FindScorpion(int i,Scorpion scorpion)
        {
            if (i == 0)
            {
                return FindScorpion(i + 1,scorpion);
            }
            if (i >= this.num)
                return null;
            int[] stingNeighbors = GetNearOnes(i);
            if (stingNeighbors.Length != 1)
            {
                FindScorpion(i + 1, scorpion);
                return null;
            }
            scorpion.Stinger = i;
            int[] tailNeighbors = GetNearOnes(stingNeighbors[0]);
            if (tailNeighbors.Length != 2)
            {
                FindScorpion(i + 1, scorpion);
                return null;
            }
            int waist = tailNeighbors.Where(x => x != scorpion.Stinger).FirstOrDefault();
            scorpion.Waist = waist;
            scorpion.Tail = stingNeighbors[0];
            int[] waistNeighbors = GetNearOnes(waist);
            foreach (int index in waistNeighbors)
            {
                if (index == scorpion.Tail)
                    continue;
                int[] legNeighbors = GetNearOnes(index);
                for (int j = 0; j < legNeighbors.Length; j++)
                {
                    if (legNeighbors[j] == waist)
                    {
                        continue;
                    }
                    if (!waistNeighbors.Contains(legNeighbors[j]) || legNeighbors[j] == scorpion.Tail)
                    {
                        FindScorpion(i + 1, scorpion);
                        return null;
                    }
                }
                scorpion.Legs.Add(index);
            }
            if (scorpion.Legs.Count > 0)
                return scorpion;
            else
                return null;
        }
    }

}