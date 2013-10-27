using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    class CleanupPhaseSpecification : TurnletSpecification
    {
        private PlayerState player;

        public CleanupPhaseSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            throw new NotImplementedException();
        }
    }
}
