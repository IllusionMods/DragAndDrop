using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using UnityEngine;

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
            }).ToList();

            if (goodFiles.Count() == 0)
            {
                Logger.LogMessage("No files to handle");
                return;
            }

            if (CardHandlerMethods.GetActiveCardHandler(out var cardHandler))
            {
                List<string> characterFiles = new List<string>();
                List<string> coordinateFiles = new List<string>();

                foreach (var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if (BoyerMoore.ContainsSequence(bytes, StudioToken))
                    {
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            cardHandler.Scene_Import(file, aPos);
                        else
                            cardHandler.Scene_Load(file, aPos);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, FemaleToken))
                    {
                        characterFiles.Add(file);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, MaleToken))
                    {
                        characterFiles.Add(file);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, FemaleCoordinateToken) || BoyerMoore.ContainsSequence(bytes, MaleCoordinateToken))
                    {
                        coordinateFiles.Add(file);
                    }
                    else if (BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else
                    {
                        Logger.LogMessage("This file does not contain any PlayHome related data");
                    }
                }

                if (characterFiles.Count > 0)
                {
                    var bytes = File.ReadAllBytes(characterFiles[0]);
                    var sex = new BoyerMoore(FemaleToken).Search(bytes, 0).Any() ? (byte)1 : (byte)0;
                    if (cardHandler is StudioHandler)
                    {
                        ((StudioHandler)cardHandler).Character_Load(characterFiles, aPos, sex);
                    }
                    else
                    {
                        foreach (var file in characterFiles)
                        {
                            cardHandler.Character_Load(file, aPos, sex);
                        }
                    }
                }

                if (coordinateFiles.Count > 0)
                {
                    if (cardHandler is StudioHandler)
                    {
                        ((StudioHandler)cardHandler).Coordinate_Load(coordinateFiles, aPos);
                    }
                    else
                    {
                        foreach (var file in coordinateFiles)
                        {
                            cardHandler.Coordinate_Load(file, aPos);
                        }
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
