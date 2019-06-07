using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EconSettings
{
    public static float ADJUST_RATIO = 1.1f;
}

/// <summary>
/// TO DO: PULL PROBABILITY DISTRIBUTIONS
/// TO DO: ADD UNIFORM DISTRIBUTION
/// TO DO: ACQUIRE CHARTING LIBRARY
/// TO DO: PROCESS SIMULATION
/// TO DO: CHART SIMULATION
/// </summary>


public class EconSim
{
    List<Buyer> buyers;
    List<Seller> sellers;

    public EconSim(int countBuyers, int countSellers)
    {
        for (int i = 0; i < countBuyers; i++)
        {
            buyers.Add(new Buyer(15, 10));
        }
        for (int i = 0; i < countSellers; i++)
        {
            sellers.Add(new Seller(5, 10));
        }
    }
}

public class MarketDay
{
    public void Bargain(Buyer buyer, Seller seller)
    {
        buyer.countBargains++;
        seller.countBargains++;

        float price = seller.dynamicWTS;

        if (price < buyer.dynamicWTP)
        {
            float buyerSurplus = buyer.maxWTP - price;
            buyer.totalBuyerSurplus += buyerSurplus;
            buyer.wallet -= price;
            buyer.countTrades++;

            float sellerSurplus = price - seller.minWTS;
            seller.totalSellerSurplus += sellerSurplus;
            seller.wallet += price;
            seller.countTrades++;
        }
        else
        {
            float newWTS = seller.dynamicWTS / EconSettings.ADJUST_RATIO;
            seller.dynamicWTS = newWTS < seller.minWTS ? seller.minWTS : newWTS;

            float newWTP = buyer.dynamicWTP * EconSettings.ADJUST_RATIO;
            buyer.dynamicWTP = newWTP > buyer.maxWTP ? buyer.maxWTP : newWTP;
        }
    }
}
public interface Trader
{
    float wallet { get; set; }
    int countBargains { get; set; }
    int countTrades { get; set; }
}

public abstract class TraderBase
{
    public float wallet { get; set; }
    public int countBargains { get; set; }
    public int countTrades { get; set; }
}


public class Buyer : TraderBase, Trader
{
    public float maxWTP { get; set; }
    public float dynamicWTP { get; set; }
    public float totalBuyerSurplus { get; set; }
    public Buyer(float startingMaxWTP, float startingDynamicWTP)
    {
        maxWTP = startingMaxWTP;
        dynamicWTP = startingDynamicWTP;
    }
}

public class Seller : TraderBase, Trader
{
    public float minWTS { get; set; }
    public float dynamicWTS { get; set; }
    public float totalSellerSurplus { get; set; }
    public Seller(float startingMinWTS, float startingDynamicWTS)
    {
        minWTS = startingMinWTS;
        dynamicWTS = startingDynamicWTS;
    }
}
