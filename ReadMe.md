# Darkness Randomizer Mod

A Rando4 connection mod to randomize dark rooms in Hallownest.

Search "DarknessRandomizer" on Scarab to install. Requires [Rando 4](https://github.com/homothetyhk/RandomizerMod) and [ItemChangerDataLoader](https://github.com/homothetyhk/ItemChangerDataLoader). Compatible with (but does not require) [RandoPlus](https://github.com/flibber-hk/HollowKnight.RandoPlus), specifically the "Not Lantern" setting.

Probably doesn't work well with Room Rando, but you're welcome to try.

## How it works

DarknessRandomizer subdivides Hallownest into various connected sections, somewhat more granular than Map areas.  Depending on the darkness setting you choose, some number of 'leaf' sections are converted to pure darkness, and some number of adjacent and other sections are converted to semi-darkness.  Generally, except for some one-off rooms with special handling, you will always encounter semi-darkness in a region prior to encountering and pure-darkness in that region.

This helps to provide a natural feel to the darkness, and helps ensure that Logic doesn't force Lumafly Lantern to be accessible too early. Some areas, including whatever your starting location is, as well as Dirtmouth, will always be well-lit.

## Interactables

Similar to No Eyes and the peaks toll gate, dark rooms render certain interactables inoperable without lantern if the room is dark. In general, toll gates, locked doors, and similar devices cannot be interacted with if the room is dark and the player doesn't have Lantern. If you find an interactable that doesn't obey this rule, it's probably a bug or an oversight.

## Logic Changes

Semi-darkness has no effect on Logic, and only provides a slight challenge increase. In a room with true darkness, all checks and transitions are gated either by having Lantern, or having Dark Rooms enabled in skips.

Rooms with particularly difficult platforming or enemies are not in Logic while dark unless Difficult Skips or Proficient Combat, respectively, are also enabled in skips. For instance, a true-dark Path of Pain is not in Logic without Lantern unless both Dark Rooms and Difficult Skips are enabled.

There are some minor exceptions.  Rooms with no hazards and negligible platforming can be in logic even if they are fully dark; for instance, obtaining Greenpath Stag will put the stag check in logic even if the room is completely dark, but anything outside the stag room will remain out of logic.

## Don't Forget!

While DarknessRandomizer generally adds darkness, it can also take it away! Don't forget to check the peaks' toll, darknest, No Eyes, etc., to see if you can make progress there without having Lantern.
