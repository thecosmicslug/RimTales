
## RimTales - A Story Exporter for Rimworld. 

    //TODO: Create an updated README.

    This mod was Inspired by two abandoned mods, which I felt were both lacking features but good in their own way.
    So I started to combine the two, removing the UI from RimStory and integrating it with 'Tales Log [Retold]'

    RimTales takes the best bits from both mods, condenses it into one interface while massivly expanding the Events & Incidents it records.
    The Internals of this project have changed drastically since I started by tweaking 'Tales Log [Retold]'

    This version no longer relies on the TaleManager to retrieve the events, Rimworld deletes them after sometime in-game and that bugged me!
    This version uses a Harmony hook to catch all tales and log them permamently, the same with the incidents.

# Original Description:
Every time something relevant happens on a colony, a "tale" is registered on the save so it can be used on art (like deaths, research, training and so on).
This mod adds a tab to the game window that shows to you every tale recorded on that save.
To avoid performance issues, the log will only update when you open it.
Also, this mod does not change tale recording, so it only shows what the game has recorded.

* __Filters__. Using the "filter" text field you can select what will be shown on the tab.

* __Colours__. "Show Colors": if disabled, all tales will be white.


# Thanks
1. Thanks to spuddy for fixing 'Tale Log [Retold]' for v18 and sharing the source code at 'https://bitbucket.org/spudbean/talelogretold/'
2. Thanks to Nandonalt for the original 'Tale Log'
