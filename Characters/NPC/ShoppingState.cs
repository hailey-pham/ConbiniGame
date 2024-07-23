using ConbiniGame.Scripts;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = ConbiniGame.Scripts.Debug;

public partial class ShoppingState : State
{
    //the fractional probability that a bubble shows on a bad or empty purchase
    [Export]
    private float bubbleShowProbablilty = 0.2f;
    
    //the minimum and maximum number of items the NPC will have on their shopping list, including duplica
    [Export]
    private int shoppingListMinimumItems = 2;
    [Export]
    private int shoppingListMaximumItems = 5;

    [Export]
    public int _maxCounters = 3;

    [Export]
    public Timer timer;

    //a reference to the thought bubble animation scenes
    [Export]
    public PackedScene WantItemScene;

    //a reference to the marker where the bubble scene will be spawned in
    private Marker2D bubbleSpawn;

    //saves counters
    private List<counter> _counters = new List<counter>();

    //number of counters we've been to
    private int _counterNum = 0;
    //counter we're currently at/trying to go to
    private counter currCounter;

    private NPCPreferenceModifier modifier;
    private npc npcScript;
    private Dictionary message;

    //shopping list of items the NPC wants to buy (generated at ready)
    private List<ItemRes> shoppingList = new();

    private static RandomNumberGenerator rng = new RandomNumberGenerator();
    public override void _Enter(Dictionary message)
    {
        //save a copy of our message for later
        this.message = message;

        //check for reference to parent
        if (message != null)
        {
            Variant temp;
            if (message.TryGetValue("npcScript", out temp))
            {
                //set our npcBody reference to what was passed in the message dictionary
                npcScript = (npc) temp;
            }
        }

        //get a reference to the location where we spawn speech bubbles 
        bubbleSpawn = npcScript.GetNode<Marker2D>("BubbleSpawn");

        //link our timer callback to the timeout signal of the timer
        timer.Timeout += Timer_Timeout;

        //link our navigation callback to the navigate agent of the npc script
        npcScript._navigationAgent.NavigationFinished += OnNavigationFinished;

        //get all the counters in the store and save their positions for later
        foreach (Node counter in GetTree().GetNodesInGroup("counters"))
        {
             _counters.Add(counter as counter);
        }

        GenerateShoppingList();
        GoToACounter();
    }

    //fills the shopping list with the popular items of the day, called at enter
    private void GenerateShoppingList()
    {
        int shoppingListSize = rng.RandiRange(shoppingListMinimumItems, shoppingListMaximumItems);

        for (int i = 0; i < shoppingListSize; i++)
        {
            var item = NPCPreferenceModifier.GetAPopularItem();
            shoppingList.Add(item);
        }
    }

    private ItemRes GetRandomShoppingListItem()
    {
        Debug.Assert(shoppingList.Count > 0, "Shopping Cart is empty! No random item!");

        return shoppingList[rng.RandiRange(0,shoppingList.Count-1)];
    }

    
    public override void _Exit()
    {
        //unsubscribe from callback
        npcScript._navigationAgent.NavigationFinished -= OnNavigationFinished;
    }

    public override void _Handle_Input(InputEvent input)
    {

    }


    public void OnNavigationFinished()
    {
        timer.Start();
    }
    public override void _Physics_Update(double delta)
    {

    }

    public override void _Update(double delta)
    {

    }

    private void SpawnBubbleWithProbability()
    {
        //cases where we don't want bubbles to show
        if(shoppingList.Count == 0)
        {
            return;
        }

        if (_counterNum > _maxCounters)
        {
            return;
        }

        if(rng.Randf() < bubbleShowProbablilty)
        {
            SpeechBubbleAnimation bubbleAnim = (SpeechBubbleAnimation)WantItemScene.Instantiate();
            bubbleAnim.SetItem(GetRandomShoppingListItem());
            bubbleSpawn.AddChild(bubbleAnim);
        }
    }

