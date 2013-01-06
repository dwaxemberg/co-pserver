using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.Features.Flowers
{
    public class Flowers
    {
        public Flowers()
        {
        }
        public uint id;
        private uint _RedRoses, _RedRoses2day, _Lilies, _Lilies2day, _Orchads, _Orchads2day, _Tulips, _Tulips2day = 0;
        private DateTime _LastFlowerSent;

        public DateTime LastFlowerSent
        {
            get { return _LastFlowerSent; }
            set { _LastFlowerSent = value; Update("last_flower_sent", value.ToString()); }
        }

        public uint RedRoses
        {
            get { return _RedRoses; }
            set { _RedRoses = value; Update("redroses", value.ToString()); }
        }

        public uint RedRoses2day
        {
            get { return _RedRoses2day; }
            set { _RedRoses2day = value; Update("redrosestoday", value.ToString()); }
        }

        public uint Lilies
        {
            get { return _Lilies; }
            set { _Lilies = value; Update("lilies", value.ToString()); }
        }

        public uint Lilies2day
        {
            get { return _Lilies2day; }
            set { _Lilies2day = value; Update("liliestoday", value.ToString()); }
        }

        public uint Orchads
        {
            get { return _Orchads; }
            set { _Orchads = value; Update("orchads", value.ToString()); }
        }

        public uint Orchads2day
        {
            get { return _Orchads2day; }
            set { _Orchads2day = value; Update("orchadstoday", value.ToString()); }
        }

        public uint Tulips
        {
            get { return _Tulips; }
            set { _Tulips = value; Update("tulips", value.ToString()); }
        }

        public uint Tulips2day
        {
            get { return _Tulips2day; }
            set { _Tulips2day = value; Update("tulipstoday", value.ToString()); }
        }

        public void Update(string C, string V)
        {
            FlowerSystemTable.Update(id, C, V);
        }

        //public string ToString
        //{
        //    get { return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}", RedRoses, RedRoses2day, Lilies, Lilies2day, Orchads, Orchads2day, Tulips, Tulips2day); }
        //}
    }
}
