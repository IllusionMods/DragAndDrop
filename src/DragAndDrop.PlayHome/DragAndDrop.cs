using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;

namespace DragAndDrop
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore
    {
        private static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("【PHStudio】");
        private static readonly byte[] FemaleToken = Encoding.UTF8.GetBytes("PlayHome_Female");
        private static readonly byte[] MaleToken = Encoding.UTF8.GetBytes("PlayHome_Male");
        private static readonly byte[] FemaleCoordinateToken = Encoding.UTF8.GetBytes("PlayHome_FemaleCoordinate");
        private static readonly byte[] MaleCoordinateToken = Encoding.UTF8.GetBytes("PlayHome_MaleCoordinate");
        private static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");

        internal override void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png" || ext == ".dat";
            });

            if(goodFiles.Count() == 0)
            {
                Logger.LogMessage("No files to handle");
                return;
            }

            if(CardHandlerMethods.GetActiveCardHandler(out var cardHandler))
            {
                foreach(var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, StudioToken))
                    {
                        cardHandler.Scene_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, FemaleToken))
                    {
                        cardHandler.Character_Load(file, aPos, 1);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, MaleToken))
                    {
                        cardHandler.Character_Load(file, aPos, 0);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, FemaleCoordinateToken) || BoyerMoore.ContainsSequence(bytes, MaleCoordinateToken))
                    {
                        cardHandler.Coordinate_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any PlayHome related data");
                    }
                }
            }
            else
            {
                Logger.LogMessage("No handler found for this scene");
            }
        }
    }
}
