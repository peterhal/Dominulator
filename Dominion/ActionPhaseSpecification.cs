using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class ActionPhaseSpecification : TurnletSpecification
    {
        private PlayerState player;

        public ActionPhaseSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            player.playPhase = PlayPhase.Action;
            if (player.AvailableActions > 0)
            {
                var card = player.RequestPlayerChooseCardToRemoveFromHandForPlay(
                    state,
                    Delegates.IsActionCardPredicate,
                    isTreasure: false,
                    isAction: true,
                    isOptional: true);
                if (card != null)
                {
                    if (!card.isAction)
                    {
                        throw new Exception("Can't play a non-action card.");
                    }

                    // Stay in Action phase after card is played.
                    state.AddTurnlet(this);

                    // Consume an action.
                    player.turnCounters.RemoveAction();

                    // Place card in play area.
                    // TODO: Handle duration cards
                    player.cardsPlayed.AddCard(card);
                    
                    // Play the card.
                    state.AddTurnlet(new PlayCardSpecification(card, player));
                }
            }
        }
    }
}
