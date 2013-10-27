using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    // The ordered sequence of TurnletSpecifications which are to be executed in the game.
    // Executing TurnletSpecifications produces Turnlets which represent mutations to the
    // GameState. Many Turnlets represent mutations to the TurnletQueue.
    //
    // In retrospect, this may better be called the TurnletStack...
    public sealed class TurnletQueue
    {
        public TurnletQueue(GameState state)
        {
            this.state = state;
        }

        private GameState state;
        private Stack<TurnletSpecification> queue;

        public TurnletSpecification PopNext()
        {
            return queue.Pop();
        }

        public void Add(TurnletSpecification turnlet)
        {
            queue.Push(turnlet);
        }
    }
}
