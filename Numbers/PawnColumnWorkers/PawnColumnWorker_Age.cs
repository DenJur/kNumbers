﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Numbers
{
    public class PawnColumnWorker_Age : PawnColumnWorker_Text
    {
        protected override string GetTextFor(Pawn pawn) => Math.Round(pawn.ageTracker.AgeBiologicalYearsFloat, 2).ToString("0.00");

        public override int Compare(Pawn a, Pawn b) => a.ageTracker.AgeBiologicalYearsFloat.CompareTo(b.ageTracker.AgeBiologicalYearsFloat);

        public override int GetMinWidth(PawnTable table) => base.GetMinWidth(table) + 20;
    }
}
