using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class GainCardSpecification : TurnletSpecification
    {
        private GameState state;
        private PlayerState player;
        private Card card;

        public GainCardSpecification(GameState state, PlayerState player, Card card)
        {
            this.state = state;
            this.player = player;
            this.card = card;
        }

        public override void Execute(GameState state)
        {
            throw new NotImplementedException();
        }
    }
}
