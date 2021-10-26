using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI.ViewModels
{
    public class LimitsVm : ViewModel
    {
        [Reactive] public int GameMaximum { get; set; } = 1;
        [Reactive] public int CurrentNumber { get; set; }
        [Reactive] public int DesiredMaximum { get; set; }

        public void AbsorbIn(ZombieLimitsDto? dto)
        {
            if (dto == null)
            {
                GameMaximum = 1;
                CurrentNumber = 0;
                DesiredMaximum = 0;
            }
            else
            {
                GameMaximum = dto.GameMaximum;
                CurrentNumber = dto.CurrentNumber;
                DesiredMaximum = dto.DesiredMaximum;
            }
        }
    }
}