    private void Timer_Timeout()
    {
        //Gets a reference to the counter, its area2d code with the item logic and the item spawner itself.
        var thisCounter = currCounter;
        var thisItemSpawner = thisCounter.GetNode<ItemSpawner>("ItemSpawner"); // Technically not needed anymore, maybe idk :3

        //check if there is an item to buy and that we want to buy
        if (thisItemSpawner.currItem != null)
        {
            //calculate buy probability
            modifier ??= GetNode<NPCPreferenceModifier>("/root/NpcPreferenceModifier");
            ItemRes currItem = thisItemSpawner.currItem;
            //the probability that the NPC will buy the item on this table
            var buyProbability = 0f;

            //if we have asked for this item before, we are now guaranteed to buy it
            if (shoppingList.Contains(currItem))
            {
                buyProbability = 1f;
            }
            else
            {
                buyProbability = modifier.ItemBuyProbability(currItem);
            }

            //if buy probability is greater than random num
            if(rng.Randf() < buyProbability)
            {
                //removes the item from the counter and gives it to the NPC's shopping cart
                npcScript.ShoppingCart.Add(thisItemSpawner.RemoveItemRes());

                //if this was on our list, take it off.
                if(shoppingList.Contains(currItem))
                {
                    shoppingList.Remove(currItem);
                }
            }
            else
            {
                //if the transaction was unsuccessful, try and spawn the bubble
                SpawnBubbleWithProbability();
            }
        }
        else
        {
            //if there is no item out, spawn bubble based on probability
            SpawnBubbleWithProbability();
        }

        if (_counterNum <= _maxCounters)
        {
            _counterNum++;

            //a simple but effective way to prevent picking the same table twice

            var newCounter = currCounter;

            while (newCounter == currCounter)
            {
                newCounter = _counters[rng.RandiRange(0, _counters.Count - 1)];
            }

            currCounter = newCounter;

            //if there are items we currently want
            if(shoppingList.Count > 0)
            {
                var targetCounter = GetCounterWithShoppingListItem();
                //if there's a counter with an item on it that we want
                if(targetCounter != null)
                {
                    newCounter = targetCounter;
                }
            }

            npcScript.MoveToPositionOffset(newCounter.GetMarkerPosition());
        }
        else
        {
            if (npcScript.ShoppingCart.Count > 0)
            {
                //we're done browsing, so let's now checkout and pass the npcScript through our copied message
                stateMachine.TransitionTo("CheckoutState", message);
            }
            else
            {
                stateMachine.TransitionTo("LeaveState");
            }
            
        }
    }

    //returns a reference to any counter with an item currently on our asked list, chosen at random
    //if there are no items on our asked list, return null
    private counter GetCounterWithShoppingListItem()
    {
        if(shoppingList.Count == 0)
        {
            return null;
        }

        List<counter> counterCandidates = new();

        foreach (var item in shoppingList)
        {
            var counter = GetCounterWithItem(item);
            if(counter != null)
            {
                counterCandidates.Add(counter);
            }
        }

        if (counterCandidates.Count == 0)
        {
            return null;
        }

        return counterCandidates[rng.RandiRange(0, counterCandidates.Count - 1)];
    }

    //loops through all loaded counters and returns a reference to any with a matching itemres
    //if there are multiple matches, it will return a random matchine counter
    //if there are no matches, it will return null
    private counter GetCounterWithItem(ItemRes item)
    {
        counter targetCounter = null;
        List<counter> counterCandidates = new();
        foreach(var counter in _counters)
        {
            //if the item in the counter matches the item we're looking for
            if(counter.GetNode<ItemSpawner>("ItemSpawner").currItem == item)
            {
                counterCandidates.Add(counter);
            }
        }
        
        //if there are no counter candidates, return null
        if(counterCandidates.Count == 0)
        {
            return null;
        }

        //gets a random counter from the list of candidate counters
        targetCounter = counterCandidates[rng.RandiRange(0, counterCandidates.Count - 1)];
        return targetCounter;
    }

    //tells the NPC to go to a counter, prioritizing items currently on its shopping list
    private void GoToACounter()
    {
        counter targetCounter;
        var counter = GetCounterWithShoppingListItem();

        //if we can't find a counter with an item in our shopping list
        if (counter == null)
        {
            //go to a random counter
            targetCounter = _counters[rng.RandiRange(0, _counters.Count - 1)];
        }
        else
        {
            targetCounter = counter;
        }

        //keep track of our current counter
        currCounter = targetCounter;
        //increment the number of counters we've navigated to
        _counterNum++;
        //move to the damn counter
        npcScript.MoveToPositionOffset(currCounter.GetMarkerPosition());
    }
}
