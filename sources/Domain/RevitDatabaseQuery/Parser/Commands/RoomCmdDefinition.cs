﻿using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands
{
    internal class RoomCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("r: ", "r:[room]", "select elements from a given room");
        private readonly DataBucket<RoomCmdArgument> dataBucket = new DataBucket<RoomCmdArgument>(0.61);
              

        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var room in new FilteredElementCollector(document).WherePasses(new Autodesk.Revit.DB.Architecture.RoomFilter()))
            {
                dataBucket.Add(new AutocompleteItem(room.Name, room.Name, null), new RoomCmdArgument(room.Id, room.Name), room.Name);

            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
            return dataBucket.ProvideAutoCompletion(prefix);
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "r";
            yield return "room";        
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => true;


        public ICommand Create(string cmdText, string argument)
        {
            var args = dataBucket.FuzzySearch(argument);
            return new RoomCmd(cmdText, args);
        }
    }


    internal class RoomCmdArgument : CommandArgument<ElementId>
    {
        public RoomCmdArgument(ElementId roomId, string name) : base(roomId)
        {
            CmdType = CmdType.Room;
            Name = $"{name}.ClosedShell";
            Label = name;
        }
    }


    internal class RoomCmd : Command
    {
        public RoomCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(CmdType.Room, text, matchedArguments, null)
        {
        }
    }
}