Version:
1.0.1

----------------------------------------------
LiveSplit Version Tested:
1.7.4

----------------------------------------------
Notes:

- Fixed a bug where initial values in the settings window were incorrect.

- Made it so you can no longer make the window a 1x1 window.

- When creating an SGL component, the new window can now be behind the settings screen.

- If LiveSplit is far enough to the right of the screen when the SGL screen gets created, it will be put in the center of the screen instead.

- Locked down the SGL window's visibility... If you try to close it without removing the component, another window will appear in it's place. (Additionally, if you add two or more SGL components, and remove one, the window will reappear)

- Added some debug stuff to test, if for any reason your window gets stuck.