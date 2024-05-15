using Rhino;

namespace GltfAttributesExporter.Utilities
{
    public static class Utility
    {
        public static float ScaleModel(RhinoDoc doc)
        {
            var modelUnit = doc.ModelUnitSystem;
            var scale = 1.0f;

            switch (modelUnit)
            {
                case UnitSystem.None:
                case UnitSystem.Meters:
                    scale = 1.0f;
                    break;
                case UnitSystem.Millimeters:
                    scale = 0.001f;
                    break;
                case UnitSystem.Centimeters:
                    scale = 0.01f;
                    break;
                case UnitSystem.Inches:
                    scale = 0.0254f;
                    break;
                case UnitSystem.Feet:
                    scale = 0.3048f;
                    break;
            }
            return scale;
        }
    }
}
