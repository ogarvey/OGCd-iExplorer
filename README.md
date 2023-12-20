# OGCd-iExplorer - W.I.P
An App for exploring CD-i files, and interacting with the various data types within. Built as a way to learn Avalonia, and also determine useful functionality for OGLibCD-i

Uses OGLibCD-i to gather and display info, and then interact with files that have been extracted from CD-i discs/images.

So far, you can load an extracted file, hit the Analyse button and get a breakdown of the number of Audio, Video and Data Sectors in the file. You will also see a grid containing a list of all sectors, these sectors can be selected in any order or number as required, and then exported to their own file for further analysis.

> [!WARNING]
> Early days so things are still quite rough at the moment, on the off chance you find yourself using this and get stuck, feel free to ping me.

## Burn:Cycle.rtf example

The below is what you will see after opening, and then analysing the file in the app

![Screenshot of the app, after opening and analysing Burn:Cycle.rtf](https://github.com/ogarvey/OGCd-iExplorer/assets/994386/ec78cbfb-df4a-46e3-a471-236611f16261)

You can then select the sectors you are interested in (the options will be expanded, atm both image/palette set the selected sectors as those to export), right click for the menu to select the type of data, and then once again for the option to save the sectors to a file on disk

![Screenshot showing context menu](https://github.com/ogarvey/OGCd-iExplorer/assets/994386/23e2a9ae-cd2b-4e2e-bc98-bf7f4b14f367)

## HexView example

Small video showing the preview of selected sectors in a Hex View.

![Video showing Hex View](https://github.com/ogarvey/OGCd-iExplorer/assets/994386/f6ab44a9-370b-487b-8d9f-e9a307a67b46)

## Palette Preview example

Small video showing the preview of a selected sector as a palette. 

![Video showing Palette Preview](https://github.com/ogarvey/OGCd-iExplorer/assets/994386/396d6bbb-fd9e-4411-9f06-ea9247d31ec2_

