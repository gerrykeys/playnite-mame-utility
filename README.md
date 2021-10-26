# Playnite - MAME Utility Extension

![Alt text](assets/logo/mame-utility-extension-banner.png?raw=true "Banner")

MAME Utility is an extension for Playnite Library (https://playnite.link/) designed to improve MAME games management imported on Playnite. 
It offers several features that help to organize better your games within the library.

If you like the extension and it has been useful to you, you can buy me a beer :beer:  

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/donate?hosted_button_id=5ATXBE94C6VCQ)

## Features

- Rename your imported MAME roms using the correct name 
- Remove from library all non-games (*bios/device/sample roms*)
- Remove from library all clone games
- Remove from library all mechanical games
- Tag your games by following tags: *Parent, Clone, Bios, Device, Sample, Mechanical*

> **_NOTE:_**  The extension does not modify or remove in any way your original imported roms. 

## Compatibility
- MAME Utility Extension can be installed on Playnite 9 or higher

## Installation
Download **.pext** extension and open it with Playnite in order to install it.

## Why to use MAME Utility Extension?
Let's say you have a MAME romset (Full-Merged, Merged, Split, doesn't matter), and you
decide to import all roms on Playnite in order to catalog your games.
You may come across following problems:

- Roms imported has not real name. For example the rom ***mslug*** correpsonds to ***Metal Slug***, as well as ***rbisland*** corresponds to ***Raibow Islands***. You want to see the real name of the game on Playnite and not the one relative to the rom file.
- Some of roms that you have imported are not games. For example Bios, Device and Sample are roms that are required by other games in order to play them. They are not games and you don't want to see them on Playnite.
- Some of roms are mechanical games but you only want arcade games.
- Some of roms are clones of other games but you only want the original rom (Parent).
- You have cover arts and background images, and you want to assign them automatically to each game.
- You want to classify roms using some information: Is game a Parent? Is game a Clone of other game? Is it a mechanical game?

With MAME Utility extension you can solve all these issues.

## Configuration
The only thing to do is to set the path of the MAME executable relating to the version of your romset or the xml/dat MAME file list.
If you choose to use MAME executable you have to download the MAME application (compatible with your romset) from the site https://www.mamedev.org/. After download of MAME application, you have to go on Playnite and set the MAME executable file path under MAME Utility Extension settings section.
If you choose to load xml/dat MAME file, you have to download or generate it and then set the appropriate path in the settings.

## How to use
### Rename Games using correct name
Select your MAME games on Playnite that you want to rename and then select the option `Extensions > MAME Utility > Rename Selected Game`.

Original rom name                                                  |  Renamed
:-----------------------------------------------------------------:|:-------------------------:
![Alt text](assets/screenshots/imported-raw.png?raw=true "Imported Raw")  |  ![Alt text](assets/screenshots/renamed.png?raw=true "Renamed")

### Clean library from non-Games
Select all your MAME games on Playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove non-Games from selection`.
This will remove from Playnite all the bios/device/sample elements which are not games.
In the following example, "neogeo" is not a game but a rom bios, and it will be removed from library.

Games and non-Games                                                |  Only Games
:-----------------------------------------------------------------:|:-------------------------:
![Alt text](assets/screenshots/renamed.png?raw=true "Non Games")          |![Alt text](assets/screenshots/cleaned-nongames.png?raw=true "Only Games")

### Clean library from clone Games
Select all your MAME games on Playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove clone Games from selection`.
This will remove all clone games from library.

### Clean library from mechanical Games
Select all your MAME games on Playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove mechanical Games from selection`.
This will remove all mechanical games from library in order to have only Arcade games.

### Set cover images for Games
Select your MAME games and then select the option `Extensions > MAME Utility > Media > Set cover image for selected Games`.
Then it will be requested to choose the folder which contains your cover images.
Remember that images must have the same name of the original imported rom.

Games without cover                                                 | Games with cover.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](assets/screenshots/without-cover.png?raw=true "Without Cover") |![Alt text](assets/screenshots/with-cover.png?raw=true "With Cover")

### Set background image for Games
Select your MAME games and then select the option `Extensions > MAME Utility > Media > Set background image for selected Games`.
Then it will be requested to choose the folder which contains your background images.
Remember that images must have the same name of the original imported rom.

Games without background                                            | Games with bakcground.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](assets/screenshots/without-snap.png?raw=true "Without Snap")   |![Alt text](assets/screenshots/with-snap.png?raw=true "With Snap")

### Tag your Games
Select your MAME games and then select the option `Extensions > MAME Utility > Cataloguer > Tag selected Games`.
This wiil apply the following tags according to the rom type: *Parent, Clone, Bios, Device, Sample, Mechanical*


Games without tag                                                   | Games with tag.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](assets/screenshots/without-tag.png?raw=true "With Tag")        |![Alt text](assets/screenshots/with-tag.png?raw=true "Without Tag")

## License

MIT
