﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Numbers
{
    public class PawnColumnWorker_Equipment : PawnColumnWorker
    {
        private int width;
        private static readonly int baseWidth = 6 * 28; //6 boxes, 28 wide each.

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            Pawn p1 = pawn;
            if (p1.RaceProps.Animal) return;
            if (p1.equipment != null)
            {
                GUI.BeginGroup(rect);
                float x = 0;
                float gWidth = 28f;
                float gHeight = 28f;
                foreach (ThingWithComps thing in p1.equipment.AllEquipmentListForReading)
                {
                    Rect rect2 = new Rect(x, 0, gWidth, gHeight);
                    DrawThing(rect2, thing, p1);
                    x += gWidth;
                }

                if (p1.apparel != null)
                    foreach (Apparel thing in from ap in p1.apparel.WornApparel
                                              orderby ap.def.apparel.bodyPartGroups[0].listOrder descending
                                              select ap)
                    {
                        Rect rect2 = new Rect(x, 0, gWidth, gHeight);
                        DrawThing(rect2, thing, p1);
                        x += gWidth;
                        if (x > width)
                            this.width = (int)x;
                    }
                GUI.EndGroup();
            }
        }

        public override int GetMinWidth(PawnTable table) => Mathf.Max(this.width, baseWidth);

        private void DrawThing(Rect rect, Thing thing, Pawn selPawn)
        {
            if (Widgets.ButtonInvisible(rect) && Event.current.button == 1)
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>
                {
                    new FloatMenuOption("ThingInfo".Translate(), () => Find.WindowStack.Add(new Dialog_InfoCard(thing)))
                };

                //leans heavily on ITab_Pawn_Gear
                if (selPawn.IsColonistPlayerControlled)
                {
                    Action action = null;
                    ThingWithComps eq = thing as ThingWithComps;
                    if (thing is Apparel ap && selPawn.apparel != null && selPawn.apparel.WornApparel.Contains(ap))
                    {
                        action = delegate
                        {
                            selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.RemoveApparel, ap));
                        };
                    }
                    else if (eq != null && selPawn.equipment.AllEquipmentListForReading.Contains(eq))
                    {
                        action = delegate
                        {
                            selPawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.DropEquipment, eq));
                        };
                    }
                    else if (!thing.def.destroyOnDrop)
                    {
                        action = delegate
                        {
                            selPawn.inventory.innerContainer.TryDrop(thing, selPawn.Position, selPawn.Map, ThingPlaceMode.Near, out Thing unused);
                        };
                    }
                    list.Add(new FloatMenuOption("DropThing".Translate(), action));
                }
                FloatMenu window = new FloatMenu(list, thing.LabelCap, false);
                Find.WindowStack.Add(window);
            }
            GUI.BeginGroup(rect);
            if (thing.def.DrawMatSingle?.mainTexture != null)
            {
                Widgets.ThingIcon(new Rect(3f, 3f, 27f, 27f), thing);
            }
            GUI.EndGroup();
            TooltipHandler.TipRegion(rect, new TipSignal(thing.LabelCap));
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return (a.equipment.HasAnything() ? a.equipment.AllEquipmentListForReading.First().LabelCap[0] : 0).CompareTo           (b.equipment.HasAnything() ? b.equipment.AllEquipmentListForReading.First().LabelCap[0] : 0);
        }
    }
}
