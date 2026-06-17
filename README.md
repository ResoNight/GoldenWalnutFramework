# Golden Walnut Framework

This is a Framework Mod that lets you add custom Golden Walnuts and Parrot Upgrade Perches. You can add Walnut Bushes, bury Walnuts, drop them when you destroy a Stone and much more.
And you can add new Parrot Upgrade Perches (The Parrots that sit on a stick and where you can trade Walnuts for a Map change) and trigger Map Overrides, destroy some tiles or anything else.


## Contents
* [Installation](#installation)
* [Setting up your Content Pack](#setting-up-your-content-pack)
* [Having multiple Content Packs in one folder](#having-multiple-content-packs-in-one-folder)
* [General](#general)
* [GoldenWalnuts](#goldenwalnuts)
  * [Hint](#hint)
  * [Singular](#singular)
    * [Language Token](#language-token)
  * [ID](#id)
  * [Type](#type)
    * [Bush](#bush)
    * [Buried](#buried)
    * [Fishing](#fishing)
    * [Stone](#stone)
    * [MonsterLoot](#monsterloot)
    * [Custom](#custom)
  * [Location](#location)
  * [X & Y](#x-&-y)
  * [Width & Height](#width-&-height)
  * [ExtraTiles](#extratiles)
  * [Chance](#chance)
  * [Count](#walnut-count)
  * [DropAtOnce](#dropatonce)
  * [StoneTypes](#stonetypes)
  * [MonsterTypes](#monstertypes)
  * [Secret Notes and Conditions](#secret-notes-and-conditions)
* [ParrotUpgradePerches](#parrotupgradeperches)
  * [Base Entries](#base-entries)
  * [DestroyAreas](#destroyAreas)
  * [FromFile, FromArea, ToArea](#fromfile,-fromarea,-toarea)
  * [Condition](#condition)
* [Settings](#settings)
  * [ModID & Content Patcher Compatibility](#modid-&-content-patcher-compatibility)
  * [AutomaticWalnutIDs](#automaticwalnutids)
  * [SeparateHints](#separatehints)
  * [MailFlagsForUsedWalnuts](#mailflagsforusedwalnuts) <- this one is important
  * [DisableWalnutCap](#disablewalnutcap)
  * [DisableSeasonalFeaturesForMaps](#disableSeasonalFeaturesForMaps)
  * [IgnorePathsWarning](#ignorepathswarning)
* [Console Commands](#console-commands)
  * [ShowWalnuts](#showwalnuts)
  * [RemoveWalnut](#removewalnut)
  * [ShowMailFlags](#showmailflags)
  * [RemoveMailFlag](#removemailflag)
  * [ShowAllWalnutIDs](#showallwalnutids)
  * [ShowAllStoneTypeIDs](#showallstonetypeids)
  * [/recountNuts](#recountNuts)
* [Example File](#example-file) 

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Golden Walnut Framework** from [GitHub](https://github.com/ResoNight/GoldenWalnutFramework) or [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/???/)
3. **Unzip FarmTypeManager** into your `Stardew Valley\Mods` folder.

## Setting up your Content Pack
1. **Create a new folder** with your content pack in the Mods folder
2. **Add a content.json and a manifest.json file**
3. **for the manifest.json, follow the instructions** on the [Stardew Valley Wiki](https://stardewvalleywiki.com/Modding:Modder_Guide/Get_Started#Add_your_manifest).
4. **Add this field in your manifest.json**:
```
"ContentPackFor": {
    "UniqueID": "ResoNight.GoldenWalnutFramework"
}
```
Now start the game. Unless you see `No content.json for GoldenWalnutFramework has been found` pop up in the SMAPI Console, you're good to go!

## Having multiple Content Packs in one Folder
If you also have a **Content Pack** for **Content Patcher** or any other Framework or a **C# Mod already** in your mod folder, which you most likely have, then your
**content** and **manifest.json** for this **Content Pack** must go into a **subfolder**, as well as the folders for **every other mod or content Pack** as well.
To put it simple, if you have multiple manifest.jsons for multiple whatevers, they must all be similarly deep into the folder structure. Meaning, the data path of each
manifest.json must have the same length, or otherwise SMAPI will overlook the manifest.jsons, that lie deeper into a folder than any other manifest.json.

## General
Now you can start writing stuff into your **content.json**. There are a few things I want to mention before you jump into adding your new entries. The [Contents](#contents)
field above roughly matches the structure that your content.json will have. IDs work in a pretty unusual way, so please read [Walnut ID](#walnut-ids) and the Setting
[AutomaticWalnutIDs](#automaticwalnutids). Also if you have a Content Pack for Content Patcher (which you most likely have), please look into 
[ModID & Content Patcher Compatibility](#modid-&-content-patcher-compatibility) before you do anything else. If you are testing your implemented Walnuts, I have a bunch of error logs, so you can't do anything wrong. All entries will be checked when you start the game and they will be checked again, if you load into a save. The check for your [Location](#location) entry can only happen after loading into a save. If you are adding/changing entries in your content.json, you don't need to fully close the game and reopen it. You can just exit the save and re-enter it. You will get a little warning into your SMAPI console, when you add or remove a new [Walnut Group](#hint), but don't worry, your collected Walnut Count will just be off a bit. After restarting, it will always be correct then. Now lets get started with **GoldenWalnuts!**

## GoldenWalnuts
The Basic structure for Golden Walnuts looks like this:
```
"GoldenWalnuts": {
    "Hint1": [
        {
            //entries for walnut 1
        },
        {
            //entries for walnut 2
        },
        ...
    ],
    "Hint2": [
        ...
    ],
    ...
}
```
For each Walnut, there are those fields:
Field|Value|Description
-----|-----|-----------
[ID](#id) | string | A unique ID for walnuts. Walnuts with type [Bush](#bush) do not have an ID. IDs can be generated automatically (see below at [AutomaticWalnutIDs](#automaticwalnutids))
[Type](#type) | string | The type can either be [Bush](#bush), [Buried](#buried), [Fishing](#fishing), [Stone](#stone), [MonsterLoot](#monsterloot) or [Custom](#custom)
[Location](#location) | string | The Location of the Walnut
[X](x-&-y) | int | The X-Coordinate of the Walnut
[Y](x-&-y) | int | The Y-Coordinate of the Walnut
[Width](#width-&-height) | int | The width for the area in that the Walnut is obtainable
[Height](#width-&-height) | int | The height for the area in that the Walnut is obtainable
[ExtraTiles](#extratiles) | List with different elements (see at [ExtraTiles](#extratiles)) | additional areas in that the Walnut is obtainable (so you can assign a more specific area than just one rectangle)
[Chance](#chance) | int | The chance for the Walnut to drop. The number must be between 0 and 1.
[Count](#count) | int | The amount of walnuts that can be dropped from this Walnut entry. (assigning a Count will change the [ID](#id))
[DropAtOnce](#dropatonce) | [int, int] | The amount of walnuts that will be dropped at once. Cannot exceed the given [Count](#count)
[StoneTypes](#stonetypes) | [int or string, int or string, ...] | if assigned, only the given StoneTypes can drop a Walnut. To get a list of all possible values, use the [Console Command](#console-commands) [ShowStoneTypeIDs](#showstonetypeids). Supports custom Stone Types
[MonsterTypes](#monstertypes) | [int or string, int or string, ...] | if assigned, only the given MonsterTypes can drop a Walnut. To get a list of all possible values, open the Data/Monsters file of the game. The entries on the **left** side are the possible values. Or lookup this [table](https://stardewvalleywiki.com/Modding:Monster_data#Monster_IDs) on the Stardew Valley Wiki. The entries on the **right** side are possible values
[Condition](#secret-notes-and-conditions) | string | a [MailFlag](https://stardewvalleywiki.com/Modding:Mail_data#Mail_flags) after that the Walnut becomes obtainable, for example after reading a [Secret Note](#secret-notes-and-conditions)
[Singular](#singular) | string | lets you assign a singular form for the [Hint](#hint), if the player only has one remaining Walnut under the Hint. This is a very special field, read below at [Singular](#singular)

The only always **mandatory** field is the [type](#type). Each Walnut type has a different set of possible fields that you can assign, for example for [Bush](#bush) type Walnuts, you cannot assign an [ID](walnut-id), [MonsterLoot](#monsterloot) type Walnuts are the only type that supports the [MonsterTypes](#monstertypes) field and so on. Look for each type which kind of entries are possible and which are not.

## Hint
As you can see [above](#goldenwalnuts), each **Golden Walnut** that you add is part of a **Walnut Group** under a **Hint**. The Hint is what you can see when you
right click the Parrot in the Island Hut on IslandEast. The vanilla game writes the hints in a quite unique way, that you might want to follow. There are two ways how a Hint can
be written. The first way, is, you just write the hint and no matter how many Walnuts under this hint are remaining, it will say the same thing. But you
can also let the Parrot say the amount of remaining walnuts by adding `{0}` somewhere in the hint. So for example, if there are 5 Walnuts in a group
that you haven't collected yet, then this: 

`{0} buried in the north...` 

would automatically turn into this, when talking to the Parrot: 

`5 buried in the north...` 

***Important!*** always write `{0}` with a 0,
***NEVER*** any other number than that!

For those hints in specific, concernedApe always wrote them in a way so that the **singular** and **plural** form
are the same. So he never wrote f.e. `{0} Walnuts in the Ocean...`, since this could say `1 Walnuts in the Ocean...`. However, I just added a way so you can
write a [singular](#singular) form that will be shown instead, when you have 1 Walnut remaining of this group. The Hint field and the [Singular](#singular)
field are the only ones, that support the [Language Token](#language-token). The Setting [SeparateHints](#separatehints) might also be interesting for you. One last thing, don't make your hints too long or they might go **off screen**!

## Singular
If your [Hint](#hint) contains a `{0}` and you want to have a singular form of the string, when the player has only one remaining Walnut in this group, you can assign the Singular field. To use this field, your first "Walnut" entry must be just the field for singular. So it should look like this:
```
"YourHint": [
    {
        "Singular": "YourHintInSingularForm"
    },
    {
        //entries for Walnut 1
    },
    {
        //entries for Walnut 2
    },
    ...
]
```
The Singular field also supports the [Language Token](#language-token). This field technically also supports `{0}`, but this will always be replaced with 1, since this will only be shown if you have 1 walnut remaining.

## Language Token
For the [Hint](#hint) and its [Singular](#singular) form, you can use the language token, that works similar to how [Content Patcher's language token](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#i18n) works. The token looks like this: `{{i18n:...}}` and for the ..., you enter the keyword that you are using in your language files. So to set everything up, create a new **Folder** called `i18n` next to your content and manifest files. In this foldeer, you add a default.json for english and then you add a file for each language that you want to add. The language files must be the language code with .json behind it, so `de.json` for german, `es.json` for spanish and so on (See the [Translation Guide](https://stardewvalleywiki.com/Modding:Translations) on the Stardew Valley Wiki). In those files, you add the keyword that you used in your token. So for example, if you have this:
```
"{{i18n:Buried_North}}": [
    {
        "Singular": {{i18n:Buried_North_S}}
    },
    ...
]
```
then your default.json would look like this:
```
{
    "Buried_North": "{0} Golden Walnuts buried in the north...",
    "Buried_North_S": "1 Golden Walnut buried in the north..."
}
```
and your, lets say de.json would look like this:
```
{
    "Buried_North": "{0} Walnüsse vergraben im Norden...",
    "Buried_North_S": "1 Walnuss vergraben im Norden..."
}
```
If you didn't add the language file the player currently uses or the language file is missing the entry for the hint that the game tries to show the player, it will just look for the entry in the default.json instead. So don't worry about missing entries or missing language files!

## ID
The ID for Golden Walnuts works in a unique way and has some edge cases. The ID that you assign here is the ID that will be added under `Game1.player.team.collectedNutTracker`. IDs should be assigned as unique as possible, so using the `{{ModID}}` token is strongly advised (see below at [ModID & Content Patcher Compatibility](#modid-&-content-patcher-compatibility)). Bushes cannot have a custom ID, because it needs a specific ID structure, so the game can connect a Walnut to a Walnut Bush. So Bushes' ID will automatically be this: `Bush_Location_X_Y`. If you assigned a [Count](#count) to a Walnut that is 2 or higher, each individual Walnut will automatically have its own ID. So if you have, lets say, Count set to 3 and your ID is "TestID", the IDs of those Walnuts will be: `TestID_1`, `TestID_2` and `TestID_3`. This is especially important if you assign a Walnut with the [Type](#type) [Custom](#custom), since you need to add the right IDs to your collectedNutTracker. You can also activate [AutomaticWalnutIDs](#automaticwalnutids) so you don't have to write an ID for each Walnut that you add ([Custom](#custom) Type Walnuts must **always** have an ID assigned to them).

## Type
There are a total of 6 Types that a Walnut can have. Each type has different fields that it **must** have, **can** have and **cannot** have. I will go through them one by one.

# Bush
![Indoors Bush](docs/images/Example_Bush_Indoors.png)

Possible Fields|Status
---------------|------
[Type](#type) | required
[Location](#location) | required
[X](#x-&-y) | required
[Y](#x-&-y) | required

Example:
```
{
    "Type": "Bush",
    "Location": "Sunroom",
    "X": 3,
    "Y": 7
}
```

A **Bush** must always have those fields and cannot have any other than that. You cannot assign an [ID](#id) for Bushes, since I need to automatically generate the ID so that the game can connect the Walnut to the Bush. For this example, the ID of the Walnut would be `Bush_Sunroom_3_7`. On the Paths TileSheet when creating maps, there is a tile that lets you spawn in Walnut Bushes. ***DO NOT USE THIS!*** Adding a Walnut with Type Bush will spawn it in automatically. When you place them yourself, the Framework cannot keep track of them! What you can do though is place the tile with index 7 from the paths TileSheet on the Paths layer (see [Paths Layer](https://stardewvalleywiki.com/Modding:Maps#Paths_layer) on the Wiki). This is a tile that does not have any effect at all, so it is good for yourself to keep track, where you placed Bushes. One more thing, just like normal Bushes, spawning a Bush once will let it stay in the save file. However, you don't have to worry about that. You will occasionally see a `x Bushes removed` in the console, since GWF automatically removes any Walnut Bushes that have been placed using the framework, but don't have any matching entry currently.

# Buried
![Buried in Town](docs/images/Example_Buried_Town.png)

Possible Fields|Status
---------------|------
[ID](#id) | required (optional if [automaticWalnutIDs](#automaticWalnutIDs) is enabled)
[Type](#type) | always required
[Location](#location) | always required
[X](#x-&-y) | always required
[Y](#x-&-y) | always required
[Count](#count) | optional
[Condition](#secret-notes-and-conditions) | optional

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

A **Buried Walnut** works pretty much exactly how you think it would work. If you assign a Count, all those Walnuts will be dropped at once. Especially for buried walnuts, the *Condition* field might be very useful to f.e. only let a walnut be dropped *after* the player has read a **Secret Note**. GWF does *not* provide a framework for Secret Notes, so see below at [Secret Notes and Conditions](#secret-notes-and-conditions). Keep in mind that the tile for your walnut must be *diggable*.

# Fishing
![Fishing at Log in Mountain](docs/images/Example_Fishing_Log.png)

Possible Fields|Status
---------------|------
[ID](#id) | required (optional if [automaticWalnutIDs](#automaticWalnutIDs) is enabled)
[Type](#type) | always required
[Location](#location) | always required
[X](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Y](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Width](#width-&-height) | optional
[Height](#width-&-height) | optional
[ExtraTiles](#extratiles) | optional
[Count](#count) | optional
[Chance](#chance) | optional
[Condition](#secret-notes-and-conditions) | optional

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

Fishing Type Walnuts can either be fished across a whole map or they can be in specific areas. With the [X & Y](#x-&-y) Coordinates as wellas the [Width](#width-&-height) and [Height](#width-&-height), you can assign a rectangle in that the walnut can be fished. If you need a more specific area, you can use the field [ExtraTiles](#extratiles). If you assign a [Count](#count) and the last Walnut is being fished, the game will play a small soundeffect, so that the player knows, that he has all walnuts of this kind. The [DropAtOnce](#dropatonce) feature unfortunately does not work for **Fishing** type walnuts, since you are actively fishing one walnut instead of x walnuts being dropped into the world. For the [Chance](#chance), please keep in mind that the player is fishing pretty slowly and you can only fish one at a time. So whereas a 0.05 chance for a [Stone](#stone) type Walnut in a larger quarry would be perfectly reasonable, a 0.05 chance for fishing, especially if you assign a Count like 5, would be terrifyingly frustrating. So, in short, think of what you are doing and always think of the unlucky ones :) You can also assign a Condition (see below at [Secret Notes and Conditions](#secret-notes-and-conditions)).

# Stone
![Stone in MountainQuarry](docs/images/Example_Stone_Mountain.png)

Possible Fields|Status
---------------|------
[ID](#id) | required (optional if [automaticWalnutIDs](#automaticWalnutIDs) is enabled)
[Type](#type) | always required
[Location](#location) | always required
[X](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Y](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Width](#width-&-height) | optional
[Height](#width-&-height) | optional
[ExtraTiles](#extratiles) | optional
[Count](#count) | optional
[DropAtOnce](#dropatonce) | optional
[Chance](#chance) | optional
[StoneTypes](#stonetypes) | optional
[Condition](#secret-notes-and-conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_MountainQuarry",
    "Type": "Stone",
    "Location": "Mountain",
    "Chance": 0.05,
    "Count": 10,
    "DropAtOnce": [1, 3]
}
```

This type causes Stones to drop Walnuts if you break them in any way, just like the MusselStones on IslandWest. You can assign a **rectangle** using [X, Y](#x-&-y), [Width](#width-&-height) and [Height](#width-&-height). If you need a more specific area, you can use the field [ExtraTiles](#extratiles). If you leave all of them out, the area will just be the whole map. If you assign a Count, the game will play a soundeffect whenever the player collects the last walnut of this kind. For Stones, you can also assign the DropAtOnce field. Whenever the Stone is going to drop Walnuts, it will drop a random amount of walnuts between your left and right number. So in the case of the example, a stone will drop 1, 2 or 3 walnuts at once. However, the [Count](#count) you assigned will always be the upper limit. So for example if you assign something like this: `"DropAtOnce": [3, 5]` with the Count 10 and 9 out of 10 Walnuts have already been dropped, the last Stone will forcefully drop only 1 Walnut. There is also the field [StoneTypes](#stonetypes) which, when set, only lets the given Stones drop Walnuts, including Custom Stones (see below at [StoneTypes](#stonetypes). For the [Chance](#chance) field, you should really think about how you are going to set this. You have to consider the size of your quarry, the amount of stones that can drop walnuts, the amount that can be dropped at once and the bad luck of some players. For example my example from above was actually not that good. Upon testing, in 10 out of 10 cases with a full quarry, I got the 10 Walnuts, often with the very first bomb. So for above's example, I would completely leave out DropAtOnce and then I would say it would be decently balanced. So maybe you want to go out and just test, what Chance you want to set. This Walnut Type also supports the Condition field (see below at [Secret Notes and Conditions](#secret-notes-and-conditions)).

# MonsterLoot
![Slime at Moonscythe Island](docs/images/Example_Slime_MS_Island.png)

Possible Fields|Status
---------------|------
[ID](#id) | required (optional if [automaticWalnutIDs](#automaticWalnutIDs) is enabled)
[Type](#type) | always required
[Location](#location) | always required
[X](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Y](#x-&-y) | optional (required for [automaticWalnutID](#automaticWalnutIDs))
[Width](#width-&-height) | optional
[Height](#width-&-height) | optional
[ExtraTiles](#extratiles) | optional
[Count](#count) | optional
[DropAtOnce](#dropatonce) | optional
[Chance](#chance) | optional
[MonsterTypes](#monstertypes) | optional
[Condition](#secret-notes-and-conditions) | optional

Example:
```
{
    "ID": "{{ModID}}_Moonscythe_Island_MonsterLoot",
    "Type": "MonsterLoot",
    "Location": "{{ModID}}_Moonscythe_Island",
    "Count": 5,
    "DropAtOnce": [1, 2],
    "Chance": 0.25,
    "MonsterTypes": ["Sludge"]
}
```

To make it short, this Type works basically *exactly* like the [StoneTypes](#stonetypes) walnut. Keep in mind, the area that you assign with [X, Y](#x-&-y), [Width](#width-&-height) and [Height](#width-&-height) refers to the last tile on which the monster has been killed, **NOT** where you spawned the monster (since I cannot trace back where a monster has been spawned). You can also specify, which kind of monsters can drop Walnuts by using the [MonsterTypes](#monstertypes) field.

# Custom
![Fountain at Farming Island](docs/images/Example_Fountain_F_Island.png)

Possible Fields|Status
---------------|------
[ID](#id) | always required
[Type](#type) | always required
[Count](#count) | optional

Example:
```
{
    "ID": "{{ModID}}_F_Island_FountainWalnuts",
    "Type": "Custom",
    "Count": 5
}
```

