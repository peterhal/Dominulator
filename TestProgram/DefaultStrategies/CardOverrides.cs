﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;

namespace Program.DefaultStrategies
{
    static class DefaultResponses
    {
        public static MapOfCardsFor<IPlayerAction> GetCardResponses(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<IPlayerAction>();

            result[Cards.Ambassador]       = new Ambassador(playerAction);
            result[Cards.Alchemist]        = new Alchemist(playerAction);
            result[Cards.BandOfMisfits]    = new BandOfMisfits(playerAction);
            result[Cards.Cartographer]     = new Cartographer(playerAction);
            result[Cards.Catacombs]        = new Catacombs(playerAction);            
            result[Cards.Chancellor]       = new Chancellor(playerAction);
            result[Cards.Golem]            = new Golem(playerAction);
            result[Cards.HorseTraders]     = new HorseTraders(playerAction);
            result[Cards.IllGottenGains]   = new IllGottenGainsAlwaysGainCopper(playerAction);
            result[Cards.Library]          = new Library(playerAction);
            result[Cards.MarketSquare]     = new MarketSquare(playerAction);
            result[Cards.Rebuild]          = new Rebuild(playerAction);
            result[Cards.Trader]           = new Trader(playerAction);
            result[Cards.Watchtower]       = new Watchtower(playerAction);

            return result;
        }

        public static MapOfCardsFor<GameStatePlayerActionPredicate> GetCardShouldPlayDefaults(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<GameStatePlayerActionPredicate>();

            result[Cards.Remodel] = Strategies.HasCardToTrashInHand;
            result[Cards.Salvager] = Strategies.HasCardToTrashInHand;
            result[Cards.Lookout] = Lookout.ShouldPlay;            

            return result;
        }
    }

    internal class Alchemist
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Alchemist(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return true;
        }
    }

    internal class Ambassador
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Ambassador(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return playerAction.trashOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
        }

        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            return 2;
        }
    }

    internal class BandOfMisfits
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public BandOfMisfits(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            throw new NotImplementedException();
        }
    }

    internal class Cartographer
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Cartographer(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            // good for cartographer, not sure about anyone else.
            foreach (Card card in gameState.Self.CardsBeingRevealed)
            {
                bool shouldDiscard = card.isVictory || card == Cards.Copper;
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
        }
    }

    internal class Catacombs
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Catacombs(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            return PlayerActionChoice.PutInHand;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }
    }

    internal class Chancellor
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Chancellor(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return true;
        }
    }

    internal class Golem
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Golem(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            Card result = playerAction.actionOrder.GetPreferredCard(
                gameState,
                card => card == card1 || card == card2);

            // choose a reasonable default
            if (result == null)
            {
                result = playerAction.defaultActionOrder.GetPreferredCard(
                    gameState,
                    card => card == card1 || card == card2);
            }

            return result;
        }
    }

    internal class HorseTraders
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public HorseTraders(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGainsAlwaysGainCopper
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGainsAlwaysGainCopper(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGains
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGains(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            Card result = playerAction.gainOrder.GetPreferredCard(
                gameState,
                c => c == card);

            // do a reasonable default for gaining copper on ill gotten gain
            if (result == null)
            {
                return ShouldGainCopper(gameState, playerAction.purchaseOrder);                
            }

            return false;
        }

        private static bool ShouldGainCopper(GameState gameState, ICardPicker gainOrder)
        {
            PlayerState self = gameState.Self;

            int minValue = self.ExpectedCoinValueAtEndOfTurn;
            int maxValue = minValue + Strategies.CountInHand(Cards.IllGottenGains, gameState);

            if (maxValue == minValue)
                return false;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(self);

                return currentCardCost >= minValue &&
                        currentCardCost <= maxValue;
            };

            Card cardType = gainOrder.GetPreferredCard(gameState, shouldGainCard);
            if (cardType == null)
                return false;

            int coppersToGain = PlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
        }
    }

    internal class Library
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Library(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return !playerAction.discardOrder.DoesCardPickerMatch(gameState, card);
        }
    }

    internal class Lookout
    {
        public static bool ShouldPlay(GameState gameState, PlayerAction playerAction)
        {            
            int cardCountToTrash = Strategies.CountInDeck(Cards.Copper, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Estate))
            {
                cardCountToTrash += Strategies.CountInDeck(Cards.Estate, gameState);
            }

            cardCountToTrash += Strategies.CountInDeck(Cards.Curse, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.Hovel, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.Necropolis, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.OvergrownEstate, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Lookout))
            {
                cardCountToTrash += Strategies.CountInDeck(Cards.Lookout, gameState);
            }           

            int totalCardsOwned = gameState.Self.CardsInDeck.Count;

            return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
        }
    }

    internal class MarketSquare
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public MarketSquare(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class Rebuild
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Rebuild(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }

        public override Card NameACard(GameState gameState)
        {
            PlayerState self = gameState.Self;

            int pointLead = Strategies.PlayersPointLead(gameState); 

            //Name Duchy
            if (Strategies.CountOfPile(Cards.Duchy, gameState) > 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                (Strategies.CountInDeckAndDiscard(Cards.Province, gameState) == 0 ||
                    Strategies.CountInDeck(Cards.Province, gameState) == 0 &&
                    Strategies.CountInDeck(Cards.Duchy, gameState) > 0 &&
                    Strategies.CountInDeck(Cards.Estate, gameState) > 0))
            {
                return Cards.Duchy;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategies.CountInDeck(Cards.Estate, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) >= 0 &&
                Strategies.CountInDeck(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) == 0
                && Strategies.CountInDeckAndDiscard(Cards.Province, gameState) >= 0
                && Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Estate if you can end it with a win                    
            if (Strategies.CountInHand(Cards.Rebuild, gameState) + 1 >= Strategies.CountOfPile(Cards.Province, gameState) && 
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Estate, gameState) > 0)
            {
                return Cards.Estate;
            }

            //Name Province if Duchy is in Draw and Draw contains more P than E
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Duchy, gameState) > 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) > Strategies.CountInDeck(Cards.Estate, gameState))
            {
                return Cards.Province;
            }

            //Name Estate if you're ahead and both P and E are left in draw
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) > 0 &&
                Strategies.CountInDeck(Cards.Estate, gameState) > 0 && 
                pointLead > 2)
            {
                return Cards.Estate;
            }

            //Name Estate over Province if you're way ahead
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) < 3 &&
                Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 && 
                pointLead > 4)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies left
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountAllOwned(Cards.Duchy, gameState) == 0 &&
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies not in hand
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0 && 
                pointLead > 2)
            {
                return Cards.Estate;
            }

            if (Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0)
            {
                return Cards.Province;
            }

            return Cards.Estate;
        }    
    }    

    internal class Trader
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Trader(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.DefaultGetCardFromHandToTrash(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return playerAction.trashOrder.DoesCardPickerMatch(gameState, cardFor) &&
                   !playerAction.purchaseOrder.DoesCardPickerMatch(gameState, cardFor);
        }
    }

    internal class Watchtower
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Watchtower(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }   
}
