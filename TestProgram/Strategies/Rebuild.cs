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
        public static class Rebuild
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("Rebuild",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: Default.TreasurePlayOrder(),
                        actionOrder: ActionOrder())
                {
                }

                public override Type NameACard(GameState gameState)
                {
                    
                    PlayerState currentPlayer = gameState.players.CurrentPlayer;
                    
                    if (gameState.GetPile<CardTypes.Duchy>().Count() == 0)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    if (CountMightDraw<CardTypes.Province>(gameState) > 0)
                    {
                        return typeof(CardTypes.Province);
                    }

                    if (CountMightDraw<CardTypes.Estate>(gameState) > 0)
                    {
                        return typeof(CardTypes.Duchy);
                    }                    
                    
                    return typeof(CardTypes.Province);
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountAllOwned<CardTypes.Estate>(gameState) < CountAllOwned<CardTypes.Rebuild>(gameState)),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2 || gameState.GetPile<CardTypes.Duchy>().Count() == 0),
                           CardAcceptance.For<CardTypes.Rebuild>(gameState => CountAllOwned<CardTypes.Rebuild>(gameState) < 3),
                           CardAcceptance.For<CardTypes.Gold>(),                           
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Rebuild>());
            }                        
        }
    }
}