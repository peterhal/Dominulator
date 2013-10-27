using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class PlayTreasuresSpecification : TurnletSpecification
    {
        private PlayerState player;

        public PlayTreasuresSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            player.playPhase = PlayPhase.PlayTreasure;
            var card = player.actions.GetTreasureFromHandToPlay(state, acceptableCard => true, isOptional: true);
            if (card != null)
            {
                if (!card.isTreasure)
                {
                    throw new Exception("Can't play non-treasure in play treasures phase");
                }

                // Stay in play treasure state.
                state.AddTurnlet(this);

                player.RemoveCardFromHand(card);

                // Place card in play area.
                player.cardsPlayed.AddCard(card);

                // Play the card.
                state.AddTurnlet(new PlayCardSpecification(card, player));
            }
        }
    }
}
