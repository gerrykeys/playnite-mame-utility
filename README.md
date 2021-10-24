# Playnite - MAME Utility Extension

![Alt text](logo/mame-utility-extension-banner.png?raw=true "Banner")

MAME Utility is an extension for Playnite Library (https://playnite.link/) designed for improve MAME roms management imported in Playnite. It offers a number of features that help better organize your games within library.

## Features

- Rename your imported MAME roms using correct one 
- Remove from library all non-games (*bios/device/sample roms*)
- Remove from library all clone roms
- Remove from library all mechanical game roms
- Tag your roms by following tags: *Parent, Clone, Bios, Device, Sample, Mechanical*

> **_NOTE:_**  The extension does not touch (modify/remove) in any way your original imported rom files. 

## Compatibility
- Extensions can be installed in Playnite 9 or higher

## Installation
Download **.pext** extension and open it in Playnite in order to install it.

## Why MAME Utility?
Let's say you have a MAME romset (Full-Merged, Merged, Split, doesn't matter), and you
decide to import all roms in Playnite in order to catalog your games.
You will encounter in the following problems:
- Roms imported has no effective real name. For example the rom ***mslug*** correpsonds to ***Metal Slug***, as well as ***rbisland*** corresponds to ***Raibow Islands***. You would therefore prefer to see real names in Playnite and not the one relative to the rom file.
- Some of roms that you have imported are not games. For example Bios, Device and Sample are roms that are required by other games in order to play them, but they are not games and you probably don't want to see in Playnite library.
- Some of roms are mechanical games but you only want to have arcade games.
- Some of roms are clone of other games but you only want to have the original rom (Parent).
- You have cover art and images that you want to assign automatically to each game.
- You want to classify roms by some information: Is game a Parent? Is game a clone of other game? Is game mechanical?

With MAME Utility extension you can solve all this needs.

## Configuration
The only thing to do is to set the path of the mame executable related to the version of your romset.
For example, if you are using MAME romset 0.235 you have to download the MAME 0.235 application from the site https://www.mamedev.org/.
Once the application is downloaded, then you have to go in Playnite and set the MAME executable file path under MAME Utility extension settings section.

![Alt text](screenshots/settings.png?raw=true "Settings")


## How to use
### Renaming Games using correct name
Select your MAME game in playnite which you want to rename and then select the option `Extensions > MAME Utility > Rename Selected Game`.

Original rom name                                                  |  Renamed
:-----------------------------------------------------------------:|:-------------------------:
![Alt text](screenshots/imported-raw.png?raw=true "Imported Raw")  |  ![Alt text](screenshots/renamed.png?raw=true "Renamed")

### Clean library from non-games
Select all your MAME game in playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove non-Games from selection`.
This will remove from library all the bios/device/sample entries which are not games.
In the example "neogeo" is not a Game bu a rom bios, and it will be removed from library.

Games and non-Games                                                |  Only Games
:-----------------------------------------------------------------:|:-------------------------:
![Alt text](screenshots/renamed.png?raw=true "Non Games")          |![Alt text](screenshots/cleaned-nongames.png?raw=true "Only Games")

### Clean library from clone Games
Select all your MAME game in playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove clone Games from selection`.
This will remove all clone games from library.

### Clean library from mechanical Games
Select all your MAME game in playnite and then select the option `Extensions > MAME Utility > Cleaner > Remove mechanical Games from selection`.
This will remove all mechanical games from library in order to have only Arcade games.

### Set cover image for Games
Select your MAME game and then select the option *`Extensions > MAME Utility > Media > Set cover image for selected Games`.
Then you will be requested to choose the folder which contains your cover images.
Remember that the images must have the same name of the original rom imported.


Games without cover                                                 | Games with cover.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](screenshots/without-cover.png?raw=true "Without Cover") |![Alt text](screenshots/with-cover.png?raw=true "With Cover")

### Set background image for Games
Select your MAME game and then select the option `Extensions > MAME Utility > Media > Set background image for selected Games`.
Then you will be requested to choose the folder which contains your background images.
Remember that the images must have the same name of the original rom imported.

Games without background                                            | Games with bakcground.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](screenshots/without-snap.png?raw=true "Without Snap")   |![Alt text](screenshots/with-snap.png?raw=true "With Snap")

### Tag your Games
Select your MAME game and then select the option `Extensions > MAME Utility > Cataloguer > Tag selected Games`.
This wiil apply the following tags according to the rom type: *Parent, Clone, Bios, Device, Sample, Mechanical*


Games without tag                                                   | Games with tag.
:------------------------------------------------------------------:|:-------------------------:
![Alt text](screenshots/without-tag.png?raw=true "With Tag")        |![Alt text](screenshots/with-tag.png?raw=true "Without Tag")

## License

MIT
