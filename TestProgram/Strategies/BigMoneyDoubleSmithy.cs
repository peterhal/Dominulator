﻿using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class BigMoneyDoubleSmithy
        {

            public static PlayerAction Player(int playerNumber)
            {
                return CustomPlayer(playerNumber);
            }
            // big money smithy player
            public static PlayerAction CustomPlayer(int playerNumber, int secondSmithy = 15)
            {
                return new PlayerAction(
                            "BigMoneyDoubleSmithy",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(secondSmithy));
            }

            private static CardPickByPriority PurchaseOrder(int secondSmithy)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Smithy, gameState => CountAllOwned(Cards.Smithy, gameState) < 1),
                           CardAcceptance.For(Cards.Smithy, gameState => CountAllOwned(Cards.Smithy, gameState) < 2 &&
                                                                             gameState.Self.AllOwnedCards.Count >= secondSmithy),
                           CardAcceptance.For(Cards.Silver));

            }           
        }
    }
}
