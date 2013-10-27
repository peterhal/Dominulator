using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    // Begins a player's turn.
    public sealed class BeginTurnSpecification : TurnletSpecification
    {
        private PlayerState player;

        public BeginTurnSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            state.players.CurrentPlayer = player;
            player.InitializeTurn();

            // TODO: Cache the turnlets so that they don't need to be allocated each turn.
            state.AddTurnlet(new BeginTurnSpecification(state.players.NextPlayer(player)));
            state.AddTurnlet(new CleanupPhaseSpecification(player));
            state.AddTurnlet(new PlayTreasuresSpecification(player));
            state.AddTurnlet(new BuyPhaseSpecification(player));
            state.AddTurnlet(new ActionPhaseSpecification(player));
            // TODO: Add Before turn turnlets...
        }
    }
}