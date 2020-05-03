using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DragAndDrop
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class DragAndDrop : DragAndDropCore
    {
        private static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【AIS_Chara】");
        private static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        private static readonly byte[] StudioOldToken = Encoding.UTF8.GetBytes("KStudio"); // Compatibility with older scenes created before the 11-08 update
        private static readonly byte[] StudioNewToken = Encoding.UTF8.GetBytes("StudioNEOV2");
        private static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("AIS_Clothes");
        private static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");
        private static readonly byte[] HouseToken = Encoding.UTF8.GetBytes("【AIS_Housing】");
        
        internal override void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png";
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

                    if(BoyerMoore.ContainsSequence(bytes, StudioNewToken) || BoyerMoore.ContainsSequence(bytes, StudioOldToken))
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            cardHandler.Scene_Import(file, aPos);
                        else
                            cardHandler.Scene_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CharaToken))
                    {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.Character_Load(file, aPos, sex);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CoordinateToken))
                    {
                        cardHandler.Coordinate_Load(file, aPos);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, HouseToken))
                    {
                        cardHandler.HouseData_Load(file, Logger);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any AIS related data");
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
