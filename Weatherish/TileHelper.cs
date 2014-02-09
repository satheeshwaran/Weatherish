/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Phone.Shell;

namespace Weatherish
{
    public class TileHelper
    {
        // Do whatever action the button requested
        public void DoTileAction(string tileAction, Uri tileId, ShellTile tileToFind, ShellTileData tileData)
        {
            switch (tileAction)
            {
                case "create":
                    ShellTile.Create(tileId, tileData, true);
                    break;

                case "update":
                    tileToFind.Update(tileData);
                    break;

                case "delete":
                    tileToFind.Delete();
                    break;
            }
        }

        // Form our iconic tile
        public void IconicTile(
            Uri tileId,
            string title = null,
            string widecontent1 = null,
            string widecontent2 = null,
            string widecontent3 = null,
            int count = 0,
            Uri smalliconimage = null,
            Uri iconimage = null,
            Color backgroundcolor = new Color(),
            string tileAction = null,
            bool tileProperties = false)
        {
            try
            {


                // Go find the tile object for the tile ID we passed in
                var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileId.ToString()));

                // Give ourselves an IconicTileData type to work with
                IconicTileData tileData;

                // If we chose the "C#" button, go here
                if (tileProperties)
                {
                    // Create the IconicTileData with properties
                    tileData = new IconicTileData
                    {
                        Title = title,
                        Count = count,
                        WideContent1 = widecontent1,
                        WideContent2 = widecontent2,
                        WideContent3 = widecontent3,
                        SmallIconImage = smalliconimage,
                        IconImage = iconimage,
                        BackgroundColor = backgroundcolor
                    };
                }
                // If we chose the "XML" button, go here
                else
                {
                    // Convert the color to hex for XML consumption
                    string hexColor = String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", backgroundcolor.A, backgroundcolor.R, backgroundcolor.G, backgroundcolor.B);

                    // Lazy templification of the elements
                    string countElement = "<wp:Count";
                    string titleElement = "<wp:Title";
                    string iconImageElement = "<wp:IconImage>";
                    string smallIconImageElement = "<wp:SmallIconImage>";
                    string wideContent1Element = "<wp:WideContent1";
                    string wideContent2Element = "<wp:WideContent2";
                    string wideContent3Element = "<wp:WideContent3";
                    string backgroundColorElement = "<wp:BackgroundColor>";
                    string clearElement = " Action=\"Clear\">";

                    // Determine if we're supposed to be clearing some values
                    if (count == 0) { countElement = countElement + clearElement; } else { countElement = countElement + ">"; }
                    if (title == "") { titleElement = titleElement + clearElement; } else { titleElement = titleElement + ">"; }
                    if (widecontent1 == "") { wideContent1Element = wideContent1Element + clearElement; } else { wideContent1Element = wideContent1Element + ">"; }
                    if (widecontent2 == "") { wideContent2Element = wideContent2Element + clearElement; } else { wideContent2Element = wideContent2Element + ">"; }
                    if (widecontent3 == "") { wideContent3Element = wideContent3Element + clearElement; } else { wideContent3Element = wideContent3Element + ">"; }

                    // Form the XML template for the tile
                    string tileDataXmlString =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\">" +
                           "<wp:Tile Id=\"" + tileId + "\" Template=\"IconicTile\">" +
                              titleElement + title + "</wp:Title>" +
                              countElement + count + "</wp:Count>" +
                              wideContent1Element + widecontent1 + "</wp:WideContent1>" +
                              wideContent2Element + widecontent2 + "</wp:WideContent2>" +
                              wideContent3Element + widecontent3 + "</wp:WideContent3>" +
                              smallIconImageElement + smalliconimage + "</wp:SmallIconImage>" +
                              iconImageElement + iconimage + "</wp:IconImage>" +
                              backgroundColorElement + hexColor + "</wp:BackgroundColor>" +
                           "</wp:Tile> " +
                        "</wp:Notification>";

                    // Construct IconicTileData using the overload with XML string
                    tileData = new IconicTileData(tileDataXmlString);
                }

