
## RimTales - A Story Exporter for Rimworld. 

    This mod was Inspired by two abandoned mods, 'Tales Log [Retold]' and 'RimStory'
    which I felt were both lacking features but good in their own way.
    So I started to combine the two! RimTales takes the best bits from both mods, 
    condenses it into one interface while massivly expanding the Events & Incidents it records.
    The Internals of this project have changed drastically since I started.

    This version no longer relies on the TaleManager to retrieve the events,
    Rimworld deletes them after sometime in-game and that bugged me! 
    This version uses a Harmony hook to catch all tales and log them 
    permamently, the same with the incidents.

# Development:

I Work on this project using Visual Studio Code for Linux, to use on Windows the project paths will need adjusting.
I use the following extensions while I work:

1. GitLens                  -   https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens
2. Super One Dark Theme     -   https://marketplace.visualstudio.com/items?itemName=seansassenrath.vscode-theme-superonedark
3. Material Icon Theme      -   https://marketplace.visualstudio.com/items?itemName=PKief.material-icon-theme
4. Save All Button          -   https://marketplace.visualstudio.com/items?itemName=nanlei.save-all
5. Todo Tree                -   https://marketplace.visualstudio.com/items?itemName=Gruntfuggly.todo-tree
6. Rainbow Brackets         -   https://marketplace.visualstudio.com/items?itemName=2gua.rainbow-brackets
7. Better Comments          -   https://marketplace.visualstudio.com/items?itemName=aaron-bond.better-comments


# Thanks
1. Thanks to spuddy for fixing 'Tale Log [Retold]' for v18 and sharing the source code at 'https://bitbucket.org/spudbean/talelogretold/'
3. Thanks to 'emipa606' who have carried on fixing 'RimStory' 'https://github.com/emipa606/RimStory'
