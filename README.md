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
  * [Synchronize with Content Patcher](#synchronize-with-content-patcher)
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
[Synchronizing with Content Patcher](synchronize-with-content-patcher) before you do anything else. Now lets get started with **GoldenWalnuts!**

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
field are the only ones, that support the [Language Token](#language-token). The Setting [SeparateHints](#separatehints) might also be interesting for you.

## Singular
