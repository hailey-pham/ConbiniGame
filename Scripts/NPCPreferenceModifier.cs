using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NPCPreferenceModifier : Node
{
    private Calendar calendar;
    private SceneManager sceneManager;
    private globals globals;

    private static List<ItemWithProb> itemProbabilities = new List<ItemWithProb>();

    struct ItemWithProb
    {
        public ItemWithProb(ItemRes item,float prob)
        {
            itemRes = item;
            itemProb = prob;
        }
        public ItemRes itemRes { get; init; }
        public float itemProb { get; init; }

    }
    public override void _Ready()
    {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        sceneManager.SceneChanged += OnSceneChanged;
        globals = GetNode<globals>("/root/Globals");
    }
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

    private void OnSceneChanged(string sceneName)
    {
        //if we have just switched to the game scene
        if(sceneName == "gamescene")
        {
            CalculatePopularItems();
        }
    }

    //at the beginning of each day, create a item dictionary sorted by probability.
    private void CalculatePopularItems()
    {
        var items = globals.Stock.Values;

        foreach (var item in items)
        {
            float buyProb = ItemBuyProbability(item);
            itemProbabilities.Add(new ItemWithProb(item, buyProb));
        }

        //sort by probability rankings in descending order
        itemProbabilities.Sort(delegate(ItemWithProb x, ItemWithProb y)
        {
            return y.itemProb.CompareTo(x.itemProb);
        });
    }

    //returns one of the 3 most popular items
    public static ItemRes GetAPopularItem()
    {
        int numPopularItems = 3;
        ItemRes[] popularItems = new ItemRes[numPopularItems];
        for (int i = 0; i < numPopularItems; i++)
        {
            popularItems[i] = itemProbabilities[i].itemRes;
        }

        var rng = new RandomNumberGenerator();

        return popularItems[rng.RandiRange(0, popularItems.Length - 1)];
    }
}
