# Golden Walnut Framework (GWF)

This is a Framework Mod that lets you add custom Golden Walnuts and Parrot Upgrade Perches. You can add Walnut Bushes, bury Walnuts, drop them when you destroy a Stone and much more.
And you can add new Parrot Upgrade Perches (The Parrots that sit on a stick and where you can trade Walnuts for a Map change) and trigger Map Overrides, destroy some tiles or anything else.


## Contents
* [Installation](#installation)
* [Setting up your Content Pack](#setting-up-your-content-pack)
* [General](#general)
* [GoldenWalnuts](#goldenwalnuts)
  * [Unique Key](#unique-key)
    * [Hint](#hint)
    * [Singular](#singular)
    * [SeparateHint](#separatehint)
    * [ShowThisHint](#showthishint)
    * [HintConditions](#hintconditions)
    * [Walnuts](#walnuts)
      * [ID](#id)
      * [Type](#type)
        * [Bush](#bush)
        * [Buried](#buried)
        * [Fishing](#fishing)
        * [Stone](#stone)
        * [MonsterLoot](#monsterloot)
        * [Custom](#custom)
      * [Location](#location)
      * [X and Y](#x-and-y)
      * [Areas](#Areas)
      * [Chance](#chance)
      * [Count](#count)
      * [DropAtOnce](#dropatonce)
      * [StoneTypes](#stonetypes)
        * [How to add custom Stones](#how-to-add-custom-stones) 
      * [MonsterTypes](#monstertypes)
      * [Conditions](#conditions)
* [ParrotUpgradePerches](#parrotupgradeperches)
  * [ID (PUP)](#id-pup)
  * [Location (pup)](#location-pup)
  * [Nuts](#nuts)
  * [ParrotTile](#parrottile)
  * [StickType](#sticktype)
  * [ParrotArea](#parrotarea)
  * [DestroyAreas](#destroyAreas)
  * [FromFile, FromArea, ToArea](#fromfile,-fromarea,-toarea)
  * [StoneAnimation](#stoneanimation)
  * [Condition](#condition)
* [Settings](#settings)
  * [WalnutShops](#walnutshops) <- this one is important
  * [DisableWalnutCap](#disablewalnutcap)
  * [DisableSeasonalFeaturesForMaps](#disableseasonalfeaturesformaps)
* [Console Commands](#console-commands)
  * [ShowWalnuts](#showwalnuts)
  * [RemoveWalnut](#removewalnut)
  * [ShowMailFlags](#showmailflags)
  * [RemoveMailFlag](#removemailflag)
  * [ShowAllWalnutIDs](#showallwalnutids)
  * [ShowAllStoneTypeIDs](#showallstonetypeids)
  * [/recountNuts](#recountNuts)
* [GameStateQueries](#gamestatequeries)
* [Example File](#example-file) 

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Golden Walnut Framework** from [GitHub](https://github.com/ResoNight/GoldenWalnutFramework) or [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/???/)
3. **Unzip Golden Walnut Framework** into your `Stardew Valley\Mods` folder.

## Setting up your Content Pack
1. **Setup a Content Pack for ContentPatcher** (see [Author Guide for Content Patcher](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md#readme))
3. **Add this field in your content.json:**
```
{
    "Action": "EditData",
    "Target": "Mods/GoldenWalnutFramework/Data",
    "Entries": {
        //Here goes all the entries
    }
}
```


One small tip, you might want to have all your content in a separate file, so you can write something like this into your content.json:
```
{
    "Action": "Include",
    "FromFile": "code/GoldenWalnuts.json" //the path to your new file, relative to the content.json file.
}
```
and then you can write all your entries into that separate file.

## General
Now you can start writing stuff into the **Entries** field. There are a few things I want to mention before you jump into adding your new entries. The [Contents](#contents)
field above roughly matches the structure that your entries will have. All entries will be checked when you start the game and they will be checked again, after you load into a save. The check for your [Location](#location) entries can only happen after loading into a save. If you are adding/changing entries in your content.json, you don't need to fully close the game and reopen it. You can just exit the save and re-enter it. You will get a little warning into your SMAPI console, when you add or remove a [Walnut Group](#hint), but don't worry, your collected Walnut Count will just be off for that session. After restarting, it will always be correct. Now lets get started with **GoldenWalnuts!**

## GoldenWalnuts
The Basic structure for Golden Walnuts looks like this:
```
"GoldenWalnuts": {
    "UniqueKey1": {
        "Hint": "...",
        "Singular": "...",
        "SeparateHint": true/false //optional
        "ShowThisHint": true/false //optional
        "Walnuts": [
            {
                //Entries for the first walnut
            },
            {
                //Entries for the second walnut
            }
            ...
        ]
    },
    "UniqueKey2": {
        ...
    }
    ...
}
```


## Unique Key
This is just a unique key that you must assign, that stands for one walnut group. Make this as unique as possible to avoid conflicts with other mods. Using the `{{ModID}}` token is strongly advised (see [ModID in Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#ModId)).

## Hint
The Hint is what you will see when you right click the Parrot in the Island Hut on IslandEast. Using the [Language Token](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#i18n) is strongly advised, even if you are not planning to add other languages. Using this token makes it possible for other people to add a translation for your mod. For simplicity, the rest of this Guide will not use this token. The vanilla game writes the hints in a quite unique way, that you might want to follow. There are two ways how a Hint can
be written. The first way is, you just write the hint and no matter how many Walnuts under this hint are remaining, it will say the same thing. But you
can also let the Parrot say the amount of remaining walnuts by adding `{0}` somewhere in the hint. So for example, if there are 5 Walnuts in a group
that you haven't collected yet, then this: 

`{0} buried in the north...` 

would automatically turn into this, when talking to the Parrot: 

`5 buried in the north...` 

***Important!*** always write `{0}` with a 0, ***NEVER*** any other number than that!

For those hints in specific, concernedApe always wrote them in a way so that the **singular** and **plural** form
are the same. So he never wrote f.e. `{0} Walnuts in the Ocean...`, since this could say `1 Walnuts in the Ocean...`. However, I just added a way so you can
write a [singular](#singular) form that will be shown instead, when you have 1 Walnut remaining of this group. One last thing, don't make your hints too long or they might go **off screen**! To test how a Hint looks on the screen, you can set [ShowThisHint](#showthishint) to true for one hint.

## Singular
If your [Hint](#hint) contains a `{0}` and you want to have a singular form of the string, when the player has only one remaining Walnut in this group, you can assign the Singular field. Using the [Language Token](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#i18n) is strongly advised. This field technically also supports `{0}`, but this will always be replaced with 1, since this will only be shown if you have 1 walnut remaining.

## SeparateHint
There are two separate pools of [Hints](#hint) that work a bit differently. There is the vanilla pool, where there is one hint per day. This is where your Hints usually get put into. However, when you set "SeparateHint": true, your Hint will be shown **after** a Hint from the main pool. And for the separate pool, whenever the player has collected all Walnuts from the hint for the day, another hint will be generated, instead of the Parrot not saying anything at all for the rest of the day. It is generally recommended to assign this for either none or all of your hints, but you don't have to.

## ShowThisHint
This is a specific setting, just to test things. When you set "ShowThisHint": true for a hint, the Hint of the first day when you load into a save will be overwritten by the hint for that you assigned this field. The hints for the coming days will work as usual. You can assign this field for only one hint at a time. Once for a Hint from the normal pool and once for a Hint that has [SeparateHint](#separatehint) set to true. DO NOT forget to remove this field before uploading your Mod!

## HintConditions
This field lets you set a [GameStateQuery](#gamestatequeries) as a condition, after that the [Hint](#hint) can appear at the Parrot in the hut. For example like this:

`PLAYER_HAS_MAIL Host ThisIsAMailFlagYey Received`

This field works just like the [conditions](#conditions) field for walnuts, I hope that the naming just makes things less confusing. It is a usual "Conditions" field like most other mods. Also all Walnuts within this group will automatically have this as a condition as well, so the player **cannot** collect Walnuts of still unavailable hints. But one important thing is, while it might sound odd, I would probably not recommend using this field. First of all, the player is most likely not aware of even the possibility of conditional hints. So depending on what you are doing, you should maybe think of telling the player in some way. And also, when a player is getting no hints anymore but has missing walnuts, it can be a little frustrating, because it is literally the point of the hint to tell the player where the rest is. So I guess one good way to use this field would be for example, you add an event with the wizard and he spawns in 100 new walnuts in the valley and then you give all your hints this event as a condition. Or maybe you could make a questline where you let the wizard say something like "after you found 30 Walnuts, I will hide some more for you". Just something where the player would naturally know that there are new hints/obtainable Walnuts now. You also have to keep in mind that this only gives your Hint the *chance* to appear, it will not appear for sure, since you have to keep in mind that both [Hintpools](#separatehint) are shared by all mods. So for example while you could chain Hints together using the added "COMPLETED_WALNUTGROUP xy" [GameStateQuery](#gamestatequeries), you must keep in mind that the player might have to wait multiple days before he can get your new hint. Also the player doesn't have to actually see your hint for the Walnuts to become obtainable, which could also lead to some unintuitive situations. So, to summarize all this, you should really try to be a good game designer and think, what feels natural and what is fun for the player. This field is bad gamedesign most of the times, but it *can* be used well, if you really think how to use it properly.

## Walnuts
For each Walnut, there are those fields:
Field|Value|Description
-----|-----|-----------
[ID](#id) | string | A unique ID for a Walnut
[Type](#type) | string | The type can either be [Bush](#bush), [Buried](#buried), [Fishing](#fishing), [Stone](#stone), [MonsterLoot](#monsterloot) or [Custom](#custom)
[Location](#location) | string | The Location of the Walnut
[X](x-and-y) | int | The X-Coordinate of the Walnut
[Y](x-and-y) | int | The Y-Coordinate of the Walnut
[Areas](#Areas) | List with different elements (see at [Areas](#Areas)) | Areas in that the Walnut is obtainable
[Chance](#chance) | float | The chance for the Walnut to drop. The number must be between 0 and 1.
[Count](#count) | int | The amount of walnuts that can be dropped from this Walnut entry. (assigning a Count will change the [ID](#id))
[DropAtOnce](#dropatonce) | int or string | The amount of walnuts that will be dropped at once. Cannot exceed the given [Count](#count)
[StoneTypes](#stonetypes) | [int or string, int or string, ...] | if assigned, only the given StoneTypes can drop a Walnut. To get a list of all possible values, use the [Console Command](#console-commands) [ShowStoneTypeIDs](#showstonetypeids). Supports custom Stone Types
[MonsterTypes](#monstertypes) | [string, string, ...] | if assigned, only the given MonsterTypes can drop a Walnut. To get a list of all possible values, look at the [Monsters](https://stardewvalleywiki.com/Modding:Monster_data#Monster_IDs) page on the Wiki. The entries on the **right** side are possible values
[Conditions](#conditions) | string | a [GameStateQuery](#gamestatequeries) after that the Walnut becomes obtainable, for example after reading a [Secret Note](#conditions)

Aside from the [ID](#id), the [type](#type) field is also always **mandatory**. Each Walnut type has a different set of possible fields that you can assign, for example a [fishing](#fishing) walnut can have the field [areas](#areas), whereas a [buried](#buried) walnut can only have direct coordinates [x and y](#x-and-y).

## ID
The ID that you assign here is the ID that will be added under `Game1.player.team.collectedNutTracker`. IDs should be assigned as unique as possible, so using the `{{ModID}}` token is strongly advised (see [ModID in Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#ModId)). If you assigned a [Count](#count) to a Walnut that is 2 or higher, each individual Walnut will automatically have its own ID. So if you have, lets say, Count set to 3 and your ID is "TestID", the IDs of those Walnuts will be: `TestID_1`, `TestID_2` and `TestID_3`. This is especially important if you assign a Walnut with the [Type](#type) [Custom](#custom), since you need to add the right IDs to your collectedNutTracker.

## Type
There are a total of 6 Types that a Walnut can have. Each type has different fields that it **must** have, **can** have and **cannot** have. I will go through them one by one.

# Bush
![Indoors Bush](docs/images/Example_Bush_Indoors.png)

Possible Fields|Status
---------------|------
[Type](#type) | required
[Location](#location) | required
[X](#x-and-y) | required
[Y](#x-and-y) | required
[Conditions](#conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_Sunroom_Bush"
    "Type": "Bush",
    "Location": "Sunroom",
    "X": 3,
    "Y": 7
}
```

A **Bush** must always have those fields and cannot have any other than that. On the Paths TileSheet when creating maps, there is a tile that lets you spawn in Walnut Bushes. ***DO NOT USE THIS!*** Adding a Walnut with Type Bush will spawn it in automatically. When you place them yourself, the Framework cannot keep track of them! What you can do though is place the tile with index 7 from the paths TileSheet on the Paths layer (see [Paths Layer](https://stardewvalleywiki.com/Modding:Maps#Paths_layer) on the Wiki). This is a tile that does not have any effect at all, so it is good for yourself to keep track, where you placed Bushes. Bushes do also support the [Conditions](#conditions) field, however, they cannot spawn in as soon as you meet those conditions, since checking every tick would be wayyy too much. Therefore, everytime you enter a location, the game will check for the Conditions of the Bushes in that area and therefore, the Bushes will spawn. You might want to keep that in mind, depending on how you use the Conditions field. Also, keep in mind that once a Bush has been spawned, it will not disappear, when the player doesn't meet the conditions anymore (So for example `TIME 600 900` will not make the Bush despawn after 9 am). One more thing, just like normal Bushes, spawning a Bush once would normally let it stay in the save file permanently. However, you don't have to worry about that. You will occasionally see a `x Bushes removed` in the console, since GWF automatically removes any Walnut Bushes that have been placed using the framework, but don't have any matching current entry.

# Buried
![Buried in Town](docs/images/Example_Buried_Town.png)

Possible Fields|Status
---------------|------
[ID](#id) | required
[Type](#type) | required
[Location](#location) | required
[X](#x-and-y) | required
[Y](#x-and-y) | required
[Count](#count) | optional
[Conditions](#conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_Buried_Town_01"
    "Type": "Buried",
    "Location": "Town",
    "X": 25,
    "Y": 51
}
```

A **Buried Walnut** works pretty much exactly how you think it would work. If you assign a Count, all those Walnuts will be dropped at once. Especially for buried walnuts, the *Conditions* field might be very useful to f.e. only let a walnut be dropped *after* the player has read a **Secret Note**. GWF does *not* provide a framework for Secret Notes, so see below at [Conditions](#conditions). Keep in mind that the tile for your walnut must be *diggable*!

# Fishing
![Fishing at Log in Mountain](docs/images/Example_Fishing_Log.png)

Possible Fields|Status
---------------|------
[ID](#id) | required
[Type](#type) | required
[Location](#location) | required
[X](#x-and-y) | optional
[Y](#x-and-y) | optional
[Areas](#areas) | optional
[Count](#count) | optional
[Chance](#chance) | optional
[Conditions](#conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_Fishing_Mountain_Log",
    "Type": "Fishing",
    "Location": "Mountain",
    "X": 66,
    "Y": 31,
    "Width": 6,
    "Height": 6,
    "Chance": 0.25,
    "Count": 3
}
```

For Fishing type Walnuts, you can either assign [X and Y](#x-and-y) for one specific tile or you can use the [Areas](#areas) field to assign a larger area. See below for more details on how to use the [Areas](#areas) field. If you use neither, the area will just be the whole map. If you assign a [Count](#count) and the last Walnut is being fished, the game will play a small soundeffect, so that the player knows, that he got all walnuts of one entry. The [DropAtOnce](#dropatonce) feature unfortunately does not work for **Fishing** type walnuts, since you are actively fishing one walnut instead of x walnuts being dropped into the world. For the [Chance](#chance), please keep in mind that the player is fishing pretty slowly and you can only fish one at a time. So whereas a 0.05 chance for a [Stone](#stone) type Walnut in a larger quarry would be perfectly reasonable, a 0.05 chance for fishing, especially if you assign a Count like 5, would be terrifyingly frustrating. So, in short, think of what you are doing and always think of the unlucky ones :) You can also assign Conditions (see below at [Conditions](#conditions)).

# Stone
![Stone in MountainQuarry](docs/images/Example_Stone_Mountain.png)

Possible Fields|Status
---------------|------
[ID](#id) | required
[Type](#type) | required
[Location](#location) | required
[X](#x-and-y) | optional
[Y](#x-and-y) | optional
[Areas](#areas) | optional
[Count](#count) | optional
[DropAtOnce](#dropatonce) | optional
[Chance](#chance) | optional
[StoneTypes](#stonetypes) | optional
[Conditions](#conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_MountainQuarry",
    "Type": "Stone",
    "Location": "Mountain",
    "Chance": 0.05,
    "Count": 10,
    "DropAtOnce": "1/3"
}
```

This type causes Stones to drop Walnuts if you break them in any way, just like the MusselStones on IslandWest. You can either assign [X and Y](#x-and-y) for one specific tile or you can use the [Areas](#areas) field to assign a larger area. See below for more details on how to use the [Areas](#areas) field. If you use neither, the area will just be the whole map. If you assign a Count, the game will play a soundeffect whenever the player collects the last walnut from one entry. For Stones, you can also assign the [DropAtOnce](#dropatonce) field. Whenever the Stone is going to drop Walnuts, it will drop a random amount of walnuts between your left and right number. So in the case of the example, a stone will drop 1, 2 or 3 walnuts at once (See [DropAtOnce](#dropatonce) for more details). There is also the field [StoneTypes](#stonetypes) which, when set, only lets the given Stones drop Walnuts, including Custom Stones (see below at [StoneTypes](#stonetypes). For the [Chance](#chance) field, you should really think about how you are going to set this. You have to consider the size of your quarry, the amount of stones that can drop walnuts, the amount that can be dropped at once and the bad luck of some players. For example my example from above was actually not that good. Upon testing, in 10 out of 10 cases with a full quarry, I got the 10 Walnuts, often with the very first bomb. So for above's example, I would completely leave out DropAtOnce and then I would say it would be decently balanced. So maybe you want to go out and just test, what Chance you want to set. This Walnut Type also supports the Conditions field (see below at [Conditions](#conditions)).

# MonsterLoot
![Slime at Moonscythe Island](docs/images/Example_Slime_MS_Island.png)

Possible Fields|Status
---------------|------
[ID](#id) | required
[Type](#type) | required
[Location](#location) | required
[X](#x-and-y) | optional
[Y](#x-and-y) | optional
[Areas](#areas) | optional
[Count](#count) | optional
[DropAtOnce](#dropatonce) | optional
[Chance](#chance) | optional
[MonsterTypes](#monstertypes) | optional
[Conditions](#conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_Moonscythe_Island_MonsterLoot",
    "Type": "MonsterLoot",
    "Location": "{{ModID}}_Moonscythe_Island",
    "Count": 5,
    "DropAtOnce": 3,
    "Chance": 0.1,
    "MonsterTypes": ["Sludge"]
}
```

To make it short, this Type works basically *exactly* like the [StoneTypes](#stonetypes) walnut. Keep in mind, the area that you assign with the [Areas](#areas) field refers to the last tile on which the monster has been killed, **NOT** where you spawned the monster (since I cannot trace back where a monster has been spawned). You can also specify, which kind of monsters can drop Walnuts by using the [MonsterTypes](#monstertypes) field. One more thing, I hope this is already clear, but if you f.e. spawn in monsters using [FarmTypeManager](https://github.com/Esca-MMC/FarmTypeManager), ***DO NOT*** add a Walnut as loot. This framework handles the loot on its own, you don't need to add it

# Custom
![Fountain at Farming Island](docs/images/Example_Fountain_F_Island.png)

Possible Fields|Status
---------------|------
[ID](#id) | required
[Type](#type) | required
[Count](#count) | optional

Example:
```
{
    "ID": "{{ModID}}_F_Island_FountainWalnuts",
    "Type": "Custom",
    "Count": 5
}
```

If you want to give the player Walnuts in any other way than the options from above, you can do this (This is C# territory). By adding a Custom Type walnut, you can synchronize your walnut with the whole walnut calculation and hint system. So, lets say, you add a fountain that gives you 5 Walnuts, if you throw a specific item in there. The whole item throwing in is your job. If you want to let a walnut drop on the ground, you can use something like this:
```Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(Xf, Yf) * 64f, Game1.random.Next(4), null);```
The item 73 is the golden Walnut. The vector is the pixels. One tile contains of 64*64 pixels from the mechanical perspective, so multiplying that vector by 64 gives you the Tile. This means, you can also let a Walnut drop at half a tile or pretty much wherever you want. Game1.random.Next(4) gives you a random number between 0 and 3, which are the 4 directions. So if you want the Walnut to be dropped to the top (like in the fountain image above), you would want to enter 0 (0 is up, 1 is right, 2 is down, 3 is left). the null is the location, which defaults to Game1.player.currentLocation. So normally null works fine, but if you let the walnut being dropped through the host for example, you might want to enter something else there. So this lets you drop in a Golden Walnut. When you don't want to drop it on the floor, you need to know that, when the game adds a Golden Walnut to your inventory, it instantly deletes it again and increases your Walnut Count by 1. So adding a Walnut to the Inventory in any way will increase the collected amount automatically. However, the Walnut itself does not contain any data like an ID whatsoever. Actually, the Walnut itself never has an ID or something, the game just drops a Walnut and *simultaneously* mark it as collected. So, theoretically, if you drop a walnut and somehow manage to not collect it (which is usually basically impossible), the game actually marks it as collected, even though you haven't collected the Walnut. So because of this, you can just drop the walnut like above and then you have to mark the Walnut as collected. To do this. lets take this entry as an example:

```
{
    "Type": "Custom"
    "ID": "{{ModID}}_CasinoPrize"
}
```
And lets say your ModID is this: `ResoNight.IslandExpansion` (Using {{ModID}} is not technically necessary, but ***strongly*** advised), then you would have to mark the Walnut as collected by doing this:
```Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_CasinoPrize")```
But this is only the right way if you have no [Count](#count) assigned. If we go back to my example from above:
```
{
    "ID": "{{ModID}}_F_Island_FountainWalnuts",
    "Type": "Custom",
    "Count": 5
}
```
you have to keep in mind that the [ID](#id) for the Walnut is slightly getting changed. The required IDs are always this:
```
ID_1
ID_2
ID_3
... //up until the Count
```
Therefore, in the case of my fountain example, I do this instead:
```
Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_FountainWalnuts_1")
Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_FountainWalnuts_2")
Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_FountainWalnuts_3")
Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_FountainWalnuts_4")
Game1.player.team.collectedNutTracker.Add("ResoNight.IslandExpansion_FountainWalnuts_5")
```
or in short:
```
for (int i = 1; i <= 5;  i++)
{
    Game1.player.team.collectedNutTracker.Add($"ResoNight.IslandExpansion_F_Island_FountainWalnuts_{i}");
}
```
Keep in mind, if you are doing a loop like this, you should let i go from 1 to 5, not 0 to 4 (you could also write i + 1 in the ID if you like that more). This would mark all of the Walnuts as collected at once. But of course, sometimes you do not want to add all of them at once. Lets say you have a shop, where you can buy Golden Walnuts and lets say this trade exists 5 times. Then you would have a JSON entry like this:
```
{
    "ID": "{{ModID}}_CasinoTrade",
    "Type": "Custom",
    "Count": 5
}
```
How you do the shopping part itself is on your side. Important is, when someone buys a Walnut (which automatically itself tries to put it into the inventory, where it increases the current amount already on its own), you should do a loop like this:
```
for (int i = 1; i <= 5; i++)
{
    string id = "ResoNight.IslandExpansion_CasinoTrade_" + i;
    if (!Game1.player.team.collectedNutTracker.Contains(id)
    {
        Game1.player.team.collectedNutTracker.Add(id);
        break;
    }
}
```
As you can see, I really want to make sure that you do it the right way XD. If you ever need to look up the Walnuts that the player currently has, you can type [ShowWalnuts](#showwalnuts) into the SMAPI console and you get a list of them. The rest is on you. As long as you properly mark the Walnuts as collected in the **NutTracker** and you make sure that even in multiplayer, a walnut cannot accidentally be dropped multiple times (since you wouldn't get any warning or error of any kind for that), everything should be fine. Please also read [WalnutShops](#walnutshops) below. Now we are through with all the possible values for the [Type](#type) field. Now we can go on with the other fields.

# Location
For the location field, there are a few things to keep in mind. First, whenever you start the game, you can already see the error logs for your entries. But the location field can only be checked after you loaded into a save, since GWF needs to check for existing locations. For the location field, you need to add the name that the in-game location has, NOT the name of its file. So for example there is the file `Island_N`, but the location is named `IslandNorth` and you need to write *IslandNorth* into the locations field. If you don't know the name of a location, there are a few ways to find the name. First, you can go into the Data/Locations.json and actively look for the name in there. But this can be a bit annoying sometimes. So if you have the Mod FarmTypeManager installed, you can just go to the location and type `whereami` into the console and it will tell you the name of the location. Or probably the easiest way, you look at this [table](https://stardewvalleywiki.com/Modding:Location_data#Location_names) from the Wiki. Everything about the location entry is similar for the Locations field for [Golden Walnuts](#goldenwalnuts) and for [Parrot Upgrade Perches](#parrotupgradeperches).

# X and Y
Those two fields are pretty self-explanatory. They assign one specific tile. For [Bushes](#bushes), this is the left tile of the Bush.

# Areas
This field lets you add multiple areas in that a Walnut can be obtained. The entries of this field must look like this:
```
"ExtraTiles": [
    {
        "X": 23,
        "Y": 48,
        "Width": 3,
        "Height": 2
    },
    {
        "X": 22,
        "Y": 47,
        "Width": 2
    },
    ...
]
```
For each {}, you can assign the [X and Y](x-and-y) coordinates and optionally also the [Width and Height](#width-and-height). If one of those or both are omitted, they just default to 1.

# Chance
The chance is a number between 0 and 1. Genuinely think about what chance you can assign, it can quickly get frustrating, if someone is unlucky. And if many people play your mod, there *will* be unlucky people. For Fishing walnuts especially, you shouldn't make the chance too low, because you have to actively spent a ton of ingame time to get a chance one by one, whereas for monsters and stones, you can quickly go in, kill them once or destroy them once and then you got your chance for today, so fishing is generally much more frustrating than the other types.

# Count
If you assign a Count to a Walnut, the ID gets changed a bit. Instead of one walnut having the [ID](#id) that you assigned (or that is automatically generated (see below at [AutomaticWalnutIDs](#automaticWalnutIDs)), each walnut gets its own individual ID, that is simply ID_1, ID_2, ID_3 and so on, up until your Count. The Count that you assign always determine the upper limit of how many walnuts you can get from this Walnut entry (So the field [DropAtOnce](#dropatonce) cannot exceed this limit)). For [Buried](#buried) Walnuts, all Walnuts will be dropped at once. For each other Walnut, you can get the Walnuts one at a time. If the player collects the last Walnut of a Walnut entry, the game will play a little soundeffect ("jingle1" if you are interested). Setting the Count to 1 is completely pointless, just don't. It will *NOT* change the ID to ID_1 in that case.

# DropAtOnce
A valid entry for this field must look like this:

`"lowerBorder/upperBorder`

So for example this:

`"1/3"`

But it can also be just a single number, if you don't need a range. The [Count](#count) field always has priority over the DropAtOnce field. This means, if you were to assign for example this: `3` and your Count is 10, the first three stones or monster or whatever would drop 3 Walnuts and the last one would drop 1.

# StoneTypes

If you assign this field, only the given **StoneTypes** can drop a Walnut in the area you assigned, instead of every stone in an area being able to drop a Walnut. Possible values are integers and strings, since the ID of some stones are strings. An example entry would look like this:

`[2, 4, 6, 8, 10, 12, 14, "{{ModID}}_ExpStoneNode"]`

These IDs are all the gem stones as well as a custom Stone that I used for example. The IDs are the entries in the `Data/Objects.json` file from the game. If you want to know, which Node has which ID, just type [ShowStoneTypeIDs](#showstonetypeids) into the smapi console and GWF will list you all of them as well as some minor explanations, which is which. I just added this because it is a bit annoying to find this out on your own.

# How to add custom Stones

This is normally something that lies outside of GWF, but I still want to briefly explain, how you can do it, since you have to do some workaround that I could only figure out with the help of **Esca**, the creator of the Framework Mod [FarmTypeManager](https://github.com/Esca-MMC/FarmTypeManager). So first of all, you need to add the Node as an Object into the Object.json. In your [Content Patcher](https://github.com/Pathoschild/StardewMods/tree/develop/ContentPatcher) Content Pack (what a sentence), you just do for example this:
```
{
    "Action": "EditData",
    "Target": "Data/Objects",
    "Entries": {
        "{{ModID}}_ExpStoneNode": {
            "Name": "Stone",
            "DisplayName": "ExpStoneNode",
            "Description": "ExpStoneNode",
            "Type": "Litter",
            "Category": -999,
            "Price": 0,
            "Texture": "LooseSprites/{{ModID}}_Objects",
            "SpriteIndex": 3,
            "ColorOverlayFromNextIndex": false,
            "Edibility": -300,
            "IsDrink": false,
            "Buffs": null,
            "GeodeDropsDefaultItems": false,
            "GeodeDrops": null,
            "ArtifactSpotChances": null,
            "CanBeGivenAsGift": true,
            "CanBeTrashed": true,
            "ExcludeFromFishingCollection": false,
            "ExcludeFromShippingCollection": false,
            "ExcludeFromRandomSale": false,
            "ContextTags": null,
            "CustomFields": null
        }
    }
},
{
    "Action": "Load",
    "Target": "LooseSprites/{{ModID}}_Objects",
    "FromFile": "assets/Others/Custom_Objects.png"
},
```

What you can see here, is, first, I add the entry to the Object.json. Then I load in the texture png file, so that the `"Texture"` field can find the texture. The important thing here is, make sure that the Field `"Name"` has the Value `"Stone"`. This makes the game recognize the object as a stone. This means, it can now be broken with a pickaxe and it has the default breaking animation. Unfortunately, this also defaults the stone's dropped item to a regular stone (or multiple of them). Maybe you can figure out, how to change/get rid of the normal stone drop. I couldn't. So I can just tell you how to add a drop and you do this by adding this line in your ModEntry:
```helper.Events.World.ObjectListChanged += World_ObjectListChanged;```
And then you do this function:
```
private void World_ObjectListChanged(object? sender, ObjectListChangedEventArgs e)
{
    foreach (var removedObj in e.Removed)
    {
        if (removedObj.Value.ItemId.ToString() == "ResoNight.IslandExpansion_ExpStoneNode")
        {
            Game1.createItemDebris(ItemRegistry.Create("ResoNight.IslandExpansion_ExpStone"), new Vector2(removedObj.Key.X * 64f, removedObj.Key.Y * 64f), Game1.random.Next(4));
        }
    }
}
```
Of course, you have to change the Item ID, that the if checks for, to the ID that you gave your own Stone and the ID of the Item that you want to be dropped. Of course you can also add a chance to this, this code just always drops 1 of this item, 100% of the time. So with all this, you added an Object that is breakable and that will drop an item of your choice. Now you need to actually spawn the Stone in. You can obviously also do this with C#, but if you want the whole Quarry spawning logic, [FarmTypeManager](https://github.com/Esca-MMC/FarmTypeManager) definitely makes your life easier. However, there is a problem. The field [Ore_Spawn_Settings](https://github.com/Esca-MMC/FarmTypeManager#ore-spawn-settings) only works with vanilla Nodes. So you are going to spawn the Nodes in using the [Forage_Spawn_Settings](https://github.com/Esca-MMC/FarmTypeManager#forage-spawn-settings). So normally, you would have an entry for the items to spawn in like this:
```
"SpringItemIndex": [
    "ResoNight.IslandExpansion_ExpFlower"
],
"SummerItemIndex": [
    "ResoNight.IslandExpansion_ExpFlower"
],
"FallItemIndex": [
    "ResoNight.IslandExpansion_ExpFlower"
],
"WinterItemIndex": [
    "ResoNight.IslandExpansion_ExpFlower"
],
```
(If you wonder, in my case, the map has no seasons and therefore I spawn in the same plant in all seasons). But now, if you want to spawn in the Nodes, you must do something like this instead:
```
"SpringItemIndex": [
          {
            "Category": "Object",
            "Name": "ResoNight.IslandExpansion_ExpStoneNode",
            "CanBePickedUp": false
          }
        ],
        "SummerItemIndex": [
          {
            "Category": "Object",
            "Name": "ResoNight.IslandExpansion_ExpStoneNode",
            "CanBePickedUp": false
          }
        ],
        "FallItemIndex": [
          {
            "Category": "Object",
            "Name": "ResoNight.IslandExpansion_ExpStoneNode",
            "CanBePickedUp": false
          }
        ],
        "WinterItemIndex": [
          {
            "Category": "Object",
            "Name": "ResoNight.IslandExpansion_ExpStoneNode",
            "CanBePickedUp": false
          }
        ],
```
This, as you can probably guess, prevents the Node from being picked up. And this is the last necessary piece to add Custom Stones. And coming back to GWF, if you add Custom Stones, you can also use its ID in the [StoneTypes](#stonetypes) field. So for example you could pixel a "Walnut Stone" or something and add it to the Quarry on IslandNorth, just to give you some ideas.

# MonsterTypes
If you assign the **MonsterTypes** field, only the given **MonsterTypes** in your assigned area can drop a Walnut. This basically looks for monster classes, so even if you add custom monsters, they can still be referred to by its category. The list of available Monster types can be seen on the [Monsters](https://stardewvalleywiki.com/Modding:Monster_data#Monster_IDs) page on the wiki. The table that you see there all the way to the bottom shows all types of Monsters. Keep in mind, the entry that you must enter here is always the ID, so the entry on the right side of the table. This means that for example for the monster that is usually called dust sprite, you must take the ID called "Dust Spirit" instead. If you happen to add multiple Monsters in one location with the same type, but different sprites, you can also put the sprite path in here, instead of the actual MonsterType. To do this, you must have loaded your sprite into the game somewhere. So if you have something like this:
```
{
    "Action": "Load",
    "Target": "{{ModID}}/Monsters/Apophis,
    "FromFile": "assets/Monsters/Apophis.png
}
```
You would write this into the MonsterTypes field:
```
"MonsterTypes": [ "{{ModID}}/Monsters/Apophis" ]
```


# Conditions
The **conditions** field lets you set one or multiple [GameStateQueries](#gamestatequeries) as a condition after that the Walnut becomes available. This field works generally like most conditions fields in other mods. Whereas you can do this for practically any reason, the most obvious reason is definitely by using a Secret Note. GWF does *not* provide a framework for Secret Notes, because [Secret Note Framework](https://github.com/ichortower/SecretNoteFramework) by ichortower already exists and it already has every feature one could ask for. So if you want to use that framework, please read through its author guide. So lets say you already have a working Secret Note. Then you have something like for example this:
```
"{{ModID}}_SecretNote_Buried_F_Island_1": {
    "Title": "Hidden Walnut #1",
    "NoteImageTexture": "Mods/{{ModId}}/Exp_SecretNote_F_Island_1",
    "NoteImageTextureIndex": 3,
    "LocationContext": "Island"
},
```
Then you can write this into the Conditions field (that is a custom GameStateQuery by the SecretNoteFramework): 

"Conditions": "ichortower.SecretNoteFramework_PLAYER_HAS_MOD_NOTE Any {{ModID}}_SecretNote_Buried_F_Island_1"

This will effectively make the Walnut obtainable after any player has read the Secret Note. You could also write Current, All or Host instead of Any. One important thing is, while Walnuts have the Conditions field, the whole Walnutgroup and the [Hint](#hint) have the field [HintConditions](#hintconditions) as well. That field works exactly the same like the Conditions field for a single Walnut. But important to note is, every Walnut of the same group also have the conditions set in the [HintConditions](#hintconditions) field. Therefore, as long as the [Hint](#hint) itself is not available, none of its Walnuts are either.

## ParrotUpgradePerches