                // Go do something to the tile based on the tile action
                DoTileAction(tileAction, tileId, tileToFind, tileData);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // Form our flip tile
        public void FlipTile(
            Uri tileId,
            string title = null,
            string backtitle = null,
            string backcontent = null,
            string widebackcontent = null,
            int count = 0,
            Uri smallbackgroundimage = null,
            Uri backgroundimage = null,
            Uri backbackgroundimage = null,
            Uri widebackgroundimage = null,
            Uri widebackbackgroundimage = null,
            string tileAction = null,
            bool tileProperties = false)
        {
            try
            {

                var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileId.ToString()));

                FlipTileData tileData;

                if (tileProperties)
                {

                    tileData = new FlipTileData
                    {
                        Title = title,
                        BackTitle = backtitle,
                        BackContent = backcontent,
                        WideBackContent = widebackcontent,
                        Count = count,
                        SmallBackgroundImage = smallbackgroundimage,
                        BackgroundImage = backgroundimage,
                        BackBackgroundImage = backbackgroundimage,
                        WideBackgroundImage = widebackgroundimage,
                        WideBackBackgroundImage = widebackbackgroundimage,
                    };
                }
                else
                {
                    // Lazy templification of the elements
                    string titleElement = "<wp:Title";
                    string backTitleElement = "<wp:BackTitle";
                    string backContentElement = "<wp:BackContent";
                    string countElement = "<wp:Count";
                    string wideBackContentElement = "<wp:WideBackContent";
                    string smallBackgroundImageElement = "<wp:SmallBackgroundImage>";
                    string backgroundImageElement = "<wp:BackgroundImage>";
                    string backBackgroundImageElement = "<wp:BackBackgroundImage>";
                    string wideBackgroundImageElement = "<wp:WideBackgroundImage>";
                    string wideBackBackgroundImageElement = "<wp:WideBackBackgroundImage>";

                    // Determine if we're supposed to be clearing some values
                    if (count == 0) { countElement = countElement + " Action=\"Clear\">"; } else { countElement = countElement + ">"; }
                    if (title == "") { titleElement = titleElement + " Action=\"Clear\">"; } else { titleElement = titleElement + ">"; }
                    if (backtitle == "") { backTitleElement = backTitleElement + " Action=\"Clear\">"; } else { backTitleElement = backTitleElement + ">"; }
                    if (backcontent == "") { backContentElement = backContentElement + " Action=\"Clear\">"; } else { backContentElement = backContentElement + ">"; }
                    if (widebackcontent == "") { wideBackContentElement = wideBackContentElement + " Action=\"Clear\">"; } else { wideBackContentElement = wideBackContentElement + ">"; }

                    // Form the XML template for the tile
                    string tileDataXmlString =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\">" +
                           "<wp:Tile Id=\"" + tileId + "\" Template=\"FlipTile\">" +
                              titleElement + title + "</wp:Title>" +
                              backTitleElement + backtitle + "</wp:BackTitle>" +
                              backContentElement + backcontent + "</wp:BackContent>" +
                              wideBackContentElement + widebackcontent + "</wp:WideBackContent>" +
                              countElement + count + "</wp:Count>" +
                              smallBackgroundImageElement + smallbackgroundimage + "</wp:SmallBackgroundImage>" +
                              backgroundImageElement + backgroundimage + "</wp:BackgroundImage>" +
                              backBackgroundImageElement + backbackgroundimage + "</wp:BackBackgroundImage>" +
                              wideBackgroundImageElement + widebackgroundimage + "</wp:WideBackgroundImage>" +
                              wideBackBackgroundImageElement + widebackbackgroundimage + "</wp:WideBackBackgroundImage>" +
                           "</wp:Tile> " +
                        "</wp:Notification>";

                    // Construct FlipTileData using the overload with XML string
                    tileData = new FlipTileData(tileDataXmlString);
                }

