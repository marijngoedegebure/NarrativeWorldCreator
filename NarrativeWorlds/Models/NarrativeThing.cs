﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeThing
    {
        public string Name { get; set; }
        public bool Placed { get; set; }

        public NarrativeThing(string Name)
        {
            this.Name = Name;
        }
    }
}