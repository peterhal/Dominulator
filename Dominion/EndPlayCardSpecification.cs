using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class EndPlayCardSpecification : TurnletSpecification
    {
        private PlayerState player;

        public EndPlayCardSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            player.gameLog.PopScope();
        }
    }
}
