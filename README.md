# SpeedGuidesLive

Component for LiveSplit which allows the user to create a guide for any given speedrun.

A separate file will be saved as an .sgl file of the same name of whichever split is currently loaded...

How to use:
- Place SpeedGuides.dll in ./LiveSplit/Components/ folder
- [Restart LiveSplit if it's running]
- Add your component to your layout...
	- Right-Click on LiveSplit
	- Edit Layout...
	- Click the '+' button
	- Other->Speed Guides Live
	- Double-Click the Speed Guides Live item in the list inside the Layout Editor

Property Menu:
Items in the menu can be changed during a run, but may be prone to causing issues... (None that I know of)
Font: Changes the font in the guide window
Font Size: The size of the font in the guide window
Edit Guide: Allows you to edit the guide based on the current loaded split set...
	- Change the test in the boxes next to the repective split
	- HIT "Save" button BEFORE you close the window...
	- "Reload" will reload what was ever saved.
	
Importing from Spreadsheets:
You may import a tsv (tab-separated file) from Google Drive, Excel, etc. Go into SGL settings, under Extra Stuff, and click the Import button for spreadsheets. 
The first column is the split name (case sensitive) and the right column(s) are the notes. 
You may import specific splits if you'd like (Splits not in the spreadsheet will NOT override existing split notes)

	
Developed by iNightfall

Website: http://www.nightgamedev.com/

Twitch: http://twitch.tv/iNightfall

Support my developments: https://www.patreon.com/iNightGaming

If you like the app, or have some suggestions, send me a shout-out on my Twitter: https://twitter.com/inightfaller

## Additions by Phantom5800

- Notes are now markdown formatted
- Images are supported (currently needs a full path and does not support resizing in markdown)