                // Go do something to the tile based on the tile action
                DoTileAction(tileAction, tileId, tileToFind, tileData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        // Form our cycle tile
        public void CycleTile(
            Uri tileId,
            string title = null,
            int count = 0,
            Uri smallbackgroundimage = null,
            Uri cycleimage1 = null,
            Uri cycleimage2 = null,
            Uri cycleimage3 = null,
            Uri cycleimage4 = null,
            Uri cycleimage5 = null,
            Uri cycleimage6 = null,
            Uri cycleimage7 = null,
            Uri cycleimage8 = null,
            Uri cycleimage9 = null,
            string tileAction = null,
            bool tileProperties = false)
        {
            try
            {
                var uris = new List<Uri> 
                {
                    cycleimage1, 
                    cycleimage2, 
                    cycleimage3,
                    cycleimage4,
                    cycleimage5,
                    cycleimage6,
                    cycleimage7,
                    cycleimage8,
                    cycleimage9,
                };

                var tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileId.ToString()));

                CycleTileData tileData;

                if (tileProperties)
                {
                    tileData = new CycleTileData
                    {
                        Title = title,
                        Count = count,
                        SmallBackgroundImage = smallbackgroundimage,
                        CycleImages = uris,
                    };
                }
                else
                {
                    // Lazy templification of the elements
                    string titleElement = "<wp:Title";
                    string countElement = "<wp:Count";
                    string smallBackgroundImageElement = "<wp:SmallBackgroundImage>";
                    string cycleImage1Element = "<wp:CycleImage1>";
                    string cycleImage2Element = "<wp:CycleImage2>";
                    string cycleImage3Element = "<wp:CycleImage3>";
                    string cycleImage4Element = "<wp:CycleImage4>";
                    string cycleImage5Element = "<wp:CycleImage5>";
                    string cycleImage6Element = "<wp:CycleImage6>";
                    string cycleImage7Element = "<wp:CycleImage7>";
                    string cycleImage8Element = "<wp:CycleImage8>";
                    string cycleImage9Element = "<wp:CycleImage9>";

                    // Determine if we're supposed to be clearing some values
                    if (count == 0) { countElement = countElement + " Action=\"Clear\">"; } else { countElement = countElement + ">"; }
                    if (title == "") { titleElement = titleElement + " Action=\"Clear\">"; } else { titleElement = titleElement + ">"; }

                    // Form the XML template for the tile
                    string tileDataXmlString =
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\">" +
                           "<wp:Tile Id=\"" + tileId + "\" Template=\"CycleTile\">" +
                              countElement + count + "</wp:Count>" +
                              titleElement + title + "</wp:Title>" +
                              smallBackgroundImageElement + smallbackgroundimage + "</wp:SmallBackgroundImage>" +
                              cycleImage1Element + cycleimage1 + "</wp:CycleImage1>" +
                              cycleImage2Element + cycleimage2 + "</wp:CycleImage2>" +
                              cycleImage3Element + cycleimage3 + "</wp:CycleImage3>" +
                              cycleImage4Element + cycleimage4 + "</wp:CycleImage4>" +
                              cycleImage5Element + cycleimage5 + "</wp:CycleImage5>" +
                              cycleImage6Element + cycleimage6 + "</wp:CycleImage6>" +
                              cycleImage7Element + cycleimage7 + "</wp:CycleImage7>" +
                              cycleImage8Element + cycleimage8 + "</wp:CycleImage8>" +
                              cycleImage9Element + cycleimage9 + "</wp:CycleImage9>" +
                           "</wp:Tile> " +
                        "</wp:Notification>";

                    // Construct IconicTileData using the overload with XML string
                    tileData = new CycleTileData(tileDataXmlString);
                }

                // Go do something to the tile based on the tile action
                DoTileAction(tileAction, tileId, tileToFind, tileData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

    }
}
