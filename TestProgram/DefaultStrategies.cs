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
        public static class Default
        {
            public static CardPickByPriority EmptyPickOrder()
            {
                return new CardPickByPriority();
            }

            public static ICardPicker DefaultActionPlayOrder(ICardPicker purchaseOrder)
            {
                return new CardPickFromWhatsInHand(new SortCardByDefaultActionOrder(purchaseOrder));
            }

            private class CardPickFromWhatsInHand
                : ICardPicker
            {
                private readonly IComparerFactory comparerFactory;

                public CardPickFromWhatsInHand(IComparerFactory comparerFactory)
                {
                    this.comparerFactory = comparerFactory;
                }

                public int AmountWillingtoOverPayFor(Card card, GameState gameState)
                {
                    throw new NotImplementedException();
                }

                public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);
                    
                    Card cardToPlay = gameState.Self.Hand.Where(card => cardPredicate(card)).OrderBy(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay;
                }

                public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
                {
                    IComparer<Card> comparer = this.comparerFactory.GetComparer(gameState);

                    Card cardToPlay = gameState.Self.Hand.Where(card => cardPredicate(card)).OrderByDescending(card => card, comparer).FirstOrDefault();
                    if (cardToPlay == null)
                        return null;

                    return cardToPlay;
                }

                public IEnumerable<Card> GetNeededCards()
                {
                    yield break;
                }
            }

            private interface IComparerFactory
            {
                IComparer<Card> GetComparer(GameState gameState);
            }

            private class SortCardByDefaultActionOrder
                : IComparerFactory
            {
                private readonly ICardPicker purchaseOrder;

                public SortCardByDefaultActionOrder(ICardPicker purchaseOrder)
                {
                    this.purchaseOrder = purchaseOrder;
                }

                public IComparer<Card> GetComparer(GameState gameState)
                {
                    return new Comparer(gameState, purchaseOrder);
                }

                private class Comparer
                    : IComparer<Card>
                {
                    private readonly GameState gameState;
                    private readonly ICardPicker purchaseOrder;

                    public Comparer(GameState gameState, ICardPicker purchaseOrder)
                    {
                        this.gameState = gameState;
                        this.purchaseOrder = purchaseOrder;
                    }

                    public int Compare(Card first, Card second)
                    {
                        if (first.plusAction != 0 ^ second.plusAction != 0)
                        {
                            return first.plusAction != 0 ? -1 : 1;
                        }

                        if (first.DefaultCoinCost != second.DefaultCoinCost)
                        {
                            return first.DefaultCoinCost - second.DefaultCoinCost;
                        }

                        if (first.isRuins && second.isRuins)
                        {
                            int result = CompareRuins(first, second, this.gameState, this.purchaseOrder);
                            if (result != 0)
                                return result;
                        }

                        return 0;
                    }
                }

                // TODO:  implement a better default choice of which Ruins to player.
                private static int CompareRuins(Card first, Card second, GameState gameState, ICardPicker purchaseOrder)
                {
                    PlayerState self = gameState.Self;

                    int coinsToSpend = self.ExpectedCoinValueAtEndOfTurn;

                    if (first == Cards.AbandonedMine || second == Cards.AbandonedMine)
                    {
                        CardPredicate shouldGainCard = delegate(Card card)
                        {
                            int currentCardCost = card.CurrentCoinCost(self);

                            return currentCardCost == coinsToSpend + 1;
                        };

                        Card cardType = purchaseOrder.GetPreferredCard(gameState, shouldGainCard);
                        if (cardType != null)
                            return first == Cards.AbandonedMine ? 0 : 1;

                        //Card Card1 = purchaseOrder.GetPreferredCard(
                        //        gameState,
                        //        card => coinsToSpend >= card.CurrentCoinCost(currentPlayer) &&
                        //        gameState.GetPile(card).Any());
                        //Card Card2 = purchaseOrder.GetPreferredCard(
                        //        gameState,
                        //        card => coinsToSpend + 1 >= card.CurrentCoinCost(currentPlayer) &&
                        //        gameState.GetPile(card).Any());

                        //if (Card1 != Card2)
                        //    return first.name == "Abandoned Mine" ? 0 : 1;
                    }
                    return 0;
                }
            }

            public static CardPickByPriority DefaultTreasurePlayOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Contraband),       // play early to provide opponent as little information when banning
                    // base set first
                    CardAcceptance.For(Cards.Platinum),
                    CardAcceptance.For(Cards.Gold),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Copper),
                    CardAcceptance.For(Cards.Spoils),
                    CardAcceptance.For(Cards.Potion),
                    // alphabetical, all other treasures that dont really depend on order
                    CardAcceptance.For(Cards.Cache),
                    CardAcceptance.For(Cards.FoolsGold),
                    CardAcceptance.For(Cards.Loan),
                    CardAcceptance.For(Cards.Harem),
                    CardAcceptance.For(Cards.Hoard),
                    CardAcceptance.For(Cards.Masterpiece),
                    CardAcceptance.For(Cards.PhilosophersStone),
                    CardAcceptance.For(Cards.Quarry),
                    CardAcceptance.For(Cards.Stash),
                    CardAcceptance.For(Cards.Talisman),
                    // cards whose benefit is sensitive to ordering
                    CardAcceptance.For(Cards.Venture),          // playing this card might increase the number of treasures played
                    CardAcceptance.For(Cards.CounterFeit),      // after venture so that you have more variety to counterfeit
                    CardAcceptance.For(Cards.IllGottenGains),   // by playing after venture, you have more information about whether to gain the copper
                    CardAcceptance.For(Cards.HornOfPlenty),     // play relatively last so it has the most variety of cards to trigger with
                    CardAcceptance.For(Cards.Bank));            // try to make bank as valuable as possibile.
            }

            public static CardPickByPriority DefaultDiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Province),
                    CardAcceptance.For(Cards.Duchy),
                    CardAcceptance.For(Cards.Estate),
                    CardAcceptance.For(Cards.OvergrownEstate),
                    CardAcceptance.For(Cards.Hovel),
                    CardAcceptance.For(Cards.Ruins),
                    CardAcceptance.For(Cards.Curse),
                    CardAcceptance.For(Cards.Copper));
            }

            public static ICardPicker DefaultTrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Curse),
                    CardAcceptance.For(Cards.RuinedVillage),
                    CardAcceptance.For(Cards.RuinedMarket),
                    CardAcceptance.For(Cards.Survivors),
                    CardAcceptance.For(Cards.RuinedLibrary),
                    CardAcceptance.For(Cards.AbandonedMine), 
                    CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) == 0),
                    CardAcceptance.For(Cards.OvergrownEstate),
                    CardAcceptance.For(Cards.Hovel),
                    CardAcceptance.For(Cards.Copper));
            }        

            public static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(Cards.Gold, gameState) > 2;
            }

            public static bool ShouldGainIllGottenGains(GameState gameState)
            {
                return CountOfPile(Cards.Curse, gameState) > 0;
            }

            public static bool ShouldPlaySalvager(ICardPicker trashOrder, GameState gameState)
            {
                return HasCardFromInHand(trashOrder, gameState);
            }

            public static GameStatePredicate ShouldPlaySalvager(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    return HasCardFromInHand(trashOrder, gameState);
                };
            }

            public static GameStatePredicate ShouldPlayLookout(GameStatePredicate shouldBuyProvinces = null)
            {
                if (shouldBuyProvinces == null)
                {
                    shouldBuyProvinces = ShouldBuyProvinces;
                }
                return delegate(GameState gameState)
                {
                    return ShouldPlayLookout(gameState, shouldBuyProvinces);
                };
            }

            public static bool ShouldPlayLookout(GameState gameState, GameStatePredicate shouldBuyProvinces)
            {
                int cardCountToTrash = CountInDeck(Cards.Copper, gameState);

                if (!shouldBuyProvinces(gameState))
                {
                    cardCountToTrash += CountInDeck(Cards.Estate, gameState);
                }

                cardCountToTrash += CountInDeck(Cards.Hovel, gameState);
                cardCountToTrash += CountInDeck(Cards.Necropolis, gameState);
                cardCountToTrash += CountInDeck(Cards.OvergrownEstate, gameState);

                cardCountToTrash += CountInDeck(Cards.Lookout, gameState);

                int totalCardsOwned = gameState.Self.CardsInDeck.Count;

                return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
            }
        }
    }
}