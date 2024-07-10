using Godot;
using System;

public partial class NPCPreferenceModifier : Node
{
    private Calendar calendar;
    public float ItemBuyProbability(ItemRes item)
    {
        //we assume that npc's don't want to buy item
        float prob = 0f;

        //a short way of saying if calendar is null, get the calendar node
        calendar ??= GetNode<Calendar>("/root/Calendar");

        //replace this with a call to calendar to get the next disaster
        //0 - no disaster
        //Earthquake = 1
        //Tsunami = 2
        //Typhoon = 3
        //WildFire = 4
        //FlashFlood = 5
        //HeavySnow = 6

        // calendar.GetNextDayDisasterIndex(); <<- i think something like that

        //this is temporary
        var tomorrowDisasterInt = 1;

        //increase buy probability if disaster lines up
        if (item.DisasterFlags.HasFlag((ItemRes.DisastersEnum)(1 << tomorrowDisasterInt)))
        {
            prob += 0.5f;
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
