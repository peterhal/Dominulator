using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    class BuyPhaseSpecification : TurnletSpecification
    {
        private PlayerState player;

        public BuyPhaseSpecification(PlayerState player)
        {
            this.player = player;
        }

        public override void Execute(GameState state)
        {
            player.playPhase = PlayPhase.Buy;
            if (player.AvailableBuys > 0)
            {
                var boughtCard = player.actions.GetCardFromSupplyToBuy(state, state.CardAvailableForPurchaseForCurrentPlayer);
                if (boughtCard != null)
                {
                    if (!state.CardAvailableForPurchaseForCurrentPlayer(boughtCard))
                    {
                        throw new Exception("Can't buy that card.");
                    }

                    // Pay for the card.
                    player.turnCounters.RemoveCoins(boughtCard.CurrentCoinCost(player));
                    player.turnCounters.RemovePotions(boughtCard.potionCost);
                    player.turnCounters.RemoveBuy();
                    if (boughtCard.canOverpay)
                    {
                        // TODO: Turn into a turnlet ...
                        player.RequestPlayerOverpayForCard(boughtCard, state);
                    }

                    // Record the buying of the card.
                    player.turnCounters.cardsBoughtThisTurn.Add(boughtCard);

                    // From Hinterlands:
                    // A buy is a buy, followed by an on-buy reactions, ...
                    // then (usually) followed by a gain, followed by on-gain reactions.
                    state.AddTurnlet(new OnBuySpecification(state, player, boughtCard));
                }
            }
        }
    }
}
