using System;
using TR.Utilities;
using UnityEngine;

namespace TR.TGraphics;

public static class TRTibWidgets
{
    public static Color ColorFor(Enum enumType)
    {
        if (enumType is TiberiumValueType tibType)
        {
            return tibType.GetColor();
        }

        if (enumType is AtmosphericValueType atmosType)
        {

        }
        return Color.white;
    }
}