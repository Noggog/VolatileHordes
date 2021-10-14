using NSubstitute;
using NUnit.Framework;
using VolatileHordes.Allocation;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Settings.User.Allocation;

namespace VolatileHordes.Tests.Director
{
    public class GameStageCalculatorTests
    {
        private IPlayer GetPlayer(int gamestage)
        {
            var ret = Substitute.For<IPlayer>();
            ret.TryGameStage().Returns(gamestage);
            return ret;
        }
        
        [Test]
        public void Empty()
        {
            var calc = new GameStageCalculator(new AllocationSettings());
            var group = new PlayerGroup(calc);
            Assert.True(calc.GetGamestage(group).EqualsWithin(0));
        }
        
        [Test]
        public void Single()
        {
            var calc = new GameStageCalculator(new AllocationSettings());
            var group = new PlayerGroup(calc);
            group.Players.Add(new PlayerZone(group, GetPlayer(3)));
            Assert.True(calc.GetGamestage(group).EqualsWithin(3));
        }
        
        [Test]
        public void Multiple()
        {
            var calc = new GameStageCalculator(new AllocationSettings());
            var group = new PlayerGroup(calc);
            group.Players.Add(new PlayerZone(group, GetPlayer(5)));
            group.Players.Add(new PlayerZone(group, GetPlayer(1)));
            Assert.True(calc.GetGamestage(group).EqualsWithin(5.2f));
        }
    }
}