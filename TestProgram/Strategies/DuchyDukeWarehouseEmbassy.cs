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
        public static class DuchyDukeWarehouseEmbassy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "DuchyDukeWarehouseEmbassy",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For(Cards.Embassy, gameState => CountAllOwned(Cards.Embassy, gameState) < 1),
                     CardAcceptance.For(Cards.Duchy),
                     CardAcceptance.For(Cards.Duke));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Silver));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Warehouse),
                           CardAcceptance.For(Cards.Embassy));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Duchy),
                    CardAcceptance.For(Cards.Duke),
                    CardAcceptance.For(Cards.Estate),
                    CardAcceptance.For(Cards.Copper),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.Copper),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Embassy),
                    CardAcceptance.For(Cards.Gold));
            }
        }
    }
}
