If you hate reading, you can find a Video that explains everything that is written in here. The title is "How to use Golden Walnut Framework for Stardew Valley" by ResoNight.

This is a complete Guide on how to create a mod that adds Golden Walnuts and Parrot Upgrade Perches. PLEASE DO NOT try learning by doing with this Framework Mod. There are a lot of things that you have to know, otherwise it will lead to a lot of confusion and errors. If you get confused by my explanations, I added an example_content.json in this folder. That is the content.json that I use for my own Mod 'Island Expansion', so you can just look how I did specific things.

First, how to setup your own mod/content pack. You've already installed this framework mod and hopefully you also put it in the mods folder of your game. Now you create a new folder that will be your mod. In this folder, you need to create a content.json and a manifest.json file. If you already have a mods folder, potentially with a content pack for content patcher, then you need to create another subfolder and there you put the files in. Here is an example how my Mod Structure looks like:

Mods
> GoldenWalnutFramework
> Content Patcher
> Island Expansion
>> [CP] Island Expansion
>>> content.json
>>> manifest.json
>> [FTM] Island Expansion
>>> content.json
>>> manifest.json
>> [GWF] Island Expansion
>>> content.json
>>> manifest.json

(The [GWF] is not necessary for the folder's name, thats just how I did it). So, as you can see, I have the Main Folder Island Expansion and three Subfolders, one for Content Patcher, one for FarmTypeManager and one for my Mod, GoldenWalnutFramework. The only important thing is, that all your manifest.json files are 'on the same depth'. If in this structure above, I had one manifest.json that was not in the subfolder, but in the mainfolder, it would make all other manifest.json files invisible for SMAPI. 

So for the manifest.json, go on Stardew Valley Wiki and look up 'Modding'. This will lead you to the Modding Index page. On this page, you can look up, what needs to be written into a manifest.json. The only thing you need to add is this:
"ContentPackFor": {
    "UniqueID": "ResoNight.GoldenWalnutFramework"
  }

Now you can start the game. If you do not see the error "content.json has not been found", then my framework is successfully recognizing your mod. This is everything about the setup. Now you can write into the content.json file.


So, for your content.json, there are 3 Main fields that you can use. "GoldenWalnuts", "ParrotUpgradePerches", "Settings". So your basic structure of the entire content.json would look like this:

{
	"GoldenWalnuts": {
	
	}
	"ParrotUpgradePerches": [
	
	]
	"Settings": {
	
	}
}

Notice that for the ParrotUpgradePerches, you use [] instead of {} like for the other two. I'll go through everything one by one, starting with the GoldenWalnuts field.


---Golden Walnuts---
Here is the basic structure for entries in the GoldenWalnuts field:
    "GoldenWalnuts": {
        "HereGoesYourHint": [
	    {
	        HereTheEntriesForTheFirstWalnut
	    },
	    {
	        HereTheEntriesForTheSecondWalnut
	    },
	    {
	        HereTheEntriesForTheThirdWalnut
	    },
	    ...
        ],
	"HereGoesYourNextHint": [
	    {
		HereTheEntriesForTheFirstWalnutOfThisGroup
	    },
	    {
		HereTheEntriesForTheSecondWalnutOfThisGroup
	    },
	    ...
	],
	...
    }


While the structure might seem weird at first, trust me, this makes the most sense. As you can see, every Walnut that you add is a part of a group of Walnuts under a hint.

-Hint-
The hint is the hint that comes up when you right click the Parrot in the Island Hut in Island East. The Hint that you write here will be added to the pool of hints from the vanilla game. Keep in mind, every vanilla hint ends on ... (Like for example "5 buried in the north..."), in case you want to follow that. 
Another thing, if you want to add the amount of remaining Walnuts in the Hint, you can add the placeholder {0} in the Hint. So like this: "{0} buried in the north..."
The game will automatically replace the placeholder with the number left in that group, that the player hasn't collected yet. So if the player has 3 out of the 5 buried Walnuts in the north, the Parrot will say "2 buried in the north...". 
And keep in mind that you ALWAYS write {0} with the 0, NOT the amount of walnuts or anything else like that. The hint also supports the language token, just like the language token that Content Patcher uses. So, if you want to add multiple languages to your mod instead of just english, you can instead write the language token into the hint instead of the hint. To set this up, you must add a folder called "i18n" next to where your json files are. In this folder, you add a bunch of json files. First you add "default.json", which is the english version. Then you can add the files for all the languages that you want to add. For example in my case, I added de.json for german and ja.json for japanese (don't question my language skills). So instead of having a hint like this:

"{0} buried in the north...": [...]

You can write a hint like this:

"{{i18n:Island_N_Buried}}: [...]

What you see here is the basic token structure which is this: {{i18n:...}} and for "...", you put in the keyword that you will use in your language files. So in the case above, the keyword is "Island_N_Buried". Now in my default.json, I would write this:

"Island_N_Buried": "{0} buried in the north..."

And in my de.json:

"Island_N_Buried": "{0} vergraben im Norden..."

And in my ja.json:

"Island_N_Buried": "北に埋まっている{0}つ。。。"

If you assign the "Island_N_Buried" field in your default.json, but not in your ja.json, it will automatically look for it in the default.json, if it doesn't exist in the right language json. So don't worry if you have empty or partially empty language json files in that folder.

Sometimes though, when you use the token {0} and the player has only 1 remaining walnut, you might want to have a singular. So lets say you have a hint like this: "{0} Walnuts in the ocean in the west...", you would get this: "1 Walnuts in the ocean in the west...". The vanilla game actually avoids this issue by writing the hints in such a way that you don't have a difference in singular and plural. Like the one from before. 5 buried or 1 buried works both. But if you don't want to do this, you can also just assign a field called "Singular". The placement for this field is a bit odd, but I couldn't come up with a better solution. So, your Hint has the basic structure like this:
"Hint": [
    {
	//walnut1
    },
    {
	//walnut2
    }
]

But now you do this:

"Hint": [
    {
	"Singular": "SingularOfTheHint"
    },
    {	
	//walnut1
    }
    ...
]

So your first "walnut" is instead just the entry "Singular" with your singular form of the hint. Of course this field also supports the language token. So in practice, you'd have something like this:

"{0} Walnuts under the Ocean in the west...": [
    {
	"Singular": "1 Walnut under the Ocean in the west..."
    },
    ...
]

For the singular form, you can also use {0}, but it will obviously always replace it with 1, so writing 1 or {0} has practically no difference. That was the complicated half, the walnuts will get a bit easier.





For each invidividual walnut, there are a total of 14 different fields that you can assign. However, each type of walnut has different fields that can be assigned, so I will go through each type.

-Type-
As of this version, there are six Types that you can assign. "Bush", "Buried", "Fishing", "Stone", "MonsterLoot" and "Custom".

-Bush-
Example:
{
    "Type": "Bush",
    "Location": "IslandNorth"
    "X": 10,
    "Y": 10
}
For a Bush, you can only assign the 4 fields above. A Bush is also the only type that is not allowed to have an ID, due to how the game handles those kind of bushes. The ID will be automatically generated in this format: "Type_Location_X_Y" and in this case, the walnut's ID would be "Bush_Island_N_10_10". 

On the paths TileSheet, there is this tile for a WalnutBush. DO NOT use this tile, since it will successfully spawn a Walnut Bush, but without its walnut being registered properly. This Framework places the bush for you! One more thing, since this Framework also allows you to place bushes on non-island Maps, the bush needs different seasonal designs. For this reason, this Framework also provides seasonal bush designs that will always apply, unless the LocationContext for the map is set to Island. However, if you are unhappy with my designs, you can simply replace the images that you can find in the TerrainFeatures folder of this Framework Mod with your own ones.

-Buried-
Example:
{
    "ID": "{{ModID}}_Buried_Town_1",
    "Type": "Buried",
    "Location": "Town",
    "X": 40,
    "Y": 40,
    "Count": 5 //optional
    "Condition": "{{ModID}}_HasSeen_SecretNote_Island_N_1" //optional
}
You can see a couple of things here. First, the token for ModID. This will be explained below at the Settings field. How the buried Walnut works is pretty self explanatory. The only requirement is that the tile is actually diggable. If you assign a count, like, lets say 3, it will drop all 3 walnuts at once. If you do assign a count, the ID will be adjusted a bit. Read below at the settings field at automaticWalnutIDs (very useful). And the last thing you can see here is the Condition. In most cases, you wouldn't assign a condition at all I guess, but specifically for Secret Notes, it is very useful. This framework does not let you write custom Secret Notes, however, there is already a very well made Framework mod for specifically this case by Ichortower called Secret Note Framework. I am not the one to explain you how this framework actually works, but if you want to connect a Secret Note to a Walnut, you just have to add this field:

"ActionsOnFirstRead": [
    "AddMail Host {{ModID}}_HasSeen_SecretNote_Island_F_1 Received"
]

And this part: "{{ModID}}_HasSeen_SecretNote_Island_F_1" will be the one that you add as a condition for your walnut. Important things, since walnuts in general are shared across players, this whole Framework works though the Host/MasterPlayer only. So every MailFlag or condition or whatever you add, give and check for the MailFlags ALWAYS through the Host, that is why you see AddMail Host here. And at the end, the "Received" will cause the Host to instantly get this mail, so you must write Received there, not now, not tomorrow or whatever.

But in general, for the condition, you can just add any MailFlag that you want. So if you want to make it available after an event or something, just give the host the MailFlag that you put in here as the condition.

-Fishing-
Example:
{
    "ID": "{{ModID}}_Fishing_Mountain_1",
    "Type": "Fishing",
    "Location": "Mountain",
    "X": 75,
    "Y": 25,
    "Width": 5, //optional
    "Height": 5, //optional
    "ExtraTiles": [
	{
	    "X": 70,
	    "Y": 25,
	    "Width": 5 //optional
            "Height": 2 //optional
	},
	...
    ], //optional
    "Count": 5, //optional
    "Chance": 0.25, //optional
    "Condition": "WhenHostHasThisMailFlag" //optional
}
Here are a bunch of things that you can see. First of all, DropAtOnce unfortunately doesn't work for this Walnut type, since I am replacing the fish on your hook with a walnut, so I cannot really replace it with multiple walnuts at once. So lets get to the rest. Width and Height are optional and always default to 1 if not set. With X, Y, Width and Height, the rectangle on where you can fish the walnut is being set. If you need a more complex structure and a rectangle can't match the area that you want to assign, you can also assign the ExtraTiles field. This field is a list of multiple rectangles that you can assign. By using them, you can specifically set each tile where you want the walnut to be fishable. 
The chance is a number between 0 and 1 with 0.2 being a 20% chance, 0.05 being a 5% chance and so on. Just a little reminder for the chance, please don't make it too low, especially if you have a high count. This example here with 5 walnuts and 25% chance make a total of 20 throws on average that you need. However, especially if your mod is very popular, you will have people that need 80 or even 100 throws to just get those 5 walnuts. If you just have to fish at an ocean where the player normally is fishing, it's perfectly fine, but if you make the player fish 100 times at a small pond that only drops trash otherwise, it can get really frustrating. So, to be short, be a good game designer :) One more thing to the count, when the player got the last walnut, the game will play a little jingle, so that the player knows, that he got all walnuts here (it is the same jingle when you f.e. collect the last piece of coal of a quest or something). And for the condition, read above at the buried walnut.

-Stone-
Example: 
{
    "ID": "WhateverIDKItIsAUniqueID_Stone_Mountain_69",
    "Type": "Stone",
    "Location": "Mountain",
    "X": 80, //optional
    "Y": 0, //optional
    "Width": 999, //optional
    "Height": 999, //optional
    "ExtraTiles": [
	{
	    "X": 20,
	    "Y": 10,
	    "Width": 5 //optional
            "Height": 2 //optional
	},
	...
    ], //optional
    "Chance": 0.1, //optional
    "Count": 10, //optional
    "StoneTypes": [2, 4, 6, 8, 10, 12, 14, "BasicCoalNode0"], //optional
    "DropAtOnce": [2, 4], //optional
    "Condition": "WhenHostHasThisMailFlag" //optional
}
Stone Type Walnuts can take almost every field. (Oh yeah, if not clear, Stone Type walnut means, when you destroy a stone in a set area, it can drop a walnut, just like the MusselStone Walnuts on IslandWest). Width, Height, ExtraTiles, the Chance, Count and Condition are just like for the Fishing Walnut. Notice that you can exceed the border of the map without any issues (just no negative numbers please). If you omit X, Y, Width and Height, it will just take the entire Map. The field StoneTypes is unique to this Walnut Type. So lets say you have a quarry like the one at the mountain map. By assigning StoneTypes, you decide, what kind of stones can drop a Walnut. In the example above, the Stones with the ID 2, 4, 6, 8, 10, 12 and 14, as well as the Stone with the ID "BasicCoalNode0" can drop a walnut. The ID is referring to the entry of the Node in the Objects.json file. However, don't worry, you don't have to find out which ID is which specific stone. Just start the game and type "ShowStoneTypeIDs" into the SMAPI console. This will give you all the Stone Type IDs and some explanations, which stone specifically it is, because it is a bit annoying to find out, which ID refers to which stone. If you added your own Stone, you can also just add its ID. So f.e. if you add a "Walnut Stone Node" or something and you let this Node spawn in the Mountain Quarry, you can just write the ID of your added Stone into the StoneTypes field and then, only that Stone will have the chance to drop a walnut. If you don't know how to add custom Stones, go on my Channel ResoNight, I have an explanation video there. And last, the DropAtOnce field. This lets a Stone drop x Walnuts, with x being a number in between the two numbers you wrote in here. So if you've assigned DropAtOnce like above, a Stone will drop 2, 3 or 4 walnuts at once, if it got through the Chance check. Each count is equally likely. However, keep in mind that the assigned Count has absolute Priority. So lets say you assign the DropAtOnce Range [3, 3], but your Count is 10, then your first three Stones will drop 3 Walnuts and the last one will drop 1. This doesn't cause any issues at all, but just so you know that you might see less than expected Walnuts to be dropped.

-MonsterLoot-
Example: 
{
    "ID": "ThisIsAVeryUniqueID",
    "Type": "MonsterLoot",
    "Location": "IslandWest",
    "X": 1, //optional
    "Y": 1, //optional
    "Width": 999, //optional
    "Height": 1,  //optional
    "ExtraTiles": [
	{
	    "X": 20,
	    "Y": 10,
	    "Width": 5 //optional
            "Height": 2 //optional
	},
	...
    ], //optional
    "Count": 5, //optional
    "Chance": 0.2, //optional
    "DropAtOnce": [1, 5]
    "MonsterTypes": ["Sludge", "Spirit"], //optional
    "Condition": "WhenHostHasThisMailFlag" //optional
}
MonsterLoot Walnuts work pretty much the same way that Stone Type Walnuts work. There are only 2 things that are important. First, keep in mind that the area that you assign here is referring to where the Monster died, NOT where the monster spawned. So if you do assign an area, make the area very large or add some NPC Barrier Tiles to restrain the area where the monsters can be (it works that way because I cannot trace back where the Monster spawned that the player killed, so I must take the Monster's last position). And the other thing is obviously the MonsterType. If you just want a specific monster to drop the Walnut, then you can write this MonsterType into the MonsterTypes field. If you don't know the names of the monsters, open the Monsters.json file in the Content/Data folder. The names that this field needs are the entries on the LEFT side. It is mostly the same, but for example, in the game, the dust sprite from the frozen levels is actually called Dust spirit. (Is it a Typo from ConcernedApe? XD).

-Custom-
Example:
{
    "ID": "CustomIDForThisNut",
    "Type": "Custom",
    "Count": 3 //optional
}
This type exists so you can add a Walnut in any kind of way you want. A walnut with the type set to custom will just be functionally added into the game. You just have to do two things. You have to make a walnut obtainable in any way and you have to add the walnut to the Walnuttracker. How to make it obtainable depends on what you want to do. If you want to drop the walnut in any way, you should look into Game1.createItemDebris. To add the Walnut to the nuttracker, you should do the following:
Game1.player.team.collectedNutTracker.Add("CustomIDForThisNut");

If you want to give the player multiple walnuts for something, you can also add the Count field here as well. This would mean that you have to mark every one of those walnuts as collected. Walnuts that have a Count assigned to them will automatically create different unique IDs. Lets say you have a Walnut of whatever kind that has the ID "TestID" and the Count 3. Those three IDs will be this:
TestID_1
TestID_2
TestID_3

So each individual walnut gets a unique ID with an _ and a Count. This means, if you write an Entry for a custom Walnut that also has a count, you must not do this:
Game1.player.team.collectedNutTracker.Add("TestID");
You must do this:
Game1.player.team.collectedNutTracker.Add("TestID_1");
Game1.player.team.collectedNutTracker.Add("TestID_2");
Game1.player.team.collectedNutTracker.Add("TestID_3");
up to the amount of your count. If you're not dropping them at once, you might have to do a loop roughly like this:
for (int i = 1; i <= 5; i++)
{
    if (!Game1.player.team.collectedNutTracker.Contains($"TestID_{i}")
    {
	Game1.player.team.collectedNutTracker.Add($"TestID_{i}");
	break;
    }
}
As you can see, I really want to make sure that you make it right XD Whenever you have to look up an ID of a walnut, look below at Commands. And some important things to be careful about, please make sure that a Walnut cannot be dropped multiple times (focus ESPECIALLY on Multiplayer) and also please make sure that you add them properly. If you didn't add them properly, you can test that. Please look below at /requiredNuts at Commands.


That is about it with the Walnuts. Now let's go to the ParrotUpgradePerches.









---ParrotUpgradePerches---
(In case you are confused, a ParrotUpgradePerch is the Parrot sitting on a stick that you can give walnuts to make a change on the map, like opening a path)
For ParrotUpgradePerches (For short PUPs), there are several important things you need to know. Opposing to how the game does it, this Framework doesn't add the PUPs once per game, it adds them each time from a list when loading into a Savefile. This means, if the player completes a PUP, it will stay in the game as completed, but after a reload, it is completely GONE. The only thing that actually stays in the save data is the Mailflag that you get when you completed the PUP. This means, I always reinitialize the PUP, as long as the player didn't receive the Mailflag yet. This unfortunately also means, when you are testing and you saved a day with a completed PUP, the Mailflag will stay in the game's savefile, which makes the PUP with this specific ID never appear again. So if you need to remove a Mailflag again, the command RemoveMailFlag can help you. See below at Commands.

So, the fundamental structure of a ParrotUpgrade looks like this:

{
    "ID": "{{ModID}}_Island_Boulder_Removed",
    "Location": "IslandNorth",    
    "Nuts": 5,
    "ParrotTile": {
	"X": 15,
	"Y": 15
    },
    "ParrotArea": {
	"X": 20,
	"Y": 20,
	"Width": 6,
	"Height": 2
    }
}

These are all the areas that are required for a PUP to appear. In specific situations, you might want to leave it like that. Read below for Blank PUP. So first, let me explain each of those entries.

-ID-
The ID that you assign a PUP with, will be the Mailflag that the MasterPlayer, e.g. the Host (NOT the current player) receives. This means, you can also give other things this Mailflag as a condition, like for example if you want to make a shop sell a specific item AFTER you triggered a PUP, you could add the ID of the PUP as a condition. 
However, there is one cool trick. There is actually some unused code for a different kind of an animation. When you add the word "Volcano" (case sensitive) somewhere in your ID, you don't get the usual wooden sound with planks flying around, you get the parrots to make a pickaxe sound and some stone particles and an explosion sound to make it seem like they destroy something. So I would highly recommend to just add the word Volcano to a PUP, so you can see what I mean. This does feel a bit sketchy, yeah, but for now I'll leave it like this. Maybe in a future version, I will get into this, but now, if it works, it works XD. Another good sidenote, those MailFlags here will automatically be added as a condition for Mr Qis shop for the Walnut trade, so the player cannot sell the walnuts before completing every PUP. This also means that you don't have to worry about hiding too many walnuts that you don't have use for. They can just be sold after all PUPs have been completed.

-Location-
For the location, you can add your own Map's location or original map locations. For vanilla locations, keep in mind that the location names are NOT the names of the file. So, for example, the Map name for the file Island_S is actually IslandSouth. The location field is not case sensitive, so islandsouth would also work. If you don't know the name of the location, you can look it up in the file at Data/Locations in the Content folder (if you unpacked it, hopefully) or just on the Stardew Valley Wiki. If you have installed FarmTypeManager, go to the Map and type 'WhereAmI' into the console and it will show you. The check if the location you added actually exists happens after a save is loaded, not when the game is launched like all the other error checks. The Location field for the walnuts work exactly the same.

-Nuts-
This is just the amount of walnuts that activating the parrot requires.

-ParrotTile-
This should be the coordinates where your parrot is. Note that you have to add the stick, where the Parrot sits on, yourself. Use the coordinates of the bottom part of the stick bush thingy, not the top part where the parrot actually sits, or your parrot will be one tile too high. If you don't know what I mean, don't worry, you will see yourself when you are testing.

-ParrotArea-
This is the area where all the Parrots come down and start, I don't know, picking the wood or something. Most of the times, you just want to have them the same as your DestroyArea or ToArea (see below), but you can also just freely decide where exactly you want the Parrots animation to play.

-Blank PUP-
Those five entries above are the only one that are necessary for the PUP. If you only assign those fields, you will ultimately create a blank PUP. A blank PUP has the effect that it does the visuals and it adds the ID to the Mailflags that the MasterPlayer (Host) received. Meaning, with a blank PUP, you can freely decide what you want to do when the PUP has been triggered. This is an example of how your code could look like:

helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
{
    if (Game1.MasterPlayer.mailReceived.Contains("TheIDOfYourPUP")
	{
	    ... //This if exists so your code gets retriggered after reloading into the game
	}
    Game1.MasterPlayer.mailReceived.OnValueAdded += MailReceived_OnValueAdded;
}

private void MailReceived_OnValueAdded(string value)
{
    if (value == "TheIDOfYourPUP") 
	{
	    ...
	}
}

By doing this, you can trigger any effect you want as soon as the PUP has been triggered. (A little side note, the MailFlag will be added when the special soundeffect plays and the parrots start leaving, NOT the second you activate the Parrot). Of course, you can do this exact thing AND trigger a Map Override or destroy some tiles, this is just the backbone in case you want to do practically anything you want.

Now lets get into the optional fields:
{
    ...
    "DestroyAreas": [
	{
	    "X": 20,
	    "Y": 20,
	    "Width": 3,
	    "Height": 2,
	    "Layers": ["Buildings2", "Front-1"]
	},
	...
    ],
    "FromFile": "..\\[CP] Island Expansion\\assets\\Maps\\PUPOverrides.tmx",
    "FromArea": {
	"X": 0,
	"Y": 0,
	"Width": 5,
	"Height": 5
    },
    "ToArea": {
	"X": 20,
	"Y": 15,
	"Width": 5,
	"Height": 5
    },
    "Condition": "ThisIsAMailFlagCondition"
}

-DestroyAreas-
This will practically just destroy a rectangle at the coordinates at the given layers. You can assign multiple Areas, so you can destroy exactly what you want to. Depending on what you want to do, you might want to trick a little and instead of overriding the map, you could just destroy tiles instead. For example, lets say you want to open up a cave entry into a wall. Then you could just place the wall with the opening on the Buildings layer and the complete wall on Buildings2 and then you only destroy the Tiles on Buildings2, as well as one tile from the Buildings layer that blocked the entrance. This is often times much easier than completely overriding the map.
If you want a stoney destroying animation instead of a woody building animation of the Parrots, read above at ID

-FromFile-
This is where you assign the MapPath to your map that you want to use to apply a MapOverride. Keep in mind that Overriding the Map is always like doing PatchMode: Replace in Content Patcher. This means, it will fully remove all the tiles on all layers that your Override Map has. So you might have to just copy some tiles from the original area and put them into your Override Map's area. the This path should be relative to wherever your manifest.json is located at. Often times, especially when you also use content patcher, this means that you want to climb up in your Map path. to climb up, just use "..". Here is an example of a Mappath from my Mod that uses this Framework: "..\\[CP] Island Expansion\\assets\\Maps\\PUPOverrides.tmx". If you experience a delay when applying the Override, you have too many TileSheets in your map. Since this is a realtime Map Override, it is a bit sensitive and the more TileSheets your map for the override has, the bigger the delay. So, what I did, is, I just took all the TileSheets that are needed and I fused them into one TileSheet, so the game only has to load in a single TileSheet. This basically removes any delay completely. Maybe in the future when I know why the delay is so heavy, I might be able to make it so you don't have to worry about lag.

-FromArea-
This field requires a FromFile field. This is just the area of the Map that you use to make a Map Override. Similar to the FromArea for Content Patcher when you use the EditMap field. If you notice a delay when activating a PUP with a MapOverride, see below at FromFile.

-ToArea-
This is just the area where your override will be, just like the ToArea from Content Patcher.



-Condition-
You can also add one MailFlag as a condition. This will make the Parrot not appear as long as the MasterPlayer/Host does not have this MailFlag. You can use any Vanilla MailFlag or your own Mailflag, doesn't matter. You can also put the ID of another PUP in there, so this PUP's Parrot will only appear after the other has been completed (like the Mailbox and the Obelisk at the Island Farmhouse)

-General-
About ParrotUpgradePerches, there are a few general things you need to know. First of all, be careful to NOT softlock the player! You always have to imagine a player with a save where he already collected all 130 Walnuts, therefore he cannot get more Walnuts than you give him. So, if you lock an area behind a PUP, ALWAYS place the necessary amount of walnuts somewhere, where the player can collect them. For the DestroyAreas field and the Override fields, you can assign both things for one PUP. This means, you can Override a part of the map while also destroying a few tiles somewhere else. The Map override will always be executed at the very last, meaning, when you assign a Map Override and you also want to destroy a tile on this new map part for some reason, it won't work since the destroying always happens earlier. You can also assign none of those actions at all (see above at Blank PUP). That is about it with ParrotUpgradePerches.




---Settings---
Example:
{
    "EnableAutomaticWalnutIDs": true,
    "IgnorePathsWarning": true,
    "DisableWalnutCap": true,
    "SeparateHints": true,
    "ModID": "ResoNight.IslandExpansion",
    "DisableSeasonalFeaturesForMaps": ["Town", "Mountain"],
    "MailFlagsForUsedWalnuts": {
	"{{ModID}}_GotFountainWalnuts": 1
    }
}

Those are all the settings fields that you can assign. You don't need to add even one of those fields, so for example if you don't want to disable the Walnut Cap, you don't need to write "DisableWalnutCap": false, you can just, not write it. Most of the times, you will only have 2, 3 Settings anyways.

-EnableAutomaticWalnutIDs-
This is next to ModID the most useful command out of all of these. But this might make things a bit more confusing. So, there is a very structured way, how the game calls most of its own walnuts and I just mimic this structure. If you enable automatic IDs, you can omit the field for the ID for all the walnuts, except custom Walnuts. The automatically generated ID will be: "ModID_Type_Location_X_Y". If you've assigned a count, it will be: "ModID_Type_Location_X_Y_1", "ModID_Type_Location_X_Y_2", ... (for ModID, read below). Fortunately though, for custom walnuts, the only type of walnuts where you really have to work with the ID, you have to assign your ID yourself there. The ID of Bushes are always automatically generated and will always just be Bush_Location_X_Y, since the game needs this structure for the bushes to work. If you have a walnut where you didn't assign a X and Y coordinate, you'll also have to add an ID there. Also, if you assign an ID, it will always override any automatically generated ID.

-IgnorePathsWarning-
This one is very niche in its use case. If you want to destroy a Tile on the Paths layer, I just give you a fairly long explanation that explains to you why f.e. bushes or logs can't be removed that way. If you do destroy a tile on the Paths layer, you might want to set this to true, so the Player doesn't get my long ass console message.

-DisableWalnutCap-
The game's vanilla Walnut Cap is 130 and any walnut that is being collected after that won't be added to your counter. I decided to simply move this bar up to 130 + the amount of the added Walnuts, which means that I did NOT remove this limit. However, if you just want to test things or you want to give the player the opportunity to collect infinite walnuts, you can do this by setting this field to true. If you do give the player the option to collect infinite walnuts, you don't have to worry about all this ID stuff and adding an entry for every walnut and so on, this whole thing is just so rigid so the perfection tracker works properly. If you give the player infinite Walnuts, I guess you just accept a broken Count for obvious reasons XD Also keep in mind that this will disable the Walnut cap for ALL the mods that someone has simultaneously installed. But again, if you do give the player infinite walnuts, I guess you don't need to worry at all

-SeparateHints-
This one is a very useful one. So, as of now, when you don't have this field, your custom Hints will be mixed into the pool of the Vanilla Walnuts. However, if you set this field to true, your walnuts will land into a separate pool. Your hints will be shown right after the vanilla walnuts then. And additionally, this separate Pool has the difference that, when you collect all walnuts of today's hint, you will get a new Hint at the Parrot, opposing to the vanilla pool, where you have to sleep a day to get a new hint.

-ModID-
This is the most important setting out of all of these I would say. So, as you may know by know, this Framework supports the {{ModID}} token. The problem is, the ModID for your GoldenWalnutFramework Content Pack must be a different one than the one for your Content Patcher Content Pack. So my {{ModID}} gives you a different ModID than content patcher's {{ModID}}. So this field gives you the option, what {{ModID}} will be. If you assign this field, the token will just replace {{ModID}} with whatever you wrote into this field. So what you should do, you should go to your Content Pack that is for Content Patcher, open its manifest.json and copy its UniqueID. Then you paste this UniqueID into this ModID field. By doing this, you synchronize your Content Patcher's ModID with this ModID here.

-DisableSeasonalFeaturesForMaps-
This Framework automatically gives you seasonal Palm Trees and Walnut Bushes. They will be applied whenever the player is on a map neither has the Map Property LocationContext set to Island nor Desert. But if the seasonal Trees and Bushes are applied at a moment where you don't want this(for example indoor locations), just add those Maps into this field and my seasonal bushes and palmtrees won't apply there.

-MailFlagsForUsedWalnuts-
This one is a very important setting. The name of this setting is a bit weird, but trust me, I have been thinking wayyy too long about a better name, but I just go with this now XD So, you can add a custom way to spend walnuts, that is not a ParrotUpgradePerch, without any issues. The only thing is, I need to keep track of your actual Walnut count. So, if you do this, you can just add a MailFlag that you give to the player when the player spent those Walnuts. For example, in my Island Expansion Mod, I have a fountain. You can throw one walnut into there to trigger something, meaning I subtract the Walnut counter by one (btw, the WalnutCounter is at 'Game1.netWorldState.Value.GoldenWalnuts'. The one with .GoldenWalnutsFound is your totally collected amount). In order to make this not cause any mismatches, I added this field:

"MailFlagsForUsedWalnuts": {
    "{{ModID}}_GotFountainWalnuts": 1
}

The MailFlag {{ModID}}_GotFountainWalnuts is what I give the MasterPlayer right after someone threw a walnut into the fountain. The amount 1 after this refers to the amount of walnuts that I spent there. The number is important for me to keep track of your WalnutCount, the MailFlag will be added at the Qi Shop Trade where you can spend Golden Walnuts for Qi Gems. You can also assign the amount of 0, if you just want to add a MailFlag as a condition for this one trade at the Qi Shop. Idk, maybe you added a romance Mod and you also only want to make this trade appear after you have 10 hearts with Mr Qi or something. I just give you the options, do with it whatever you want.


---Commands---
There are a total of 6 commands that this Framework adds to the SMAPI Console. All of them can only be used after a save has been loaded. Commands are generally case insensitive, I only use uppercase letters here for visibility.

-ShowAllWalnutIDs-
This command lists you all the IDs for the walnuts that you added. Broken walnut IDs are excluded.

-ShowWalnuts-
This command lists you all the walnuts that the team has currently collected. Vanilla walnuts will also be listed.

-RemoveWalnut [walnutID]-
This will remove the walnut from the collected walnut list. This is only to remove any entry in the collectedNutTracker that you saved and do not want to have in there. The ID is case sensitive, so if you are unsure, just copy the ID after using the command ShowWalnuts. This will NOT reduce your walnut counter and it also cannot reset the state of Golden Walnut bushes. If you want to reset your total walnut counter, write /recountNuts in the ingame chat. Remember to save the day!

-ShowMailFlags-
This command lists you all the Mailflags that the player has received so far. In case you are confused about the word MailFlag, this is basically like the storage system of the game. Some MailFlags can be an actual Mail in game, but they don't have to be. This command will also list all of the vanilla MailFlags and all the MailFlags that other Mods added, since I cannot tell which are from you and which are not, so sorry if the list is long. 

-RemoveMailFlag [ParrotUpgradePerchID]-
This command lets you remove any MailFlag that the MasterPlayer (Host) currently has. The idea of this command is to reset any completed ParrotUpgradePerch or generally just to remove any left-over ParrotUpgradePerch IDs (A completed ParrotUpgradePerch means that its ID will be saved as a MailFlag). However, you can technically remove any MailFlag with this, but I cannot guarantee for anything if you decide to remove any other MailFlag from any other source. Remember to save the day!

-ShowStoneTypeIDs-
This one is a bit of a special one, this is useful if you are adding the field StoneTypes for a Walnut and you need to know which stoneType is actually which stone. So this gives you a list and some explanations, which stone is which.



---Helpful smapi commands---
There are a bunch of commands provided by smapi that are super useful for testing. The most useful command that you should use is:

debug item 73
This is the ID for the golden walnut. This gives you one walnut and if you want to get more at a time, just put the number behind it. F.e. you can do
debug item 73 50
to get 50 of them at a time. Keep in mind, the game initially had a cap at 130 walnuts and after this, you were unable to increase your walnut counter at all. For now, I decided to move this limit up to the total amount of walnuts, I did NOT remove this limit. This means, if you are testing and you accidentally obtained and spent too many walnuts already, you can use a command from the vanilla game actually. To use it, you have to open the ingame chat (usually by pressing T) and then type:
/recountNuts
This will reset your total Walnut count and recalculate your actual amount. This will remove all the walnuts you obtained via debug item 73 and then you can do this command again to get more walnuts. And just in general, this is good to reset your walnutcount. One little site note, if you use this command in vanilla without any mods on a perfect game, you will realize that it gives you 14 walnuts back. This is because the game actually doesn't count the walnuts you spent at Mr Qi at all and it just gives you all of them back. So, if you are testing around and use this command, don't get confused when you have some walnuts more than you expected.

debug warp locationname x y
This will warp you to the location where you want to test stuff. A little trick for the locationname, smapi will look for any location that has this sequence of letters somewhere in a location. This means, if I want to teleport to f.e. the golden Parrot on islandNorth, I actually just type:

debug warp ndn 14 15
The coordinates are not necessary, so to get quickly back to the farmhouse, just:
debug warp rmh

debug save
This will just give you a sound effect that it turned off. If you reuse this command, it will activate saving again and play a different sound effect. So, this command is useful to not save any day that you sleep through, if you need to test something where you need to skip a day for.

I would also recommend to go and read at the Stardew Valley Wiki what else debug commands you can use.


That is about it, I hope that now you are ready to start your mod. You can always come back and read again some things, if you are confused, you might want to watch the video "How to use Golden Walnut Framework for Stardew Valley" by ResoNight, if this video is easier to follow for you.


