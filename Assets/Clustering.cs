using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum ClusterNumberIdentificationType { NBased, ScreeElbow, Silhouette, Gap, KizanowskiLai, Hartigan, VRC, Omnibus, None};
public class Clustering
{       
    public int[] ClusterKMeans(float[][] Data,
        ClusterNumberIdentificationType type, int DefaultK, int MaxK)
    {
        int bestK = IdentifyOptimalClusterNumber(Data, type, DefaultK, MaxK);
        mDebug.Log("Best # of Clusters is " + bestK, false);
        float[][] InitCentroids = KMeansInitKZZ(Data, bestK);
        int[] KClass = KMeansClassification(Data, InitCentroids, 2);
        return KClass;
    }

    #region K-Means Wrappers
    // Wrapper for MapGen
    public int[,] ClusterMap(MapGen map, int numCats)
    {
        List<string> qualityFields = new List<string>() { "Longitude", "Latitude", "WaterFlux", "Temperature" };

        int[,] indexMap = new int[map.xDim, map.yDim];

        List<float[]> dataList = new List<float[]>();

        int index = 0;
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Dictionary<string, float> qualities = map.GetLocationQualities(x, y);
                dataList.Add(DimensionsFromQualities(qualities, qualityFields));

                indexMap[x, y] = index;
                index++;
            }
        }

        float[][] dataArray = dataList.ToArray();

        int[] clusters = ClusterKMeans(dataArray, ClusterNumberIdentificationType.None, numCats, 3*numCats);

        int[,] clusterMap = new int[map.xDim, map.yDim];
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                clusterMap[x, y] = clusters[indexMap[x, y]];
            }
        }

        return clusterMap;
    }
    float[] DimensionsFromQualities(Dictionary<string, float> qualities, List<string> fieldNames)
    {
        List<float> dimList = new List<float>();

        foreach (string fieldName in fieldNames)
        {
            dimList.Add(qualities[fieldName]);
        }

        return dimList.ToArray();
    }
    #endregion

    #region K-Means Core
    // K-Means Core
    public int[] KMeansClassification(float[][] Data, float[][] InitialCentroids, float pnorm)
    {
        int nK = InitialCentroids.Length;
        int nD = InitialCentroids[0].Length;
        int N = Data.Length;
        int[] DataClass = new int[N];
        // 1. Classify Data depending on Centroids
        for (int i = 0; i < N; i++)
        {
            int classval = ClosestCentroid(InitialCentroids, Data[i], pnorm);
            DataClass[i] = classval;
        }
        // 2. Recalulate Centroids
        List<float[]>[] KMCL = KMeansClassificationList(Data, DataClass);

        float[][] Centroids = CalculateCentroids(KMCL);
        bool delta = true;
        int[] OldClass = DataClass;
        int[] NewClass = DataClass;
        while (delta == true)
        {
            // 1. Re-classify data depending on centroids
            for (int i = 0; i < N; i++)
            {
                int classval = ClosestCentroid(Centroids, Data[i], pnorm);
                NewClass[i] = classval;
            }
            delta = DeltaClassification(OldClass, NewClass);
            OldClass = NewClass;
            // 2. Re-calculate Centroids
            KMCL = KMeansClassificationList(Data, NewClass);
            Centroids = CalculateCentroids(KMCL);
        }
        return NewClass;
    }

    public bool DeltaClassification(int[] OldClass, int[] NewClass)
    {
        bool delta = false;
        int N = OldClass.Length;
        for (int i = 0; i < N; i++)
        {
            if (OldClass[i] != NewClass[i])
            {
                delta = true;
                break;
            }
        }
        return delta;
    }
    public float MinkowskiDistance(float[] Adims, float[] Bdims, float p)
    {
        float rootthis = 0;
        for (int i = 0; i < Adims.Length; i++)
        {
            rootthis += Convert.ToSingle(Math.Pow(Math.Abs(Adims[i] - Bdims[i]), p));
        }
        return Convert.ToSingle(Math.Pow(rootthis, (1 / p)));
    }
    public int ClosestCentroid(float[][] Centroids, float[] DataPoint, float pnorm)
    {
        int closest = 0;
        float shortestdistance = float.PositiveInfinity;
        for (int i = 0; i < Centroids.Count(); i++)
        {
            float localdistance = MinkowskiDistance(DataPoint, Centroids[i], pnorm);
            if (localdistance < shortestdistance)
            {
                shortestdistance = localdistance;
                closest = i;
            }
        }
        return closest;
    }
    public int SecondClosestCentroid(float[][] Centroids, float[] DataPoint, float pnorm)
    {
        int closest = 0;
        int secondclosest = 0;
        float shortestdistance = float.PositiveInfinity;
        float secondshortestdistance = float.PositiveInfinity;

        for (int i = 0; i < Centroids.Count(); i++)
        {
            float localdistance = MinkowskiDistance(DataPoint, Centroids[i], pnorm);
            if (localdistance < shortestdistance)
            {
                secondshortestdistance = shortestdistance;
                shortestdistance = localdistance;
                secondclosest = closest;
                closest = i;
            }
            else if (localdistance < secondshortestdistance)
            {
                secondshortestdistance = localdistance;
                secondclosest = i;
            }
        }
        return secondclosest;
    }
    public List<float[]>[] KMeansClassificationList(float[][] Data, int[] DataClass)
    {
        // this function takes the data and a keyarray that sorts them into clusters and returns an array of lists of data points where each list corresponds to one cluster and the data points are each represented by an array of their dimensions
        int K = DataClass.Max() + 1;
        int N = Data.Length;
        List<float[]>[] ArrayOfClustersDims = new List<float[]>[K];
        for (int k = 0; k < K; k++)
        {
            ArrayOfClustersDims[k] = new List<float[]>();
            for (int i = 0; i < N; i++)
            {
                if (DataClass[i] == k)
                {
                    // data[j] is in cluster[i]
                    ArrayOfClustersDims[k].Add(Data[i]);
                }
            }
        }
        return ArrayOfClustersDims;
    }
    public int nDimsFromKMCL(List<float[]>[] KMCL)
    {
        int K = KMCL.Length;
        int N = nDataFromKMCL(KMCL);
        int returnval = 1;
        for (int i = 0; i < N; i++)
        {
            if (KMCL[i].Count() != 0)
            {
                returnval = KMCL[i][0].Length;
                break;
            }
        }
        return returnval;
    }
    public int nDataFromKMCL(List<float[]>[] KMCL)
    {
        int K = KMCL.Length; // the total number of clusters 
        int ndata = 0;
        for (int i = 0; i < K; i++)
        {
            ndata += KMCL[i].Count();
        }
        return ndata;
    }
    public float[][] CalculateCentroids(List<float[]>[] KMCL)
    {
        // Takes an array of lists of data points in each cluster and calculates the centroids
        int K = KMCL.Length;
        int nData = nDataFromKMCL(KMCL);
        int nDims = nDimsFromKMCL(KMCL);
        float[][] CalculatedCentroids = new float[K][];
        for (int k = 0; k < K; k++)
        {
            // for each cluster
            CalculatedCentroids[k] = new float[nDims];
            float[] Kcentroid = new float[nDims];
            for (int i = 0; i < nDims; i++)
            {
                // for each dimension
                float dimsum = 0;
                int count = 0;
                for (int j = 0; j < KMCL[k].Count(); j++)
                {
                    // go through the list of data points in that cluster
                    dimsum += KMCL[k][j][i];
                    count++;
                }
                Kcentroid[i] = dimsum / count;
            }
            for (int ii = 0; ii < nDims; ii++)
            {
                CalculatedCentroids[k][ii] = Kcentroid[ii];
            }
        }
        return CalculatedCentroids;
    }
    public float[] DataDimensionMax(float[][] Data)
    {
        int nData = Data.Length;
        int nDims = Data[0].Length;
        float[] DimMax = new float[nDims];
        for (int i = 0; i < nDims; i++)
        {
            float localMax = float.NegativeInfinity;
            for (int j = 0; j < nData; j++)
            {
                if (Data[j][i] > localMax)
                {
                    localMax = Data[j][i];
                }
            }
        }
        return DimMax;
    }
    public float[] DataDimensionMin(float[][] Data)
    {
        int nData = Data.Length;
        int nDims = Data[0].Length;
        float[] DimMin = new float[nDims];
        for (int i = 0; i < nDims; i++)
        {
            float localMin = float.PositiveInfinity;
            for (int j = 0; j < nData; j++)
            {
                if (Data[j][i] < localMin)
                {
                    localMin = Data[j][i];
                }
            }
        }
        return DimMin;
    }

    #endregion

    #region Cluster Initialization

    // KZZ Initializations
    public float[][] KMeansInitKZZ(float[][] Data, int K)
    {
        float p = 2; // euclidean distance
        int nData = Data.Length;
        int nDims = Data[0].Length;
        // 1. official method is to start with a random seed, but that is an absolutely terrible idea, so we will start with the M along all dimensions as an estimate of the densest point of data
        //Random rnd = new Random();
        //int firstindex = rnd.Next(nData);
        //float[] firstcentroids = Data[firstindex];
        float[] firstcentroids = VectorMeans(Data);
        float[][] returnarray = new float[K][];
        List<float[]> InitCentroidsList = new List<float[]>();

        returnarray[0] = firstcentroids;
        InitCentroidsList.Add(firstcentroids);
        // 2. next centroid is that with the greatest distance to the 1st centroid
        for (int k = 1; k < K; k++)
        {
            // for each new cluster
            int[] NearestCentroidsIndex = new int[nData];
            float[] NearestCentroidsDistance = new float[nData];
            for (int x = 0; x < nData; x++)
            {
                float[] Location = Data[x];
                float mindistance = float.PositiveInfinity;
                int nearestcentroidindex = 0;
                for (int j = 0; j < k; j++)
                {
                    float DtoX = MinkowskiDistance(Location, InitCentroidsList[j], p);
                    if (DtoX < mindistance)
                    {
                        mindistance = DtoX;
                        nearestcentroidindex = j;
                    }
                }
                NearestCentroidsIndex[x] = nearestcentroidindex;
                NearestCentroidsDistance[x] = mindistance;
            }
            float farthestpointdistance = float.NegativeInfinity;
            int farthestpointindex = 0;
            for (int j = 0; j < nData; j++)
            {
                if (NearestCentroidsDistance[j] > farthestpointdistance)
                {
                    farthestpointdistance = NearestCentroidsDistance[j];
                    farthestpointindex = j;
                }
            }
            float[] ThisCentroid = Data[farthestpointindex];
            InitCentroidsList.Add(ThisCentroid);
            returnarray[k] = ThisCentroid;
        }
        // 3. redo k init 0
        int[] NearestCentroidsIndexFix = new int[nData];
        float[] NearestCentroidsDistanceFix = new float[nData];
        for (int x = 0; x < nData; x++)
        {
            float[] Location = Data[x];
            float mindistance = float.PositiveInfinity;
            int nearestcentroidindex = 0;
            for (int j = 1; j < K; j++)
            {
                float DtoX = MinkowskiDistance(Location, InitCentroidsList[j], p);
                if (DtoX < mindistance)
                {
                    mindistance = DtoX;
                    nearestcentroidindex = j;
                }
            }
            NearestCentroidsIndexFix[x] = nearestcentroidindex;
            NearestCentroidsDistanceFix[x] = mindistance;
        }
        float farthestpointdistanceFix = float.NegativeInfinity;
        int farthestpointindexFix = 0;
        for (int j = 0; j < nData; j++)
        {
            if (NearestCentroidsDistanceFix[j] > farthestpointdistanceFix)
            {
                farthestpointdistanceFix = NearestCentroidsDistanceFix[j];
                farthestpointindexFix = j;
            }
        }
        float[] ThisCentroidFix = Data[farthestpointindexFix];
        InitCentroidsList.Add(ThisCentroidFix);
        returnarray[0] = ThisCentroidFix;

        return returnarray;
    }
    public float[] VectorMeans(float[][] Data)
    {
        int N = Data.Length;
        int D = Data[0].Length;
        float[] Ms = new float[D];
        for (int d = 0; d < D; d++)
        {
            float sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += Data[i][d];
            }
            Ms[d] = sum / N;
        }
        return Ms;
    }
    #endregion

    #region Cluster Number Identification

    public int IdentifyOptimalClusterNumber(float[][] Data,
        ClusterNumberIdentificationType type, int DefaultK, int MaxK)
    {
        int bestK = 0;

        switch (type)
        {
            case ClusterNumberIdentificationType.None:
                return DefaultK;
            // 0. Find K from N-based heuristic
            case ClusterNumberIdentificationType.NBased:
                {
                    return ClusterNumberNBased(Data);
                }
            // 1. Find K from elbow in Wk "Scree" Plot
            case ClusterNumberIdentificationType.ScreeElbow:
                {
                    return ClusterNumberScreeElbow(Data, MaxK);
                }
            // 2. Find K from maximum Silhouette
            case ClusterNumberIdentificationType.Silhouette:
                {
                    return ClusterNumberSilhouette(Data, MaxK, 2);
                }
            // 3. Find K from Gap between actual and simulated
            case ClusterNumberIdentificationType.Gap:
                {
                    return ClusterNumberGap(Data, MaxK);
                }
            // 4. Find K from cluster compactness
            case ClusterNumberIdentificationType.KizanowskiLai:
                {
                    return ClusterNumberKLDiff(Data, MaxK);
                }
            // 5. Find K from change in Wk
            case ClusterNumberIdentificationType.Hartigan:
                {
                    return ClusterNumberHartigan(Data, MaxK);
                }
            // 6. Find K from change in ratio between Wk and Bk
            case ClusterNumberIdentificationType.VRC:
                {
                    return OptimalKfromVRC(Data, MaxK);
                }
            // 7. All of them! -> Min
            case ClusterNumberIdentificationType.Omnibus:
                {
                    List<int> OptimalKList = new List<int>();
                    OptimalKList.Add(ClusterNumberNBased(Data));
                    OptimalKList.Add(ClusterNumberScreeElbow(Data, MaxK));
                    OptimalKList.Add(ClusterNumberSilhouette(Data, MaxK, 2));
                    OptimalKList.Add(ClusterNumberGap(Data, MaxK));
                    OptimalKList.Add(ClusterNumberKLDiff(Data, MaxK));
                    OptimalKList.Add(ClusterNumberHartigan(Data, MaxK));
                    OptimalKList.Add(OptimalKfromVRC(Data, MaxK));
                    return OptimalKList.Min();
                }
        }

        return bestK;
    }

    // N-Based Heuristic
    int ClusterNumberNBased(float[][] Data)
    {
        return (int)Math.Round(Math.Sqrt(Data.Length / 2), 0);
    }

    // Scree Elbow
    int ClusterNumberScreeElbow(float[][] Data, int MaxK)
    {
        string sAirline = "";
        bool summary = false;

        float[] Wk = WkForEachKFull(Data, MaxK);
        BuildWkTable(sAirline, Wk);
        return ElbowInScree(Wk, summary);
    }

    public void BuildWkTable(string sAirline, float[] Wk)
    {
        string sTableName = "LegClusterDensity";
        string sSQL = "";

        sSQL = "CREATE TABLE " + sTableName + " (Airline char(3) NOT NULL, NumberClusters tinyint NOT NULL, DensityWk REAL NOT NULL, Type char(20))";

        //FcstUtil.PassSQLQueryContained(sSQL, sSQLConnection);
        int iNumberOfClusters = Wk.Length;
        for (int iwk = 0; iwk < iNumberOfClusters; iwk++)
        {
            sSQL = "INSERT INTO " + sTableName + " (Airline, NumberClusters, DensityWk, Type) Values ('" +
                    sAirline + "'," + (iwk + 1) + "," + Wk[iwk] + ", 'Dummy')";
            //FcstUtil.PassSQLQueryContained(sSQL, sSQLConnection);
        }
    }
    public int ElbowInScree(float[] Y, bool summary)
    {
        int N = Y.Length;
        float[] X = GenerateTime(1, 1, N);
        List<bool> bSlopesAreNegativeList = new List<bool>();
        List<float> errorList = new List<float>();
        for (int i = 1; i < N - 2; i++)
        {
            float[][] SplitY = SplitfloatArray(Y, i);
            float[][] SplitX = SplitfloatArray(X, i);
            float dFirstSlope = 0f;// FcstUtil.LinRegSlope(SplitX[0], SplitY[0]);
            float dSecondSlope = 0f;// FcstUtil.LinRegSlope(SplitX[1], SplitY[1]);
            float[] fit1 = new float[N];// FcstUtil.LinRegFitted(SplitX[0], dFirstSlope, FcstUtil.LinRegIntercept(SplitX[0], SplitY[0]));
            float[] fit2 = new float[N];// FcstUtil.LinRegFitted(SplitX[1], dSecondSlope, FcstUtil.LinRegIntercept(SplitX[1], SplitY[1]));
            float error = 0f;// FcstUtil.MSE(FcstUtil.Error(SplitY[0], fit1)) + FcstUtil.MSE(FcstUtil.Error(SplitY[1], fit2));
            errorList.Add(error);
            if (dFirstSlope < 0 && dSecondSlope < 0)
            {
                bSlopesAreNegativeList.Add(true);
            }
            else
            {
                bSlopesAreNegativeList.Add(false);
            }
        }
        float[] errorArray = errorList.ToArray();
        float minval = float.PositiveInfinity;
        int minindex = 0;
        for (int i = 0; i < errorArray.Length; i++)
        {
            if (errorArray[i] < minval && bSlopesAreNegativeList[i] == true)
            {
                minval = errorArray[i];
                minindex = i;
            }
        }
        int optimalK = (minindex + 3);
        return optimalK;
    }
    public float[][] SplitfloatArray(float[] x, int SplitAfter)
    {
        int N = x.Length;
        float[][] SplitArray = new float[2][];
        List<float> List1 = new List<float>();
        List<float> List2 = new List<float>();
        for (int i = 0; i < (SplitAfter + 1); i++)
        {
            List1.Add(x[i]);
        }
        for (int i = (SplitAfter + 1); i < N; i++)
        {
            List2.Add(x[i]);
        }
        SplitArray[0] = List1.ToArray();
        SplitArray[1] = List2.ToArray();
        return SplitArray;
    }
    public float[] GenerateTime(int start, int add, int T)
    {
        float[] Time = new float[T];
        for (int t = 0; t < T; t++)
        {
            Time[t] = start + add * t;
        }
        return Time;
    }

    // Silhouette Method
    int ClusterNumberSilhouette(float[][] Data, int MaxK, float p)
    {
        float[] AvgSilVal = new float[MaxK - 2];
        for (int k = 0; k < MaxK - 2; k++)
        {
            AvgSilVal[k] = AverageSilhouetteVal(Data, (k + 2), p);
        }


        int maxindex = 0;
        float maxval = float.NegativeInfinity;
        for (int k = 0; k < MaxK - 2; k++)
        {
            if (AvgSilVal[k] > maxval)
            {
                maxval = AvgSilVal[k];
                maxindex = k;
            }
        }
        maxindex += 2;
        return maxindex;
    }

    float SilhouetteAverageDistance(float[] x, List<float[]> ClusterData, float p)
    {
        float sumDistance = 0;
        int N = ClusterData.Count();
        for (int i = 0; i < N; i++)
        {
            sumDistance += MinkowskiDistance(x, ClusterData[i], p);
        }
        float avgDistance = sumDistance / N;
        return avgDistance;
    }
    float SilhouetteBi(float[] x, int xK, List<float[]>[] ClustersData, float p)
    {
        int K = ClustersData.Length;
        float[] ClusterDistances = new float[K];
        for (int i = 0; i < K; i++)
        {
            ClusterDistances[i] = SilhouetteAverageDistance(x, ClustersData[i], p);
        }
        float xHomeClusterDistance =
            ClusterDistances
                .Min(); // the cluster to which x has the shortest average distance must be the cluster in which x resides
        float minval = float.PositiveInfinity;
        for (int i = 0; i < K; i++)
        {
            if (ClusterDistances[i] < minval && i != xK)
            {
                minval = ClusterDistances[i];
            }
        }
        return minval;
    }
    float SilhouetteVal(float[] x, int xK, List<float[]>[] ClustersData, float p)
    {
        float Ai = SilhouetteAverageDistance(x, ClustersData[xK], p);
        float Bi = SilhouetteBi(x, xK, ClustersData, p);
        float Si = 0;
        if (Ai > Bi)
        {
            Si = Bi / Ai - 1;
        }
        if (Ai < Bi)
        {
            Si = 1 - Ai / Bi;
        }
        return Si;
    }
    float AverageSilhouetteVal(float[][] Data, int K, float p)
    {
        float[][] InitialCentroids = KMeansInitKZZ(Data, K);
        int[] KClass = KMeansClassification(Data, InitialCentroids, p);
        List<float[]>[] ClusteredData = KMeansClassificationList(Data, KClass);
        int N = Data.Length;
        List<float> SVlist = new List<float>();
        for (int i = 0; i < N; i++)
        {
            float SV = SilhouetteVal(Data[i], KClass[i], ClusteredData, p);
            SVlist.Add(SV);
        }
        float avgSV = SVlist.Average();
        return avgSV;
    }

    // Gap Statistic (Tibshirani et al 2001)
    public int ClusterNumberGap(float[][] Data, int MaxK)
    {
        float[] WkForEachK = WkForEachKFull(Data, MaxK);
        int K = WkForEachK.Length;
        // in the end, return the optimal K as an int; 

        // Use same K-mean centroid init method in a MonteCarlo of uniform reference distribution
        int nMonteCarlo = 50;
        float[] DummyWk = new float[K];
        for (int i = 0; i < K; i++)
        {
            DummyWk[i] = 1;
        }
        for (int m = 0; m < nMonteCarlo; m++)
        {
            float[][] RefData = GapStatRefUniform(Data);
            float[] WkArray = new float[K];
            for (int k = 1; k < K; k++)
            {
                float[][] initcentroids = KMeansInitKZZ(Data, k);
                int[] KClass = KMeansClassification(RefData, initcentroids, 2);
                float[][] Centroids = CalculateCentroids(KMeansClassificationList(RefData, KClass));
                //ClusterGraph(RefData, KClass);
                float wk = WithinKSumDistanceFromCenter(Data, Centroids, KClass, 2);
                DummyWk[k] += (wk / nMonteCarlo);
            }
        }
        DummyWk = NaturalLogOnArray(DummyWk);
        float[] LnWk = NaturalLogOnArray(WkForEachK);
        // find k that maximizes (LnWk - DummyWk)
        float[] diff = new float[K];
        for (int k = 0; k < K; k++)
        {
            diff[k] = DummyWk[k] - LnWk[k];
        }
        float MaxDiff = float.NegativeInfinity;
        int maxindex = 0;
        for (int k = 0; k < MaxK; k++)
        {
            if (diff[k] > MaxDiff)
            {
                MaxDiff = diff[k];
                maxindex = k;
                Console.WriteLine("MaxIndex is " + k);
            }
        }
        return maxindex + 1;
    }

    public float[] NaturalLogOnArray(float[] dArray)
    {
        float dMinimumValue = 0.00001f;

        float[] dNaturalLogArray = new float[dArray.Length];
        for (int i = 0; i < dArray.Length; i++)
        {
            if (dArray[i] > dMinimumValue)
            {
                dNaturalLogArray[i] = Convert.ToSingle(Math.Log(dArray[i]));
            }
            else
            {
                dNaturalLogArray[i] = Convert.ToSingle(Math.Log(dMinimumValue));
            }
        }
        return dNaturalLogArray;
    }
    public float[][] GapStatRefUniform(float[][] Data)
    {
        int nData = Data.Length;
        int nDims = Data[0].Length;
        float[] DimMax = DataDimensionMax(Data);
        float[] DimMin = DataDimensionMin(Data);
        float[][] Ref = new float[nData][];
        for (int i = 0; i < nData; i++)
        {
            float[] simpoint = new float[nDims];
            Random rand = new Random();
            for (int j = 0; j < nDims; j++)
            {
                double r = rand.NextDouble();
                simpoint[j] = Convert.ToSingle(DimMin[j] + (DimMax[j] - DimMin[j]) * r);
            }
            Ref[i] = simpoint;
        }
        return Ref;
    }

    // KL-Diff (Kizanowski-Lai, 1988); aka Compactness
    public int ClusterNumberKLDiff(float[][] data, int MaxK)
    {
        int datapermaxk = 5;
        int bestof = (int)Math.Round((double)MaxK / datapermaxk, 0);
        float[] KL = KLDiff(data, MaxK);
        // the optimal k is that which minimizes KL[k] i.e., the improvement of adding another cluster suddenly decreases
        int mindex = 0;
        float minval = float.PositiveInfinity;
        for (int i = 0; i < KL.Length; i++)
        {
            if (KL[i] < minval)
            {
                minval = KL[i];
                mindex = i;
            }
        }
        int Kbest = mindex + 2;
        return Kbest;
    }

    public float[] KLDiff(float[][] Data, int MaxK)
    {
        // KL[0] corresponds to K = 1
        MaxK += 2;
        int p = Data[0].Length; // p is the number of dimensions
        float[] Wk = WkForEachKFull(Data, MaxK);
        float[] DIFF = new float[MaxK];
        for (int k = 0; k < MaxK - 1; k++)
        {
            int actualk = k + 1;
            DIFF[k] = Convert.ToSingle(Math.Pow((actualk - 1), (2 / p)) * Wk[actualk - 1] -
                                        Math.Pow(actualk, (2 / p)) * Wk[actualk]);
        }
        float[] KL = new float[MaxK - 2];
        for (int k = 0; k < MaxK - 2; k++)
        {
            KL[k] = Math.Abs(DIFF[k] / DIFF[k + 1]);
        }
        return KL;
    }

    // Hartigan's (1975) Stopping Rule aka Delta Wk
    public int ClusterNumberHartigan(float[][] Data, int MaxK)
    {
        int nDims = Data[0].Length;
        int nData = Data.Length;
        float[] Wk = WkForEachKFull(Data, MaxK);
        float[] Hk = HartiganStat(Wk, nData);
        int optimalK = 0;
        int J = Hk.Length;
        for (int j = 0; j < J; j++)
        {
            optimalK = j;
            if (Hk[j] > 10)
                break;
        }
        return optimalK;
    }

    public float[] HartiganStat(float[] Wk, int n)
    {
        float[] H = new float[Wk.Length];
        Console.WriteLine("Hartigan's n = " + n);
        for (int k = 0; k < Wk.Length - 1; k++)
        {
            H[k] = ((Wk[k] / Wk[k + 1]) - 1) / (n - k);
            Console.WriteLine("H[" + k + "] = " + H[k]);
        }
        return H;
        // the optimal k is the smallest k for which H >= 10
    }

    // Calinski-Harabasz (1974) VRC method
    int OptimalKfromVRC(float[][] Data, int MaxK)
    {
        float[] VRCd = new float[MaxK];
        for (int k = 2; k < MaxK; k++)
        {
            VRCd[k - 2] = VRC(k, Data);
        }
        float[] Wk = new float[MaxK];
        for (int k = 2; k < MaxK - 1; k++)
        {
            Wk[k] = (VRCd[k + 1] - VRCd[k]) - (VRCd[k] - VRCd[k - 1]);
        }
        int minwkindex = 0;
        float minwk = float.PositiveInfinity;
        for (int k = 2; k < MaxK - 1; k++)
        {
            if (Wk[k] < minwk)
            {
                minwk = Wk[k];
                minwkindex = k;
            }
        }
        return minwkindex;
    }

    float VRC(int k, float[][] Data)
    {
        int n = Data.Length;
        float[][] InitCentroids = KMeansInitKZZ(Data, k);
        int[] KClass = KMeansClassification(Data, InitCentroids, 2);
        List<float[]>[] KMCL = KMeansClassificationList(Data, KClass);
        float[][] Centroids = CalculateCentroids(KMCL);
        float SSBk = BetweenKSumDistanceFromCenter(KMCL);
        float SSWk = WithinKSumDistanceFromCenter(Data, Centroids, KClass, 2);
        float VRCtop = SSBk / (k - 1);
        float VRCbot = SSWk / (n - k);
        float VRCk = VRCtop / VRCbot;
        return VRCk;
    }
    float BetweenKSumDistanceFromCenter(List<float[]>[] Data)
    {
        // this is SSB, the separateness of the clusters
        int K = Data.Length;
        int D = Data[0][0].Length;
        float[][] centroids = CalculateCentroids(Data);
        float SSB = 0;
        float[] MeanMean = new float[D];
        for (int i = 0; i < D; i++)
        {
            MeanMean[i] = 0;
            for (int k = 0; k < K; k++)
            {
                MeanMean[i] += centroids[k][i] / K;
            }
        }
        for (int i = 0; i < K; i++)
        {
            float[] local = centroids[i];
            for (int j = 0; j < Data[i].Count(); j++)
            {
                SSB += MinkowskiDistance(local, MeanMean, 2);
            }
        }
        return SSB;
    }

    // K-Means Cluster Number Identification General Functions
    public float WithinKSumDistanceFromCenter(float[][] Data, float[][] Centroids, int[] KClass, float p)
    {
        // the sum of the sum of squared distance between the points in a cluster and its centroid across all clusters
        int nData = Data.Length;
        int nDims = Data[0].Length;
        int K = KClass.Max() + 1;
        float[] WkArray = new float[K];
        for (int k = 0; k < K; k++)
        {
            float localWk = 0;
            float[] LocalCentroid = Centroids[k];
            List<float[]> KList = new List<float[]>();
            for (int i = 0; i < nData; i++)
            {
                if (KClass[i] == k)
                {
                    float[] Local = Data[i];
                    localWk += MinkowskiDistance(Local, LocalCentroid, p);
                }
            }
            WkArray[k] = localWk;
        }
        float WkSum = WkArray.Sum();
        return WkSum;
    }

    public float[] WkForEachKFull(float[][] Data, int MaxK)
    {
        List<float[][]> CList = new List<float[][]>();
        List<int[]> KList = new List<int[]>();
        for (int k = 1; k < MaxK + 1; k++)
        {
            float[][] InitCentroids = KMeansInitKZZ(Data, k);
            int[] KClass = KMeansClassification(Data, InitCentroids, 2);
            CList.Add(InitCentroids);
            KList.Add(KClass);
        }
        float[] Wk = WkCurve(Data, CList, KList, 2);
        // here, Wk[0] is the Wk given by the 0th item on the list of KClassifications, which should be k = 1
        return Wk;
    }
    public float[] WkCurve(float[][] Data, List<float[][]> Centroids, List<int[]> KClasses, float p)
    {
        int MaxK = Centroids.Count();
        float[] Wk = new float[MaxK];
        for (int k = 0; k < MaxK; k++)
        {
            Wk[k] = WithinKSumDistanceFromCenter2(Data, Centroids[k], KClasses[k], p);
        }
        return Wk;
    }
    public float WithinKSumDistanceFromCenter2(float[][] Data, float[][] Centroids, int[] KClass, float p)
    {
        // the sum of the sum of squared distance between the points in a cluster and its centroid across all clusters
        int nData = Data.Length;
        int nDims = Data[0].Length;
        int K = KClass.Max() + 1;
        float[] WkArray = new float[K];
        int[] iDataCount = new int[K];
        int iSecondNearestIndex = 0;
        for (int iK = 0; iK < K; iK++)
        {
            iDataCount[iK] = 0;
        }
        for (int iSample = 0; iSample < nData; iSample++)
        {
            iDataCount[KClass[iSample]]++;
        }
        for (int k = 0; k < K; k++)
        {
            float localWk = 0;
            float[] LocalCentroid = Centroids[k];

            List<float[]> KList = new List<float[]>();
            for (int i = 0; i < nData; i++)
            {
                if (KClass[i] == k)
                {
                    float[] Local = Data[i];
                    if (iDataCount[k] <= 1)
                    {
                        // if there is only 1 sample in a cluster, then it will look like adding that cluster is very good, yet it is useless. Instead of itself, the reference centroid should be to the 2nd nearest centroid
                        iSecondNearestIndex = SecondClosestCentroid(Centroids, Local, p);
                        localWk += MinkowskiDistance(Local, Centroids[iSecondNearestIndex], p);
                    }
                    else
                    {
                        localWk += MinkowskiDistance(Local, LocalCentroid, p);
                    }
                }
            }
            WkArray[k] = localWk;
        }
        float WkSum = WkArray.Sum();
        return WkSum;
    }

    #endregion
}
