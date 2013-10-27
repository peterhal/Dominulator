using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class OnBuyCardInPlayReaction : TurnletSpecification
    {
        private GameState state;
        private PlayerState player;
        private Card boughtCard;
        private Card reactingCardInPlay;

        public OnBuyCardInPlayReaction(GameState state, PlayerState player, Card boughtCard, Card reactingCardInPlay)
        {
            this.state = state;
            this.player = player;
            this.boughtCard = boughtCard;
            this.reactingCardInPlay = reactingCardInPlay;
        }

        public override void Execute(GameState state)
        {
        }
    }
}
