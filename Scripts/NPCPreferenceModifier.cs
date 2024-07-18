using Godot;
using System;

public partial class NPCPreferenceModifier : Node
{
    private Calendar calendar;
    public float ItemBuyProbability(ItemRes item)
    {
        //we assume that npc's have a very low chance of buying each item
        float prob = 0.05f;

        //a short way of saying if calendar is null, get the calendar node
        calendar ??= GetNode<Calendar>("/root/Calendar");

        //call to calendar to get next disaster
        var tomorrowDisasterInt = calendar.GetNextDayDisasterIndex();

        //increase buy probability if disaster lines up
        if (item.DisasterFlags.HasFlag((ItemRes.DisastersEnum)(1 << tomorrowDisasterInt)))
        {
            if (!(tomorrowDisasterInt == 1) && !(tomorrowDisasterInt == 2))
            {
               prob += 0.5f;
            }
        }

        //the integer of the current season in the game
        var currSeasonInt = calendar.GetCurrentSeason();

        //increase buy probability if season lines up
        if (item.SeasonFlags.HasFlag((ItemRes.SeasonsEnum)(1 << currSeasonInt)))
        {
            prob += 0.5f;
        }

        return prob;
    }
}
