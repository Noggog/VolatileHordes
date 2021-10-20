using NSubstitute;
using NUnit.Framework;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Settings.User.Director;

namespace VolatileHordes.Tests.Director
{
    //public class GameStageCalculatorTests
    //{
    //    private IPlayer GetPlayer(int gamestage)
    //    {
    //        var ret = Substitute.For<IPlayer>();
    //        ret.TryGameStage().Returns(gamestage);
    //        return ret;
    //    }
        
    //    [Test]
    //    public void Empty()
    //    {
    //        var calc = new GameStageCalculator(new DirectorSettings());
    //        var group = new PlayerParty(calc);
    //        Assert.True(calc.GetGamestage(group).EqualsWithin(0));
    //    }
        
    //    [Test]
    //    public void Single()
    //    {
    //        var calc = new GameStageCalculator(new DirectorSettings());
    //        var group = new PlayerParty(calc)
    //        {
    //            players = { { 0, GetPlayer(3) } }
    //        };
    //        Assert.True(calc.GetGamestage(group).EqualsWithin(3));
    //    }
        
    //    [Test]
    //    public void Multiple()
    //    {
    //        var calc = new GameStageCalculator(new DirectorSettings());
    //        var group = new PlayerParty(calc)
    //        {
    //            players = { { 0, GetPlayer(5) }, { 1, GetPlayer(1) } }
    //        };
    //        Assert.True(calc.GetGamestage(group).EqualsWithin(5.2f));
    //    }
    //}
}