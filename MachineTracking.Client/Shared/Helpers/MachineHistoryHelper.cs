using MachineTracking.Domain.DTOs.MachineHistory;
using MachineTracking.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MachineTracking.Client.Shared.Helpers
{
    public static class MachineHistoryHelper
    {
        public static int MapStatusToStatusId(MachineHistoryDTO machineHistory)
        {
            return machineHistory.Status switch
            {
                "EngineOn" when machineHistory.ChainMovesPerSecond > 0 => (int)MachineStatusTypeEnum.Active,
                "EngineOn" when machineHistory.ChainMovesPerSecond == 0 => (int)MachineStatusTypeEnum.Idle,
                "EngineOff" when machineHistory.ChainMovesPerSecond <= 0 => (int)MachineStatusTypeEnum.Inactive,
                _ => (int)MachineStatusTypeEnum.Unexpected
            };
        }

        public static RenderFragment GetStatusChip(int statusId) => builder =>
        {
            builder.OpenComponent<MudChip<int>>(0);
            builder.AddAttribute(1, "Style", "margin: 0;");
            builder.AddAttribute(2, "Color", GetChipColor(statusId));
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(chipBuilder =>
            {
                chipBuilder.AddContent(0, Enum.GetName(typeof(MachineStatusTypeEnum), statusId) ?? statusId.ToString());
            }));
            builder.CloseComponent();
        };

        public static Color GetChipColor(int statusId)
        {
            return statusId switch
            {
                (int)MachineStatusTypeEnum.Active => Color.Success,
                (int)MachineStatusTypeEnum.Inactive => Color.Default,
                (int)MachineStatusTypeEnum.Idle => Color.Warning,
                (int)MachineStatusTypeEnum.Unexpected => Color.Error,
                _ => Color.Error
            };
        }

        public static string GetRelativeTime(DateTime dataReceivedData)
        {
            var timeSpan = DateTime.Now - dataReceivedData;

            if (timeSpan.TotalSeconds < 60)
                return $"{(int)timeSpan.TotalSeconds} seconds ago";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";

            return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }

        public static TimeSpan FilterMachineData(MachineLineChartTimeEnum selectedTimeRange)
        {
            return selectedTimeRange switch
            {
                MachineLineChartTimeEnum.Last_1_Hour => TimeSpan.FromHours(1),
                MachineLineChartTimeEnum.Last_2_Hour => TimeSpan.FromHours(2),
                MachineLineChartTimeEnum.Last_3_Hour => TimeSpan.FromHours(3),
                MachineLineChartTimeEnum.Last_6_Hour => TimeSpan.FromHours(6),
                MachineLineChartTimeEnum.Last_12_Hour => TimeSpan.FromHours(12),
                MachineLineChartTimeEnum.Last_24_Hour => TimeSpan.FromHours(24),
                _ => TimeSpan.Zero
            };
        }
    }
}
