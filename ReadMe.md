# Darkness Randomizer Mod

DISCLAIMER: This documentation is aspirational. The mod is under development, and not yet released.

A Rando 4 connection mod to randomize dark rooms in Hallownest.

Search "DarknessRandomizer" on Scarab to install. Requires [Rando 4](https://github.com/homothetyhk/RandomizerMod) and [ItemChanger](https://github.com/homothetyhk/HollowKnight.ItemChanger). Compatible with (but does not require) [RandoPlus](https://github.com/flibber-hk/HollowKnight.RandoPlus), specifically the "Not Lantern" setting.

Probably doesn't work well with Room Rando, but you're welcome to try.

## Darkness Randomization

DarknessRandomizer randomly selects various corners of Hallownest to make dark, and slowly spreads the darkness from them to the rest of the map based on a hand-crafted set of probability and cost weights. High-connectivity areas, especially those traversable with low movement (like crossroads) have low probabilities, while corners of the map like Oro and Soul Sanctum are weighted more highly. This ensures that, most of the time, you will have access to a large swath of the map without Lantern, allowing Logic to place Lantern late in progression and provide interesting seeds.

Darkness is gradual and smooth. Your starting position, and its immediate surroundings, are always well-lit for safety, but at almost any point you can encounter semi-darkness, and then eventually, darkness. Some areas, like Dirtmouth, always bright.

## Interactables

Similar to No Eyes and the peaks toll gate, dark rooms render certain interactables inoperable without lantern if the room is dark. In general, toll gates, locked doors, and similar devices cannot be interacted with if the room is dark and the player doesn't have Lantern. If you find an interactable that doesn't obey this rule, it's probably a bug or an oversight.

## Logic Changes

Semi-darkness has no effect on Logic, and only provides a slight challenge increase. In a room with true darkness, all checks and transitions are gated either by having Lantern, or having Dark Rooms enabled in skips.

Rooms with particularly difficult platforming or enemies are not in Logic while dark unless Difficult Skips or Proficient Combat, respectively, are also enabled in skips. For instance, a true-dark Path of Pain is not in Logic without Lantern unless both Dark Rooms and Difficult Skips are enabled.

## Don't Forget!

While DarknessRandomizer generally adds darkness, it can also take it away! Don't forget to check the areas where Hallownest is normally dark, to see if you can make progress there anyway without lantern.
