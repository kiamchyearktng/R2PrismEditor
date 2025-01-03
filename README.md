# Remnant 2 Prism Editor

This is a save parser for Remnant 2 attached to a WPF user interface that allows the user to make quick edits to the prisms within. Current version: v0.1.0

Preceding work for the Remnant 2 save parser is done by Andrew Savinykh at <https://github.com/AndrewSav/lib.remnant2.saves>, as well as his credited individuals [t1nky](https://github.com/t1nky/remnant-item-finder) and [crackedmind](https://github.com/crackedmind).

I did not add to the save parser part of the code, only created abstraction layers and user interface, and a small number of guardrails. Nevertheless, save editing remains an imprecise endeavor. The resultant file is not guaranteed to be readable. Please back up your saves before using the program.

## Features and usage

Using the user interface, the user can view the information of their prisms, and make some supported edits. Then the new save file can be used to replace the existing save.

BACK UP ALL YOUR SAVES BEFORE RUNNING ANYTHING! The available options are sorted by how likely they are to make your save unreadable or leave visible traces of save editing in the game.

Current features:
- View prism details: level, internal level, experience, RNG seed, pending choice, prism segments, and fed fragments
- Reroll one or all prisms' RNG seed, or just reset their pending choice

Future features
- More options for saving and loading files
- Edit remaining experience
- Edit segments and fed fragments

## How to run

- You may go to the [releases page](https://github.com/kiamchyearktng/R2PrismEditor/releases) and download the latest executable.
    - You may need to install .NET 8.0 from [Microsoft's download page](https://dotnet.microsoft.com/en-us/download).
- If you don't trust the binaries, I applaud you for your dedication to security. You can download the code and compile it yourself, or run it in debug mode if you wish to understand it.
    - I use Visual Studio 2022 and the project runs .NET 8.0.

## Code overview

- TBD

## Known issues

- Duplicated prisms may show up under another character, and the character may not exist.
- Negative prism seeds may not be saved properly.

## Contributions and derivative work

I welcome any feedback, bug report, or code contribution. For now, I am mainly available on the Remnant Toolkit discord.

Those looking to fork the project can do so at will for non-commercial purposes, as long as credit is given to me, plus the three individuals mentioned above.

Visit the [Remnant Toolkit](https://www.remnant2toolkit.com) and [its discord](https://discord.gg/kgVaU3zAQ7) for more helpful tools and discussion relating to Remnant 2.

## Latest changes

### v0.1.0
- Enabled load in place and save steam buttons
- Reworded some warnings more strongly
- v0.1.0 executable released
