using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    // A TurnletSpecification is a specification of an action or choice.
    // For example: select a card to play, draw a card.
    // Executing a TurnletSpecification results in a Turnlet, which 
    // describes the change in GameState caused by executing the
    // TurnletSpecification.
    public abstract class TurnletSpecification
    {
        protected TurnletSpecification()
        {
        }

        // Execute's this TurnetSpecification on the given GameState.
        // Note that this TurnletSpecification will be removed from 
        // the GameState's TurnletQueue before being executed.
        public abstract void Execute(GameState state);
    }
}
