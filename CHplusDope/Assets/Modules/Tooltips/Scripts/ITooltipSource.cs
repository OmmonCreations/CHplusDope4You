using System.Collections.Generic;
using StateMachines;

namespace Tooltips
{
    public interface ITooltipSource
    {
        TooltipController ShowTooltip(ISpatialTargetable target, Tooltip tooltip);

        TooltipController ShowTooltip(ICanvasTargetable target, Tooltip tooltip);
    }
}