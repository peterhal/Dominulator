using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public sealed class PlayCardSpecification : TurnletSpecification
    {
        private Card card;
        private PlayerState player;

        public PlayCardSpecification(Card card, PlayerState player)
        {
            this.card = card;
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            player.gameLog.PlayedCard(player, card);
            player.gameLog.PushScope();

            state.AddTurnlet(new EndPlayCardSpecification(player));
            card.Play(state, player);
        }
    }
}
