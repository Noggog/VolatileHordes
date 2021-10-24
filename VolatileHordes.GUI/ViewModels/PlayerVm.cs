using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using DynamicData;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerVm : ViewModel
    {
        public int EntityId { get; }
        [Reactive] public RectangleF SpawnRectangle { get; private set; }
        [Reactive] public float Rotation { get; private set; }

        public PlayerVm(int entityId)
        {
            EntityId = entityId;
        }

        public void Absorb(PlayerDto dto)
        {
            SpawnRectangle = dto.SpawnRectangle;
            Rotation = dto.Rotation % 360;
        }
    }
}