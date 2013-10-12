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
        public static class BigMoneyDoubleJack
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyDoubleJack",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.JackOfAllTrades.card, gameState => CountAllOwned(CardTypes.JackOfAllTrades.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.JackOfAllTrades.card, gameState => CountAllOwned(CardTypes.JackOfAllTrades.card, gameState) < 2 && gameState.Self.AllOwnedCards.Count > 15),                           
                           CardAcceptance.For(CardTypes.Silver.card));
            }            
        }

        public static class BigMoneyDoubleJackSlog
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyDoubleJackSlog",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.JackOfAllTrades.card, gameState => CountAllOwned(CardTypes.JackOfAllTrades.card, gameState) < 3),                           
                           CardAcceptance.For(CardTypes.Silver.card));
            }
        }
    }
}
