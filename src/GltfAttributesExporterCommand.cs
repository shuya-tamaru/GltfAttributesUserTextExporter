using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace GltfAttributesExporter
{
    public class GltfAttributesExporterCommand : Command
    {
        public GltfAttributesExporterCommand()
        {
            Instance = this;
        }

        public static GltfAttributesExporterCommand Instance { get; private set; }

        public override string EnglishName => "GltfAttributesExport";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command is under construction.", EnglishName);
            return Result.Success;
        }
    }
}
