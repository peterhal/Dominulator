using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class OnBuySpecification : TurnletSpecification
    {
        private GameState state;
        private PlayerState player;
        private Card boughtCard;

        public OnBuySpecification(GameState state, PlayerState player, Card boughtCard)
        {
            this.state = state;
            this.player = player;
            this.boughtCard = boughtCard;
        }

        public override void Execute(GameState state)
        {
            // embargo
            int embargoCount = state.pileEmbargoTokenCount[boughtCard];
            for (int i = 0; i < embargoCount; ++i)
            {
                state.AddTurnlet(new GainCardSpecification(state, player, Cards.Curse));
            }

            // on-buy reactions from cards in play
            if (player.ownsCardWithSpecializedActionOnBuyWhileInPlay)
            {
                foreach (Card reactingCardInPlay in player.CardsInPlay.Where(cardInPlay => cardInPlay.HasSpecializedActionOnBuyWhileInPlay))
                {
                    state.AddTurnlet(new OnBuyCardInPlayReaction(state, player, boughtCard, reactingCardInPlay));
                }
            }
            // TODO: on-buy reactions from cards in hand

            // gain card
            state.AddTurnlet(new GainCardSpecification(state, player, boughtCard));
        }
    }
}
