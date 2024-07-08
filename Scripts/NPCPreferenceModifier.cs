using Godot;
using System;

public partial class NPCPreferenceModifier : Node
{
    private Calendar calendar;
    public float ItemBuyProbability(ItemRes item)
    {
        //have yet to add disaster logic

        calendar ??= GetNode<Calendar>("/root/Calendar");

        var currSeasonInt = calendar.GetCurrentSeason();

        if (item.SeasonFlags.HasFlag((ItemRes.SeasonsEnum)(1 << currSeasonInt)))
        {
            return 1f;
        }
        else
        {
            return 0.0f;
        }
    }
}
