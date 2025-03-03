﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevitDBExplorer.Domain.RevitDatabaseQuery;
using RevitDBExplorer.Domain.RevitDatabaseQuery.FuzzySearch;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;
using RevitTestLibrary.MSTest;

namespace RevitDBExplorer.Tests.RDQ
{
    [TestClass]
    public class CommandParserTests
    {
        [RevitTestMethod]
        [DataRow("BuiltInCategory.OST_Walls")]
        [DataRow("OST_Walls")]
        [DataRow("Walls")]
        [DataRow("c:BuiltInCategory.OST_Walls")]
        [DataRow("c:OST_Walls")]
        [DataRow("c:Walls")]
        public void CanParseCategoryCommand(UIApplication uia, string cmd)
        {           
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(CategoryCmd));

            var match = result.Arguments.First() as CategoryCmdArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(BuiltInCategory.OST_Walls, match.Value);
        }

        [RevitTestMethod]
        [DataRow("Wall")]       
        [DataRow("t:Wall")]     
        public void CanParseClassCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(ClassCmd));

            var match = result.Arguments.First() as ClassCmdArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(typeof(Wall), match.Value);

        }

        [RevitTestMethod]
        [DataRow("123456")]
        [DataRow("i:123456")]
        public void CanParseElementIdCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(ElementIdCmd));

            var match = result.Arguments.First() as ElementIdCmdArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(new ElementId(123456), match.Value);
        }

        [RevitTestMethod]
        [DataRow("element type")]
        [DataRow("elementtype")]
        [DataRow("type")]
        public void CanParseElementTypeCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(ElementTypeCmd));
        }

        [RevitTestMethod]
        [DataRow("Level 1")]
        [DataRow("l:Level 1")] 
        public void CanParseLevelCommand(UIApplication uia, string cmd)
        {
            var document = uia.Application.NewProjectDocument(UnitSystem.Metric);

            
            CommandParser.LoadDocumentSpecificData(document);
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(LevelCmd));

            var match = result.Arguments.First() as LevelCmdArgument;
            Assert.IsNotNull(match);
            //Assert.AreEqual(new ElementId(????), match.Value);

            document.Close(false);
        }

        [RevitTestMethod]
        [DataRow("W1")]
        [DataRow("n:W1")]
        public void CanParseNameCommand(UIApplication uia, string cmd)
        { 
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(ParameterCmd));

            var match = result.Arguments.First() as ParameterArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(BuiltInParameter.ALL_MODEL_TYPE_NAME, match.BuiltInParameter);

            var op = result.Operator;
            Assert.IsNotNull(op);
            Assert.AreEqual(op.Evaluate().String, "%W1%");            
        }

        [RevitTestMethod]
        [DataRow("not element type")]     
        [DataRow("not type")]
        public void CanParseNotElementTypeCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(NotElementTypeCmd));
        }

        [RevitTestMethod]
        [DataRow("DOOR_NUMBER = 7")]
        [DataRow("BuiltInParameter.DOOR_NUMBER = 7")]
        [DataRow("mARK = 7")]
        public void CanParseParameterCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(ParameterCmd));

            var match = result.Arguments.First() as ParameterArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(BuiltInParameter.DOOR_NUMBER, match.BuiltInParameter);

            var op = result.Operator;
            Assert.IsNotNull(op);
            Assert.AreEqual(op.Evaluate().Int, 7);
        }

        [RevitTestMethod]
        [DataRow("myRoom")]
        [DataRow("r:myRoom")]
        public void CanParseRoomCommand(UIApplication uia, string cmd)
        {
            var path = Path.Combine(GetDir(), @"..\..\assets\testmodel_rdq.rvt");
            var document = uia.Application.OpenDocumentFile(path);
            
            CommandParser.LoadDocumentSpecificData(document);

            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(RoomCmd));

            var match = result.Arguments.First() as RoomCmdArgument;
            Assert.IsNotNull(match);
            //Assert.AreEqual(???, match.Value);

            document.Close(false);
        }

        [RevitTestMethod]
        [DataRow("mySelectionFilter")]
        [DataRow("f:mySelectionFilter")]
        public void CanParseRuleBasedFilterCommand(UIApplication uia, string cmd)
        {
            var path = Path.Combine(GetDir(), @"..\..\assets\testmodel_rdq.rvt");
            var document = uia.Application.OpenDocumentFile(path);
     
            CommandParser.LoadDocumentSpecificData(document);

            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(RuleBasedFilterCmd));

            var match = result.Arguments.First() as RuleBasedFilterCmdArgument;
            Assert.IsNotNull(match);
            //Assert.AreEqual(???, match.Value);

            document.Close(false);
        }

        [RevitTestMethod]
        [DataRow("StructuralType.Brace")]
        [DataRow("s:Brace")]
        public void CanParseStructuralTypeCommand(UIApplication uia, string cmd)
        { 
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(StructuralTypeCmd));

            var match = result.Arguments.First() as StructuralTypeCmdArgument;
            Assert.IsNotNull(match);
            Assert.AreEqual(StructuralType.Brace, match.Value);            
        }


        [RevitTestMethod]
        [DataRow("visible")]
        [DataRow("visible in view")]
        public void CanParseVisibleInViewCommand(UIApplication uia, string cmd)
        {
            var result = CommandParser.Parse(cmd).First();
            Assert.IsInstanceOfType(result, typeof(VisibleInViewCmd));

            //var match = result.MatchedArguments.First() as VisibleInViewMatch;
            //Assert.IsNotNull(match);
            //Assert.AreEqual(StructuralType.Brace, match.Value);
        }





        public static string GetDir()
        {           
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);            
        }
    }
}